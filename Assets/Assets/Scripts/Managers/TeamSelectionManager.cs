using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    public Button teamAButton;
    public Button teamBButton;
    public Button startGameButton;

    public TextMeshProUGUI teamAStatusText;
    public TextMeshProUGUI teamBStatusText;

    private const int MaxPlayersPerTeam = 2;
    private int teamACount = 0;
    private int teamBCount = 0;
    private string playerTeam;

    void Start()
    {
        UpdateUI();
        startGameButton.interactable = false;
        startGameButton.onClick.AddListener(StartGame);
    }

    public void JoinTeamA()
    {
        if (teamACount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam))
        {
            playerTeam = "A";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", "A" } });
            photonView.RPC("UpdateTeamCount", RpcTarget.All, "A", 1);
        }
    }

    public void JoinTeamB()
    {
        if (teamBCount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam))
        {
            playerTeam = "B";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", "B" } });
            photonView.RPC("UpdateTeamCount", RpcTarget.All, "B", 1);
        }
    }

    [PunRPC]
    public void UpdateTeamCount(string team, int change)
    {
        if (team == "A") teamACount += change;
        if (team == "B") teamBCount += change;
        UpdateUI();

        startGameButton.interactable = teamACount > 0 && teamBCount > 0 && PhotonNetwork.IsMasterClient;
    }

    void UpdateUI()
    {
        teamAStatusText.text = $"Team A: {teamACount}/{MaxPlayersPerTeam}";
        teamBStatusText.text = $"Team B: {teamBCount}/{MaxPlayersPerTeam}";

        teamAButton.interactable = teamACount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam);
        teamBButton.interactable = teamBCount < MaxPlayersPerTeam && string.IsNullOrEmpty(playerTeam);
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");  // Cambiar por el nombre de la escena del juego
        }
    }
}

