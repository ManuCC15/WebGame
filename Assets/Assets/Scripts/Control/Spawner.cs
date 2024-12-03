using System.Collections;
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
        // Cada cliente es responsable de instanciar su propio jugador
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // Validar que el jugador tenga un equipo asignado
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            Debug.LogError("El jugador no tiene un equipo asignado en CustomProperties.");
            return;
        }

        // Obtener el equipo del jugador
        string team = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();

        // Elegir el prefab y el punto de spawn según el equipo
        GameObject playerPrefab = team == "A" ? playerPrefabTeam1 : playerPrefabTeam2;
        Transform spawnPoint = team == "A" ? GetRandomSpawnPoint(spawnPointsTeam1) : GetRandomSpawnPoint(spawnPointsTeam2);

        // Instanciar el jugador en la red
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        // Configurar el jugador local
        PlayerSetup playerSetup = player.GetComponent<PlayerSetup>();
        if (playerSetup != null && player.GetComponent<PhotonView>().IsMine)
        {
            playerSetup.IsLocalPlayer();
        }
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


