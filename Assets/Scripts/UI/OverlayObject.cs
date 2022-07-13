using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    // Overlay for showing piles and tokens
    public class OverlayObject : MonoBehaviour
    {
        private const float _delay = 0.05f;
        private Queue<float> GetDelays(int count)
        {
            // Generate a queue of delays in a random order
            var delay = 0f;
            return new Queue<float>(
                Enumerable.Repeat(0, count)
                .Select(x => delay += _delay)
                .Shuffle());
        }

        private static Vector2 FirstCardSpace { get; set; }
        private static HashSet<MoveableUIObject> UIObjects { get; set; }

        public bool IsEmpty => !UIObjects.Any(x => x != null);

        public void Initialize(IEnumerable<BaseSource> sources, Func<BaseSource, Vector2, Transform, float, MoveableUIObject> createUIObject)
        {
            // Space Between Cards
            const int _xOffset = (64 + 6) / 10;
            const int _yOffset = (64 + 6) / 10;

            FirstCardSpace = transform.Find("FirstCardSpace").transform.position;
            UIObjects = new HashSet<MoveableUIObject>();

            // Starting Grid Position
            var x = 0;
            var y = 0;

            var delays = GetDelays(sources.Count());
            foreach (var source in sources)
            {
                var pos = new Vector2(FirstCardSpace.x + x * _xOffset, FirstCardSpace.y + y * -_yOffset);

                var uIObject = createUIObject(source, pos, transform, delays.Dequeue());
                UIObjects.Add(uIObject);

                // Increment position and delay
                x++;

                // when row is full move to the next row
                if (x == 7)
                {
                    x = 0;
                    y++;
                }
            }
        }

        public void BeginClosing(Vector2 destination)
        {
            var delays = GetDelays(UIObjects.Count);
            foreach (var uIObject in UIObjects.Shuffle())
            {
                uIObject.SetTargetPosition(destination, true, delays.Dequeue());
            }
        }
    }
}
