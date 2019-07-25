using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HouseSelMenu : MonoBehaviour {
    public Image houseFlagImage;
    public TextMeshProUGUI houseNameText;
    public Slider houseSelSlider;

    public House selHouse;

    private void Start() {
        houseSelSlider.minValue = 1; // Ingore neutral (index 0)
        houseSelSlider.maxValue = System.Enum.GetValues(typeof(HouseType)).Length - 1;
        houseSelSlider.value = houseSelSlider.maxValue / 2;

        ChangeSelHouse(houseSelSlider.value);
    }

    public void ChangeSelHouse(float index) {
        selHouse = new House((HouseType)index);
        houseFlagImage.sprite = selHouse.houseFlag;
        houseNameText.text = "House " + selHouse.houseName;
    }

    public void Play() {
        Global.GAME_PARAM_PLAYER_HOUSE_TYPE = selHouse.houseType;
    }
}