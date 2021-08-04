using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blood_Splash : MonoBehaviour
{
    [SerializeField] Image blood;
    [SerializeField] Image background;
    Color blood_color = new Color(1, 1, 1, 1);
    Color background_color = new Color(1, 1, 1, 0.15f);
    [SerializeField] Public_Status public_status;

    void Start()
    {
        blood.color = new Color(1,1,1,0);
        background.color = new Color(1,1,1,0);
    }

    void Update()
    {
        if(public_status.status == "Dead")
        {
            blood.color = Color.Lerp(blood.color, blood_color, 0.1f);
            background.color = Color.Lerp(background.color, background_color, 0.1f);
        }
    }
}
