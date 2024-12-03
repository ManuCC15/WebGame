using Photon.Pun;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isResourceNode;
    public string resourceType;
    public int resourceAmountPerCycle = 1;
    public float gatherInterval = 5f;

    public bool isCraftingStation;
    public string requiredResource1;
    public int requiredAmount1;
    public string requiredResource2;
    public int requiredAmount2;
    public GameObject craftedPrefab;

    public Transform spawnLocation;

    private bool isPlayerGathering;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void StartGathering()
    {
        if (isResourceNode)
        {
            isPlayerGathering = true;
            InvokeRepeating(nameof(GatherResource), 0f, gatherInterval);
        }
    }

    public void StopGathering()
    {
        if (isResourceNode && isPlayerGathering)
        {
            isPlayerGathering = false;
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
            InventoryManager.Instance.ConsumeResource(requiredResource1, team, requiredAmount1);
            InventoryManager.Instance.ConsumeResource(requiredResource2, team, requiredAmount2);

            photonView.RPC("CraftAndSpawnPrefab", RpcTarget.All);
        }
        else
        {
            Debug.LogWarning("No hay suficientes recursos o el equipo del jugador no está definido.");
        }
    }

    [PunRPC]
    void CraftAndSpawnPrefab()
    {
        if (craftedPrefab != null)
        {
            Instantiate(craftedPrefab, spawnLocation.position, spawnLocation.rotation);
        }
    }

    private string GetPlayerTeam()
    {
        // Obtén el equipo del jugador desde las propiedades personalizadas de Photon.
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team))
        {
            return team as string;
        }
        return null;
    }
}









