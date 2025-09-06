# CorePulse — PC Monitoring

![CorePulse](https://img.shields.io/badge/author-PixelSmith-blue) ![Version](https://img.shields.io/badge/version-1.1-green) ![Since](https://img.shields.io/badge/founded-2025-orange)

**Live PC monitoring over LAN or via Tuna (HTTP)**

CorePulse is a lightweight utility for monitoring your PC's CPU, GPU, RAM, motherboard, and network statistics in real-time. You can view stats locally or share access remotely via **Tuna**.

---

## Features

* Real-time CPU, GPU, RAM, and motherboard stats
* Network monitoring: traffic and ping
* Local web server (LAN) or accessible via **Tuna**
* Simple, console-based setup
* Automatic page updates every second

---

## Installation

1. **Download the CorePulse repository**:

```bash
git clone https://github.com/PixelSmith/CorePulse.git
cd CorePulse
```

2. **Download Tuna (if you want remote access):**
   Go to [Tuna Releases](https://tuna.am/docs/guides/desktop/) and download the latest `tuna-desktop_windows-amd64_portable.exe`. Place it in the same folder as CorePulse or an already installed file inside the archive .

---

## Usage

1. **Run CorePulse:**
   Double-click `CorePulse.exe` or run it from terminal/PowerShell.

2. **Administrator Rights:**
   CorePulse requires admin rights to access hardware monitoring. If you run without admin, it will prompt to restart as administrator.

3. **Choose server mode:**

* `1` — Local server (LAN only)
* `2` — Via Tuna (HTTP, remote access)

**Option 1:** Access locally via `http://localhost:5000` or `http://<YourLocalIP>:5000`.

**Option 2:** Tuna will start automatically and give you a public URL like `https://xxxx.tunacloud.io`.

4. **Open in Browser:**
   Navigate to the URL provided by CorePulse. Stats update every second automatically.

---

## License

**Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)**

Copyright (c) 2025 PixelSmith

You are free to:

* Share — copy and redistribute the material in any medium or format
* Adapt — remix, transform, and build upon the material

Under the following terms:

* Attribution — You must give appropriate credit to PixelSmith, provide a link to the license, and indicate if changes were made. You may do so in any reasonable manner, but not in any way that suggests the licensor endorses you or your use.
* NonCommercial — You may not use the material for commercial purposes.

No additional restrictions — You may not apply legal terms or technological measures that legally restrict others from doing anything the license permits.

Full license text: [CC BY-NC 4.0 Legal Code](https://creativecommons.org/licenses/by-nc/4.0/legalcode)
