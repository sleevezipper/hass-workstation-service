using System;
using System.Collections.Generic;
using hass_workstation_service.Domain.Sensors;

namespace hass_workstation_service.Domain
{
    public abstract class Device
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Manufacturer { get; private set; }
        public string Model { get; private set; }
        public string Version { get; private set; }

        public ICollection<AbstractSensor> Sensors { get; set; }

    }
}