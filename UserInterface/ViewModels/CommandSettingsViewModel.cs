using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace UserInterface.ViewModels
{
    public class CommandSettingsViewModel : ViewModelBase
    {
        private ICollection<CommandViewModel> _configuredCommands;

        public ICollection<CommandViewModel> ConfiguredCommands
        {
            get => _configuredCommands;
            set => this.RaiseAndSetIfChanged(ref _configuredCommands, value);
        }

        public void TriggerUpdate()
        {
            this.RaisePropertyChanged();
        }
    }

    public class CommandViewModel : ViewModelBase
    {
        public Guid Id { get; set; }
        public AvailableCommands Type { get; set; }
        public string Name { get; set; }
    }
}