namespace TemperatureDataOutput.Model
{
    public interface IByteConverter
    {
        double GetTemperatureFromBytes(byte[] temperatureBytes);
    }
}
