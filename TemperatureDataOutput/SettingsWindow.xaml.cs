using System.Windows;
using TemperatureDataOutput.ViewModel;

namespace TemperatureDataOutput
{
    public partial class SettingsWindow : Window
    {
        public SettingsViewModel ViewModel { get; set; }

        public SettingsWindow()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
