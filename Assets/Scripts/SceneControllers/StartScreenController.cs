using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StartScreenController : MonoBehaviour
    {
        public void Start()
        {
            // Register button
            GameObject.Find("Canvas/PlayButton").GetComponent<Button>()
                .onClick.AddListener(() => LoadScene("GameScene"));

            GameObject.Find("Canvas/ManagePlayersButton").GetComponent<Button>()
                .onClick.AddListener(() => LoadScene("ManagePlayers"));

            GameObject.Find("Canvas/ManageDecksButton").GetComponent<Button>()
                .onClick.AddListener(() => LoadScene("ManageDecks"));

            GameObject.Find("Canvas/ManageCardsButton").GetComponent<Button>()
                .onClick.AddListener(() => LoadScene("ManageCards"));

            GameObject.Find("Canvas/ManageItemsButton").GetComponent<Button>()
                .onClick.AddListener(() => LoadScene("ManageItems"));
        }

        private void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
