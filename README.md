# Online TicTacToe Client

This is Online TicTacToe client made with Unity and Nakama-Unity-Client.

TicTacToe Nakama Server : https://github.com/tsen1220/LuaOnlineTicTacToeServer

We need to import Nakama and LitJson to build this app.

## Nakama Client Connect

We need to get session and client to let the websocket connected.

```
    ...
    using Nakama;
    ...
    ...
```

First, we get the client information and create a new socket with client API : client.NewSocket().

Then, try to get the session with deviceID and Nakama Client API : client.AuthenticateDeviceAsync(deviceID).

After receiving the session, Take the session to build the socket with socket API : socket.ConnectAsync(session) .

```

    private readonly IClient client = new Client("defaultkey");
    public ISocket socket;
    private ISession session;
    
    private async void Awake()
        {
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

        }

```

## Add And Remove Nakama Matchmaker Process

When our socket has built, we can add to Nakama matchmaker pool and then get a matchmakerticket with socket API : 

socket.AddMatchmakerAsync(query, minCount, maxCount, stringProperties, numericProperties) .

```

IMatchmakerTicket matchTicket= await socket.AddMatchmakerAsync(query, minCount, maxCount, stringProperties, numericProperties);


query: The match which you search contains requirements or rules. "*" means regardless of requirements.

minCount: A match can start with minimum number of people.

minCount: A match contains maximum number of people.

stringProperties: Dictionary structure. The match extra information which value contains string.

numericProperties: Dictionary structure. The match extra information which value contains number.

```

If we wish to leave the matchmaker, we can use socket API :  socket.RemoveMatchmakerAsync(matchTicket).

This action will remove your matchTicket and leave the matchmaker pool.

```
    {

    await socket.RemoveMatchmakerAsync(matchTicket);

    matchTicket: Get after adding to matchmaker pool.
     
    }
```

## Received Matchmaker Matched And Join Match

Client has the event listener to handle the information When users in Nakama matchmaker pool are matched.

This part is associated with Nakama Server match_create with lua.

With socket API : socket.ReceivedMatchmakerMatched += matchedInformation => callback() .

```

socket.ReceivedMatchmakerMatched += matched =>
        {
           IMatchmakerMatched  MatchInfo = matched;
        };

        matched : Match information which contains MatchID.

```

And players can join the match after receiving the match information from socket API : socket.ReceivedMatchmakerMatched.

This part is associated with Nakama Server match_join_attempt and match_join with lua.

```
     await socket.JoinMatchAsync(matchInfo.MatchId);
```

## Match Running

This part is associated with Nakama Server match_loop with lua.

We can send json_encode_data about match to Nakama Server with socket API : socket.SendMatchStateAsync( MatchId, opCode, json_encode_data );

```
       await socket.SendMatchStateAsync(matchInfo.MatchId, 1, drawPostion.ToJson());
```

And Receive the Nakama server data with socket API : socket.ReceivedMatchState += data => callback().


```
        socket.ReceivedMatchState += data =>
        {
            if(data.OpCode == 3)
            {
              ...
              ...
              ...
              ...
              ...
            }

            if (data.OpCode == 5)
            {
               ...
               ...
               ...
               ...
               ...
            }

        };

        data : data from Nakama Server.
```

## Match Leave

When game is over, player can click leave button to leave the match.

Use socket API : socket.LeaveMatchAsync(MatchId);

```

await socket.LeaveMatchAsync(matchInfo.MatchId);

```