using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;

namespace UserInterface.ViewModels
{
    public class AddSensorViewModel : ViewModelBase
    {
        private AvailableSensors _selectedType;
        private string _name;
        private int _updateInterval;
        private string _description;
        private bool _showQueryInput;
        private bool _showWindowNameInput;
        private string _moreInfoLink;
        private string _query;
        private string _scope;
        private string _windowName;

        public AvailableSensors SelectedType { get => _selectedType; set => this.RaiseAndSetIfChanged(ref _selectedType, value); }
        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public int UpdateInterval { get => _updateInterval; set => this.RaiseAndSetIfChanged(ref _updateInterval, value); }
        public string Description { get => _description; set => this.RaiseAndSetIfChanged(ref _description, value); }
        public bool ShowQueryInput { get => _showQueryInput; set => this.RaiseAndSetIfChanged(ref _showQueryInput, value); }
        public bool ShowWindowNameInput { get => _showWindowNameInput; set => this.RaiseAndSetIfChanged(ref _showWindowNameInput, value); }
        public string MoreInfoLink { get => _moreInfoLink; set => this.RaiseAndSetIfChanged(ref _moreInfoLink, value); }
        public string Query { get => _query; set => this.RaiseAndSetIfChanged(ref _query, value); }
        public string Scope { get => _scope; set => this.RaiseAndSetIfChanged(ref _scope, value); }
        public string WindowName { get => _windowName; set => this.RaiseAndSetIfChanged(ref _windowName, value); }
    }
}