using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class RepeatingTimer : MonoBehaviour
    {
        public Action Action { get; set; }

        private float TickLength { get; set; }
        private float Delta { get; set; }

        private float DelayLength { get; set; }
        private float DelayDelta { get; set; }

        private bool Started { get; set; }

        public void Initialize(float tickLength, Action action)
        {
            Action = action;

            TickLength = tickLength;
            Delta = 0;

            DelayLength = 0;
            DelayDelta = 0;
        }

        public void SetTickLength(float length)
        {
            TickLength = length;
            Delta = 0;
        }

        public void AddDelay(float delay)
        {
            DelayLength = delay;
            DelayDelta = 0;
        }

        public void StartTimer()
        {
            Started = true;
        }

        public void StopTimer()
        {
            Started = false;
        }

        public void Update()
        {
            if (!Started)
                return;

            if (DelayLength > 0)
            {
                DelayDelta += Time.deltaTime;
                if (DelayDelta <= DelayLength)
                {
                    return;
                }
                else
                {
                    DelayLength = 0;
                    DelayDelta = 0;
                }
            }

            Delta += Time.deltaTime;

            if(Delta >= TickLength)
            {
                Action.Invoke();
                Delta = 0;
            }
        }
    }
}
