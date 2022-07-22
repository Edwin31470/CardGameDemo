using Assets.Scripts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Extensions
{
    public static class EventSystemExtensions
    {
        public static GameObject GetHoveredCard(this EventSystem eventSystem)
        {
            var mousePos = new PointerEventData(eventSystem) { position = Input.mousePosition };
            var raycastResults = new List<RaycastResult>();

            eventSystem.RaycastAll(mousePos, raycastResults);

            var card = raycastResults
                .Select(x => x.gameObject)
                .FirstOrDefault(x => x.layer == LayerMask.NameToLayer("Card"));

            return card;
        }

        public static (GameObject, CardData) GetHoveredCardAndData(this EventSystem eventSystem)
        {
            var hoveredCard = GetHoveredCard(eventSystem);

            if (hoveredCard == null)
                return (null, new CardData());

            var id = hoveredCard.GetComponentInParent<Metadata>().GetData<int>("id");

            return (hoveredCard, DataIO.Read<CardData>(id));
        }
    }
}
