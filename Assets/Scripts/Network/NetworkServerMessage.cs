using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RiptideNetworking;
using Steamworks;
using UnityEngine;

public class NetworkServerMessage : MonoBehaviour
{
    internal enum  MessageId : ushort
    {
        playerConnected = 1,
        cameraPos,
        changeColor,
        startGame,
        startRound,
        movement,
        teleport,
        shoot,
        shootReceived,
        setLife,
        endRound,
    }

    #region Send
    public static void ServerSendOnClientConnected(ushort playerConnectedId, ulong playerSteamId)
    {
        foreach (var item in NetworkManager.Instance.Players)
        {
            Message message1 = Message.Create(MessageSendMode.reliable, MessageId.playerConnected);
            message1.AddUShort(item.Value.Id);
            message1.AddULong(item.Value.SteamId);
            message1.AddInt(item.Value.ColorIndex);
            NetworkManager.Instance.Server.Send(message1, playerConnectedId);
        }
        
        Message message2 = Message.Create(MessageSendMode.reliable, MessageId.playerConnected);
        message2.AddUShort(playerConnectedId);
        message2.AddULong(playerSteamId);
        message2.AddInt(playerConnectedId);
        NetworkManager.Instance.Server.SendToAll(message2);
    }

    public void ServerChangeCameraPos(ushort id, int posIndex)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.cameraPos);
        message.Add(posIndex);
        NetworkManager.Instance.Server.Send(message, id);
    }
    
    public static void ServerSendOnClientChangedColor(ushort id, int colorIndex)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.changeColor);
        message.AddUShort(id);
        message.AddInt(colorIndex);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    private static void ServerSendOnClientStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.startGame);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    public void ServerSendStartRound()
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.startRound);
        NetworkManager.Instance.Server.SendToAll(message);
    }
    
    private static void ServerSendOnClientMovement(ushort id, Vector3 pos)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.movement);
        message.AddUShort(id);
        message.AddVector3(pos);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }

    public void ServerSendClientTeleport(ushort id, Vector3 pos)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.teleport);
        message.AddVector3(pos);
        NetworkManager.Instance.Server.Send(message, id);
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
    
    private static void ServerSendOnClientSetLife(ushort playerId, ushort playerHitId, float life)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.setLife);
        message.AddUShort(playerHitId);
        message.AddFloat(life);
        NetworkManager.Instance.Server.SendToAll(message, playerId);
    }

    public void ServerSendOnRoundEnd(ushort playerId, bool isWin)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.endRound);
        message.AddBool(isWin);
        NetworkManager.Instance.Server.Send(message, playerId);
    }
    #endregion

    #region Receive
    [MessageHandler((ushort) NetworkClientMessage.MessageId.connected)]
    private static void OnClientConnected(ushort id, Message message)
    {
        ServerSendOnClientConnected(id, message.GetULong());
    }
    
    [MessageHandler((ushort) NetworkClientMessage.MessageId.changeColor)]
    private static void OnClientChangeColor(ushort id, Message message)
    {
        ServerSendOnClientChangedColor(id, message.GetInt());
    }
    
    [MessageHandler((ushort) NetworkClientMessage.MessageId.startGame)]
    private static void OnClientStartGame(ushort id, Message message)
    {
        if (id != 1) return;
        ServerSendOnClientStartGame();
    }

    [MessageHandler((ushort) NetworkClientMessage.MessageId.ready)]
    private static void OnClientReady(ushort id, Message message)
    {
        NetworkManager.Instance.ServerReicevedPlayerReady();
    }

    private static int startRoundReceivedCout = 0;
    [MessageHandler((ushort) NetworkClientMessage.MessageId.startRoundReceived)]
    private static void OnClientStartRoundReceived(ushort id, Message message)
    {
        startRoundReceivedCout++;

        if (startRoundReceivedCout == NetworkManager.Instance.Players.ToArray().Length)
        {
            NetworkManager.Instance.SendStartRound = false;
            startRoundReceivedCout = 0;
        }
    }
    
    [MessageHandler((ushort) NetworkClientMessage.MessageId.movement)]
    private static void OnClientMovement(ushort id, Message message)
    {
        ServerSendOnClientMovement(id, message.GetVector3());
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
    
    [MessageHandler((ushort) NetworkClientMessage.MessageId.setLife)]
    private static void OnClientSetLife(ushort id, Message message)
    {
        ServerSendOnClientSetLife(id, message.GetUShort(), message.GetFloat());
    }
    
    [MessageHandler((ushort) NetworkClientMessage.MessageId.death)]
    private static void OnClientDeath(ushort id, Message message)
    {
        NetworkManager.Instance.OnClientDeath(id);
    }
    #endregion
}