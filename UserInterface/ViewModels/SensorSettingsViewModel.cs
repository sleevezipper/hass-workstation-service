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
    }

    public class SensorViewModel : ViewModelBase
    {
        private string _value;

        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get => _value; set => this.RaiseAndSetIfChanged(ref _value, value); }
    }
}
