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
        AssignTeamAndSpawnPlayer();
    }

    private void AssignTeamAndSpawnPlayer()
    {
        // Determinar el equipo en base al orden de conexión
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber; // Identificador único del jugador
        bool isTeam1 = playerIndex % 2 != 0; // Jugadores con índice impar -> equipo 1, par -> equipo 2

        // Seleccionar prefab y punto de spawn
        GameObject playerPrefab = isTeam1 ? playerPrefabTeam1 : playerPrefabTeam2;
        Transform spawnPoint = isTeam1 ? GetRandomSpawnPoint(spawnPointsTeam1) : GetRandomSpawnPoint(spawnPointsTeam2);

        // Instanciar el jugador en la red
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        // Configurar el jugador local (puedes añadir más lógica según sea necesario)
        player.GetComponent<PlayerSetup>().IsLocalPLayer();
    }

    private Transform GetRandomSpawnPoint(Transform[] spawnPoints)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn configurados.");
            return null;
        }

        // Elegir un punto de spawn aleatorio
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
