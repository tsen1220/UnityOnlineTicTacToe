using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
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


                // 0: xIndex 1:yIndex 2:下的種類

                int[] drawPostion = new int[3];
                drawPostion[0] = xIndex;
                drawPostion[1] = yIndex;
                drawPostion[2] = 1;

                

               await socket.SendMatchStateAsync(matchInfo.MatchId, 1, drawPostion.ToJson() );

            }
            else
            {
                TicTac.text = "X";
                Turn = true;
                GameRecord[xIndex, yIndex] = 2;

                int[] drawPostion = new int[3];
                drawPostion[0] = xIndex;
                drawPostion[1] = yIndex;
                drawPostion[2] = 2;

                await socket.SendMatchStateAsync(matchInfo.MatchId, 2, drawPostion.ToJson() );

            }
        }
       

    }


    private void ReceiveMsg()
    {
        socket.ReceivedMatchState += data =>
        {
            var RecordData=  System.Text.Encoding.UTF8.GetString(data.State);
            int xindex = (RecordData[1])-48;
            int yindex = (RecordData[3])-48;
            int TicTac = (RecordData[5])-48;

            Debug.Log(GameRecord[xindex, yindex]);

            Debug.Log($"xindex:{xindex},yindex:{yindex},TicTac:{TicTac}");

        };
    }
}

