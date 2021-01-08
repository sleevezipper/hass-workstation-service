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
        private WebcamDetectionMode selectedDetectionMode;
        private string description;
        private bool showQueryInput;

        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
        public bool ShowQueryInput { get => showQueryInput; set => this.RaiseAndSetIfChanged(ref showQueryInput, value); }
        public bool ShowWindowNameInput { get => showWindowNameInput; set => this.RaiseAndSetIfChanged(ref showWindowNameInput, value); }

        public bool ShowDetectionModeOptions { get => showDetectionModeOptions; set => this.RaiseAndSetIfChanged(ref showDetectionModeOptions, value); }

        private string moreInfoLink;
        private int updateInterval;
        private bool showWindowNameInput;
        private bool showDetectionModeOptions;

        public string MoreInfoLink
        {
            get { return moreInfoLink; }
            set { this.RaiseAndSetIfChanged(ref moreInfoLink, value); }
        }


        public AvailableSensors SelectedType { get => selectedType; set => this.RaiseAndSetIfChanged(ref selectedType, value); }
        public WebcamDetectionMode SelectedDetectionMode { get => selectedDetectionMode; set => this.RaiseAndSetIfChanged(ref selectedDetectionMode, value); }

        public string Name { get; set; }
        public string Query { get; set; }
        public string WindowName { get; set; }
        public int UpdateInterval { get => updateInterval; set => this.RaiseAndSetIfChanged(ref updateInterval, value); }
    }
}
