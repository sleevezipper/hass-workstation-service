# HASS Workstation Service

This goal of this project is to provide useful sensors and services from your workstation to [Home Assistant](https://www.home-assistant.io/) through MQTT. It accomplishes this goal by:

- Running in the background as a service
- Being lightweight so you'll never notice it
- Using well defined standards
- Being local when you want it to, only communicating through your own MQTT broker
- Being easy to configure

It will try to futher accomplish this goal in the future by:

- Being platform independent
- Using secure communication

## Screenshots

![The settings screen](https://i.imgur.com/KXKQqMr.png)

![The resulting sensors in Home Assistant](https://i.imgur.com/1Yvx2Ea.png)

## Installation

You can get the installer from [here](https://hassworkstationstorage.z6.web.core.windows.net/publish/setup.exe). When using the installer, the application checks for updates on startup. This is the recommended way to install for most users.
Note: You'll get a Windows Smartscreen warning because the code was self signed. You can click "More info" and then "Run anyway" to proceed with installing.

### Standalone

If you don't want to use the installer, you can find the standalone version releases on GitHub [here](https://github.com/sleevezipper/hass-workstation-service/releases). Unpack all files to a folder and run `hass-workstation-service.exe`. This is the background service and you can use `UserInterface.exe` to configure the service. There is no automatic (or prompted) updating in the standalone version.

### Updating

If you used the installer, the app checks for updates on startup. If an update is available you will be prompted to install. If you use the standalone, just delete all files from the previous install and unpack the zip to the same location as before.

## Sensors

The application provides several sensors. Sensors can be configured with a name and this name will be used in the MQTT topic like this: `homeassistant/sensor/{Name}/state`. Sensors will expose themselves through [MQTT discovery](https://www.home-assistant.io/docs/mqtt/discovery/) and will automatically appear in Home assistant or any other platform that supports this type of configuration.

Sensors publish their state on their own interval which you can configure and only publish when the state changes.

### UserNotificationState

This sensor watches the UserNotificationState. This is normally used in applications to determine if it is appropriate to send a notification but we can use it to expose this state. Notice that this status does not watch Focus Assist. It has the following possible states:

|State|Explanation|
|---|---|
|NotPresent|A screen saver is displayed, the machine is locked, or a nonactive Fast User Switching session is in progress.   |
|Busy|A full-screen application is running or Presentation Settings are applied. Presentation Settings allow a user to put their machine into a state fit for an uninterrupted presentation, such as a set of PowerPoint slides, with a single click.|
|RunningDirect3dFullScreen|A full-screen (exclusive mode) Direct3D application is running.|
|PresentationMode|The user has activated Windows presentation settings to block notifications and pop-up messages.|
|AcceptsNotifications|None of the other states are found, notifications can be freely sent.|
|QuietTime|Introduced in Windows 7. The current user is in "quiet time", which is the first hour after a new user logs into his or her account for the first time. During this time, most notifications should not be sent or shown. This lets a user become accustomed to a new computer system without those distractions. Quiet time also occurs for each user after an operating system upgrade or clean installation.|
|RunningWindowsStoreApp|A Windows Store app is running.|

### ActiveWindow

This sensor exposes the name of the currently focused window.

### WebcamActive

This sensor shows if the webcam is currently being used. It has two detection modes:

- Registry - this is the preferred method. This will work from Windows 10 version 1903 and higher.
- OpenCV - this method tries to access the webcam and if that fails, it assumes it is currently in use. This will flash the webcam activity light at every update interval. It also uses more CPU cycles and memory.

### MicrophoneActive

This sensor shows if the microphone is currently being used. It uses the Windows registry to check and wil work from Windows 10 version 1903 and higher.

### CPULoad

This sensor checks the current CPU load. It averages the load on all logical cores every second and rounds the output to two decimals.

### UsedMemory

This sensor calculates the percentage of used memory.

### CurrentClockSpeed

This sensor returns the BIOS configured baseclock for the processor.

### WMIQuery

This advanced sensor executes a user defined [WMI query](https://docs.microsoft.com/en-us/windows/win32/wmisdk/wmi-and-sql) and exposes the result. The query should return a single value.

For example:

```sql
SELECT * FROM Win32_Processor
```

returns

`|64|9|To Be Filled By O.E.M.|3|Intel64 Family 6 Model 94 Stepping 3|252|1|Win32_Processor|4008|12|64|Intel64 Family 6 Model 94 Stepping 3|CPU0|100|198|1024|8192|0|6|4|GenuineIntel|4008|Intel(R) Core(TM) i7-6700K CPU @ 4.00GHz|4|4|8|To Be Filled By O.E.M.|False|BFEBFBFF000506E3|3|24067|CPU|False|To Be Filled By O.E.M.|U3E1|OK|3|Win32_ComputerSystem|GAME-PC-2016|8|1|False|False|`

This cannot not be used for this sensor. Instead try

```sql
SELECT CurrentClockSpeed FROM Win32_Processor
```

which results in `4008` for my PC.

You can use [WMI Explorer](https://github.com/vinaypamnani/wmie2/tree/v2.0.0.2) to find see what data is available.

### IdleTime

This sensor returns the amount of seconds the workstation has been idle for. It starts counting the moment you stop typing or moving your mouse.

### Dummy

This sensor spits out a random number every second. Useful for testing, maybe you'll find some other use for it.
