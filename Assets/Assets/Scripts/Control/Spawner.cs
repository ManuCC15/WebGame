using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviourPunCallbacks
{
    public GameObject player1;
    public GameObject player2;

    public GameObject spawnPoint1;
    public GameObject spawnPoint2;

    void Start()
    {
        GameObject player;
        GameObject spawnPoint;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // First player to join
            player = player1;
            spawnPoint = spawnPoint1;
        }
        else
        {
            // Second player or more
            player = player2;
            spawnPoint = spawnPoint2;
        }

        GameObject _player = PhotonNetwork.Instantiate(player.name,new Vector3((Random.Range(-5.5f,5.5f)),1.5f,0f),Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPLayer();
    }
}
