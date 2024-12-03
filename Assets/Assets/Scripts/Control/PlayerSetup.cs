using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public GameObject cameraObject; // Asocia la cámara del jugador en el inspector
    public PlayerController playerController; // Controlador del jugador

    // Configura el jugador local
    public void IsLocalPlayer()
    {
        if (cameraObject != null) cameraObject.SetActive(true);
        if (playerController != null) playerController.enabled = true;
    }
}

