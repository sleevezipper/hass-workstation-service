using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace UserInterface.ViewModels
{
    public class SensorSettingsViewModel : ViewModelBase
    {
        private ICollection<SensorViewModel> _configuredSensors;

        public ICollection<SensorViewModel> ConfiguredSensors
        {
            get => _configuredSensors;
            set => this.RaiseAndSetIfChanged(ref _configuredSensors, value);
        }

        public void TriggerUpdate()
        {
            this.RaisePropertyChanged();
        }
    }

    public class SensorViewModel : ViewModelBase
    {
        private string _value;

        public Guid Id { get; set; }
        public AvailableSensors Type { get; set; }
        public string Name { get; set; }
        public int UpdateInterval { get; set; }
        public string Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
                this.RaisePropertyChanged(nameof(ValueString));
            }
        }

        public string UnitOfMeasurement { get; set; }

        public string ValueString => string.IsNullOrWhiteSpace(_value) ? string.Empty : $"{_value} {UnitOfMeasurement}";
    }
}