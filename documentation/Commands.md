# Commands

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
|shutdown /s /f /t 000|Forcefully shutdown the PC immediately.|
|Rundll32.exe user32.dll,LockWorkStation|This locks the current session.|
|shutdown /s /t 300|Shuts the PC down after 5 minutes (300 seconds).|
|C:\path\to\your\batchfile.bat|Run the specified batch file.|

### KeyCommand

Sends a keystroke with the specified key. You can pick [any of these](https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes) key codes.

### Media Commands

There's several media commands available that allow you to control media playback. You can combine them into a media player entity as shown [here](https://pastebin.com/1VdL5iQm).

|Command|use|
|---|---|
|Play/Pause|The same as pressing the play/pause media key|
|Next|skip to next track|
|Previous|skip to previous track|
|Volume up|Increase system master volume|
|Volume down|Decrease system master volume|
|Mute (toggle)|Mute the system|
