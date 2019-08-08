using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HouseSelMenu : MonoBehaviour {
    public Image houseFlagImage;
    public TextMeshProUGUI houseNameText;
    public Slider houseSelSlider;
    public Slider aiDiffSlider;
    private TextMeshProUGUI aiDiffText;

    public GameObject mpPanel;

    public TMP_InputField inputIP;
    public Button btnConnect;

    public TextMeshProUGUI textHostStatus;
    public Button btnHost;
    public GameObject textIP;
    public TextMeshProUGUI textCurrIP;
    public GameObject networkManagerPrefab;

    public House selHouse;
    private bool mpHosting = false;
    private bool mpConnected = false;
    private Multiplayer.NetworkManager networkManager;

    private Multiplayer.NetworkManager.ConnectionEstablished hostEstablishedHandler;
    private Multiplayer.NetworkManager.ConnectionFailed hostFailedHandler;

    private Multiplayer.NetworkManager.ConnectionEstablished clientConnEstablishedHandler;
    private Multiplayer.NetworkManager.ConnectionFailed clientConnFailedHandler;

    private void Start() {
        // Set sliders
        houseSelSlider.minValue = 1; // Ingore neutral (index 0)
        houseSelSlider.maxValue = System.Enum.GetValues(typeof(HouseType)).Length - 1;
        houseSelSlider.value = houseSelSlider.maxValue / 2;

        aiDiffSlider.minValue = 0;
        aiDiffSlider.maxValue = System.Enum.GetValues(typeof(AIDifficulty)).Length - 1;
        aiDiffSlider.value = 1; // Default to NORMAL
        aiDiffText = aiDiffSlider.transform.parent.Find("Text AI Slider").GetComponent<TextMeshProUGUI>();

        mpPanel.SetActive(false);
        textHostStatus.text = "Not hosting";
        textIP.SetActive(false);
        textCurrIP.gameObject.SetActive(false);

        ChangeSelHouse(houseSelSlider.value);
        ChangeAIDiff(aiDiffSlider.value);

        // Register connection event handlers
        // Hosting
        hostEstablishedHandler = new Multiplayer.NetworkManager.ConnectionEstablished(HostingSuccessfull);
        Multiplayer.Server.OnConnectionEstablished += hostEstablishedHandler;
        hostFailedHandler = new Multiplayer.NetworkManager.ConnectionFailed(HostingFailed);
        Multiplayer.Server.OnConnectionFailed += hostFailedHandler;
        // Connecting
        clientConnEstablishedHandler = new Multiplayer.NetworkManager.ConnectionEstablished(ConnSuccessfull);
        Multiplayer.Client.OnConnectionEstablished += clientConnEstablishedHandler;
        clientConnFailedHandler = new Multiplayer.NetworkManager.ConnectionFailed(ConnFailed);
        Multiplayer.Client.OnConnectionFailed += clientConnFailedHandler;
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

    public void OpenMultiplayerPanel() {
        SoundManager.Play(SoundManager.SoundType.UI, "button_big");
        mpPanel.SetActive(true);
    }

    public void BtnHostClick() {
        if (!mpConnected) {
            btnHost.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            if (!mpHosting) {
                networkManager = Instantiate(networkManagerPrefab).GetComponent<Multiplayer.NetworkManager>();
                networkManager.name = "Network Manager";
                networkManager.InitServer();
            } else {
                RemoveNetworkManager();

                mpHosting = false;
                SoundManager.Play(SoundManager.SoundType.UI, "button1");
                textIP.SetActive(false);
                textCurrIP.gameObject.SetActive(false);
                textHostStatus.text = "Not hosting";
                btnHost.GetComponentInChildren<TextMeshProUGUI>().text = "Host";

                inputIP.interactable = true;
                btnConnect.interactable = true;
            }
        }
    }

    private void HostingSuccessfull() {
        mpHosting = true;
        SoundManager.Play(SoundManager.SoundType.UI, "button_select");
        textCurrIP.text = Multiplayer.NetworkManager.GetLocalIP();
        textIP.SetActive(true);
        textCurrIP.gameObject.SetActive(true);
        textHostStatus.text = "Hosting (0 connected)";
        btnHost.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";

        inputIP.interactable = false;
        btnConnect.interactable = false;
    }

    private void HostingFailed() {
        RemoveNetworkManager();

        SoundManager.Play(SoundManager.SoundType.UI, "button1");
        textHostStatus.text = "Failed";
        btnHost.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
    }

    public void BtnConnectClick() {
        if (!mpHosting) {
            if (inputIP.text != "") {
                btnConnect.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                if (!mpConnected) {
                    networkManager = Instantiate(networkManagerPrefab).GetComponent<Multiplayer.NetworkManager>();
                    networkManager.name = "Network Manager";
                    networkManager.InitClient(inputIP.text);
                } else {
                    RemoveNetworkManager();

                    mpConnected = false;
                    SoundManager.Play(SoundManager.SoundType.UI, "button1");

                    inputIP.interactable = true;
                    btnConnect.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";

                    btnHost.interactable = true;
                }
            } else {
                SoundManager.Play(SoundManager.SoundType.UI, "button1");
                btnConnect.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
        }
    }

    private void ConnSuccessfull() {
        mpConnected = true;
        SoundManager.Play(SoundManager.SoundType.UI, "button_select");

        inputIP.interactable = false;
        btnConnect.GetComponentInChildren<TextMeshProUGUI>().text = "Disconnect";

        btnHost.interactable = false;
    }

    private void ConnFailed() {
        RemoveNetworkManager();

        SoundManager.Play(SoundManager.SoundType.UI, "button1");
        inputIP.text = "Failed";

        TextMeshProUGUI btnText = btnConnect.GetComponentInChildren<TextMeshProUGUI>();
        btnText.color = Color.red;
        btnText.text = "Connect";

        mpConnected = false;
        inputIP.interactable = true;
        btnHost.interactable = true;
    }

    public void RemoveNetworkManager() {
        if (networkManager != null) {
            Destroy(networkManager.gameObject);
            networkManager = null;
        }
    }

    private void OnDestroy() {
        Multiplayer.Server.OnConnectionEstablished -= hostEstablishedHandler;
        Multiplayer.Server.OnConnectionFailed -= hostFailedHandler;

        Multiplayer.Client.OnConnectionEstablished -= clientConnEstablishedHandler;
        Multiplayer.Client.OnConnectionFailed -= clientConnFailedHandler;
    }
}