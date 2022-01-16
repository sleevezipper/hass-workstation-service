# WMIQuerySensor

The WMI query sensor is an advanced sensor that executes a user defined [WMI query](https://docs.microsoft.com/en-us/windows/win32/wmisdk/wmi-and-sql) and exposes the result. 

To use the WMI query sensor, you should create a WMI query and paste it in the box. For example, If you wanted to find the current CPU frequency you can use this command:

```sql
SELECT CurrentClockSpeed FROM Win32_Processor
```
which results in `4008` for my PC. Because this query returns a single value (CPU frequency in MHz), it can be used with the current WMI query sensor implementation.

The command ```sql SELECT * FROM Win32_Processor``` cannot be used because it returns `|64|9|To Be Filled By O.E.M.|3| ... |GAME-PC-2016|8|1|False|False|`, and the current WMI query implementation only supports commands that return a single value.


You can use [WMI Explorer](https://github.com/vinaypamnani/wmie2/releases) to construct a query, or alternatively look at the user submitted sensors below
If a class or value cannot be found in the default scope, you can use the "Scope" setting when adding or editing the sensor. 
---

## User Submitted Sensor Examples

### User Information
| Data | Query | Example Response | Info | Contributor | 
| --- | --- | --- | :---: | --- |
| Current User | `SELECT UserName FROM Win32_ComputerSystem` | `Admin` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @grizzlyjere

### Network Information
| Data | Query | Example Response | Info | Contributor | 
| --- | --- | --- | :---: | --- |
| Get IPv4 Address | `SELECT IPv4Address FROM MSFT_NetIPAddress WHERE AddressFamily=2 AND AddressState=4 AND (PrefixOrigin=1 OR PrefixOrigin=3 OR PrefixOrigin=4)` | `192.168.1.101` | [:link:](https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/hh872425\(v=vs.85\)) | @deftnerd |
| Link Speed | `SELECT Speed FROM MSFT_NetIPAddress WHERE AddressFamily=2 AND AddressState=4 AND (PrefixOrigin=1 OR PrefixOrigin=3 OR PrefixOrigin=4)` | `780000000` (In bps. Divide by 1,000,000 to get Mbps) | [:link:](https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/hh872425\(v=vs.85\)) | @deftnerd |
| Manufacturer | `SELECT DNSHostName FROM Win32_ComputerSystem` | `mycomputer` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |

### Hardware Information
| Data | Query | Example Response | Info | Contributor | 
| --- | --- | --- | :---: | --- |
| Manufacturer | `SELECT Manufacturer FROM Win32_ComputerSystem` | `Dell Inc.` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Model | `SELECT Model FROM Win32_ComputerSystem` | `OptiPlex 7010` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| System is a VM | `SELECT HypervisorPresent FROM Win32_ComputerSystem` | `0` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Physical Processors | `SELECT NumberOfProcessors FROM Win32_ComputerSystem` | `1` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Logical Processors | `SELECT NumberOfLogicalProcessors FROM Win32_ComputerSystem` | `8` (Processors * Cores * Threads) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| System Type | `SELECT PCSystemType FROM Win32_ComputerSystem` | `1` (0=Unspecified, 1=Desktop, 2=Laptop, 3=Workstation, etc) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Processor Architecture | `SELECT SystemType FROM Win32_ComputerSystem` | `64-bit Intel PC` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Free Disk Space | `SELECT FreeSpace FROM Win32_LogicalDisk WHERE DeviceID='C:'` | `299329323008` (bytes) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-logicaldisk) | @sleevezipper |

### Power & Thermal Information
| Data | Query | Example Response | Info | Contributor | 
| --- | --- | --- | :---: | --- |
| Capable of Power Management | `SELECT PowerManagementSupported FROM Win32_ComputerSystem` | `1` (0=Unsupported, 1=Supported) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Power Management State | `SELECT PowerState FROM Win32_ComputerSystem` | `1` (0=Unknown, 1=Full Power, 2=Low Power Mode, 3=Standby, etc) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Power Supply State | `SELECT PowerSupplyState FROM Win32_ComputerSystem` | `3` (1=Other, 2=Unknown, 3=Safe, 4=Warning, 5=Critical, etc) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Enclosure Thermal State | `SELECT ThermalState FROM Win32_ComputerSystem` | `3` (1=Other, 2=Unknown, 3=Safe, 4=Warning, 5=Critical, etc) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-computersystem) | @deftnerd |
| Battery Status | `SELECT BatteryStatus FROM Win32_Battery` | `2` (1=Discharging, 2=On AC, 3=Fully Charged, 4=Low, 5=Critical, 6=Charging, etc) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery)  | @deftnerd |
| % Charge Remaining | `SELECT EstimatedChargeRemaining FROM Win32_Battery` | `80` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery) | @deftnerd |
| Remaining Battery Minutes | `SELECT EstimatedRunTime FROM Win32_Battery` | `142` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery) | @deftnerd |
| Max Minutes of Battery | `SELECT ExpectedLife FROM Win32_Battery` | `230` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery) | @deftnerd |
| Minutes to Full Charge | `SELECT TimeToFullCharge FROM Win32_Battery` | `14` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery) | @deftnerd |
| Max Minutes to Recharge | `SELECT MaxRechargeTime FROM Win32_Battery` | `65` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery) | @deftnerd |
| Time on Battery in Seconds | `SELECT TimeOnBattery FROM Win32_Battery` | `3640` (0=plugged in) | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-battery) | @deftnerd |

### Windows System Assessment Tool Results
| Data | Query | Example Response | Info | Contributor | 
| --- | --- | --- | :---: | --- |
| Overall score | `SELECT CPUScore FROM Win32_WinSAT` | `5.8` | [:link:](https://docs.microsoft.com/en-us/windows/win32/winsat/win32-winsat) | @deftnerd |
| CPU Score | `SELECT CPUScore FROM Win32_WinSAT` | `9.1` | [:link:](https://docs.microsoft.com/en-us/windows/win32/winsat/win32-winsat) | @deftnerd |
| Disk Score | `SELECT DiskScore FROM Win32_WinSAT` | `7.75` | [:link:](https://docs.microsoft.com/en-us/windows/win32/winsat/win32-winsat) | @deftnerd |
| Graphics Score | `SELECT GraphicsScore FROM Win32_WinSAT` | `5.8` | [:link:](https://docs.microsoft.com/en-us/windows/win32/winsat/win32-winsat) | @deftnerd |
| Memory Score | `SELECT MemoryScore FROM Win32_WinSAT` | `9.1` | [:link:](https://docs.microsoft.com/en-us/windows/win32/winsat/win32-winsat) | @deftnerd |

### Software Information
| Data | Query | Example Response | Info | Contributor | 
| --- | --- | --- | :---: | --- |
| AntiSpyware Product | `SELECT displayName FROM AntiSpywareProduct` | `Windows Defender` | | @deftnerd |
| AntiVirus Product | `SELECT displayName FROM AntiVirusProduct` | `Windows Defender` | | @deftnerd |

### Process Information
| Data | Query | Example Response | Info | Contributor | 
| --- | --- | --- | :---: | --- |
| Is program _x_ running? | `SELECT ProcessId FROM Win32_Process WHERE Name='notepad.exe'` | `1842` | [:link:](https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-process) | @lafferlaffer |

