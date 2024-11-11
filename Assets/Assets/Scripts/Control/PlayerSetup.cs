using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
