using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;

namespace UserInterface.ViewModels
{
    public class AddCommandViewModel : ViewModelBase
    {
        private AvailableCommands _selectedType;
        private string _name;
        private string _description;
        private bool _showCommandInput;
        private bool _showKeyInput;
        private string _moreInfoLink;
        private string _command;
        private string _key;

        public AvailableCommands SelectedType { get => _selectedType; set => this.RaiseAndSetIfChanged(ref _selectedType, value); }
        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public string Description { get => _description; set => this.RaiseAndSetIfChanged(ref _description, value); }
        public bool ShowCommandInput { get => _showCommandInput; set => this.RaiseAndSetIfChanged(ref _showCommandInput, value); }
        public bool ShowKeyInput { get => _showKeyInput; set => this.RaiseAndSetIfChanged(ref _showKeyInput, value); }
        public string MoreInfoLink { get => _moreInfoLink; set => this.RaiseAndSetIfChanged(ref _moreInfoLink, value); }
        public string Command { get => _command; set => this.RaiseAndSetIfChanged(ref _command, value); }
        public string Key { get => _key; set => this.RaiseAndSetIfChanged(ref _key, value); }
    }
}