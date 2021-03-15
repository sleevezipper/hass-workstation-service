using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        private string serviceVersion;

       
        public string ServiceVersion { get => "Service version: " + serviceVersion; private set => this.RaiseAndSetIfChanged(ref serviceVersion, value); }

        public void UpdateServiceVersion(string version)
        {
            this.ServiceVersion = version;
        }

    }
}
