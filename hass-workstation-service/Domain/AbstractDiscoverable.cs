using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain
{
    public abstract class AbstractDiscoverable
    {
        public abstract string Domain { get; }
    }
}
