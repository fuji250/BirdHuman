using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird_Fly : MonoBehaviour
{
    [SerializeField] float fly_speed;
    Public_Status public_status;

    void Start()
    {
        public_status = GetComponentInChildren<Public_Status>();
    }

    void Update()
    {
        if(public_status.status != "Dead")
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position+transform.forward*fly_speed, fly_speed*Time.deltaTime);
        }
    }
}
