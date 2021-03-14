# HASS Workstation Service

This goal of this project is to provide useful sensors and services from your workstation to [Home Assistant](https://www.home-assistant.io/) through MQTT. It accomplishes this goal by:

- Running in the background as a service
- Being lightweight so you'll never notice it
- Using well defined standards
- Being local when you want it to, only communicating through your own MQTT broker
- Being easy to configure
- Using secure communication

It will try to futher accomplish this goal in the future by:

- Being platform independent

## Screenshots

![The settings screen](https://i.imgur.com/RBQx807.png)

![The resulting sensors and commands in Home Assistant](https://i.imgur.com/jXRU2cu.png)

## Installation

You can get the installer from [here](https://hassworkstationstorage.z6.web.core.windows.net/publish/setup.exe). When using the installer, the application checks for updates on startup. This is the recommended way to install for most users.
Note: You'll get a Windows Smartscreen warning because the code was self signed. You can click "More info" and then "Run anyway" to proceed with installing.

### Standalone

If you don't want to use the installer, standalone is what you need. Make sure to install [.NET 5 Runtime](https://dotnet.microsoft.com/download/dotnet/current/runtime) first. Find the standalone version releases on GitHub [here](https://github.com/sleevezipper/hass-workstation-service/releases). Unpack all files to a folder and run `hass-workstation-service.exe`. This is the background service and you can use `UserInterface.exe` to configure the service. There is no automatic (or prompted) updating in the standalone version.

### Getting Started

As a prerequisite, make sure you have an MQTT username and password available. Using Home Assistant in combination with the Mosquitto broker add-on and integration? You can both use a Home Assistant account and a local account. From a security perspective, we recommend a local account as this only provides access to the MQTT Broker and not to your Home Assistant instance.

Now that you are all set, make sure to run the `hass-workstation-service.exe` executable first. This executable is responsible for setting up the sensors and talking with your MQTT Broker. To configure the service, start the `UserInterface.exe` executable.
Add your `hostname` or `IP address`, `port`, `username` and `password` and click on Save. In case you use the Mosquitto add-in, provide port `8883` and check `Use TLS`. The application will mention "All good" when configured correctly.

### Updating

If you used the installer, the app checks for updates on startup. If an update is available you will be prompted to install. If you use the standalone, just delete all files from the previous install and unpack the zip to the same location as before.

## Need help?

Find us on us on [Discord](https://discord.gg/VraYT2N3wd).

## Development

Want to develop or build the application yourself? Make sure to install the .NET Runtime [.NET 5 Runtime](https://dotnet.microsoft.com/download/dotnet/current/runtime) and [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/current). Run the following commands from the `hass-workstation-service\hass-workstation-service` directory to get you started:

```` powershell
dotnet build
dotnet publish
````

In case you are using Visual Studio Code, open the `hass-workstation-service\hass-workstation-service` folder to take advantage of the predefined build and publish tasks.

## Sensors

The application provides several sensors. Sensors can be configured with a name and this name will be used in the MQTT topic like this: `homeassistant/sensor/{DeviceName}/{Name}/state`. Sensors will expose themselves through [MQTT discovery](https://www.home-assistant.io/docs/mqtt/discovery/) and will automatically appear in Home assistant or any other platform that supports this type of configuration.

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

This sensor shows if the webcam is currently being used. It uses the Windows registry to check will work from Windows 10 version 1903 and higher.

### MicrophoneActive

This sensor shows if the microphone is currently being used. It uses the Windows registry to check and will work from Windows 10 version 1903 and higher.

### CPULoad

This sensor checks the current CPU load. It averages the load on all logical cores every second and rounds the output to two decimals.

### GPULoad

This sensor returns the current GPU load. This should work for both NVidia and AMD GPU's.

### GPUTemperature

This sensor returns the current temperature of the GPU in °C. This should work for both NVidia and AMD GPU's.

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

Here's some queries from other users:

|Query|Explanation|Thanks|
|---|---|---|
|`SELECT username FROM Win32_ComputerSystem`|Shows the current user|@grizzlyjere|

Want to add you query here? Please create a pull request or open an issue.

### LastActive

This sensor returns the date/time that the workstation was last active. Typing or moving your mouse will reset the date/time.

### LastBoot

This sensor returns the date/time that Windows was last booted.

### SessionState

This sensor returns the current session state. It has the following possible states:

|State|Explanation|
|---|---|
|Locked|All user sessions are locked.|
|LoggedOff|No users are logged in.|
|InUse|A user is currently logged in.|
|Unknown|Something went wrong while getting the status.|

### CurrentVolume

This sensor returns the volume of the currently playing audio. So if you're listening to music and you pause, this sensor will return 0 (or at least a very low value).

|State|Explanation|
|---|---|
|Locked|All user sessions are locked.|
|LoggedOff|No users are logged in.|
|InUse|A user is currently logged in.|
|Unknown|Something went wrong while getting the status.|

### Dummy

This sensor spits out a random number every second. Useful for testing, maybe you'll find some other use for it.

## Commands

Commands can be used to trigger certain things on the client. For each command, a switch will be available in Home Assistant. Turning on the switch fires the command on the client and it will turn the switch off when it's done. Turning it off will cancel the running command.

### ShutdownCommand

This command shuts down the computer immediately. It runs `shutdown /s`.

### RestartCommand

This command restarts the computer immediately. It runs `shutdown /r`.

### LogOffCommand

This command logs off the current user. It runs `shutdown /l`.

### CustomCommand

This command allows you to run any Windows Commands. The command will be run in a hidden Command Prompt. Some examples:

|Command|Explanation|
|---|---|
|Rundll32.exe user32.dll,LockWorkStation|This locks the current session.|
|shutdown /s /t 300|Shuts the PC down after 5 minutes (300 seconds).|
|C:\path\to\your\batchfile.bat|Run the specified batch file.|

### KeyCommand

Sends a keystroke with the specified key. You can pick [any of these](https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes) key codes.

### Media Commands

There's several media commands available which are very self exlanatory.

- Play/Pause
- Next
- Previous
- Volume up
- Volume down
- Mute (toggle)

## Credits

This project depends on work done by others and they should at least get a mention. Please note that this list is not complete yet.

### [CoreAudio](https://github.com/morphx666/CoreAudio)

CoreAudio was used to check the current volume of playing audio.

### [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)

We use this for our GPU sensors.
