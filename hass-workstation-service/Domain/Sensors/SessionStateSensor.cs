using hass_workstation_service.Communication;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Sensors
{
    enum PCUserStatuses
    {
        /// <summary>
        /// all users are locked
        /// </summary>
        Locked,
        /// <summary>
        /// No users are logged in
        /// </summary>
        LoggedOff,
        /// <summary>
        /// A user is using this computer
        /// </summary>
        InUse,
        /// <summary>
        /// unable to connect to computer / other error
        /// </summary>
        Unknown
    }

    public class SessionStateSensor : AbstractSensor
    {
        public SessionStateSensor(MqttPublisher publisher, int? updateInterval = null, string name = "SessionState", Guid id = default(Guid)) : base(publisher, name ?? "SessionState", updateInterval ?? 10, id) { }
        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:lock",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        public override string GetState()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetPCUserStatus().ToString();
            }
            else return "unsupported";
        }


        [SupportedOSPlatform("windows")]
        PCUserStatuses GetPCUserStatus()
        {
            try
            {

                var scope = new ManagementScope();
                scope.Connect();
                Log.Logger.Information("Connected to Scope");
                var explorerProcesses = Process.GetProcessesByName("explorer")
                                            .Select(p => p.Id.ToString())
                                            .ToHashSet();

                Log.Logger.Information("Created explorerprocess list");
                var REprocessid = new Regex(@"(?<=Handle="").*?(?="")", RegexOptions.Compiled);

                var numberOfLogonSessionsWithExplorer = new ManagementObjectSearcher(scope, new SelectQuery("SELECT * FROM Win32_SessionProcess")).Get()
                                                            .Cast<ManagementObject>()
                                                            .Where(mo => explorerProcesses.Contains(REprocessid.Match(mo["Dependent"].ToString()).Value))
                                                            .Select(mo => mo["Antecedent"].ToString())
                                                            .Distinct()
                                                            .Count();


                Log.Logger.Information("Created numberOfLogonSessionsWithExplorer");

                var numberOfUserDesktops = new ManagementObjectSearcher(scope, new SelectQuery("select * from win32_Perfrawdata_TermService_TerminalServicesSession")).Get().Count - 1; // don't count Service desktop

                Log.Logger.Information("Created numberOfUserDesktops");

                var numberOflogonUIProcesses = Process.GetProcessesByName("LogonUI").Length;

                Log.Logger.Information("Created numberOflogonUIProcesses");

                if (numberOflogonUIProcesses >= numberOfUserDesktops)
                {
                    if (numberOfLogonSessionsWithExplorer > 0)
                        return PCUserStatuses.Locked;
                    else
                        return PCUserStatuses.LoggedOff;
                }
                else
                    return PCUserStatuses.InUse;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Exception in SessionStateSensor");
                return PCUserStatuses.Unknown;
            }
        }
    }
}
