using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class SensorSettingsViewModel : ViewModelBase
    {
        private ICollection<SensorViewModel> configuredSensors;

        public ICollection<SensorViewModel> ConfiguredSensors { get => configuredSensors; set => this.RaiseAndSetIfChanged(ref configuredSensors, value); }
        public void TriggerUpdate()
        {
            this.RaisePropertyChanged();
        }
    }

    public class SensorViewModel : ViewModelBase
    {
        private string _value;

        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int UpdateInterval { get; set; }
        public string Value
        {
            get => _value; set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
                this.RaisePropertyChanged("ValueString");
            }
        }
        public string UnitOfMeasurement { get; set; }

        public string ValueString
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_value))
                {
                    return _value + " " + UnitOfMeasurement;
                }
                else return "";
                
            }
        }
    }
}
