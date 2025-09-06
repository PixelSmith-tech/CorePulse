using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Principal;
using System.Linq;
using System.Net.NetworkInformation;
using LibreHardwareMonitor.Hardware;
using System.Management;
using System.Text.RegularExpressions;

class Program
{
    static string latestStats = "";
    static string localIP = GetLocalIPAddress();
    static string publicUrl = "";

    static async Task Main()
    {
        if (!IsAdministrator())
        {
            try
            {
                var exeName = Process.GetCurrentProcess().MainModule.FileName;
                Process.Start(new ProcessStartInfo(exeName) { Verb = "runas" });
            }
            catch
            {
                Console.WriteLine("Administrator rights not granted. Cannot continue.");
            }
            return; 

        }

        Console.WriteLine("Select the operation mode:");
        Console.WriteLine("1 — Local server (LAN only)");
        Console.WriteLine("2 — Via Tuna (accessible from any network)");
        Console.Write("Enter 1 or 2: ");
        string choice = Console.ReadLine()?.Trim();

        if (choice == "2")
        {
            publicUrl = StartTuna();
            Console.WriteLine($"Tuna started: {publicUrl}");
        }

        var computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true
        };
        computer.Open();

        _ = Task.Run(() => RunHttpServer());

        Console.Clear();
        Console.CursorVisible = false;

        Console.WriteLine("CorePulse monitoring started!");
        Console.WriteLine($"Local: http://localhost:5000");
        Console.WriteLine($"Local IP: http://{localIP}:5000");
        if (!string.IsNullOrEmpty(publicUrl))
            Console.WriteLine($"Accessible via Tuna: {publicUrl}");
        Console.WriteLine();

        while (true)
        {
            latestStats = GetStats(computer);
            Console.Clear();
            Console.WriteLine(latestStats);
            if (!string.IsNullOrEmpty(publicUrl))
                Console.WriteLine($"Tuna URL: {publicUrl}");
            await Task.Delay(1000);
        }
    }

    static async Task RunHttpServer()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://*:5000/");
        listener.Start();

        while (true)
        {
            try
            {
                var context = await listener.GetContextAsync();
                var response = context.Response;
                string statsToShow = latestStats ?? "Loading...";

                string html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<title>CorePulse</title>
<style>
body {{ background: #1e1e1e; color: #dcdcdc; font-family: Consolas, monospace; padding: 10px; }}
pre {{ white-space: pre-wrap; }}
.block {{ margin-bottom: 20px; }}
</style>
<script>
async function update() {{
    try {{
        let resp = await fetch(window.location.href);
        let text = await resp.text();
        document.getElementById('data').innerHTML = text;
    }} catch(e) {{ console.log(e); }}
}}
setInterval(update, 1000);
</script>
</head>
<body>
<h1>CorePulse — PC Monitoring</h1>
<div class='block'>
    <strong>Local IP:</strong> {localIP}:5000<br/>
    {(string.IsNullOrEmpty(publicUrl) ? "" : $"<strong>Tuna:</strong> {publicUrl}")}
</div>
<pre id='data'>{statsToShow}</pre>
</body>
</html>";

                byte[] buffer = Encoding.UTF8.GetBytes(html);
                response.ContentType = "text/html; charset=UTF-8";
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch { }
        }
    }

    static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    static string StartTuna()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "tuna-desktop_windows-amd64_portable_0.1.6.exe", 
                UseShellExecute = true,  
                CreateNoWindow = false
            };

            Process.Start(psi);

   
            return "Tuna is running — use the GUI and copy the HTTP URL";
        }
        catch
        {
            return "Error when launching Tuna";
        }
    }

    static string GetStats(Computer computer)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"CorePulse — PC Monitoring [{DateTime.Now}]\n");

        foreach (var hardware in computer.Hardware)
        {
            hardware.Update();
            sb.AppendLine($"[{hardware.HardwareType}] {hardware.Name}");
            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.Value == null) continue;
                string value = sensor.SensorType switch
                {
                    SensorType.Load => $"{sensor.Value:F1} %",
                    SensorType.Clock => $"{sensor.Value:F0} MHz",
                    SensorType.Temperature => $"{sensor.Value:F1} °C",
                    SensorType.Data => $"{sensor.Value:F1} GB",
                    SensorType.SmallData => $"{sensor.Value:F0} MB",
                    SensorType.Voltage => $"{sensor.Value:F2} V",
                    _ => sensor.Value.ToString()
                };
                sb.AppendLine($"  {sensor.SensorType,-12} {sensor.Name}: {value}");
            }
            sb.AppendLine();
        }

        try
        {
            var searcher = new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            var ram = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
            if (ram != null)
            {
                double totalKB = Convert.ToDouble(ram["TotalVisibleMemorySize"]);
                double freeKB = Convert.ToDouble(ram["FreePhysicalMemory"]);
                double usedKB = totalKB - freeKB;
                double percent = (usedKB / totalKB) * 100;
                double usedGB = usedKB / 1024 / 1024;
                double totalGB = totalKB / 1024 / 1024;
                sb.AppendLine($"[Memory] Used: {usedGB:F1} GB / {totalGB:F1} GB ({percent:F1} %)");
            }
        }
        catch
        {
            sb.AppendLine("[Memory] Error reading RAM");
        }

        try
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()
                         .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                                     n.NetworkInterfaceType != NetworkInterfaceType.Loopback))
            {
                var stats = ni.GetIPv4Statistics();
                sb.AppendLine($"[Network] {ni.Name}: Sent {stats.BytesSent / 1024.0 / 1024:F2} MB, Received {stats.BytesReceived / 1024.0 / 1024:F2} MB");
                try
                {
                    Ping ping = new Ping();
                    var reply = ping.Send("8.8.8.8", 1000);
                    sb.AppendLine($"  Ping: {(reply != null ? reply.RoundtripTime.ToString() : "error")} ms");
                }
                catch { sb.AppendLine("  Ping: error"); }
            }
        }
        catch { sb.AppendLine("[Network] Error reading network stats"); }

        return sb.ToString();
    }

    static string GetLocalIPAddress()
    {
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }
        catch { return "127.0.0.1"; }
    }
}
