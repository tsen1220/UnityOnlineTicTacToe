using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using LitJson;

public class GameRun : MonoBehaviour
{
    // True 為圈 False為叉

    // opCode= 1 : 客戶端有人下了O;
    // opCode= 2 : 客戶端有人下了X;
    // opCode= 3 : 伺服器端送訊息給客戶端;
    // opCode= 4 : 伺服器端在賽局開始時決定先攻後守;

    public static bool Turn = true;
    public static int[,] GameRecord = new int[3,3];

    private LobbyNakamaClient nakamaClient;
    private IMatchmakerMatched matchInfo;

    public static bool drawControl = true;

    private ISocket socket;

    private int xIndex, yIndex;

    private Text test;



 
    private async void Awake()
    {

        nakamaClient = GameObject.FindGameObjectWithTag(Tags.Lobby.LobbyNakamaClient).GetComponent<LobbyNakamaClient>();
        socket = nakamaClient.socket;

        test = GameObject.FindGameObjectWithTag("Test").GetComponent<Text>();
    }

    private async void Start()
    {   
        matchInfo = LobbyNakamaClient.MatchInfo;
        await socket.JoinMatchAsync(matchInfo);

        await socket.SendMatchStateAsync(matchInfo.MatchId, 4, "GameStart");

        ReceiveData();
    }


    private void Update()
    {
        for(int i = 0; i<3;i++)
        {
            for(int j = 0; j < 3; j++)
            {
                GameObject chooseObject = GameObject.FindGameObjectWithTag($"Cell{i}{j}");

                Text chooseTicTac = chooseObject.GetComponentInChildren<Text>();

                if (GameRecord[i,j] == 1)
                {
                    chooseTicTac.text = "O";
                }
                else if (GameRecord[i,j] == 2)
                {
                    chooseTicTac.text = "X";
                }
            }
        }
        test.text = drawControl.ToString();
    }


    public async void Draw()
    {
        Text TicTac = GetComponentInChildren<Text>();

        Vector3 relativePosition = transform.position - new Vector3(352, 224, 0);


        xIndex = Mathf.RoundToInt(relativePosition.x) / 160;
        yIndex = Mathf.RoundToInt(relativePosition.y) / 160;


        if (drawControl)
        {
            return;
        }
        else
        {
            if (GameRecord[xIndex,yIndex] !=0)
            {
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

                    drawControl = true;


                    //使用Nakama的TinyJson       using Nakama.TinyJson
                    await socket.SendMatchStateAsync(matchInfo.MatchId, 1, drawPostion.ToJson());

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

                    drawControl = true;

                    //使用Nakama的TinyJson       using Nakama.TinyJson

                    await socket.SendMatchStateAsync(matchInfo.MatchId, 2, drawPostion.ToJson());

                }
            }
        }
       
       

    }


    // 使用LitJson來解析Json．

    private void ReceiveData()
    {
        socket.ReceivedMatchState += data =>
        {
            if(data.OpCode == 3)
            {
                var RecordData = System.Text.Encoding.UTF8.GetString(data.State);
                int xindex = (RecordData[1]) - 48;
                int yindex = (RecordData[3]) - 48;
                int TicTac = (RecordData[5]) - 48;

                GameRecord[xindex, yindex] = TicTac; 
                drawControl = false;
            }

            if (data.OpCode == 5)
            {
                string RecordData = System.Text.Encoding.UTF8.GetString(data.State);
                JsonData theData = JsonMapper.ToObject(RecordData);
                if (theData["control"].ToString() == "False")
                {
                    drawControl = false;
                }

            }

        };
    }
}




