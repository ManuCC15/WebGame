using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefabTeam1; // Prefab para el equipo 1
    public GameObject playerPrefabTeam2; // Prefab para el equipo 2

    public Transform[] spawnPointsTeam1; // Puntos de spawn para el equipo 1
    public Transform[] spawnPointsTeam2; // Puntos de spawn para el equipo 2

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // Solo el Master Client instancia los jugadores
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        // Obtener el equipo del jugador
        string team = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();

        // Elegir el prefab y el punto de spawn según el equipo
        GameObject playerPrefab = team == "A" ? playerPrefabTeam1 : playerPrefabTeam2;
        Transform spawnPoint = team == "A" ? GetRandomSpawnPoint(spawnPointsTeam1) : GetRandomSpawnPoint(spawnPointsTeam2);

        // Instanciar al jugador en la red
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    private Transform GetRandomSpawnPoint(Transform[] spawnPoints)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn configurados.");
            return null;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}

