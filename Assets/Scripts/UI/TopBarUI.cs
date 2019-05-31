using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopBarUI : MonoBehaviour {
    private InputController inputController;
    private GamePlayer player;
    private TextMeshProUGUI[] texts;

    public void Init(InputController ic) {
        inputController = ic;
        player = inputController.player.GetComponent<GamePlayer>();

        // Find UI elements
        texts = GetComponentsInChildren<TextMeshProUGUI>();

        // Set player house flag and panel colors
        foreach (Image i in GetComponentsInChildren<Image>()) {
            if (i.gameObject.name == "House Flag") {
                i.sprite = player.house.houseFlag;
            } else if (i.gameObject.name.Contains("Panel")) {
                i.color = player.house.color;
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
