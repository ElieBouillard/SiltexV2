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
        shoot,
        shootReceived,
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
        
        Debug.Log($"ShootReceived{shootId}");
    }
    
    [MessageHandler((ushort) NetworkServerMessage.MessageId.shootReceived)]
    private static void OnServerClientShootReceived(Message message)
    {
        NetworkManager.Instance.LocalPlayer.GetComponent<PlayerLocalFireController>().ShootSended(message.GetInt());
    }
    #endregion
}