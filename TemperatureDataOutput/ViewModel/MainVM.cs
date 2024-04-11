using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using TemperatureDataOutput.Model;
using TemperatureDataOutput.Services;

namespace TemperatureDataOutput.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand UpdateOptionsCommand
        {
            get
            {
                return updateOptionsCommand ?? (updateOptionsCommand = new RelayCommand(UpdateOptions));
            }
        }

        public RelayCommand TakeTemperatureCommand 
        {
            get
            {
                return takeTemperatureCommand ?? (takeTemperatureCommand = new RelayCommand(TakeTemperature)); 
            }
        }

        public RelayCommand OpenSettingsWindowCommand
        {
            get
            {
                return openSettingsWindow ?? (openSettingsWindow = new RelayCommand(OpenSettingsWindow));
            }
        }

        public string Temperature 
        { 
            get 
            {
                return temperature;
            }

            set
            {
                temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public string LastOperationState
        {
            get
            {
                return lastOperationState;
            }

            set
            {
                lastOperationState = value;
                OnPropertyChanged("LastOperationState");
            }
        }

        public IBytePort SelectedBytePort 
        {
            get
            {
                return bytePort;
            }
            set
            {
                bytePort = value;
                OnPropertyChanged("SelectedBytePort");
            }
        }

        public List<string> ComPorts
        {
            get
            {
                return comPorts;
            }
            set
            {
                comPorts = value;
                OnPropertyChanged("ComPorts");
            }
        }

        public string SelectedComPort 
        {
            get 
            {
                return comPort;
            }
            set 
            {
                comPort = value;
                OnPropertyChanged("SelectedComPort");
            }
        }

        public int SelectedBaudRate
        {
            get
            {
                return baudRate;
            }
            set
            {
                baudRate = value;
                OnPropertyChanged("SelectedBaudRate");
            }
        }


        public int SelectedDataBits
        {
            get
            {
                return dataBits;
            }
            set
            {
                dataBits = value;
                OnPropertyChanged("SelectedDataBits");
            }
        }

        public Parity SelectedParity
        {
            get
            {
                return parity;
            }
            set
            {
                parity = value;
                OnPropertyChanged("SelectedParity");
            }
        }

        public StopBits SelectedStop
        {
            get
            {
                return stopBit;
            }
            set
            {
                stopBit = value;
                OnPropertyChanged("SelectedStop");
            }
        }


        public Parity[] Parities
        {
            get
            {
                return parities;
            }
            set
            {
                parities = value;
                OnPropertyChanged("Parities");
            }
        }

        public StopBits[] Stops
        {
            get
            {
                return stopBits;
            }
            set
            {
                stopBits = value;
                OnPropertyChanged("Stops");
            }
        }

        public int BaudRate
        {
            get
            {
                return baudRate;
            }
            set
            {
                baudRate = value;
                OnPropertyChanged("Stops");
            }
        }

        public int DataBits
        {
            get
            {
                return dataBits;
            }
            set
            {
                dataBits = value;
                OnPropertyChanged("Stops");
            }
        }

        private IBytePort bytePort;
        private DataReceiver receiver;
        private string temperature;
        private string lastOperationState;
        private RelayCommand takeTemperatureCommand;
        private RelayCommand updateOptionsCommand;
        private RelayCommand openSettingsWindow;
        private SettingsWindowService windowService;
        private List<string> comPorts = SerialPort.GetPortNames().ToList();
        private Parity[] parities = (Parity[]) Enum.GetValues(typeof(Parity));
        private StopBits[] stopBits = (StopBits[]) Enum.GetValues(typeof(StopBits));
        private int baudRate = 2400;
        private int dataBits = 8;
        private string comPort;
        private Parity parity = Parity.None;
        private StopBits stopBit = StopBits.Two;

        public MainVM() 
        {
            SerialPort serialPort = new SerialPort("COM1", 2400, Parity.None, 8, StopBits.Two);

            ByteCOMPort manager = new ByteCOMPort(serialPort);

            comPort = comPorts[0];
            receiver = new DataReceiver(manager, new TemperatureByteConverter());
            windowService = new SettingsWindowService();
        }
        

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TakeTemperature(object obj)
        {
            Temperature = receiver.GetTemperature();
            LastOperationState = receiver.LastOperationState;
        }

        private void UpdateOptions(object obj)
        {
            SerialPort serialPort = new SerialPort(comPort, baudRate, parity, dataBits, stopBit);

            ByteCOMPort manager = new ByteCOMPort(serialPort);


            receiver = new DataReceiver(manager, new TemperatureByteConverter());
            LastOperationState = receiver.LastOperationState;
        }

        private void OpenSettingsWindow(object obj)
        {
            windowService.ShowWindow(new SettingsViewModel((ByteCOMPort)SelectedBytePort));
        }
    }
}
