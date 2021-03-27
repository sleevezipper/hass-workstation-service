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
        private bool _showDetectionModeOptions;
        private string _moreInfoLink;

        public AvailableSensors SelectedType { get => _selectedType; set => this.RaiseAndSetIfChanged(ref _selectedType, value); }
        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public int UpdateInterval { get => _updateInterval; set => this.RaiseAndSetIfChanged(ref _updateInterval, value); }
        public string Description { get => _description; set => this.RaiseAndSetIfChanged(ref _description, value); }
        public bool ShowQueryInput { get => _showQueryInput; set => this.RaiseAndSetIfChanged(ref _showQueryInput, value); }
        public bool ShowWindowNameInput { get => _showWindowNameInput; set => this.RaiseAndSetIfChanged(ref _showWindowNameInput, value); }
        public bool ShowDetectionModeOptions { get => _showDetectionModeOptions; set => this.RaiseAndSetIfChanged(ref _showDetectionModeOptions, value); }
        public string MoreInfoLink { get => _moreInfoLink; set => this.RaiseAndSetIfChanged(ref _moreInfoLink, value); }
        public string Query { get; set; }
        public string WindowName { get; set; }
    }
}