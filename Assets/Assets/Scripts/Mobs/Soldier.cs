using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
public class Soldier : MonoBehaviourPunCallbacks
{
    public GameObject startPosition;
    public GameObject endPosition;
    public float speed = 5f;

    private Vector3 direction;
    private PhotonTransformViewClassic PVC;
    private void Start()
    {
        PVC = new PhotonTransformViewClassic();
    }
    public void Update()
    {
        SetPosition(startPosition.transform.position, endPosition.transform.position);
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetPosition(Vector3 start, Vector3 End)
    {
        startPosition.transform.position = start;
        endPosition.transform.position = End;

        // Recalculamos la dirección.
        direction = (endPosition.transform.position - startPosition.transform.position).normalized;

        // Posición inicial.
        transform.position = startPosition.transform.position;
    }
}
