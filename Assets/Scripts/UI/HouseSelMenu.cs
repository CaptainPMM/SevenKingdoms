using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HouseSelMenu : MonoBehaviour {
    public Image houseFlagImage;
    public TextMeshProUGUI houseNameText;
    public Slider houseSelSlider;
    public Slider aiDiffSlider;
    private TextMeshProUGUI aiDiffText;

    public House selHouse;

    private void Start() {
        // Set sliders
        houseSelSlider.minValue = 1; // Ingore neutral (index 0)
        houseSelSlider.maxValue = System.Enum.GetValues(typeof(HouseType)).Length - 1;
        houseSelSlider.value = houseSelSlider.maxValue / 2;

        aiDiffSlider.minValue = 0;
        aiDiffSlider.maxValue = System.Enum.GetValues(typeof(AIDifficulty)).Length - 1;
        aiDiffSlider.value = 1; // Default to NORMAL
        aiDiffText = aiDiffSlider.transform.parent.Find("Text AI Slider").GetComponent<TextMeshProUGUI>();

        ChangeSelHouse(houseSelSlider.value);
        ChangeAIDiff(aiDiffSlider.value);
    }

    public void ChangeSelHouse(float index) {
        selHouse = new House((HouseType)index);
        houseFlagImage.sprite = selHouse.houseFlag;
        houseNameText.text = "House " + selHouse.houseName;
    }

    public void ChangeAIDiff(float index) {
        aiDiffText.text = ((AIDifficulty)index).ToString().ToLower();
    }

    public void Play() {
        Global.GAME_PARAM_PLAYER_HOUSE_TYPE = selHouse.houseType;
        Global.GAME_PARAM_AI_DIFF = (AIDifficulty)aiDiffSlider.value;

        if (Multiplayer.NetworkManager.isServer) Multiplayer.Server.instance.StartGame();
    }
}