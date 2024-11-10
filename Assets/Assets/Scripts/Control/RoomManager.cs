using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public Transform spanwPoint;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("Game",null,null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject _player = PhotonNetwork.Instantiate(player.name,spanwPoint.position,Quaternion.identity);

    }
}
