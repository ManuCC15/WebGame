using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class Resource
    {
        public string name;
        public int quantity;
        public int maxQuantity;
    }

    public List<Resource> resourcesTeamA = new List<Resource>(); // Recursos para el equipo A
    public List<Resource> resourcesTeamB = new List<Resource>(); // Recursos para el equipo B

    // Lista de soldados almacenados por equipo
    private List<GameObject> storedSoldiersTeamA = new List<GameObject>();
    private List<GameObject> storedSoldiersTeamB = new List<GameObject>();

    public Transform spawnLocationTeamA; // Posición de spawn para el equipo A
    public Transform spawnLocationTeamB; // Posición de spawn para el equipo B

    public delegate void OnResourceUpdated(string team, string resourceName, int newQuantity);
    public event OnResourceUpdated ResourceUpdated;

    public delegate void OnSoldierCountUpdated(string team, int soldierCount);
    public event OnSoldierCountUpdated SoldierCountUpdated;

    private bool isOnCooldown = false; // Para controlar el cooldown

    private const byte UpdateResourceEventCode = 1; // Código para sincronizar los recursos

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Resource> GetResourcesForTeam(string team)
    {
        return team == "A" ? resourcesTeamA : resourcesTeamB;
    }

    public void AddResource(string resourceName, string team, int amount)
    {
        List<Resource> resources = GetResourcesForTeam(team);
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null)
        {
            int newQuantity = Mathf.Clamp(resource.quantity + amount, 0, resource.maxQuantity);
            resource.quantity = newQuantity;

            // Sincronizar con todos los jugadores del equipo
            SyncResourceUpdate(team, resourceName, newQuantity);

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(team, resourceName, resource.quantity);
        }
    }

    public void ConsumeResource(string resourceName, string team, int amount)
    {
        List<Resource> resources = GetResourcesForTeam(team);
        Resource resource = resources.Find(r => r.name == resourceName);

        if (resource != null && resource.quantity >= amount)
        {
            int newQuantity = resource.quantity - amount;
            resource.quantity = newQuantity;

            // Sincronizar con todos los jugadores del equipo
            SyncResourceUpdate(team, resourceName, newQuantity);

            // Notificar cambios a la UI
            ResourceUpdated?.Invoke(team, resourceName, resource.quantity);
        }
    }

    public bool HasEnoughResource(string resourceName, string team, int requiredAmount)
    {
        List<Resource> resources = GetResourcesForTeam(team);
        Resource resource = resources.Find(r => r.name == resourceName);
        return resource != null && resource.quantity >= requiredAmount;
    }

    // Almacenar soldado en el equipo correspondiente
    public void StoreSoldier(string team, GameObject soldierPrefab)
    {
        if (team == "A")
        {
            storedSoldiersTeamA.Add(soldierPrefab);
            SoldierCountUpdated?.Invoke("A", storedSoldiersTeamA.Count);
        }
        else if (team == "B")
        {
            storedSoldiersTeamB.Add(soldierPrefab);
            SoldierCountUpdated?.Invoke("B", storedSoldiersTeamB.Count);
        }
    }

    // Instanciar soldados almacenados de un equipo
 public void SpawnStoredSoldiers()
{
    if (isOnCooldown)
    {
        Debug.LogWarning("El botón está en cooldown. Espera antes de presionarlo nuevamente.");
        return;
    }

    string team = GetPlayerTeam();
    List<GameObject> storedSoldiers = team == "A" ? storedSoldiersTeamA : storedSoldiersTeamB;

    if (storedSoldiers.Count > 0)
    {
        GameObject soldier = storedSoldiers[0]; // Toma el primer soldado en la lista
        Transform spawnLocation = team == "A" ? spawnLocationTeamA : spawnLocationTeamB;

        // Enviar un RPC para que todos los jugadores instancien el soldado
        PhotonView.Get(this).RPC("SpawnSoldierPrefab", RpcTarget.Others, team, soldier.name, spawnLocation.position, spawnLocation.rotation);

        storedSoldiers.RemoveAt(0); // Elimina el soldado de la lista local
        SoldierCountUpdated?.Invoke(team, storedSoldiers.Count); // Notifica la nueva cantidad a la UI

        StartCoroutine(ButtonCooldown()); // Inicia el cooldown
    }
    else
    {
        Debug.LogWarning($"No hay soldados almacenados para el equipo {team}.");
    }
}

private IEnumerator ButtonCooldown()
{
    isOnCooldown = true; // Activa el cooldown
    yield return new WaitForSeconds(1f); // Tiempo de cooldown (ajustable)
    isOnCooldown = false; // Desactiva el cooldown
}


    [PunRPC]
    public void SpawnSoldierPrefab(string team, string soldierName, Vector3 position, Quaternion rotation)
    {
        //GameObject soldierPrefab = Resources.Load<GameObject>($"Prefab/{soldierName}");
        string prefabPath = $"Prefab/{soldierName}";

        if (prefabPath != null)
        {
            
            GameObject soldier = PhotonNetwork.Instantiate(prefabPath, position, rotation);

            // Transferir propiedad al cliente que llamó a esta función
            PhotonView soldierPhotonView = soldier.GetComponent<PhotonView>();
            if (soldierPhotonView != null && soldierPhotonView.Owner != PhotonNetwork.LocalPlayer)
            {
                soldierPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            // Configurar el equipo del soldado
            Soldier soldierScript = soldier.GetComponent<Soldier>();
            if (soldierScript != null)
            {
                soldierScript.teamTag = team;
            }
        }
        else
        {
            Debug.LogError($"El prefab del soldado '{soldierName}' no se encontró en la carpeta Resources/Soldiers.");
        }
    }

    private void SyncResourceUpdate(string team, string resourceName, int newQuantity)
    {
        // Llamada RPC para actualizar a todos los jugadores
        PhotonView.Get(this).RPC("UpdateResourceOnAllClients", RpcTarget.All, team, resourceName, newQuantity);
    }

    [PunRPC]
    public void UpdateResourceOnAllClients(string team, string resourceName, int newQuantity)
    {
        ResourceUpdated?.Invoke(team, resourceName, newQuantity);
    }

    private string GetPlayerTeam()
    {
        // Obtén el equipo del jugador desde las propiedades personalizadas de Photon.
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team))
        {
            return team as string;
        }
        else
        {
            return AssingTeam();
        }
        //return null;
    }

    private string AssingTeam()
    {
        string team = "A";
        return team;
    }
}










