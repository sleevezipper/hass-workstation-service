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

Not convinced yet? Check out [this excellent video](https://youtu.be/D5A7le79R5M) by GeekToolkit on YouTube.

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

Find us on [Discord](https://discord.gg/VraYT2N3wd).

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

Here is a list of the most commonly used sensors with the full documentation [here]():

|command|use|
|---|---|
|ActiveWindow|Exposes the currently selected window|
|WebcamActive|Exposes the microphone state|
|MicrophoneActive|Exposes the webcam state|

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
