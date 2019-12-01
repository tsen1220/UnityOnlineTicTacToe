using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;

public class GameRun : MonoBehaviour
{
    // True 為圈 False為叉
    public static bool Turn = true;
    public static int[,] GameRecord = new int[3,3];

    private LobbyNakamaClient nakamaClient;
    private IMatchmakerMatched matchInfo;



    private ISocket socket;

    private int xIndex, yIndex;



 
    private async void Awake()
    {

        nakamaClient = GameObject.FindGameObjectWithTag(Tags.Lobby.LobbyNakamaClient).GetComponent<LobbyNakamaClient>();
        socket = nakamaClient.socket;
    }

    private async void Start()
    {
        matchInfo = LobbyNakamaClient.MatchInfo;
        await socket.JoinMatchAsync(matchInfo);

        ReceiveMsg();


    }

    public async void Draw()
    {
        Text TicTac = GetComponentInChildren<Text>();

        Vector3 relativePosition = transform.position - new Vector3(352, 224, 0);


        xIndex = Mathf.RoundToInt(relativePosition.x) / 160;
        yIndex = Mathf.RoundToInt(relativePosition.y) / 160;


        if (TicTac.text != "") {
            return;
        }
        else
        {
            if (Turn)
            {
                TicTac.text = "O";
                Turn = false;
                GameRecord[xIndex, yIndex] = 1;
                var encode_data = GameRecord.ToJson();


               await socket.SendMatchStateAsync(matchInfo.MatchId, 1,encode_data);

            }
            else
            {
                TicTac.text = "X";
                Turn = true;
                GameRecord[xIndex, yIndex] = 2;
                var encode_data = GameRecord.ToJson();

                await socket.SendMatchStateAsync(matchInfo.MatchId, 2, encode_data);

            }
        }
       

    }


    private void ReceiveMsg()
    {
        socket.ReceivedMatchState += data =>
        {
            IMatchState Current = data;
            print(Current.State);
        };
    }
}

