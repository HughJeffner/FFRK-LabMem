# Notice
**GL end of service has been announced for 9/29.**  Although new developement on this version has ended, I will continue to maintain it until EoS.  For a version that works with JP consider checking out the [HMB-FFRK-JP-LabMem](https://github.com/bover87/HMB-FFRK-JP-LabMem) fork!

# FFRK-LabMem
Full automation for labyrinth dungeons on Android FFRK and Windows using a proxy server and [adb](https://developer.android.com/studio/command-line/adb)

![App Screenshot](/docs/img/screenshot_01.png?v=3)

Built using Visual Studio 2019 Community, Installer using Inno Setup 6, pre-compiled binaries provided on the [releases page](https://github.com/HughJeffner/FFRK-LabMem/releases)

**Note: button tap locations were calculated as a percentage of a 720 x 1280 screen, this may not work on other screen sizes, more testing needed**

## Compatibility
| Android Version                 | GL FFRK Version | Compatible |
| ------------------------------- | --------------- | ---------- |
| Android (Any)                   | 7.7.0 and lower | Not Supported|
| Android 5 (Lollipop)            | 8.0.0+          | Yes        |
| Android 6 (Marshmallow)         | 8.0.0+          | Yes        |
| Android 7 (Nougat)              | 8.0.0+          | Yes (root) |
| Android 8 (Oreo)                | 8.0.0+          | Yes (root) |
| Android 9 (Nougat)              | 8.0.0+          | Yes (root) |
| Android 10 +                    | 8.0.0+          | No ([maybe?](https://docs.mitmproxy.org/stable/howto-install-system-trusted-ca-android/#instructions-for-api-level--28))|        |

_All compatible versions using FFRK 8.0.0+ must install a certificate_

## (Somewhat) Quick Start
1. Go to the [releases page](https://github.com/HughJeffner/FFRK-LabMem/releases) and find the lastest release
2. Under 'Assets' dropdown download `FFRK-LabMem-x.x.x-Beta-Installer.exe` file and download, run, and follow the steps. (Or you can manually install by downloading and extracting the .zip file instead)
3. Start Emulator / Connect device to USB
4. Turn on 'Developer Mode' in android settings [see here](https://developer.android.com/studio/debug/dev-options)
5. Activate USB debugging in developer settings
6. Start application FFRK-LabMem.exe (it has a treasure-chest icon)
7. Press `C` to open the configuration
8. [Update ADB host](#adbconnection) if using emulator
9. Restart the bot when prompted
10. Go back into the configuration, Under proxy settings, enable Auto-configure
11. Restart the bot when prompted
12. Restart your device/emulator when prompted
13. Follow any on-screen instructions to install the certificate
14. Launch FFRK
15. On the home screen (not title screen) auto-detect screen offsets `Alt+O` ('o' for offsets)
16. Start a lab or enter one in-progress

## Basic Usage
Extract all files from the .zip file to a folder

Double-click `FFRK-LabMem.exe` it and it will run in the window.  At any time as it is running you can press `D` to disable, `E` to enable, `Ctrl+X` to exit, and `H` to minimize to system tray.

Press `C` to open configuration options

## Setup
For this to work correctly, the following must be set up:
1. Network proxy settings
2. ADB connection
3. Install trusted CA certificate
4. Screen top and bottom offsets
5. Team 1 must be able to beat anything, even at 10 fatigue.  Holy mind mage party recommended!

### <a name="proxysetting"></a>Network proxy settings
This varies by device and every network is different.  Typically with android devices you would go into the wifi settings, change proxy to manual then enter the IP address of the windows system running the app for the hostname, 8081 for the proxy port, and the following for the proxy bypass:

`127.0.0.1,lcd-prod.appspot.com,live.chartboost.com,android.clients.google.com,googleapis.com,ssl.sp.mbga-platform.jp,ssl.sp.mbga.jp,app.adjust.io`

> **Tip:** you can press `Ctrl+B` to copy the proxy bypass to the clipboard or use the button in the GUI configuration

> **Tip:** for most android emulators if you can view the device ip address for example `10.0.x.x` you can simply use `10.0.x.2` for the loopback to the host system.

If you are going to use a physical device or an emulator on another system, please make sure to open port 8081 in the firewall to allow incoming connections.  On Windows, it usually prompts you on first run to create the proper firewall rule.

### <a name="adbconnection"></a>Adb connection
This allows the application to interact with the android device. First you'll need to enable developer options in the device settings and enable USB debugging.  There are many tutorials online that cover this.

If you are connecting an acutal device via USB, you may need the proper drivers.  See [here](https://developer.android.com/studio/run/oem-usb) -OR- [here](https://adb.clockworkmod.com/)

Connecting to an emulator works over TCP.  You can set up TCP with a physical device as well but this is beyond scope.  Android emulators seem to use different TCP port numbers, you'll have to look this up.  The default host and port number configured in `FFRK-LabMem.exe.config` is `127.0.0.1:7555` which is for running MuMu app player on the local machine.

**Known Emulator host/ports**
| Emulator  | Host/Port       | Other Possible Ports? |
| --------- | --------------- | --------------------- |
| MuMu      | 127.0.0.1:7555  |                       |
| Nox (5)   | 127.0.0.1:62001 | 62025,62026,62027     |
| MeMu      | 127.0.0.1:21503 | 21513, 21523 (based on instance id)|
| LDPlayer  | 127.0.0.1:5555  | See [here](https://www.ldplayer.net/apps/adb-debugging-on-pc.html) |

### Install trusted CA certificate
If the proxy root CA certificate isn't installed the bot will copy it to the device and switch to the settings screen and offer guidance on installing it.  The root CA certificate is auto-generated on startup in a file called `rootCert.pfx` with a 10-year lifetime (so you only have to install it once).  Addtionally, the .pfx file contains the private key corresponding to the root CA public key contained in the certificate that is installed on the device.

This certificate is only used to decrypt traffic to the `ffrk.denagames.com`, all other traffic is tunneled through the proxy with no inspection.

### Screen offsets
From version 0.9.10 and higher, screen offsets can be automatically detected using `Alt+O` when on FFRK title screen.

## Configuration
### General program options

All of these settings can be accessed by pressing `C`

![Config Screenshot](/docs/img/config_lab_01.png?v=3)

<details>
  <summary>Show Options</summary>
  
| Property                  | Description                                | Default  |
| ------------------------- | ------------------------------------------ | -------- |
| console.timestamps        | Show timestamps in console                 | true     |
| console.debug             | Show general program debugging information | false    |
| adb.path                  | Path to ADB executeable, it's included     | adb.exe  |
| adb.host                  | TCP host to connect to ADB, if using, ignored if connected via USB       | 127.0.0.1:7555 |
| proxy.port                | TCP port to listen for proxy requests      | 8081     |
| proxy.secure              | Enable https proxy (FFRK 8.0.0)            | true     |
| proxy.blocklist           | Path to a file which domains should be blocked.  One domain name per line | |
| proxy.autoconfig          | Configure android device system proxy via Adb | false |
| proxy.connectionPooling   | Experimental feature                        | false |
| lab.configFile            | Lab config file path, see below            | Config/lab.balanced.json |
| lab.watchdogHangMinutes   | Number of minutes to check for a hang, 0 to disable | 3 |
| lab.watchdogCrashSeconds  | Number of seconds to check for a crash, 0 to disable | 30 |  
| screen.topOffset          | Number of pixels of the gray bar at the top of FFRK, 0 for none, -1 to prompt auto-detect | -1 |
| screen.bottomOffset       | Number of pixels of the gray bar at the bottom of FFRK, 0 for none, -1 to prompt auto-detect | -1 |
| updates.checkForUpdates   | Checks the releases page for new versions  | true     |
| updates.includePrerelease | Includes pre-release (testing) versions when checking for new releases | false |
| datalogger.enabled        | When enabled, logs various data to files in the DataLog directory | false |
| counters.dropCategories   | Bit flags for filtering drops | 15 |
| counters.logDropsToTotal  | Set to true to count drops in all-time (may grow large) | false |
</details>

### Lab walking behavior
Four different lab config files are provided: Balanced (default), Farm, Full, and Quick

Configuring the lab walker behavior and all the various options is documented [here](./FFRK-LabMem/Config/readme.md)

### Data Logging
Not enabled by default, set `Enable data logging` in general program options.  This will create the `DataLog` folder with the various csv files.  Data file formats can be found [here](./FFRK-LabMem/Data/readme.md)

### Statistics
![Counters Screenshot](/docs/img/counters_01.png?v=3)

Press `S` to show the statistic counters

## Upgrading

### Automatic upgrade (v4.5+)
If you previously installed using the installer .exe, Press `Alt+U` and confirm the download.  The installer should download (to app directory) and automatically run.
If you upgrade a manual install (extract .zip) the automatic upgrade will install the new version in your `My Documents` folder.

### Manual upgrade
Verson codes are in the format (major.minor.patch)

For a minor or patch release copy the main executeable to your install folder (and .pdb file if you want accurate error stack traces).

For a major release copy all files.  You can optionally keep your configuration files (any missing settings will use defaults and any unused settings will be ignored)

Please keep your `rootCert.pfx` file, or you will have to re-install the certificate on the android device


## Common Issues / FAQ
Check https://github.com/HughJeffner/FFRK-LabMem/wiki/Common-Issues-FAQ
