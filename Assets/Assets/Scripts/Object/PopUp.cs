using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviourPunCallbacks
{
    public GameObject PopUpPrefab;
    void Start()
    {
        PopUpPrefab.SetActive(false);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PopUpPrefab.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
        PopUpPrefab.SetActive(false);
        
    }
}
