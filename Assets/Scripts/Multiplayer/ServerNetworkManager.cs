using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

public enum ServerToClientId : ushort
{
    playerSpawned = 1,
    playerMovement,
}

public enum ClientToServerId : ushort
{
    name = 1,
    input,
}

public class ServerNetworkManager : MonoBehaviour
{
    private static ServerNetworkManager _singleton;
    public static ServerNetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(ServerNetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Server Server { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
    }

    private void FixedUpdate()
    {
        Server.Tick();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        if (ServerPlayer.list.TryGetValue(e.Id, out ServerPlayer player))
            Destroy(player.gameObject);
    }
}
