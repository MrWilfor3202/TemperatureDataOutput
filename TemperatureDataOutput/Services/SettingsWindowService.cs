using System.Windows;
using TemperatureDataOutput.ViewModel;

namespace TemperatureDataOutput.Services
{
    public class SettingsWindowService : IWindowService<SettingsViewModel>
    {
        public void ShowWindow(SettingsViewModel dataContext)
        {
            SettingsWindow window = new SettingsWindow
            {
                ViewModel = dataContext
            };

            window.Show();
        }
    }
}
