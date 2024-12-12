using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isResourceNode;
    public string resourceType;
    public int resourceAmountPerCycle = 1;
    public float gatherInterval = 5f;

    public bool isCraftingStation;
    public bool isCraftingSoldier;
    public string requiredResource1;
    public int requiredAmount1;
    public string requiredResource2;
    public int requiredAmount2;
    public GameObject craftedPrefab;
    public GameObject soldierPrefab;

    public Transform spawnLocation;

    public bool isPlayerGathering;
    private PhotonView photonView;

    // Diccionario estático para rastrear los Archers por equipo
    private static Dictionary<string, GameObject> existingArchers = new Dictionary<string, GameObject>();


    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public bool IsGathering()
    {
        return isPlayerGathering;
    }

    public void StartGathering()
    {
        if (isResourceNode && !isPlayerGathering)
        {
            isPlayerGathering = true;
            Debug.Log("Iniciando recolección de recursos...");
            InvokeRepeating(nameof(GatherResource), gatherInterval, gatherInterval);
        }
    }
    
    public void StopGathering()
    {
        if (isResourceNode && isPlayerGathering)
        {
            isPlayerGathering = false;
            Debug.Log("Deteniendo recolección de recursos...");
            CancelInvoke(nameof(GatherResource));
        }
    }
    
    void GatherResource()
    {
        string team = GetPlayerTeam(); // Obtén el equipo del jugador
        if (!string.IsNullOrEmpty(team))
        {
            InventoryManager.Instance.AddResource(resourceType, team, resourceAmountPerCycle);
        }
        else
        {
            Debug.LogWarning("El equipo del jugador no está definido. No se puede recolectar el recurso.");
        }
    }

      public void CraftItem()
    {
        string team = GetPlayerTeam(); // Obtén el equipo del jugador
        if (!string.IsNullOrEmpty(team) &&
            InventoryManager.Instance.HasEnoughResource(requiredResource1, team, requiredAmount1) &&
            InventoryManager.Instance.HasEnoughResource(requiredResource2, team, requiredAmount2))
        {
            if (photonView.IsMine)
            {
                if (existingArchers.ContainsKey(team))
                {
                    Debug.LogWarning($"Ya existe un Archer para el equipo {team}. No se puede crear otro.");
                    return; // No continuar si ya existe un Archer
                }

                // Consumir recursos en el propietario
                InventoryManager.Instance.ConsumeResource(requiredResource1, team, requiredAmount1);
                InventoryManager.Instance.ConsumeResource(requiredResource2, team, requiredAmount2);

                // Notificar al resto que debe crear el Archer
                photonView.RPC("CraftAndSpawnPrefab", RpcTarget.Others, team);
            }
        }

        else
        {
            Debug.LogWarning("No hay suficientes recursos o el equipo del jugador no está definido.");
        }
    }

    [PunRPC]
    void CraftAndSpawnPrefab(string team)
    {
        if (craftedPrefab != null)
        {
            if (existingArchers.ContainsKey(team))
            {
                Debug.LogWarning($"Ya existe un Archer para el equipo {team}. No se puede crear otro.");
                return;
            }

            // Instanciar el Archer y registrarlo en el diccionario
            GameObject newObject = PhotonNetwork.Instantiate(craftedPrefab.name, spawnLocation.position, spawnLocation.rotation);
            existingArchers[team] = newObject;
        }
    }

    public static void ClearArcherReference(string team, GameObject archer)
    {
        if (existingArchers.TryGetValue(team, out GameObject existingArcher) && existingArcher == archer)
        {
            existingArchers.Remove(team); // Limpia la referencia si coincide
        }
    }
    

    private string GetPlayerTeam()
    {
        // Obtén el equipo del jugador desde las propiedades personalizadas de Photon.
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team))
        {
            Debug.Log(team);
            return team as string;
        }
        else
        {
            return null;
        }

    }

    private string AssingTeam()
    {
        string team = "A";
        return team;
    }

    public void StoreSoldier()
    {
        string team = GetPlayerTeam(); // Obtén el equipo del jugador
        if (!string.IsNullOrEmpty(team) &&
            InventoryManager.Instance.HasEnoughResource(requiredResource1, team, requiredAmount1) &&
            InventoryManager.Instance.HasEnoughResource(requiredResource2, team, requiredAmount2))
        {
            InventoryManager.Instance.ConsumeResource(requiredResource1, team, requiredAmount1);
            InventoryManager.Instance.ConsumeResource(requiredResource2, team, requiredAmount2);

            // Almacenar el soldado en el InventoryManager
            InventoryManager.Instance.StoreSoldier(team, soldierPrefab);
        }
        else
        {
            Debug.LogWarning("No hay suficientes recursos para almacenar un soldado.");
        }
    }
}









