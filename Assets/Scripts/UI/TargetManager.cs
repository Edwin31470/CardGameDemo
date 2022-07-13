using System;
using Assets.Scripts.Enums;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Events;
using UnityEngine;
using Assets.Scripts.Bases;
using Assets.Scripts.Extensions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI
{
    public class TargetManager : BaseUIObjectManager
    {
        // Card Selection
        private HashSet<BaseUIObject> SelectedOptions { get; set; } = new();
        private HashSet<BaseUIObject> ValidOptions { get; set; } = new();
        private int MaxTargets { get; set; }


        // Render sprites on targets that are selected or hovered
        private HashSet<BaseUIObject> RenderedTargets { get; set; } = new(); // targets that had a sprite rendered on them on the previous frame
        private Color TargetColour { get; set; }
        private Sprite TargetSprite { get; set; }

        private OnFinishTargeting<ITargetable> OnFinishTargeting { get; set; }

        public bool IsProcessing { get; set; }

        public void Begin<T>(
            IEnumerable<BaseUIObject> validOptions,
            int maxTargets,
            OnFinishTargeting<ITargetable> onFinishTargeting,
            SelectionType selectionType)
            where T : ITargetable
        {
            IsProcessing = true;
            ValidOptions = new HashSet<BaseUIObject>(validOptions);
            MaxTargets = maxTargets;
            OnFinishTargeting = onFinishTargeting;

            TargetColour = GetTargetColour(selectionType);
            TargetSprite = GetTargetSprite(selectionType);
        }

        private Color GetTargetColour(SelectionType selectionType)
        {
            if (selectionType.HasFlag(SelectionType.Neutral))
            {
                return new Color(0.9f, 0.7f, 0.15f);
            }
            else if (selectionType.HasFlag(SelectionType.Positive))
            {
                return new Color(0.12f, 0.62f, 0.16f);
            }
            else if (selectionType.HasFlag(SelectionType.Negative))
            {
                return new Color(0.86f, 0.27f, 0.12f);
            }

            throw new ArgumentOutOfRangeException(nameof(selectionType), "Invalid selection type for colour");
        }

        private Sprite GetTargetSprite(SelectionType selectionType)
        {
            if (selectionType.HasFlag(SelectionType.Highlight))
            {
                return Resources.Load<Sprite>($"Sprites/CardHighlight");
            }
            else if (selectionType.HasFlag(SelectionType.Target))
            {
                return Resources.Load<Sprite>($"Sprites/Target");
            }

            throw new ArgumentOutOfRangeException(nameof(selectionType), "Invalid selection type for sprite");
        }

        /// <summary>
        /// Finish selection and return events of that selection
        /// </summary>
        /// <returns>Events resulting from selection</returns>
        public void Finish()
        {
            OnFinishTargeting.Invoke(SelectedOptions);

            DeselectCards();
            IsProcessing = false;
        }

        private void DeselectCards()
        {
            SelectedOptions = new();
        }

        private void Update()
        {
            if (!IsProcessing)
                return;

            var hoveredTarget = GetHoveredTarget();
            var renderTargets = new HashSet<BaseUIObject>(SelectedOptions);

            if (hoveredTarget != null)
            {
                renderTargets.Add(hoveredTarget);

                // Clicking on a target
                if (Input.GetMouseButtonDown(0))
                {
                    // Select Target
                    if (!SelectedOptions.Contains(hoveredTarget))
                    {
                        SelectedOptions.Add(hoveredTarget);
                    }
                    // Deselect Target
                    else
                    {
                        SelectedOptions.Remove(hoveredTarget);
                    }
                }
            }

            // Finish if max targets are selected or there are no more targets
            if (ShouldFinishSelection())
            {
                Finish();
                renderTargets.Clear();
            }

            RenderSprites(renderTargets);
        }

        // Gets the UI object the mouse is on and returns it if it is a valid target, otherwise returns null
        private BaseUIObject GetHoveredTarget()
        {
            var cardMask = IsOverlayUp ? LayerMask.GetMask("Card Overlay") : LayerMask.GetMask("Card", "Slot", "Terrain");

            // Get Colliding Target
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, cardMask);

            // Null when not hovering
            var hoveredTarget = hit.collider?.GetComponentInParent<BaseUIObject>();

            return ValidOptions.Contains(hoveredTarget) ? hoveredTarget : null;
        }

        // Render sprite symbols on all selected cards and the hovered card (if any)
        private void RenderSprites(IEnumerable<BaseUIObject> renderTargets)
        {
            // RenderedTargets is the targets with sprites rendered on the previous frame
            var addRenderTargets = renderTargets.Except(RenderedTargets).ToArray();
            var removeRenderTargets = RenderedTargets.Except(renderTargets).ToArray();

            foreach (var renderTarget in addRenderTargets)
            {
                // Create new sprite object
                var hoverSprite = GameObject.Instantiate(new GameObject());
                hoverSprite.name = "HoverSprite";

                // Set the sprite
                var spriteRenderer = hoverSprite.gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = TargetSprite;
                spriteRenderer.color = TargetColour;
                spriteRenderer.sortingLayerName = "Hover";
                spriteRenderer.enabled = true;

                // Attach sprite object to the target
                hoverSprite.transform.SetParent(renderTarget.transform, false);

                RenderedTargets.Add(renderTarget);
            }

            foreach (var renderTarget in removeRenderTargets)
            {
                // Destroy the sprite object
                var hoverSprite = renderTarget.transform.Find("HoverSprite").gameObject;
                Destroy(hoverSprite);

                RenderedTargets.Remove(renderTarget);
            }
        }

        private bool ShouldFinishSelection()
        {
            if (SelectedOptions.Count == MaxTargets) {
                return true;
            }
            if (SelectedOptions.Count == ValidOptions.Count) {
                return true;
            }
            // TODO: Only some events can have less than maximum targets selected
            if (Input.GetKeyDown(KeyCode.Return)) {
                return true;
            }

            return false;
        }
    }
}
