using System.Collections.Generic;

namespace TemperatureDataOutput.Model
{
    public interface IBytePort
    {
        Dictionary<string, object> Settings { get; }

        void OpenPort();

        byte[] ReadBytesFromPort(int count);

        byte ReadByteFromPort();

        void WriteBytesToPort(byte[] buffer);

        void WriteByteToPort(byte data);

        void ClosePort();
    }
}
