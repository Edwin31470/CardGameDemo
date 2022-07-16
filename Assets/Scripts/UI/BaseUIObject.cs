using Assets.Scripts.Bases;
using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.UI
{
    // A UI object is something that can be targeted by effects
    public abstract class BaseUIObject : MonoBehaviour
    {
        protected SpriteRenderer Sprite { get; set; }
        protected SortingGroup SortingGroup { get; set; }
        protected Canvas Canvas { get; set; }

        public abstract ITargetable SourceReference { get; }

        public void Initialize()
        {
            Sprite = GetComponent<SpriteRenderer>();
            SortingGroup = GetComponent<SortingGroup>();
            //Canvas = transform.Find("Canvas").GetComponent<Canvas>();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void SetSortingLayer(string layer)
        {
            SortingGroup.sortingLayerName = layer;
            //Canvas.sortingLayerName = layer;
        }

        public void SetSortingOrder(int order)
        {
            SortingGroup.sortingOrder = order;
        }
    }

    public abstract class MoveableUIObject : BaseUIObject
    {
        private const int MoveSpeed = 5;
        protected Vector2 TargetPosition { get; set; }
        private bool DestroyWhenOffScreen { get; set; }
        private bool IsOffScreen => !Sprite.isVisible;
        private float Delay { get; set; } // Remove from this before moving

        public bool IsInPosition => Vector2.Distance(transform.position, TargetPosition) < 0.1;

        // Move to a position over time
        public void SetTargetPosition(Vector2 position, bool destroyWhenOffScreen = false, float delay = 0f)
        {
            TargetPosition = position;
            DestroyWhenOffScreen = destroyWhenOffScreen;
            Delay = delay;
        }

        // Move to a position instantly
        public void MoveToPosition(Vector2 position)
        {
            transform.position = position;
            TargetPosition = position;
        }

        protected virtual void Update()
        {
            if (DestroyWhenOffScreen && IsOffScreen) {
                Destroy();
                return;
            }

            if (Delay > 0) {
                Delay -= Time.deltaTime;
                return;
            }

            transform.position = Vector2.LerpUnclamped(transform.position, TargetPosition, Time.deltaTime * MoveSpeed);
        }
    }
}
