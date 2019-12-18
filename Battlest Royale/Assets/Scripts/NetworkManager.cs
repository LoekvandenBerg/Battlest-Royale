using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // instance
    public static NetworkManager instance;

    public int maxPlayers = 10;

    private void Awake()
    {
        // set instance to this GameObject
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //connect to master server
        PhotonNetwork.ConnectUsingSettings();
    }

    //attempts to CREATE a room
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    //attempts to JOIN a room
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    //changes the scene using Photon's system
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
    // called when we disconnect from the photon server
    public override void OnDisconnected(DisconnectCause cause)
    {
        // load the menu scene
        SceneManager.LoadScene("Menu");
    }

    // called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // are we in the game scene?
        if (GameManager.instance)
        {
            // update alive players
            GameManager.instance.alivePlayers--;
            GameUI.instance.UpdatePlayerInfoText();

            // if we're the master client, check the win condition
            if (PhotonNetwork.IsMasterClient)
                GameManager.instance.CheckWinCondition();
        }

    }
}
