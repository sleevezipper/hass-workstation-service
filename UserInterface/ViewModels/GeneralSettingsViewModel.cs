using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserInterface.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase
    {
        private string namePrefix;

        public string NamePrefix { get => namePrefix; set => this.RaiseAndSetIfChanged(ref namePrefix, value); }

        public void Update(GeneralSettings settings)
        {
            this.NamePrefix = settings.NamePrefix;
        }
    }
}
