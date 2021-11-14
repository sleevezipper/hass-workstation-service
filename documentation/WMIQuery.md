# WMIQuerySensor

The WMI query sensor is an advanced sensor that executes a user defined [WMI query](https://docs.microsoft.com/en-us/windows/win32/wmisdk/wmi-and-sql) and exposes the result. 

To use the WMI query sensor, you should create a WMI query and paste it in the box. For example, If you wanted to find the current CPU frequency you can use this command:

```sql
SELECT CurrentClockSpeed FROM Win32_Processor
```
which results in `4008` for my PC. Because this query retuens a single value (CPU frequency in MHz), it can be used with the current WMI query sensor implementation.

The command ```sql SELECT * FROM Win32_Processor``` cannot be used because it returns `|64|9|To Be Filled By O.E.M.|3| ... |GAME-PC-2016|8|1|False|False|`, and the current WMI query implementation only supports commands that return a single value.


You can use [WMI Explorer](https://github.com/vinaypamnani/wmie2/tree/v2.0.0.2) to construct a query, or alternatively look at the user submited sensors below:


|Query|Explanation|Submitted by|
|---|---|---|
|`SELECT username FROM Win32_ComputerSystem`|Shows the current user|@grizzlyjere|
|`Select * from Win32_Process Where Name = 'notepad.exe'`|Shows if the defined process is running|@lafferlaffer|
