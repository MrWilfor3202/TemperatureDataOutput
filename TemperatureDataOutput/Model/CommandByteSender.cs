namespace TemperatureDataOutput.Model
{
    public class CommandByteSender
    {
        private IBytePort bytePort;

        public CommandByteSender(IBytePort port)
        {
            bytePort = port;
        }

        public void StartAcceptCommandBytes() => bytePort.OpenPort();

        public void StopAcceptCommandBytes() => bytePort.ClosePort();

        public byte[] SendCommandByte(CommandByteInfo command)
        {
            byte[] response = new byte[command.ResponseBytesCount];

            bytePort.WriteByteToPort(command.CommandByte);
            response[0] = bytePort.ReadByteFromPort();

            if (response[0] != command.FailedCommandByte)
                bytePort.ReadBytesFromPort(command.ResponseBytesCount - 1).CopyTo(response, 1);

            if (response[response.Length - 1] != command.SuccessCommandByte || response.Length != command.ResponseBytesCount)
                return new byte[] {};

            return response;
        }
    }
}
