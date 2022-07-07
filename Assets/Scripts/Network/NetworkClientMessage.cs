using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using RiptideNetworking.Transports;
using Steamworks;
using UnityEngine;

public class NetworkClientMessage : MonoBehaviour
{
    internal enum  MessageId : ushort
    {
        connected = 1,
        changeColor,
        startGame,
        ready,
        startRoundReceived,
        movement,
        shoot,
        shootReceived,
        setLife,
        death,
    }

    #region Send

    public void SendOnClientConnected(ulong steamId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.connected);
        message.AddULong(steamId);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnClientChangeColor(int colorIndex)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.changeColor);
        message.AddInt(colorIndex);
        NetworkManager.Instance.Client.Send(message);
    }
    
    public void SendOnStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.startGame);
        NetworkManager.Instance.Client.Send(message);
    }

    private static void SendOnStartRoundReceived()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.startRoundReceived);
        NetworkManager.Instance.Client.Send(message);
    }
    
    public void SendOnReady()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.ready);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnMovement(Vector3 pos)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.movement);
        message.AddVector3(pos);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendShoot(int shootId, Vector3 pos, Vector3 dir)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessageId.shoot);
        message.AddInt(shootId);
        message.AddVector3(pos);
        message.AddVector3(dir);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendShootReceived(ushort playerId, int shootId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.shootReceived);
        message.AddUShort(playerId);
        message.AddInt(shootId);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SetLife(ushort playerHitId, float life)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.setLife);
        message.AddUShort(playerHitId);
        message.AddFloat(life);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendOnDeath()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessageId.death);
        NetworkManager.Instance.Client.Send(message);
    }
    #endregion

    #region Receive
    [MessageHandler((ushort) NetworkServerMessage.MessageId.playerConnected)]
    private static void OnClientConnected(Message message)
    {
        NetworkManager.Instance.AddPlayerToLobby(message.GetUShort(), message.GetULong(), message.GetInt());
    }

    [MessageHandler((ushort) NetworkServerMessage.MessageId.cameraPos)]
    private static void OnServerChangeCameraPos(Message message)
    {
        CameraController.Instance.SetCameraPos(message.GetInt());
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.changeColor)]
    private static void OnClientChangedColor(Message message)
    {
        ushort playerId = message.GetUShort();
        int colorIndex = message.GetInt();
        
        foreach (var player in NetworkManager.Instance.Players)
        {
            if (player.Key == playerId)
            {
                player.Value.ChangeColor(colorIndex);
                return;
            }
        }
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.startGame)]
    private static void OnServerStartGame(Message message)
    {
        NetworkManager.Instance.StartGame();
    }
    
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.startRound)]
    private static void OnServerStartRound(Message message)
    {
        TimerRoundPannel.Instance.InitializeCouldown();
        SendOnStartRoundReceived();
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.movement)]
    private static void OnServerClientMovement(Message message)
    {
        ushort playerId = message.GetUShort();
        Vector3 pos = message.GetVector3();

        foreach (var player in NetworkManager.Instance.Players)
        {
            if (player.Key == playerId)
            {
                player.Value.GetComponent<PlayerClientMovementController>().Move(pos);
            }
        }
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.teleport)]
    private static void OnServerTeleportClient(Message message)
    {
        Vector3 pos = message.GetVector3();

        NetworkManager.Instance.LocalPlayer.transform.position = pos;
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.shoot)]
    private static void OnServerClientShoot(Message message)
    {
        ushort playerId = message.GetUShort();
        int shootId = message.GetInt();
        Vector3 pos = message.GetVector3();
        Vector3 dir = message.GetVector3();

        foreach (var player in NetworkManager.Instance.Players)
        {
            if (player.Key == playerId)
            {
                player.Value.GetComponent<PlayerClientFireController>().Shoot(playerId, shootId, pos, dir);
            }   
        }
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.shootReceived)]
    private static void OnServerClientShootReceived(Message message)
    {
        NetworkManager.Instance.LocalPlayer.GetComponent<PlayerLocalFireController>().ShootSended(message.GetInt());
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.setLife)]
    private static void OnServerClientSetLife(Message message)
    {
        ushort playerHitId = message.GetUShort();
        float life = message.GetFloat();
        
        Debug.Log($"ServerSetLife->PlayerId{playerHitId}:Life{life}");
        
        foreach (var player in NetworkManager.Instance.Players)
        {
            if (player.Key == playerHitId)
            {
                player.Value.GetComponent<PlayerHealthController>().Setlife(life);
            }
        }
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.endRound)]
    private static void OnServerEndRound(Message message)
    {
        bool isWin = message.GetBool();
        Debug.Log(isWin);
        EndRoundPannel.Instance.Enable(true,isWin);
    }
    #endregion
}