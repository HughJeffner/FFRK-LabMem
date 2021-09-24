# FFRK-LabMem
Full automation for labyrinth dungeons on Android FFRK and Windows using a proxy server and [adb](https://developer.android.com/studio/command-line/adb)

Built using Visual Studio 2019 Community, pre-compiled binaries provided on the [releases page](https://github.com/HughJeffner/FFRK-LabMem/releases)

**Note: button tap locations were calculated as a percentage of a 720 x 1280 screen, this may not work on other screen sizes, more testing needed**

## Compatibility
| Android Version                 | GL FFRK Version | Compatible |
| ------------------------------- | --------------- | ---------- |
| Android (Any)                   | 7.7.0 and lower | Not Supported|
| Android 5 (Lollipop)            | 8.0.0           | Yes        |
| Android 6 (Marshmallow)         | 8.0.0           | Yes        |
| Android 7 (Nougat)              | 8.0.0           | Yes (root) |
| Android 8 (Oreo)                | 8.0.0           | Yes (root) |
| Android 9 (Nougat)              | 8.0.0           | Yes (root) |
| Android 10 +                    | 8.0.0           | No ([maybe?](https://docs.mitmproxy.org/stable/howto-install-system-trusted-ca-android/#instructions-for-api-level--28))|        |

_All compatible versions using FFRK 8.0.0+ must install a certificate_

## Quick Start
1. Go to the [releases page](https://github.com/HughJeffner/FFRK-LabMem/releases) and find the lastest release
2. Under 'Assets' dropdown download the first .zip file and extract to directory of your choice
3. Start Emulator / Connect device to USB
4. Turn on 'Developer Mode' in android settings 
5. Activate USB debugging
6. Start application FFRK-LabMem.exe (it has a treasure-chest icon)
7. Follow any on-screen instructions
8. Load up a Labyrinth

## Basic Usage
Extract all files from the .zip file to a folder.  You can skip configuration files if they already exist to keep your settings.

This appliation runs as a command-line application in 'interactive mode'.  This means you can simply double-click it and it will continue to run in the window.  At any time as it is running you can press `D` to disable, `E` to enable, `X` to exit, and `H` to minimize to system tray.

This application uses a split-configuration system: the standard .net config file for [general program options](#configuration) and config.json for [lab-walking related behaviors](./FFRK-LabMem/Config/readme.md).  Eventually I plan to allow specifying the .net config values as command-line parameters (for creating a desktop short-cut for example).

## Setup
For this to work correctly, the following must be set up:
1. Network proxy settings
2. Install trusted certificate (FFRK 8.0.0+)
3. ADB connection
4. Screen top and bottom offsets
5. Team 1 must be able to beat anything, even at 10 fatigue.  Holy mind mage party recommended!

### Network proxy settings
This varies wildly by device and every network is different.  Typically with android devices you would go into the wifi settings, change proxy to manual then enter the IP address of the windows system running the app for the hostname, 8081 for the proxy port, and the following for the proxy bypass:

`127.0.0.1,lcd-prod.appspot.com,live.chartboost.com,android.clients.google.com,googleapis.com,ssl.sp.mbga-platform.jp,ssl.sp.mbga.jp,app.adjust.io`

> **Tip:** you can press `Ctrl+B` to copy the proxy bypass to the clipboard

> **Tip:** for most android emulators if you can view the device ip address for example `10.0.x.x` you can simply use `10.0.x.2` for the loopback to the host system.

If you are going to use a physical device or an emulator on another system, please make sure to open port 8081 in the firewall to allow incoming connections.  On Windows, it usually prompts you on first run to create the proper firewall rule.

### Install trusted CA certificate
If the proxy root CA certificate isn't installed the bot will copy it to the device and switch to the settings screen and offer guidance on installing it.  The root CA certificate is auto-generated on startup in a file called `rootCert.pfx` with a 10-year lifetime (so you only have to install it once).  Addtionally, the .pfx file contains the private key corresponding to the root CA public key contained in the certificate that is installed on the device.

This certificate is only used to decrypt traffic to the `ffrk.denagames.com`, all other traffic is tunneled through the proxy with no inspection.

### Adb connection
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


### Screen offsets
From version 0.9.10 and higher, screen offsets can be automatically detected using `Alt+O`.

To allow different screen sizes, there are `screen.topOffset` and `screen.bottomOffset` in `FFRK-LabMem.exe.config`.  This corresponds to the height in pixels of the gray bars at the top and bottom of your screen that FFRK uses to support different screen sizes.  You can take a screenshot and measure them in a image editor, or just try to guestimate it.  If you have no bars at the top or bottom just leave it the default of 0.

## Configuration
### General program options
| Property                  | Description                                | Default  |
| ------------------------- | ------------------------------------------ | -------- |
| console.timestamps        | Show timestamps in console                 | true     |
| console.debug             | Show general program debugging information | false    |
| adb.path                  | Path to ADB executeable, it's included     | adb.exe  |
| adb.host                  | TCP host to connect to ADB, if using, ignored if connected via USB       | 127.0.0.1:7555 |
| proxy.port                | TCP port to listen for proxy requests      | 8081     |
| proxy.secure              | Enable https proxy (FFRK 8.0.0)            | true     |
| lab.configFile            | Lab config file path, see below            | Config/lab.balanced.json |
| screen.topOffset          | Number of pixels of the gray bar at the top of FFRK, 0 for none, -1 to prompt auto-detect | -1 |
| screen.bottomOffset       | Number of pixels of the gray bar at the bottom of FFRK, 0 for none, -1 to prompt auto-detect | -1 |
| updates.checkForUpdates   | Checks the releases page for new versions  | true     |
| updates.includePrerelease | Includes pre-release (testing) versions when checking for new releases | false |
| datalogger.enabled        | When enabled, logs various data to files in the DataLog directory | false |

### Lab walking behavior
Four different lab config files are provided: Balanced (default), Farm, Full, and Quick

Configuring the lab walker behavior and all the various options is documented [here](./FFRK-LabMem/Config/readme.md)

## Data Logging
Not enabled by default, set `datalogger.enabled` to true in `ffrk-labmem.exe.config`.  This will create the `DataLog` folder with the various csv files.  Data file formats can be found [here](./FFRK-LabMem/Data/readme.md)
