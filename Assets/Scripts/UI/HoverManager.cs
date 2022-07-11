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
        private GameObject LargeCardHover { get; set; }

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

            if (hoveredCard != null)
            {
                CreateCardHover(hoveredCard);
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

        // Place the hover to the right and extending down from the card, unless it would go out of bounds in that direction
        private Vector2 GetHoverCoordiantes(CardObject hoveredCard)
        {
            // TODO: work out how to work out the hovered card will be out of bounds

            var pos = hoveredCard.transform.position;

            pos.x += LargeCardHover.GetComponent<SpriteRenderer>().bounds.size.x;

            return pos;
        }

        //private Vector2 GetHoverCoordiantes(TerrainObject hoveredTerrain, bool cardInSlot)
        //{

        //}

        // Gets the UI object the mouse is on and returns it if it is a valid target, otherwise returns null
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
