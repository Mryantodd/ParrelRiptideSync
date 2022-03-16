using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer : MonoBehaviour
{
    public static Dictionary<ushort, ServerPlayer> list = new Dictionary<ushort, ServerPlayer>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }
    public ServerPlayerMovement Movement => movement;

    [SerializeField] private ServerPlayerMovement movement;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {
        foreach (ServerPlayer otherPlayer in list.Values)
            otherPlayer.SendSpawned(id);

        ServerPlayer player = Instantiate(ServerGameLogic.Singleton.PlayerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<ServerPlayer>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;

        player.SendSpawned();
        list.Add(id, player);
    }

    #region Messages
    private void SendSpawned()
    {
        ServerNetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)));
    }

    private void SendSpawned(ushort toClientId)
    {
        ServerNetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)), toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out ServerPlayer player))
            player.Movement.SetInput(message.GetBools(6), message.GetVector3());
    }
    #endregion
}
