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
        DefaultState();

        attachedGameLocation = gameController.selectedLocation.GetComponent<GameLocation>();
        displayedSoldiers = new Soldiers();
        displayedSoldiers.AddSoldiers(attachedGameLocation.soldiers);

        TextMeshProUGUI[] txts = GetComponentsInChildren<TextMeshProUGUI>(); // Slider soldier amount labels
        int counter = 0; // To count soldier types
        foreach (TextMeshProUGUI txt in txts) {
            Slider s = txt.gameObject.transform.parent.gameObject.GetComponentInChildren<Slider>();
            s.maxValue = displayedSoldiers.GetSoldierTypeNum((SoldierType)counter);
            if (s.maxValue <= 0) {
                s.transform.Find("Fill Area").gameObject.SetActive(false);
            } else {
                s.transform.Find("Fill Area").gameObject.SetActive(true);
                s.value = s.maxValue;
            }
            counter++; // Increment per soldier type after all text fields are set
        }
    }

    private void DefaultState() {
        Button[] btns = GetComponentsInChildren<Button>();
        foreach (Button btn in btns) {
            if (btn.name == "Button Recruit") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_recruit");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    RecruitState();
                });
            } else if (btn.name == "Button Build") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_build");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    ClickedBuildBtn();
                });
            }
        }
    }

    private void ClickedBuildBtn() {
        gameController.OpenBuildingsMenu();
    }

    private void RecruitState() {
        Button[] btns = GetComponentsInChildren<Button>();
        foreach (Button btn in btns) {
            if (btn.name == "Button Recruit") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_back");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    DefaultState();
                });
            } else if (btn.name == "Button Build") {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("btn_recruit");
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    ClickedFinalRecruitBtn();
                });
            }
        }
    }

    private void ClickedFinalRecruitBtn() {
        // Do recruitment logic...
        DefaultState();
    }
}