using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopBarUI : MonoBehaviour {
    private GameController gameController;
    private GamePlayer player;
    private TextMeshProUGUI[] texts;

    public void Init(GameController gc) {
        gameController = gc;
        player = gameController.player.GetComponent<GamePlayer>();

        // Find UI elements
        texts = GetComponentsInChildren<TextMeshProUGUI>();

        // Set player house flag
        foreach (Image i in GetComponentsInChildren<Image>()) {
            if (i.gameObject.name == "House Flag") {
                i.sprite = player.house.houseFlag;
                break;
            }
        }

        gameObject.SetActive(true);
    }

    private void Update() {
        foreach (TextMeshProUGUI txt in texts) {
            switch (txt.gameObject.name) {
                case "Text Gold Num":
                    txt.text = player.house.gold.ToString();
                    break;
                case "Text Manpower Num":
                    txt.text = player.house.manpower.ToString();
                    break;
                default: break;
            }
        }
    }
}
