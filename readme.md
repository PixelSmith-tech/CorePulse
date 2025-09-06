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

**PixelSmith Non-Commercial License**
You may use, modify, and share CorePulse, but:

* Must retain attribution to PixelSmith
* Cannot sell CorePulse or derivatives
* Cannot use it for commercial purposes
