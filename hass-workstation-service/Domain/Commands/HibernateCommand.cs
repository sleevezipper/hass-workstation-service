using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class HibernateCommand : CustomCommand
    {
        public HibernateCommand(MqttPublisher publisher, string name = "Hibernate", Guid id = default(Guid)) : base(publisher, "shutdown /h", name ?? "Hibernate", id)
        {
            this.State = "OFF";
        }
    }
}
