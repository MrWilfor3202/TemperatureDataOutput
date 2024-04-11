using System.Collections.Generic;
using System.IO.Ports;

namespace TemperatureDataOutput.Model
{
    public class ByteCOMPort : IBytePort
    {
        public Dictionary<string, object> Settings { get => settings; }

        private readonly SerialPort serialPort;
        private readonly Dictionary<string, object> settings;

        public ByteCOMPort(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            settings = new Dictionary<string, object>();
        }

        public void OpenPort() => serialPort.Open();

        public byte[] ReadBytesFromPort(int count)
        {
            byte[] response = new byte[count];

            for (int i = 0; i < count; i++)
                response[i] = (byte)serialPort.ReadByte();

            return response;
        }

        public void WriteBytesToPort(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                serialPort.Write(buffer, i, 1);
        }

        public void ClosePort() => serialPort.Close();

        public byte ReadByteFromPort()
        {
            return (byte) serialPort.ReadByte();
        }

        public void WriteByteToPort(byte data)
        {
            byte[] buffer = new byte[] { data };
            
            serialPort.Write(buffer, 0, 1);
        }
    }
}
