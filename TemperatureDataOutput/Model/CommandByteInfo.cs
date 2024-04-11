using System;

namespace TemperatureDataOutput.Model
{
    public struct CommandByteInfo
    {
        public readonly byte CommandByte;
        public readonly byte SuccessCommandByte;
        public readonly byte FailedCommandByte;
        public readonly int ResponseBytesCount;

        public CommandByteInfo(byte commandByte, byte successCommandByte, byte failedCommandByte) : this(commandByte, successCommandByte, failedCommandByte, 1)
        { }

        public CommandByteInfo(byte commandByte, byte successCommandByte, byte failedCommandByte, int responseBytesCount )
        {
            CommandByte = commandByte;
            SuccessCommandByte = successCommandByte;
            FailedCommandByte = failedCommandByte;
            ResponseBytesCount = responseBytesCount;

            if (responseBytesCount < 1)
                throw new ArgumentException();
        }
    }
}
