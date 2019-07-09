using UnityEngine;
using TMPro;

public class Combatable : MonoBehaviour {
    public House house;
    public Soldiers soldiers = new Soldiers();
    public Combat combat = null;

    public int numSoldiers {
        get {
            return soldiers.GetNumSoldiersInTotal();
        }
    }

    // GUI elements
    private TextMeshProUGUI textNumSoldiers;

    protected void Start() {
        Transform flag = gameObject.transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = house.color;
        flag.GetChild(0).Find("House Flag").GetComponent<SpriteRenderer>().sprite = house.houseFlag;

        foreach (TextMeshProUGUI txt in GetComponentsInChildren<TextMeshProUGUI>()) {
            if (txt.name == "Text Num Soldiers") {
                textNumSoldiers = txt;
                break;
            }
        }
        UpdateGUI();
    }

    public void UpdateGUI() {
        textNumSoldiers.text = numSoldiers.ToString();
    }

    private void OnDisable() {
        GameController.DeselectLocationOnDisable(gameObject);
    }
}