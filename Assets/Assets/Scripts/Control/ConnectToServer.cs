using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public string sceneName;

    void Start()
    {
        // Verifica si ya está conectado a Photon
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Ya conectado a Photon. Intentando unirse al lobby...");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.Log("Conectándose a Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(sceneName);
    }
    
}
