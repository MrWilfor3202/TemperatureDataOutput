using System;
using System.Linq;

namespace TemperatureDataOutput.Model
{
    public class DataReceiver
    {
        public string LastOperationState { get; private set; } = "Внесены изменения в конфигурацию";

        private readonly IByteConverter byteConverter;
        private readonly CommandByteSender byteSender;
        
        public DataReceiver(IBytePort port, IByteConverter converter)
        {
            byteConverter = converter;
            byteSender = new CommandByteSender(port);
        }

        public string GetTemperature()
        {
            try
            {
                byte[] response;
                byte[] data;
                double temperature;

                byteSender.StartAcceptCommandBytes();
                response = byteSender.SendCommandByte(TemperatureSensorByteCommands.FindDeviceCommand);

                if (!IsResponseSuccess(response))
                {
                    LastOperationState = "Устройство не обнаружено!";
                    byteSender.StopAcceptCommandBytes();
                    return "Ошибка!";
                }

                response = byteSender.SendCommandByte(TemperatureSensorByteCommands.TransmitDataCommand);

                if (!IsResponseSuccess(response))
                {
                    LastOperationState = "Произошла ошибка передачи данных!";
                    byteSender.StopAcceptCommandBytes();
                    return "Ошибка";
                }

                byteSender.StopAcceptCommandBytes();

                data = response.Where((obj, index) => index < response.Length - 1).ToArray();

                temperature = byteConverter.GetTemperatureFromBytes(data);
                LastOperationState = "Температура считана";

                return temperature.ToString();
            }
            catch (TimeoutException)
            {
                LastOperationState = "Микроконтроллер не ответил";
                return "Ошибка!";
            }
        }

        private bool IsResponseSuccess(byte[] response)
        {
            return response.Length > 0;
        }
    }
}
