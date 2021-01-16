using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class AddCommandViewModel : ViewModelBase
    {
        private AvailableCommands selectedType;
        private string description;

        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
        public bool ShowCommandInput { get => showCommandInput; set => this.RaiseAndSetIfChanged(ref showCommandInput, value); }

        private string moreInfoLink;
        private bool showCommandInput;

        public string MoreInfoLink
        {
            get { return moreInfoLink; }
            set { this.RaiseAndSetIfChanged(ref moreInfoLink, value); }
        }

        public AvailableCommands SelectedType { get => selectedType; set => this.RaiseAndSetIfChanged(ref selectedType, value); }

        public string Name { get; set; }
        public string Command { get; set; }
    }
}
