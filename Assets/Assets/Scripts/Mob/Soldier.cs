using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Soldier : MonoBehaviour
{
    public int speed;
    public GameObject target;
    public float direction;

    private PhotonView PhotonView;

    public void Start()
    {
        PhotonView = GetComponent<PhotonView>();
    }
    void Update()
    {
        transform.position += new Vector3(-speed*Time.deltaTime,0f,0f); 
    }

    public void Move()
    {
        direction = Vector3.Distance(transform.position, target.transform.position);

        transform.position += transform.position * direction * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Castle"))
        {
            Destroy(gameObject);
            Debug.Log("colsione");
        }
    }
}
