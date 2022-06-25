using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using Steamworks;
using UnityEngine;

public class NetworkClientMessage : MonoBehaviour
{
    internal enum  MessageId : ushort
    {
        startGame = 1,
        movement,
    }

    #region Send
    public void SendOnStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.startGame);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnMovement(Vector3 pos, float rotY)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.movement);
        message.AddVector3(pos);
        message.AddFloat(rotY);
        NetworkManager.Instance.Client.Send(message);
    }
    #endregion

    #region Receive
    [MessageHandler((ushort) NetworkServerMessage.MessageId.playerConnected)]
    private static void OnClientConnected(Message message)
    {
        NetworkManager.Instance.AddPlayerToLobby(message.GetUShort());
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.startGame)]
    private static void OnServerStartGame(Message message)
    {
        NetworkManager.Instance.StartGame();
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.movement)]
    private static void OnServerClientMovement(Message message)
    {
        ushort playerId = message.GetUShort();
        Vector3 pos = message.GetVector3();
        float rotY = message.GetFloat();

        foreach (var player in NetworkManager.Instance.Players)
        {
            if (player.Key == playerId)
            {
                player.Value.GetComponent<PlayerClientMovementController>().Move(pos, rotY);
            }
        }
    }
    #endregion
}