using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMessagesService
{
    private readonly Dictionary<int, HashSet<ushort>> _subscriptions = new();
    
    public event Action<NetworkConnectionToClient, ushort> ClientSubscribed;
    
    public void StartServer()
    {
        NetworkServer.RegisterHandler<SubscriptionMessage>(OnSubscriptionMessage, false);
    }
    
    private void OnSubscriptionMessage(NetworkConnectionToClient connection, SubscriptionMessage message)
    {
        if (!_subscriptions.ContainsKey(connection.connectionId))
        {
            _subscriptions[connection.connectionId] = new HashSet<ushort>();
        }

        _subscriptions[connection.connectionId].Add(message.MessageId);
        
        Debug.Log($"SERVER: Client {connection.connectionId} subscribed to message {message.MessageId}");
        
        ClientSubscribed?.Invoke(connection, message.MessageId);
    }
    
    public void Subscribe<T>(Action<T> handler) where T : struct, NetworkMessage
    {
        NetworkClient.RegisterHandler<T>(handler);
        ushort messageId = NetworkMessages.GetId<T>();
        NetworkClient.Send(new SubscriptionMessage { MessageId = messageId });
    }
    
    public void Send<T>(NetworkConnectionToClient connection, T message) where T : struct, NetworkMessage
    {
        ushort messageId = NetworkMessages.GetId<T>();

        if (!_subscriptions.ContainsKey(connection.connectionId))
            return;

        if (!_subscriptions[connection.connectionId].Contains(messageId))
            return;

        connection.Send(message);
    }
}

