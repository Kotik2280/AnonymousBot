using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonimusBot
{
    public class TempCommand
    {
        public BotCommandType CommandType { get; set; }
        public List<string> ParameterList { get; }
        public int InputTime { get; set; }
        public TempCommand(BotCommandType commandType, int inputTime)
        {
            CommandType = commandType;
            InputTime = inputTime;
            ParameterList = new List<string>();
        }
    }
}
