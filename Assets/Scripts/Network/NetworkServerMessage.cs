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
        shoot,
        shootReceived,
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

    private static void ServerSendOnClientShoot(ushort id, int shootId, Vector3 pos, Vector3 dir)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.shoot);
        message.AddUShort(id);
        message.AddInt(shootId);
        message.AddVector3(pos);
        message.AddVector3(dir);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    private static void ServerSendOnClientShootReceived(ushort playerId, int shootId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.shootReceived);
        message.AddInt(shootId);
        NetworkManager.Instance.Server.Send(message, playerId);
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
    
    [MessageHandler((ushort) NetworkClientMessage.MessageId.shoot)]
    private static void OnClientShoot(ushort id, Message message)
    {
        ServerSendOnClientShoot(id, message.GetInt(), message.GetVector3(), message.GetVector3());
    }
    
    [MessageHandler((ushort) NetworkClientMessage.MessageId.shootReceived)]
    private static void OnClientShootReceived(ushort id, Message message)
    {
        ServerSendOnClientShootReceived(message.GetUShort(), message.GetInt());
    }
    #endregion
}