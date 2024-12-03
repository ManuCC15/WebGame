using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    public Button teamAButton;
    public Button teamBButton;
    public Button startGameButton;

    public TextMeshProUGUI teamAStatusText;
    public TextMeshProUGUI teamBStatusText;

    private const int MaxPlayersPerTeam = 2;
    private string playerTeam;

    void Start()
    {
        // Verificar que todos los botones estén asignados
        if (teamAButton == null || teamBButton == null || startGameButton == null)
        {
            Debug.LogError("Uno o más botones no están asignados en el Inspector.");
            return;
        }

        // Configurar botones
        teamAButton.onClick.AddListener(JoinTeamA);
        teamBButton.onClick.AddListener(JoinTeamB);
        startGameButton.onClick.AddListener(StartGame);

        
        startGameButton.interactable = false;
    }

    public void JoinTeamA()
    {
        if (PhotonNetwork.LocalPlayer != null)
        {
            if (IsTeamJoinable("A"))
            {
                playerTeam = "A";
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", "A" } });
                UpdateTeamInRoomProperties("A", 1);
            }
            else
            {
                Debug.LogWarning("No se puede unir al Equipo A (lleno o ya en un equipo).");
            }
        }
        else
        {
            Debug.LogError("LocalPlayer no está inicializado.");
        }
    }

    public void JoinTeamB()
    {
        if (PhotonNetwork.LocalPlayer != null)
        {
            if (IsTeamJoinable("B"))
            {
                playerTeam = "B";
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", "B" } });
                UpdateTeamInRoomProperties("B", 1);
            }
            else
            {
                Debug.LogWarning("No se puede unir al Equipo B (lleno o ya en un equipo).");
            }
        }
        else
        {
            Debug.LogError("LocalPlayer no está inicializado.");
        }
    }

    private bool IsTeamJoinable(string team)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            if (roomProperties.ContainsKey($"Team{team}Count"))
            {
                int teamCount = (int)roomProperties[$"Team{team}Count"];
                return teamCount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam);
            }
        }
        return false;
    }

    private void UpdateTeamInRoomProperties(string team, int change)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            string propertyKey = $"Team{team}Count";
            int currentCount = (int)roomProperties[propertyKey];
            roomProperties[propertyKey] = Mathf.Clamp(currentCount + change, 0, MaxPlayersPerTeam);

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable updatedProperties)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            int teamACount = roomProperties.ContainsKey("TeamACount") ? (int)roomProperties["TeamACount"] : 0;
            int teamBCount = roomProperties.ContainsKey("TeamBCount") ? (int)roomProperties["TeamBCount"] : 0;

            teamAStatusText.text = $"Team A: {teamACount}/{MaxPlayersPerTeam}";
            teamBStatusText.text = $"Team B: {teamBCount}/{MaxPlayersPerTeam}";

            teamAButton.interactable = teamACount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam);
            teamBButton.interactable = teamBCount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam);

            startGameButton.interactable = teamACount > 0 && teamBCount > 0 && PhotonNetwork.IsMasterClient;
        }
        else
        {
            Debug.LogWarning("PhotonNetwork.CurrentRoom es null. No se puede actualizar la UI.");
        }
    }


    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("LoadGameScene", RpcTarget.All);
        }
    }

    [PunRPC]
    public void LoadGameScene()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnJoinedRoom()
    {
        // Configurar las propiedades iniciales de la sala al unirse
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "TeamACount", 0 },
                { "TeamBCount", 0 }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
        UpdateUI();
    }
}


