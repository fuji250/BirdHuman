using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class Detector : Utilities
{
    GameObject player;
    Public_Status public_status;
    string[] predator_tags = {"Wolf"};
    GameObject chaser;
    Public_Status chaser_status;
    bool chased = false;

    void OnTriggerEnter(Collider col)
    {
        if(IsContaining(col.tag, predator_tags) && !chased)
        {
            chased = true;
            chaser = col.transform.root.gameObject;
            chaser_status = chaser.GetComponent<Public_Status>();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(IsContaining(col.tag, predator_tags))
        {
            chased = false;
            chaser = null;
            chaser_status = null;
        }
    }

    void Start()
    {
        player = transform.root.gameObject;
        public_status = player.GetComponent<Public_Status>();
    }

    void Update()
    {
        if(chased)
        {
            if(chaser_status.status == "Attack" && chaser_status.prey == transform.root.gameObject)
            {
                public_status.status = "Dead";
            }
        }
    }
}
