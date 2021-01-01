using hass_workstation_service.Communication.InterProcesCommunication.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.ViewModels
{
    public class BackgroundServiceSettingsViewModel : ViewModelBase
    {
        private string message;
        private bool isRunning;
        private bool isAutostartEnabled;

        public bool IsAutoStartEnabled { 
            get => isAutostartEnabled; 
            private set => this.RaiseAndSetIfChanged(ref isAutostartEnabled, value); }
        public bool IsRunning { get => isRunning; private set => this.RaiseAndSetIfChanged(ref isRunning, value); }
        public string Message { get => message; private set => this.RaiseAndSetIfChanged(ref message, value); }

        public void UpdateStatus(bool isRunning, string message)
        {
            this.IsRunning = isRunning;
            this.Message = message;
        }

        public void UpdateAutostartStatus(bool isEnabled)
        {
            this.IsAutoStartEnabled = isEnabled;
        }

    }
}
