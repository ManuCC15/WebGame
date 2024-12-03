using System.Collections;
using System.Collections.Generic;
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

    // Configura los jugadores no locales
    public void DisableNonLocalPlayer()
    {
        if (cameraObject != null) cameraObject.SetActive(false);
        if (playerController != null) playerController.enabled = false;
    }
}
