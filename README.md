# Online TicTacToe Client

This is Online TicTacToe client made with Unity and Nakama-Unity-Client.

TicTacToe Nakama Server : https://github.com/tsen1220/LuaOnlineTicTacToeServer

We need to import Nakama and LitJson to build this app.

## Nakama Client

We need to get session and client to let the websocket connected.

First, we get the client information and create a new socket with client API : client.NewSocket().

Then, try to get the session with deviceID and Nakama Client API : client.AuthenticateDeviceAsync(deviceID).

After receiving the session, Take the session to build the socket with socket API : socket.ConnectAsync(session) .

```
    private readonly IClient client = new Client("defaultkey");
    public ISocket socket;
    private ISession session;

     if (socket != null)
        {
           await socket.CloseAsync();
        }

        socket = client.NewSocket();
        socket.Closed += () => Debug.Log("Socket closed.");
        socket.Connected += () => Debug.Log("Socket connected.");
        socket.ReceivedError += e => Debug.LogErrorFormat("Socket error: {0}", e.Message);

        var deviceId = SystemInfo.deviceUniqueIdentifier;
        session = await client.AuthenticateDeviceAsync(deviceId);
        await socket.ConnectAsync(session);

```
