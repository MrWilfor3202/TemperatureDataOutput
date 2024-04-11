using System.Windows;

namespace TemperatureDataOutput.Services
{
    public interface IWindowService<VM> 
    {
        void ShowWindow(VM dataContext);
    }
}
