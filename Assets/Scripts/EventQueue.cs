using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class EventQueue<T> where T : BaseEvent
    {
        private Queue<T> Queue { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEmpty => !Queue.Any();

        public EventQueue()
        {
            Queue = new Queue<T>();
            IsLocked = false;
        }

        public T DequeueEvent()
        {
            if (IsLocked || IsEmpty)
                return null;

            return Queue.Dequeue();
        }

        public void EnqueueEvent(T baseEvent)
        {
            Queue.Enqueue(baseEvent);
        }

        public T Peek()
        {
            return Queue.Peek();
        }

        public IEnumerable<T> PeekAll()
        {
            return Queue.ToList();
        }

        public void RemoveEvent(T baseEvent)
        {
            Queue = new Queue<T>(Queue.Where(x => x != baseEvent));
        }
    }
}
