using System.Collections.Generic;
using TemperatureDataOutput.Model;

namespace TemperatureDataOutput.ViewModel
{
    public class SettingsViewModel 
    {
        public Dictionary<string, object> Settings { get; set; }
        

        public SettingsViewModel(ByteCOMPort bytePort)
        {
            Settings = bytePort.Settings;
        }
    }
}
