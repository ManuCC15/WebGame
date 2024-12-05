using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;         // Referencia al transform del jugador
    public Vector3 preparationOffset = new Vector3(0, 0, -10);  // Offset de la cámara en la fase de preparación
    public Vector3 battlePosition = new Vector3(0, 0, -10);     // Posición estática de la cámara en la fase de batalla
    public float battleZoomSize = 15f;       // Zoom de la cámara para la fase de batalla
    public float transitionSpeed = 2f;       // Velocidad de transición entre las fases

    public Camera mainCamera;               // Referencia a la cámara principal
    private bool isPreparationPhase = true;  // Indica si estamos en la fase de preparación
    private float iniCamOrthographicSize = 0;

    void Start()
    {
        iniCamOrthographicSize = mainCamera.orthographicSize;

        // Asegúrate de que tenemos una referencia válida al jugador
        if (playerTransform == null && PhotonNetwork.LocalPlayer != null)
        {
            // Buscar el objeto del jugador controlado por el cliente
            var playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }

    void Update()
    {
        // Obtener el estado de la fase desde el PhotonGameManager
        isPreparationPhase = PhotonGameManager.Instance.IsPreparationPhase();

        if (isPreparationPhase)
        {
            FollowPlayer();
        }
        else
        {
            MoveToBattlePosition();
        }
    }

    // Sigue al jugador durante la fase de preparación
    void FollowPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position + preparationOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
        }

        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, iniCamOrthographicSize, Time.deltaTime * transitionSpeed);
        }
    }

    // Se aleja y se posiciona para la fase de batalla
    void MoveToBattlePosition()
    {
        Vector3 targetPosition = battlePosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);

        // Ajustar el tamaño del zoom (solo si es una cámara ortográfica)
        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, battleZoomSize, Time.deltaTime * transitionSpeed);
        }
    }
}

