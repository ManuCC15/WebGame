using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;

    public string game;

    public void CreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(createRoom.text,new RoomOptions() { MaxPlayers = 4, IsVisible = true, IsOpen = true },TypedLobby.Default,null);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoom.text);
    }

    public void JoinRoomInList(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(game);
    }
}
