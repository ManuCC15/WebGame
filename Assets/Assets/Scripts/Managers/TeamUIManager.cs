using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class TeamUIManager : MonoBehaviourPunCallbacks
{
    public GameObject teamAPanel;
    public GameObject teamBPanel;

    public TextMeshProUGUI teamAResourcesText;
    public TextMeshProUGUI teamBResourcesText;

    public Button startGameButton;

    private const int MaxPlayersPerTeam = 2;
    private int teamACount = 0;
    private int teamBCount = 0;
    private string playerTeam;

    // Aquí puedes agregar la lista de recursos específicos de cada equipo.
    private int teamAResources = 0;
    private int teamBResources = 0;

    void Start()
    {
        // Al empezar, intentamos obtener el equipo del jugador.
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team))
        {
            playerTeam = team.ToString();
            SetupUIForTeam(playerTeam);
        }

        // Inicializa la UI de recursos.
        UpdateResourcesUI(playerTeam);
    }

    void SetupUIForTeam(string team)
    {
        if (team == "A")
        {
            teamAPanel.SetActive(true);
            teamBPanel.SetActive(false);
        }
        else if (team == "B")
        {
            teamAPanel.SetActive(false);
            teamBPanel.SetActive(true);
        }
    }

    // Esta función se puede llamar para actualizar los recursos de un equipo.
    public void UpdateResourcesUI(string team)
    {
        if (team == "A")
        {
            teamAResourcesText.text = $"Resources: {teamAResources}";
        }
        else if (team == "B")
        {
            teamBResourcesText.text = $"Resources: {teamBResources}";
        }
    }

    // Método para incrementar los recursos de un equipo.
    public void AddResourcesToTeam(string team, int amount)
    {
        if (team == "A")
        {
            teamAResources += amount;
        }
        else if (team == "B")
        {
            teamBResources += amount;
        }

        UpdateResourcesUI(team);  // Actualiza la UI después de modificar los recursos.
    }

    // Método para restar recursos de un equipo.
    public void SubtractResourcesFromTeam(string team, int amount)
    {
        if (team == "A")
        {
            teamAResources = Mathf.Max(0, teamAResources - amount);  // Evita que los recursos sean negativos
        }
        else if (team == "B")
        {
            teamBResources = Mathf.Max(0, teamBResources - amount);  // Evita que los recursos sean negativos
        }

        UpdateResourcesUI(team);  // Actualiza la UI después de modificar los recursos.
    }

    // Método que se ejecuta cuando el jugador se une al equipo A.
    public void JoinTeamA()
    {
        if (teamACount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam))
        {
            playerTeam = "A";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", "A" } });
            photonView.RPC("UpdateTeamCount", RpcTarget.All, "A", 1);
        }
    }

    // Método que se ejecuta cuando el jugador se une al equipo B.
    public void JoinTeamB()
    {
        if (teamBCount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam))
        {
            playerTeam = "B";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", "B" } });
            photonView.RPC("UpdateTeamCount", RpcTarget.All, "B", 1);
        }
    }

    // Método RPC para actualizar el número de jugadores por equipo.
    [PunRPC]
    public void UpdateTeamCount(string team, int change)
    {
        if (team == "A") teamACount += change;
        if (team == "B") teamBCount += change;

        UpdateUI();

        // El botón de inicio solo se habilita cuando ambos equipos tienen al menos un jugador.
        startGameButton.interactable = teamACount > 0 && teamBCount > 0 && PhotonNetwork.IsMasterClient;
    }

    // Actualizar la UI de los equipos.
    void UpdateUI()
    {
        teamAResourcesText.text = $"Team A: {teamACount}/{MaxPlayersPerTeam}";
        teamBResourcesText.text = $"Team B: {teamBCount}/{MaxPlayersPerTeam}";
    }

    // Método que se llama cuando se inicia el juego.
    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
}

