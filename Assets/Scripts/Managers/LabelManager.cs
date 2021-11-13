using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class LabelManager : MonoBehaviour
    {
        private Player FrontPlayer { get; set; }
        private Player BackPlayer { get; set; }

        private static float BarMoveSpeed = 5;
        public bool BarIsInPosition => Vector2.Distance(transform.position, BarTargetPositon) < 0.01;
        private Vector2 BarTargetPositon { get; set; }
        private GameObject CurrentEventBar { get; set; }
        private TextMesh CurrentEventText { get; set; }

        private Text FrontTotalAttack { get; set; }
        private Text FrontTotalDefence { get; set; }
        private Text FrontHealth { get; set; }
        private Text FrontDeckCount { get; set; }
        private Text FrontDestroyedCount { get; set; }
        private Text FrontRedMana { get; set; }
        private Text FrontGreenMana { get; set; }
        private Text FrontBlueMana { get; set; }
        private Text FrontPurpleMana { get; set; }

        private Text BackTotalAttack { get; set; }
        private Text BackTotalDefence { get; set; }
        private Text BackHealth { get; set; }
        private Text BackDeckCount { get; set; }
        private Text BackDestroyedCount { get; set; }
        private Text BackRedMana { get; set; }
        private Text BackGreenMana { get; set; }
        private Text BackBlueMana { get; set; }
        private Text BackPurpleMana { get; set; }

        public void Start() 
        {
            CurrentEventBar = GameObject.Find("CurrentEventBar");
            CurrentEventText = CurrentEventBar.GetComponentInChildren<TextMesh>();

            FrontTotalAttack = GameObject.Find("Canvas/FrontPlayer/TotalAttack").GetComponent<Text>();
            FrontTotalDefence = GameObject.Find("Canvas/FrontPlayer/TotalDefence").GetComponent<Text>();
            FrontHealth = GameObject.Find("Canvas/FrontPlayer/Health").GetComponent<Text>();
            FrontDeckCount = GameObject.Find("Canvas/FrontPlayer/DeckCount").GetComponent<Text>();
            FrontDestroyedCount = GameObject.Find("Canvas/FrontPlayer/DestroyedCount").GetComponent<Text>();
            FrontRedMana = GameObject.Find("Canvas/FrontPlayer/RedMana").GetComponent<Text>();
            FrontGreenMana = GameObject.Find("Canvas/FrontPlayer/GreenMana").GetComponent<Text>();
            FrontBlueMana = GameObject.Find("Canvas/FrontPlayer/BlueMana").GetComponent<Text>();
            FrontPurpleMana = GameObject.Find("Canvas/FrontPlayer/PurpleMana").GetComponent<Text>();

            BackTotalAttack = GameObject.Find("Canvas/BackPlayer/TotalAttack").GetComponent<Text>();
            BackTotalDefence = GameObject.Find("Canvas/BackPlayer/TotalDefence").GetComponent<Text>();
            BackHealth = GameObject.Find("Canvas/BackPlayer/Health").GetComponent<Text>();
            BackDeckCount = GameObject.Find("Canvas/BackPlayer/DeckCount").GetComponent<Text>();
            BackDestroyedCount = GameObject.Find("Canvas/BackPlayer/DestroyedCount").GetComponent<Text>();
            BackRedMana = GameObject.Find("Canvas/BackPlayer/RedMana").GetComponent<Text>();
            BackGreenMana = GameObject.Find("Canvas/BackPlayer/GreenMana").GetComponent<Text>();
            BackBlueMana = GameObject.Find("Canvas/BackPlayer/BlueMana").GetComponent<Text>();
            BackPurpleMana = GameObject.Find("Canvas/BackPlayer/PurpleMana").GetComponent<Text>();
        }

        public void Initialize(Player frontPlayer, Player backPlayer)
        {
            FrontPlayer = frontPlayer;
            BackPlayer = backPlayer;
        }

        public void Update()
        {
            // Move Bar
            if (!BarIsInPosition)
            {
                CurrentEventBar.transform.position = Vector2.Lerp(
                    CurrentEventBar.transform.position,
                    BarTargetPositon,
                    Time.deltaTime * BarMoveSpeed);
            }

            FrontTotalAttack.text = FrontPlayer.TotalAttack.ToString();
            FrontTotalDefence.text = FrontPlayer.TotalDefence.ToString();
            FrontHealth.text = FrontPlayer.Health.Get().ToString();
            FrontDeckCount.text = FrontPlayer.DeckCount().ToString();
            FrontDestroyedCount.text = FrontPlayer.GetDestroyed().Count().ToString();
            FrontRedMana.text = FrontPlayer.RedMana.Get().ToString();
            FrontGreenMana.text = FrontPlayer.GreenMana.Get().ToString();
            FrontBlueMana.text = FrontPlayer.BlueMana.Get().ToString();
            FrontPurpleMana.text = FrontPlayer.PurpleMana.Get().ToString();

            BackTotalAttack.text = BackPlayer.TotalAttack.ToString();
            BackTotalDefence.text = BackPlayer.TotalDefence.ToString();
            BackHealth.text = BackPlayer.Health.Get().ToString();
            BackDeckCount.text = BackPlayer.DeckCount().ToString();
            BackDestroyedCount.text = BackPlayer.GetDestroyed().Count().ToString();
            BackRedMana.text = BackPlayer.RedMana.Get().ToString();
            BackGreenMana.text = BackPlayer.GreenMana.Get().ToString();
            BackBlueMana.text = BackPlayer.BlueMana.Get().ToString();
            BackPurpleMana.text = BackPlayer.PurpleMana.Get().ToString();
        }

        public void SetCurrentEvent(string eventName)
        {
            if (eventName != null)
            {
                ShowCurrentEvent(eventName);
            }
            else
            {
                HideCurrentEvent();
            }
        }

        private void ShowCurrentEvent(string eventName)
        {
            BarTargetPositon = new Vector2(5.2f, 0f);
            CurrentEventText.text = eventName;

        }

        private void HideCurrentEvent()
        {
            BarTargetPositon = new Vector2(29.5f, 0f);
            CurrentEventText.text = string.Empty;
        }
    }
}
