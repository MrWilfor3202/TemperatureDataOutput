using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace TemperatureDataOutput.Model
{
    public static class FileParser
    {
        public static Dictionary<string, CommandByteInfo> ReadCommandsFromFile(string fileName)
        {
            string[] lines = File.ReadAllText(fileName).Split('\n')
                .Select(line => line.TrimEnd('\r'))
                .Select(line => line + " " + '1')
                .ToArray();

            return lines.Select(line => line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                   .ToDictionary(command0 => command0[0],
                     command =>
                      new CommandByteInfo(Convert.ToByte(command[1]), Convert.ToByte(command[2]), Convert.ToByte(command[3]), Convert.ToInt32(command[4])));
        }
    }
}
