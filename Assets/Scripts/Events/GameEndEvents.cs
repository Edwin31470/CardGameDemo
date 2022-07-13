using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events
{
    // Events that trigger before anything else and end the game
    public abstract class BaseGameEndEvent : BaseEvent
    {
        public abstract string Process();
    }

    public class GameEndEvent : BaseGameEndEvent
    {
        private string Message { get; set; }

        public GameEndEvent(string message)
        {
            Message = message;
        }

        public override string Process()
        {
            return Message;
        }
    }
}
