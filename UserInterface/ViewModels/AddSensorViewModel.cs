using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class AddSensorViewModel : ViewModelBase
    {
        private AvailableSensors selectedType;
        private string description;

        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        private string moreInfoLink;

        public string MoreInfoLink
        {
            get { return moreInfoLink; }
            set { this.RaiseAndSetIfChanged(ref moreInfoLink, value); }
        }


        public AvailableSensors SelectedType { get => selectedType; set => this.RaiseAndSetIfChanged(ref selectedType, value); }
        public string Name { get; set; }
    }
}
