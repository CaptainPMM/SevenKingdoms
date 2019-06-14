using UnityEngine;
using UnityEngine.UI;

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
                //SetupRecruitSliders(); TODO...
            } else {
                if (displayedSoldiers.GetNumSoldiersInTotal() != attachedGameLocation.soldiers.GetNumSoldiersInTotal()) {
                    BasicSetup();
                }
            }
        }
    }

    void OnEnable() {
        DefaultState();
        attachedGameLocation = gameController.selectedLocation.GetComponent<GameLocation>();
        BasicSetup();
    }

    private void BasicSetup() {
        displayedSoldiers = new Soldiers();
        displayedSoldiers.AddSoldiers(attachedGameLocation.soldiers);

        int counter = 0; // To count soldier types
        foreach (Slider s in GetComponentsInChildren<Slider>()) {
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
        inRecruitState = false;

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

        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            s.onValueChanged.RemoveAllListeners();
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
                    BasicSetup();
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
        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            s.value = 0;
        }
        SetupRecruitSliders();
    }

    private void SetupRecruitSliders() {
        recruitment.Update();

        int counter = 0; // To count soldier types
        foreach (Slider s in GetComponentsInChildren<Slider>()) {
            SoldierType st = (SoldierType)counter;
            s.maxValue = recruitment.GetMaxAvailableSoldierTypeNum(st);
            if (s.maxValue <= 0) {
                s.transform.Find("Fill Area").gameObject.SetActive(false);
            } else {
                s.transform.Find("Fill Area").gameObject.SetActive(true);
                s.onValueChanged.AddListener(val => {
                    recruitment.SetRecruitSoldierTypeNum(st, (int)val);
                });
            }
            counter++; // Increment per soldier type after all text fields are set
        }
    }

    private void ClickedFinalRecruitBtn() {
        // Do recruitment logic...
        DefaultState();
        BasicSetup();
    }
}