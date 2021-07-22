# FFRK-LabMem
Full automation for labyrinth dungeons on Android FFRK and Windows using a proxy server and [adb](https://developer.android.com/studio/command-line/adb)

Built using Visual Studio, pre-compiled binaries provided on the [releases page](https://github.com/HughJeffner/FFRK-LabMem/releases)

**Note: button tap locations were calculated as a percentage of a 538 x 958 screen, this may not work on other screen sizes, more testing needed**

## Basic Usage
This appliation runs as a command-line application in 'interactive mode'.  This means you can simply double-click it and it will continue to run in the window.  At any time as it is running you can press `D` to disable, `E` to enable, `X` to exit, and `H` to minimize to system tray.

This application uses a split-configuration system: the standard .net config file for general program options and config.json for lab-walking related behaviors.  Eventually I plan to allow specifying the .net config values as command-line parameters (for creating a desktop short-cut for example).

For this to work correctly, the following must be set up:
* Network proxy settings
* ADB connection
* Screen top and bottom offsets
* Team 1 must be able to beat anything (except the boss), even at 10 fatigue.  Holy mind mage party recommended!

### Network proxy settings
This varies wildly by device and every network is different.  Typically with android devices you would go into the wifi settings, change proxy to manual then enter the IP address of the windows system running the app for the hostname, 8081 for the proxy port, and the following for the proxy bypass:

`127.0.0.1,lcd-prod.appspot.com,live.chartboost.com,android.clients.google.com,googleapis.com,ssl.sp.mbga-platform.jp,ssl.sp.mbga.jp,app.adjust.io`

Tip: for most android emulators if you can view the device ip address for example `10.0.x.x` you can simply use `10.0.x.2` for the loopback to the host system.

### Adb connection
This allows the application to interact with the android device. First you'll need to enable developer options in the device settings and enable USB debugging.  There are many tutorials online that cover this.

You can connect via USB for physical devices or TCP using an emulator.  You can set up TCP with a physical device as well but this is beyond scope.  Android emulators seem to use different TCP port numbers, you'll have to look this up.  The default host and port number configured in `FFRK-LabMem.exe.config` is `127.0.0.1:7555` which is for running MuMu app player on the local machine.

### Screen offsets
To allow different screen sizes, there are `screen.topOffset` and `screen.bottomOffset` in `FFRK-LabMem.exe.config`.  This corresponds to the height in pixels of the gray bars at the top and bottom of your screen that FFRK uses to support different screen sizes.  You can take a screenshot and measure them in a image editor, or just try to guestimate it.  If you have no bars at the top or bottom just leave it the default of 0.

## Configuration
Four different lab config files are provided: Balanced (default), Farm, Full, and Quick

Configuring the lab walker behavior and all the various options is documented [here](./FFRK-LabMem/Config/readme.md)