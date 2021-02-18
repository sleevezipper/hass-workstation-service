using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain
{
    public abstract class AbstractDiscoverable
    {
        public abstract string Domain { get; }
        public string Name { get; protected set; }

        public string ObjectId
        {
            get
            {
                return Regex.Replace(this.Name, "[^a-zA-Z0-9_-]", "_");
            }
        }
        public Guid Id { get; protected set; }

        public abstract DiscoveryConfigModel GetAutoDiscoveryConfig();
    }
}
