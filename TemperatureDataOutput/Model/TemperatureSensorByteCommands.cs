namespace TemperatureDataOutput.Model
{
    public static class TemperatureSensorByteCommands
    {
        public static readonly CommandByteInfo FindDeviceCommand = new CommandByteInfo(0x01, 0xFF, 0xFE);
        public static readonly CommandByteInfo TransmitDataCommand = new CommandByteInfo(0x02, 0xFF, 0xFE, 3);
    }
}
