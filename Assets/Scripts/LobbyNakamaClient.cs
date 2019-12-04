using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;

public class LobbyNakamaClient : MonoBehaviour
{
    private readonly IClient client = new Client("defaultkey");
    public ISocket socket;
    private ISession session;
    private IMatchmakerTicket matchTicket;

    public GameObject playMatch, QuitMatch, WaitMsg, Title;

    public Tags Tags;

    public bool readyToGo = false;

    public static IMatchmakerMatched MatchInfo;
  


    private async void Awake()
    {

        //物件顯示以及隱藏
        QuitMatch.SetActive(false);
        WaitMsg.SetActive(false);



        if (socket != null)
        {
           await socket.CloseAsync();
        }

        socket = client.NewSocket();
        socket.Closed += () => Debug.Log("Socket closed.");
        socket.Connected += () => Debug.Log("Socket connected.");
        socket.ReceivedError += e => Debug.LogErrorFormat("Socket error: {0}", e.Message);

        var deviceId = $"SystemInfo.deviceUniqueIdentifier-f-{5}";
        session = await client.AuthenticateDeviceAsync(deviceId);
        await socket.ConnectAsync(session);



        // Nakama配對監聽器
        matchComplete();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (readyToGo)
        {
            SceneManager.LoadScene(1);
            readyToGo = false;
        }
    
    }



    public async void  addGameMatchList()
    {
        //選擇要配對的類型 *代表全部
        string query = "*";

        //遊玩最大人數
        int maxCount = 2;
        // 遊玩最小人數
        int minCount = 2;

        // 地區位置等
        var stringProperties = new Dictionary<string, string>() {
  {"region", "asia"}
};
        // 牌位,房間資訊等
        var numericProperties = new Dictionary<string, double>() {
  {"Room", 2}
};

        // nakama socket API  當加入Nakama的排隊系統時 會有一個Ticket
        matchTicket= await socket.AddMatchmakerAsync(query, minCount, maxCount, stringProperties, numericProperties);


        playMatch.SetActive(false);
        Title.SetActive(false);
        QuitMatch.SetActive(true);
        WaitMsg.SetActive(true);

    }




    public async void CancelGameMatchList()
    {

        await socket.RemoveMatchmakerAsync(matchTicket);
        playMatch.SetActive(true);
        Title.SetActive(true);
        QuitMatch.SetActive(false);
        WaitMsg.SetActive(false);
        
    }


    /// <summary>
    /// 配對完成就會將Ticket 刪除
    ///
    /// </summary>
    public void matchComplete()
    {
        socket.ReceivedMatchmakerMatched += matched =>
        {
            MatchInfo = matched;
            readyToGo = true;
        };
    }

}
