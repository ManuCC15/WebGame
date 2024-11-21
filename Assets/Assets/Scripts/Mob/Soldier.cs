using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public int speed;
    public GameObject target;
    public float direction;
    void Update()
    {
        direction = Vector3.Distance(transform.position, target.transform.position);

        transform.position += transform.position * direction * Time.deltaTime;
    }
}
