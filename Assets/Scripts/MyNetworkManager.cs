using Mirror;
using UnityEngine;
using VContainer;

public class MyNetworkManager : NetworkManager
{
    [Inject]
    private NetworkMessagesService _messagesService;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        _messagesService.StartServer();
        _messagesService.ClientSubscribed += OnClientSubscribed;
    }
    
    private void OnClientSubscribed(NetworkConnectionToClient connection, ushort messageId)
    {
        ushort helloMessageId = NetworkMessages.GetId<HelloMessage>();

        if (messageId != helloMessageId)
            return;
        
        HelloMessage message = new HelloMessage
        {
            Text = "Hello Client!"
        };

        _messagesService.Send(connection, message);
    }
    
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        _messagesService.Subscribe<HelloMessage>(OnHelloMessage);
    }

    private void OnHelloMessage(HelloMessage message)
    {
        Debug.Log(message.Text);
    }
}