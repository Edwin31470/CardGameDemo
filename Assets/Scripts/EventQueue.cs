using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class EventQueue<T> where T : class
    {
        private Queue<T> Queue { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEmpty => !Queue.Any();

        public EventQueue()
        {
            Queue = new Queue<T>();
            IsLocked = false;
        }

        public T Dequeue()
        {
            if (IsLocked || IsEmpty)
                return null;

            return Queue.Dequeue();
        }

        public void Enqueue(T baseEvent)
        {
            Queue.Enqueue(baseEvent);
        }

        public void Empty()
        {
            while (Queue.Count > 0)
            {
                Queue.Dequeue();
            }
        }
    }
}
