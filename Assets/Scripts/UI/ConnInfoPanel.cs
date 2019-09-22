using UnityEngine;
using TMPro;
using Multiplayer;

public class ConnInfoPanel : MonoBehaviour {
    public static ConnInfoPanel instance;

    public GameObject panel;
    public TextMeshProUGUI infoText;
    private const float DISPLAY_TIME = 4f; // in sec

    private NetworkManager.ConnectionEstablished clientConnEstablishedHandler;
    private NetworkManager.ConnectionFailed clientConnFailedHandler;

    private NetworkManager.ConnectionEstablished hostingSuccessHandler;
    private NetworkManager.ConnectionFailed hostingFailedHandler;

    private NetworkManager.ConnectionEstablished serverClientConnectHandler;
    private NetworkManager.ConnectionFailed serverClientDisconnectHandler;

    private void Awake() {
        instance = this;
        panel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start() {
        // Register events handlers
        clientConnEstablishedHandler = new NetworkManager.ConnectionEstablished(ClientConnEstablishedHandler);
        clientConnFailedHandler = new NetworkManager.ConnectionFailed(ClientConnFailedHandler);

        hostingSuccessHandler = new NetworkManager.ConnectionEstablished(HostingSuccessHandler);
        hostingFailedHandler = new NetworkManager.ConnectionFailed(HostingFailedHandler);

        serverClientConnectHandler = new NetworkManager.ConnectionEstablished(ServerClientConnectHandler);
        serverClientDisconnectHandler = new NetworkManager.ConnectionFailed(ServerClientDisconnectHandler);

        Client.OnConnectionEstablished += clientConnEstablishedHandler;
        Client.OnConnectionFailed += clientConnFailedHandler;

        Server.OnConnectionEstablished += hostingSuccessHandler;
        Server.OnConnectionFailed += hostingFailedHandler;

        Server.OnClientConnect += serverClientConnectHandler;
        Server.OnClientDisconnect += serverClientDisconnectHandler;
    }

    private void ClientConnEstablishedHandler() {
        ShowPanel("Connected to host");
    }

    private void ClientConnFailedHandler() {
        ShowPanel("Connection to host failed");
    }

    private void HostingSuccessHandler() {
        ShowPanel("Hosting started");
    }

    private void HostingFailedHandler() {
        ShowPanel("Hosting failed");
    }

    private void ServerClientConnectHandler() {
        ShowPanel($"Client connected (now {Server.instance.clients.Count})");
    }

    private void ServerClientDisconnectHandler() {
        ShowPanel($"Client lost (now {Server.instance.clients.Count})");
    }

    public void ShowPanel(string msg) {
        infoText.text = msg;
        panel.SetActive(true);
        SoundManager.Play(SoundManager.SoundType.UI, "slider_click");
        StartCoroutine("DisplayOverTime");
    }

    private System.Collections.IEnumerator DisplayOverTime() {
        yield return new WaitForSeconds(DISPLAY_TIME);
        panel.SetActive(false);
    }

    private void OnDestroy() {
        Client.OnConnectionEstablished -= clientConnEstablishedHandler;
        Client.OnConnectionFailed -= clientConnFailedHandler;

        Server.OnConnectionEstablished -= hostingSuccessHandler;
        Server.OnConnectionFailed -= hostingFailedHandler;

        Server.OnClientConnect -= serverClientConnectHandler;
        Server.OnClientDisconnect -= serverClientDisconnectHandler;
    }
}
