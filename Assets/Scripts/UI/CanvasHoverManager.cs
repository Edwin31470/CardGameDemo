using Assets.Scripts.Extensions;
using Assets.Scripts.IO;
using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CanvasHoverManager : MonoBehaviour
    {
        // Resources
        private GameObject LargeCardCanvasObject { get; set; }

        // Fields for UI
        private Transform CanvasTransform { get; set; }
        private GameObject ExistingHover { get; set; }

        public void Start()
        {
            // Load Resources
            LargeCardCanvasObject = Resources.Load<GameObject>("Prefabs/LargeCardCanvasObject");

            // Find GameObjects
            CanvasTransform = GameObject.Find("Canvas").transform;
        }

        public void Update()
        {
            var (hoveredCard, cardData) = EventSystem.current.GetHoveredCardAndData();

            // Destroy existing hover
            if (ExistingHover != null)
                Destroy(ExistingHover);

            // When not hovering, nothing changes
            if (hoveredCard == null)
                return;

            RenderHover(hoveredCard, cardData);
        }

        private void RenderHover(GameObject hoveredCard, CardData cardData)
        {
            var pos = hoveredCard.transform.position;
            pos.x += 128;
            pos.y -= 32;

            var largeCardCanvasObject = Instantiate(LargeCardCanvasObject, pos, Quaternion.identity, CanvasTransform);

            largeCardCanvasObject.transform.localScale = Vector3.one * 2;
            largeCardCanvasObject.GetComponent<Image>().sprite = SpritesManager.GetLargeCard(cardData.Colour);
            largeCardCanvasObject.transform.Find("Name").GetComponent<TMP_Text>().text = cardData.Name;
            largeCardCanvasObject.transform.Find("EffectText").GetComponent<TMP_Text>().text = cardData.EffectText;
            largeCardCanvasObject.transform.Find("SubTypes").GetComponent<TMP_Text>().text = cardData.SubTypes.ToString();

            ExistingHover = largeCardCanvasObject;
        }
    }
}
