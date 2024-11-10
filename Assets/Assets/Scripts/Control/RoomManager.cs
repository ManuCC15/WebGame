using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public GameObject player2;
    public Transform spawnPoint;
    public GameObject canvas;

    CreateAndJoin CreateAndJoin;
    void Start()
    {
        CreateAndJoin = GetComponent<CreateAndJoin>();
        PhotonNetwork.ConnectUsingSettings();
        canvas.SetActive(true);
    }

    //public override void OnConnectedToMaster()
    //{
    //    base.OnConnectedToMaster();

    //    PhotonNetwork.JoinLobby();
    //}

    //public override void OnJoinedLobby()
    //{
    //    base.OnJoinedLobby();

    //    PhotonNetwork.JoinOrCreateRoom(null,new RoomOptions() {MaxPlayers = 2},TypedLobby.Default);
    //}

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("2");

        //GameObject selectedPlayerPrefab;

        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    // First player to join
        //    selectedPlayerPrefab = player;
        //}
        //else
        //{
        //    // Second player or more
        //    selectedPlayerPrefab = player2;
        //}

        PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);

        canvas.SetActive(false);
    }
}
