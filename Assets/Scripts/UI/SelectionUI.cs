using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionUI : MonoBehaviour {
    private GameController gameController;
    private GameLocation attachedGameLocation;
    private Soldiers displayedSoldiers;
    private float elapsedTime;

    public void Init(GameController gc) {
        gameController = gc;
        elapsedTime = 0f;
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= Global.SEL_UI_UPDATE_TIME) {
            elapsedTime = 0f;
            if (displayedSoldiers.GetNumSoldiersInTotal() != attachedGameLocation.soldiers.GetNumSoldiersInTotal()) {
                OnEnable();
            }
        }
    }

    void OnEnable() {
        attachedGameLocation = gameController.selectedLocation.GetComponent<GameLocation>();
        displayedSoldiers = new Soldiers();
        displayedSoldiers.AddSoldiers(attachedGameLocation.soldiers);

        TextMeshProUGUI[] txts = GetComponentsInChildren<TextMeshProUGUI>();
        int counter = 0; // To count soldier types
        foreach (TextMeshProUGUI txt in txts) {
            switch (txt.gameObject.name) {
                case "Text Soldier Name":
                    txt.text = Soldiers.GetSoldierTypeStats((SoldierType)counter).soldierName;
                    break;
                case "Text Soldier Num":
                    txt.text = displayedSoldiers.GetSoldierTypeNum((SoldierType)counter).ToString();
                    break;
                case "Text Sel Soldier Num":
                    Slider s = txt.gameObject.transform.parent.gameObject.GetComponentInChildren<Slider>();
                    s.maxValue = displayedSoldiers.GetSoldierTypeNum((SoldierType)counter);
                    if (s.maxValue <= 0) {
                        s.transform.Find("Fill Area").gameObject.SetActive(false);
                    } else {
                        s.transform.Find("Fill Area").gameObject.SetActive(true);
                        s.value = s.maxValue;
                    }
                    counter++; // Increment per soldier type after all text fields are set
                    break;
                case "Text Location Name":
                    txt.text = attachedGameLocation.locationName;
                    break;
            }
        }
    }
}
