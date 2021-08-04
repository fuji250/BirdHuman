using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI_Base;

[RequireComponent(typeof(Public_Status))]

public class AI_Rabbit : Navi_Utilities
{
    protected override Animator anim {get; set;}
    protected override NavMeshAgent navi {get; set;}

    string before_stateName;
    State current_state;
    Public_Status public_status;

    Idol idol = new Idol(); Walk walk = new Walk(); Eat eat = new Eat();
    Run escape = new Run(); Dead dead = new Dead();

    string eat_clip_name = "WildRabbit_eat";
    float eat_clip_length;

    [SerializeField] float idol_speed; 
    List<GameObject> predators;
    GameObject killer;
    bool move_able = true;
    string[] predator_tags = {"Player", "Wolf"};

    [SerializeField, Range(1, 5)] float model_scale;

    // モデルのサイズに依存する要素
    [SerializeField] float walk_speed;
    [SerializeField] float run_speed;



    void IWRAnimChanger()
    {
        if(move_able)
        {
            float speed = navi.velocity.magnitude;

            if(speed < idol_speed)
            {
                anim.SetInteger("State", 0);
            }
            else
            {
                anim.SetInteger("State", 1);

                float norm_speed = Mathf.Clamp(Normalizer(speed, 0, run_speed), 0, 1);
                anim.SetFloat("Move", norm_speed);
            }
        }
    }



    void Idol()
    {
        navi.speed = idol_speed;
        float random_timer = Random.Range(3.0f, 8.0f);
        float eat_rate = 0.2f;
        float k = Random.Range(0, 1.0f);
        if(0 <= k && k < eat_rate)
        {
            DelayCall(random_timer, () => current_state = eat);
        }
        else
        {
            DelayCall(random_timer, () => current_state = walk);
        }
    }
    void Walk()
    {
        navi.speed = walk_speed;
        SetDestination2(NaviRandomPos(50, 45));
    }
    void Eat()
    {
        move_able = false;
        anim.SetInteger("State", 2);
        DelayCall(eat_clip_length, () => move_able = true, () => current_state = idol);
    }    
    void Escape()
    {
        navi.speed = run_speed;
        SetDestination2(NaviRandomPos(50,50));
    }
    void Dead()
    {
        move_able = false;
        anim.SetInteger("State", 3);
        public_status.killer = killer;
        if(killer.tag == "Player")
        {
            navi.enabled = false;
        }
        else
        {
            Destroy(gameObject, 8);
        }
    }



    void OnValidate()
    {
        transform.localScale = new Vector3(model_scale, model_scale, model_scale);
    }



    void OnTriggerStay(Collider col)
    {
        if(IsContaining(col.tag, predator_tags))
        {
            if(!IsContaining(col.transform.root.gameObject, predators))
            {
                predators.Add(col.transform.root.gameObject);
            }
            if(current_state != dead && current_state != eat && current_state != escape)
            {
                current_state = escape;
            }
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        if(IsContaining(col.tag, predator_tags) && IsContaining(col.transform.root.gameObject, predators) && current_state == escape)
        {
            predators.Remove(col.transform.root.gameObject);
            if(predators.Count < 1)
            {
                current_state = walk;
            }
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        navi = GetComponent<NavMeshAgent>();

        before_stateName = "";
        current_state = idol;
        public_status = GetComponent<Public_Status>();

        idol.execution = Idol; walk.execution = Walk; eat.execution = Eat;
        escape.execution = Escape; dead.execution = Dead;

        eat_clip_length = GetClipLength(eat_clip_name);

        predators = new List<GameObject>();

        walk_speed *= model_scale;
        run_speed *= model_scale;
        navi.stoppingDistance *= model_scale;
    }

    void Update()
    {
        if(current_state.GetStateName() != before_stateName)
        {
            StopAllCoroutines();
            navi.ResetPath();
            before_stateName = current_state.GetStateName();
            public_status.status = current_state.GetStateName();
            current_state.Execute();
        }

        IWRAnimChanger();

        if(Arrived())
        {
            if(current_state == walk)
            {
                current_state = idol;
            }
            else if(current_state == escape)
            {
                SetDestination2(NaviRandomPos(50, 45));
            }
            else
            {
                return;
            }
        }

        if(predators.Count > 0)
            {
                if(IEnumSeeker(predators, (obj) => obj.GetComponent<Public_Status>().status == "Attack" && obj.GetComponent<Public_Status>().prey == gameObject, out killer))
                {
                    current_state = dead;
                }
            }
    }
}
