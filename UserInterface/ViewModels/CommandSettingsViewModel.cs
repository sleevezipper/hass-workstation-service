using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class CommandSettingsViewModel : ViewModelBase
    {
        private ICollection<CommandViewModel> configuredCommands;

        public ICollection<CommandViewModel> ConfiguredCommands { get => configuredCommands; set => this.RaiseAndSetIfChanged(ref configuredCommands, value); }
        public void TriggerUpdate()
        {
            this.RaisePropertyChanged();
        }
    }

    public class CommandViewModel : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
