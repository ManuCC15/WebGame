using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;

    public string game;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoom.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoom.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(game);
    }
}
