# Sensors

Sensors are used to transfer data about the host system to an automation hub, where it can be processed. `hass-workstation-service` provides many sensors, and they are lisited below in the same order as listed in the configuration GUI.

### ActiveWindowSensor

The active window sensor returns the title of the currently selected window, and is the same value shown when hovering over the icon in the windows task bar. 

This sensor is commonly used to trigger automations when a specific program is in use, such as pausing audio during a skype call. This sensor can be unreliable when used with applications such as web browsers that update their title based on the current context. You can partially resolve this using regular expressions.

### CPULoadSensor

The CPU load sensor is used to determine the current utilization of the CPU, and returns a percentage value. 

### CurrentClockSpeedSensor

The current clock speed sensor returns the base system clock as configured in the bios. **It does not return the current operating frequency of the CPU**

### CurrentVolumeSensor

This sensor returns the current volume of playing audio. **It does not return the master volume.** If you have no sound playing the value will be 0.

### MasterVolumeSensor

This sensor returns the master volume for the currently selected default audio device.

###  DummySensor

This sensor produces a random output every second, and is intended to test latency and connectivity.

### GPULoadSensor

This is the same as the [CPULoadSensor](https://github.com/sleevezipper/hass-workstation-service/new/master/documentation#cpuloadsensor), but for the GPU.

### GPUTemperatureSensor

The GPU temperature returns the current operating temperature of the GPU. This sensor is useful for controling external cooling systems such as air conditioning.

### LastActiveSensor

The last active sensor returns the time when the workstation was last active (last keyboard and mouse input). It is useful as a form of presence detection when combined with motion sensors or software such as room assistant, although may not be reliable if used with auto clickers or other macro software commonly used for video game automation.

### LastBootSensor

The last boot sensor returns the time the windows computer booted. It can be used to calculate uptime, if combined with another sensor to detect system shutdowns.

### MemoryUsageSensor

This returns the amount of system memory used as a percentage value, as indicated by the task manager.

### MicrophoneActiveSensor

This is a binary sensor that can be used to detect if the microphone is in use. **It does not return what process is using it**

### NamedWindowSensor

The named window sensor is similar to the [ActiveWindowSensor](https://github.com/sleevezipper/hass-workstation-service/new/master/documentation#activewindowsensor), however it is a binary sensor that returns true if a window with a title matching a pre determined value is detected.

### SessionStateSensor

The session state sensor can be used to detect if someone is logged in. It has the following values :

|State|Explanation|
|---|---|
|Locked|All user sessions are locked.|
|LoggedOff|No users are logged in.|
|InUse|A user is currently logged in.|
|Unknown|Something went wrong while getting the status.|

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

### WebcamActiveSensor

The webcam active sensor returns the status of the webcam.

### WMIQuerySensor

Please see the specific documentaion page [here](https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/WMIQuery.md#wmiquerysensor).
