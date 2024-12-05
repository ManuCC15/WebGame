using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;         // Referencia al transform del jugador
    public Vector3 preparationOffset = new Vector3(0, 0, -10);  // Offset de la c�mara en la fase de preparaci�n
    public Vector3 battlePosition = new Vector3(0, 0, -10);     // Posici�n est�tica de la c�mara en la fase de batalla
    public float battleZoomSize = 15f;       // Zoom de la c�mara para la fase de batalla
    public float transitionSpeed = 2f;       // Velocidad de transici�n entre las fases

    public Camera mainCamera;               // Referencia a la c�mara principal
    private bool isPreparationPhase = true;  // Indica si estamos en la fase de preparaci�n
    private float iniCamOrthographicSize = 0;

    void Start()
    {
        iniCamOrthographicSize = mainCamera.orthographicSize;

        // Aseg�rate de que tenemos una referencia v�lida al jugador
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

    // Sigue al jugador durante la fase de preparaci�n
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

        // Ajustar el tama�o del zoom (solo si es una c�mara ortogr�fica)
        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, battleZoomSize, Time.deltaTime * transitionSpeed);
        }
    }
}

