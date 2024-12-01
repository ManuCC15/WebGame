using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Este script asegura que cada jugador
//solo controle su propio personaje mientras que otros jugadores no puedan interferir.
public class PlayerSetup : MonoBehaviour
{
    public GameObject GO;

    public PlayerController playerController;

    public void IsLocalPLayer()
    {
        GO.SetActive(true);
        playerController.enabled = true;
    }
}
