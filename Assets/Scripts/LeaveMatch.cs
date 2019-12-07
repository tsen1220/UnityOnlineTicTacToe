using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Nakama;

public class LeaveMatch : MonoBehaviour
{
    private IMatchmakerMatched matchInfo;
    private ISocket socket;
    public GameObject LeaveButton;
    private bool Victory;
    private LobbyNakamaClient nakamaClient;



    private void Awake()
    {
        nakamaClient = GameObject.FindGameObjectWithTag(Tags.Lobby.LobbyNakamaClient).GetComponent<LobbyNakamaClient>();
        socket = nakamaClient.socket;
    }

    private void Start()
    {
        matchInfo = LobbyNakamaClient.MatchInfo;
    }

    private void Update()
    {
        Victory = GameObject.FindObjectOfType<GameRun>().Victory;

        if (Victory)
        {
            LeaveButton.SetActive(true);
        }
    }




    public async void MatchLeave()
    {
        await socket.LeaveMatchAsync(matchInfo.MatchId);

        Destroy(GameObject.FindGameObjectWithTag(Tags.Lobby.LobbyNakamaClient));

        SceneManager.LoadSceneAsync(0);
    }
}
