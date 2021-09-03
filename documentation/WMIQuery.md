# WMIQuerySensor

The WMI query sensor is and advanced sensor that executes a user defined [WMI query](https://docs.microsoft.com/en-us/windows/win32/wmisdk/wmi-and-sql) and exposes the result. It should return a single value.

For example, If you wanted to find the current CPU frequency, the command :

```sql
SELECT * FROM Win32_Processor
```

returns

`|64|9|To Be Filled By O.E.M.|3|Intel64 Family 6 Model 94 Stepping 3|252|1|Win32_Processor|4008|12|64|Intel64 Family 6 Model 94 Stepping 3|CPU0|100|198|1024|8192|0|6|4|GenuineIntel|4008|Intel(R) Core(TM) i7-6700K CPU @ 4.00GHz|4|4|8|To Be Filled By O.E.M.|False|BFEBFBFF000506E3|3|24067|CPU|False|To Be Filled By O.E.M.|U3E1|OK|3|Win32_ComputerSystem|GAME-PC-2016|8|1|False|False|`

This query cannot be used, and instead you should try

```sql
SELECT CurrentClockSpeed FROM Win32_Processor
```

which results in `4008` for my PC. Because this query retuens a single value (CPU frequency in MHz), it can be used with the current WMI query sensor implementation.

You can use [WMI Explorer](https://github.com/vinaypamnani/wmie2/tree/v2.0.0.2) to find see what data is available, or alternatively look at the user submited sensors below:


|Query|Explanation|Submitted by|
|---|---|---|
|`SELECT username FROM Win32_ComputerSystem`|Shows the current user|@grizzlyjere|
|`Select * from Win32_Process Where Name = 'notepad.exe'`|Shows if the defined process is running|@lafferlaffer|
