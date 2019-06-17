using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionUI : MonoBehaviour {
    private GameController gameController;
    private GameLocation attachedGameLocation;
    private Soldiers displayedSoldiers;
    private float elapsedTime;

    private bool inRecruitState;
    private Recruitment recruitment;

    public void Init(GameController gc) {
        gameController = gc;
        elapsedTime = 0f;
        inRecruitState = false;
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= Global.SEL_UI_UPDATE_TIME) {
            elapsedTime = 0f;
            if (inRecruitState) {
                SetupRecruitSliders();
            } else {
                if (displayedSoldiers.GetNumSoldiersInTotal() != attachedGameLocation.soldiers.GetNumSoldiersInTotal()) {
                    UpdateSoldierSelectionSliders();
                }
            }
        }
    }

    void OnEnable() {
        attachedGameLocation = gameController.selectedLocation.GetComponent<GameLocation>();
        DefaultState();
    }

    private void DefaultState() {
        inRecruitState = false;

        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            s.onValueChanged.RemoveAllListeners();
        }

        UpdateSoldierSelectionSliders();

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

    private void UpdateSoldierSelectionSliders() {
        displayedSoldiers = new Soldiers();
        displayedSoldiers.AddSoldiers(attachedGameLocation.soldiers);

        int counter = 0; // To count soldier types
        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            s.maxValue = displayedSoldiers.GetSoldierTypeNum((SoldierType)counter);
            s.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = s.value.ToString();
            if (s.maxValue <= 0) {
                s.transform.Find("Fill Area").gameObject.SetActive(false);
            } else {
                s.transform.Find("Fill Area").gameObject.SetActive(true);
                s.value = s.maxValue;
            }
            counter++; // Increment per soldier type after all text fields are set
        }
    }

    private void ClickedBuildBtn() {
        gameController.OpenBuildingsMenu();
    }

    private void RecruitState() {
        inRecruitState = true;

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

        recruitment = new Recruitment(gameController.player.GetComponent<GamePlayer>());
        SetupRecruitSliders();
    }

    private void SetupRecruitSliders() {
        recruitment.Update();

        int counter = 0; // To count soldier types
        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            SoldierType st = (SoldierType)counter;

            int currSoldierTypeNum = recruitment.GetRecruitSoldiers().GetSoldierTypeNum(st);
            s.value = currSoldierTypeNum;
            s.maxValue = currSoldierTypeNum + recruitment.GetMaxAvailableSoldierTypeNum(st);

            s.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = s.value + " / " + s.maxValue;

            if (s.value <= 0 && s.maxValue <= 0) {
                s.transform.Find("Fill Area").gameObject.SetActive(false);
            } else {
                s.transform.Find("Fill Area").gameObject.SetActive(true);
                s.onValueChanged.RemoveAllListeners();
                s.onValueChanged.AddListener(val => {
                    recruitment.SetRecruitSoldierTypeNum(st, (int)val);
                    SetupRecruitSliders();
                });
            }
            counter++; // Increment per soldier type after all text fields are set
        }
    }

    private void ClickedFinalRecruitBtn() {
        // Do recruitment logic...
        DefaultState();
    }
}