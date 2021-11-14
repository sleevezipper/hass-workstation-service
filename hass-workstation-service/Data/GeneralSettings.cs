using hass_workstation_service.Domain.Sensors;
using System;

namespace hass_workstation_service.Data
{
    public class GeneralSettings
    {
        /// <summary>
        /// If set, all sensor and command names will be be prefixed with this
        /// </summary>
        public string NamePrefix { get; set; }
    }
}