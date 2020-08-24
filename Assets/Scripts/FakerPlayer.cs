using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class FakerPlayer : MonoBehaviour
{
    private readonly IClient client = new Client("defaultkey");
    private ISocket socket;
    private ISession session;
    private IMatchmakerTicket matchTicket;

    public Tags Tags;

    public async void CreateFakerPlayer(int index)
    {
        try
        {
            socket = client.NewSocket();
            socket.Connected += () =>
            {
                Debug.Log("Faker Connected");
            };
            socket.Closed += () =>
            {
                Debug.Log("Faker Disconnected");
            };

            socket.ReceivedError += e => Debug.LogErrorFormat("Socket error: {0}", e.Message);

            session = null;
            var deviceID = $"{SystemInfo.deviceUniqueIdentifier}-f-{index}";

            if(session == null)
            {
                session = await client.AuthenticateDeviceAsync(deviceID);
                await socket.ConnectAsync(session);
            }
            
            AddFakerPlayer();
            FakerMatchComplete();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    public async void AddFakerPlayer()
    {
        //選擇要配對的類型 *代表全部
        string query = "*";

        //遊玩最大人數
        int maxCount = 2;
        // 遊玩最小人數
        int minCount = 2;

        // 地區位置等
        var stringProperties = new Dictionary<string, string>() 
        {
            {"region", "asia"}
        };
        // 牌位,房間資訊等
        var numericProperties = new Dictionary<string, double>() 
        {
            {"Room", 2}
        };

        // nakama socket API  當加入Nakama的排隊系統時 會有一個Ticket
        matchTicket = await socket.AddMatchmakerAsync(query, minCount, maxCount, stringProperties, numericProperties);
    }

    public void FakerMatchComplete()
    {
        socket.ReceivedMatchmakerMatched += async matched =>
        {
            await socket.JoinMatchAsync(matched.MatchId);
        };
    }
}
