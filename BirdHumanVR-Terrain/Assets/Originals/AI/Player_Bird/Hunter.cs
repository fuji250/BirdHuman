using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class Hunter : Utilities
{
    GameObject player;
    string[] prey_tags = {"Rabbit"};
    bool captured = false;
    GameObject prey;
    Public_Status public_status;
    Public_Status prey_status;


    void Start()
    {
        player = transform.root.gameObject;
        public_status = player.GetComponent<Public_Status>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(IsContaining(col.tag, prey_tags) && !captured)
        {
            prey = col.transform.root.gameObject;
            public_status.prey = prey;
            public_status.status = "Attack";
            prey_status = prey.GetComponent<Public_Status>();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(prey != null)
        {
            if(prey_status.killer == player)
            {
                captured = true;
                prey.transform.position = transform.position;
                prey.transform.parent = transform;
                prey = null;
            }
        }
    }
}
