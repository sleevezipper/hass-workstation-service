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

## Installation

You can get the installer from [here](https://hassworkstationstorage.z6.web.core.windows.net/publish/setup.exe). When using the installer, the application checks for updates on startup.
Note: You'll get a Windows Smartscreen warning because the code was self signed. You can click "More info" and then "Run anyway" to proceed with installing.

Alternatively, you can find releases on GitHub [here](https://github.com/sleevezipper/hass-workstation-service/releases).

## Sensors

The application provides several sensors.

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

### Dummy

This sensor spits out a random number every second. Useful for testing, maybe you'll find some other use for it.
