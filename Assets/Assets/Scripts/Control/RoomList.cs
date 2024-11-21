using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject roomPrefab;
    public GameObject[] AllRoom;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < AllRoom.Length; i++)
        {
            if (AllRoom[i] != null)
            {
                Destroy(AllRoom[i]);
            }
        }

        AllRoom = new GameObject[roomList.Count];

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].IsOpen && roomList[i].IsVisible && roomList[i].PlayerCount >= 1) 
            {
                GameObject Room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Content").transform);
                Room.GetComponent<Room>().roomName.text = roomList[i].Name;

                AllRoom[i] = Room;
            }
            
        }
    }
    public void RefreshRoomList()
    {
        Debug.Log("Solicitando actualización manual de la lista de salas...");
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby(); // Asegúrate de estar en un lobby
            Debug.Log("Unido al lobby...");
        }
        else
        {
            Debug.LogWarning("No conectado a Photon. Conéctate antes de unirte al lobby.");
        }
    }

    // Método para testear el botón de refresco manual
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RefreshRoomList(); // Presiona R para forzar actualización manual
        }
    }
}
