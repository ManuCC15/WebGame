using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PhotonGameManager : MonoBehaviour
{
    public static PhotonGameManager Instance;

    public float preparationPhaseDuration = 60f; // Duración de la fase de preparación
    public float battlePhaseDuration = 120f;    // Duración de la fase de batalla

    private float phaseTimer;                   // Temporizador de la fase
    private bool isPreparationPhase = true;     // ¿Estamos en la fase de preparación?

    public Button spawnSoldierButton;           // Botón para instanciar soldados

    public event Action OnBattlePhaseStart;     // Evento para la fase de batalla

    AudioSource music;

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
        music = GetComponent<AudioSource>();
        
    }

    void Start()
    {
        phaseTimer = preparationPhaseDuration;
        spawnSoldierButton.gameObject.SetActive(false); // Ocultar botón inicialmente
        
    }

    void Update()
    {
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0)
        {
            if (isPreparationPhase)
            {
                StartBattlePhase();
            }
            else
            {
                StartPreparationPhase();
            }
        }
    }

    public float PhaseTimer
    {
        get { return phaseTimer; }
    }

    private void StartPreparationPhase()
    {
        isPreparationPhase = true;
        phaseTimer = preparationPhaseDuration;

        // Desactivar el botón de instanciar soldados
        spawnSoldierButton.gameObject.SetActive(false);
    }

    private void StartBattlePhase()
    {
        isPreparationPhase = false;
        phaseTimer = battlePhaseDuration;

        // Activar el botón de instanciar soldados
        spawnSoldierButton.gameObject.SetActive(true);

        // Notificar a los arqueros que comienza una nueva fase de batalla
        OnBattlePhaseStart?.Invoke();
    }

    public bool IsPreparationPhase()
    {
        return isPreparationPhase;
    }

    [PunRPC]
    private void MusicPlay()
    {
        music.Play();
    }
}

