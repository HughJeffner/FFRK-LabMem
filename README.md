# FFRK-LabMem
Full automation for labyrinth dungeons on Android FFRK and Windows using a proxy server and [adb](https://developer.android.com/studio/command-line/adb)

Built using Visual Studio, pre-compiled binaries provided on the [releases page](https://github.com/HughJeffner/FFRK-LabMem/releases), download the .zip file under 'Assets'

**Note: button tap locations were calculated as a percentage of a 720 x 1280 screen, this may not work on other screen sizes, more testing needed**

## Basic Usage
Extract all files from the .zip file to a folder.  You can skip configuration files if they already exist to keep your settings.

This appliation runs as a command-line application in 'interactive mode'.  This means you can simply double-click it and it will continue to run in the window.  At any time as it is running you can press `D` to disable, `E` to enable, `X` to exit, and `H` to minimize to system tray.

This application uses a split-configuration system: the standard .net config file for general program options and config.json for lab-walking related behaviors.  Eventually I plan to allow specifying the .net config values as command-line parameters (for creating a desktop short-cut for example).

For this to work correctly, the following must be set up:
1. Network proxy settings
1. ADB connection
1. Screen top and bottom offsets
1. Team 1 must be able to beat anything (except the boss), even at 10 fatigue.  Holy mind mage party recommended!

### Network proxy settings
This varies wildly by device and every network is different.  Typically with android devices you would go into the wifi settings, change proxy to manual then enter the IP address of the windows system running the app for the hostname, 8081 for the proxy port, and the following for the proxy bypass:

`127.0.0.1,lcd-prod.appspot.com,live.chartboost.com,android.clients.google.com,googleapis.com,ssl.sp.mbga-platform.jp,ssl.sp.mbga.jp,app.adjust.io`

Tip: for most android emulators if you can view the device ip address for example `10.0.x.x` you can simply use `10.0.x.2` for the loopback to the host system.

If you are going to use a physical device or an emulator on another system, please make sure to open port 8081 in the firewall to allow incoming connections.  On Windows, it usually prompts you on first run to create the proper firewall rule.

### Adb connection
This allows the application to interact with the android device. First you'll need to enable developer options in the device settings and enable USB debugging.  There are many tutorials online that cover this.

If you are connecting an acutal device via USB, you may need the proper drivers.  See [here](https://developer.android.com/studio/run/oem-usb) -OR- [here](https://adb.clockworkmod.com/)

Connecting to an emulator works over TCP.  You can set up TCP with a physical device as well but this is beyond scope.  Android emulators seem to use different TCP port numbers, you'll have to look this up.  The default host and port number configured in `FFRK-LabMem.exe.config` is `127.0.0.1:7555` which is for running MuMu app player on the local machine.

**Known Emulator host/ports**
| Emulator  | Host/Port       | Other Possible Ports? |
| --------- | --------------- | --------------------- |
| MuMu      | 127.0.0.1:7555  |                       |
| Nox (5)   | 127.0.0.1:62001 | 62025,62026,62027     |
| MeMu      | 127.0.0.1:21503 |                       |
| LDPlayer  | 127.0.0.1:5555  | See [here](https://www.ldplayer.net/apps/adb-debugging-on-pc.html) |


### Screen offsets
To allow different screen sizes, there are `screen.topOffset` and `screen.bottomOffset` in `FFRK-LabMem.exe.config`.  This corresponds to the height in pixels of the gray bars at the top and bottom of your screen that FFRK uses to support different screen sizes.  You can take a screenshot and measure them in a image editor, or just try to guestimate it.  If you have no bars at the top or bottom just leave it the default of 0.

## Configuration
Four different lab config files are provided: Balanced (default), Farm, Full, and Quick

Configuring the lab walker behavior and all the various options is documented [here](./FFRK-LabMem/Config/readme.md)
