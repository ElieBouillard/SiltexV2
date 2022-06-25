using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using Steamworks;
using UnityEngine;

public class NetworkServerMessage : MonoBehaviour
{
    internal enum  MessageId : ushort
    {
        playerConnected = 1,
        startGame,
        movement,
    }

    #region Send
    private static void ServerSendOnClientStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.startGame);
        NetworkManager.Instance.Server.SendToAll(message);
    }
    
    public void ServerSendOnClientConnected(ushort playerConnectedId)
    {
        foreach (var item in NetworkManager.Instance.Players)
        {
            Message message1 = Message.Create(MessageSendMode.reliable, MessageId.playerConnected);
            message1.AddUShort(item.Value.Id);
            NetworkManager.Instance.Server.Send(message1, playerConnectedId);
        }
        
        Message message2 = Message.Create(MessageSendMode.reliable, MessageId.playerConnected);
        message2.AddUShort(playerConnectedId);
        NetworkManager.Instance.Server.SendToAll(message2);
    }

    private static void ServerSendOnClientMovement(ushort id, Vector3 pos, float rotY)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.movement);
        message.AddUShort(id);
        message.AddVector3(pos);
        message.AddFloat(rotY);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }
    #endregion

    #region Receive
    [MessageHandler((ushort) NetworkClientMessage.MessageId.startGame)]
    private static void OnClientStartGame(ushort id, Message message)
    {
        if (id != 1) return;
        ServerSendOnClientStartGame();
    }

    [MessageHandler((ushort) NetworkClientMessage.MessageId.movement)]
    private static void OnClientMovement(ushort id, Message message)
    {
        ServerSendOnClientMovement(id, message.GetVector3(), message.GetFloat());
    }
    #endregion
}