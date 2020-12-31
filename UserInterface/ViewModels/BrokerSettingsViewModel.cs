using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class BrokerSettingsViewModel : ViewModelBase
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
