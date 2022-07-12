using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    // Responsible for displaying information about things hovered over

    public class HoverManager : MonoBehaviour
    {
        private GameObject _largeCardHover;
        private float _largeCardWidth;
        private float _largeCardHeight;

        private GameObject LargeCardHover
        {
            get => _largeCardHover;
            set
            {
                _largeCardHover = value;
                var spriteRender = LargeCardHover.GetComponent<SpriteRenderer>();

                // 1.25f because the sprite is scaled by 1.5 - may change later
                _largeCardWidth = spriteRender.size.x * 1.25f;
                _largeCardHeight = spriteRender.size.y * 1.25f;
            }
        }


        private Dictionary<Colour, Sprite> LargeCardSprites { get; set; }

        private HashSet<GameObject> HoverObjects { get; set; } = new(); // remember the objects to be cleared the following frame

        public bool Activated { get; set; } = true;

        private void Start()
        {
            LargeCardHover = Resources.Load<GameObject>("Prefabs/LargeCardHover");

            LargeCardSprites = new()
            {
                { Colour.Red, Resources.Load<Sprite>("Sprites/RedCardLarge") },
                { Colour.Green, Resources.Load<Sprite>("Sprites/GreenCardLarge") },
                { Colour.Blue, Resources.Load<Sprite>("Sprites/BlueCardLarge") },
                { Colour.Purple, Resources.Load<Sprite>("Sprites/PurpleCardLarge") },
            };
        }

        private void Update()
        {
            if (!Activated)
                return;

            // Destroy all hover objects from the previous frame
            foreach(var previousHoverObject in HoverObjects)
            {
                Destroy(previousHoverObject);
            }
            HoverObjects.Clear();

            // Get the new objects hovered over
            (CardObject hoveredCard, TerrainObject hoveredTerrain) = GetHoveredObjects();

            if (hoveredCard != null && hoveredCard.IsInPosition)
            {
                CreateCardHover(hoveredCard);
            }

            if (hoveredTerrain != null)
            {
                CreateTerrainHover(hoveredTerrain, hoveredCard != null);
            }
        }

        private void CreateCardHover(CardObject hoveredCard)
        {
            var hoverObject = Instantiate(LargeCardHover, GetHoverCoordiantes(hoveredCard), Quaternion.identity);

            // set sprite colour
            hoverObject.GetComponent<SpriteRenderer>().sprite = LargeCardSprites[hoveredCard.CardReference.Colour];

            // set text
            var canvas = hoverObject.transform.Find("Canvas");
            canvas.transform.Find("Name").GetComponent<Text>().text = hoveredCard.CardReference.Name;
            canvas.transform.Find("EffectText").GetComponent<Text>().text = hoveredCard.CardReference.EffectText;
            canvas.transform.Find("Types").GetComponent<Text>().text = hoveredCard.CardReference.SubTypes.ToString();

            HoverObjects.Add(hoverObject);
        }

        // TODO: create a seperate graphic for terrain rather than using the red card graphic
        private void CreateTerrainHover(TerrainObject hoveredTerrain, bool hoveringOnCard)
        {
            var hoverObject = Instantiate(LargeCardHover, GetHoverCoordiantes(hoveredTerrain, hoveringOnCard), Quaternion.identity);

            // set sprite colour
            hoverObject.GetComponent<SpriteRenderer>().sprite = LargeCardSprites[Colour.Red]; 

            // set text
            var canvas = hoverObject.transform.Find("Canvas");
            canvas.transform.Find("Name").GetComponent<Text>().text = hoveredTerrain.TerrainReference.Name;
            canvas.transform.Find("EffectText").GetComponent<Text>().text = hoveredTerrain.TerrainReference.EffectText;
            canvas.transform.Find("Types").GetComponent<Text>().text = string.Empty;

            HoverObjects.Add(hoverObject);
        }

        private Vector2 GetHoverCoordiantes(CardObject hoveredCard)
        {
            var pos = hoveredCard.transform.position;

            pos.x += _largeCardWidth;
            pos.y -= _largeCardHeight / 3;

            FixPositioning(ref pos);

            return pos;
        }

        private Vector2 GetHoverCoordiantes(TerrainObject hoveredTerrain, bool hoveringOnCard)
        {
            var pos = hoveredTerrain.transform.position;

            pos.x -= _largeCardWidth;
            pos.y -= _largeCardHeight / 3;

            return pos;
        }

        // If a side of the hover would be out of the screen, move it into the screen
        private void FixPositioning(ref Vector3 hoverObjectPos)
        {
            if (Camera.main.WorldToViewportPoint(new Vector2(hoverObjectPos.x, hoverObjectPos.y - _largeCardHeight / 3)).y < 0)
                hoverObjectPos.y += _largeCardHeight / 1.5f;
            else if (Camera.main.WorldToViewportPoint(new Vector2(hoverObjectPos.x, hoverObjectPos.y + _largeCardHeight / 3)).y > 1)
                hoverObjectPos.y -= _largeCardHeight / 1.5f;

            if (Camera.main.WorldToViewportPoint(new Vector2(hoverObjectPos.x - _largeCardWidth / 3, hoverObjectPos.y)).x < 0)
                hoverObjectPos.x += _largeCardWidth * 2f;
            else if (Camera.main.WorldToViewportPoint(new Vector2(hoverObjectPos.x + _largeCardWidth / 3, hoverObjectPos.y)).x > 1)
                hoverObjectPos.x -= _largeCardWidth * 2f;
        }

        private (CardObject, TerrainObject) GetHoveredObjects()
        {
            // Get Colliding Targets
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D cardHit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Card"));
            RaycastHit2D terrainHit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Terrain"));

            // Null when not hovering
            var hoveredCard = cardHit.collider?.GetComponentInParent<BaseUIObject>() as CardObject;
            var hoveredTerrain = terrainHit.collider?.GetComponentInParent<BaseUIObject>() as TerrainObject;

            return (hoveredCard, hoveredTerrain);
        }
    }
}
