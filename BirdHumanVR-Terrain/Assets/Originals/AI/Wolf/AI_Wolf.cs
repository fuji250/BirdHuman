using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI_Base;

[RequireComponent(typeof(Public_Status))]

public class AI_Wolf : Navi_Utilities
{
    // 必須コンポーネント
    protected override Animator anim {get; set;}
    protected override NavMeshAgent navi {get; set;}
    
    string before_stateName;
    State current_state;
    Public_Status public_status;

    // current_stateに入れるクラス達
    // 動物によって異なる
    Idol idol = new Idol(); Walk walk = new Walk(); Howl howl = new Howl();
    Run chase = new Run(); Attack attack = new Attack(); Eat eat = new Eat();

    // Animation Clipの名前(現時点ではベタ打ち)
    string howl_clip_name = "Wolf_howl"; string attack_clip_name = "Wolf_attack"; string eat_clip_name = "Wolf_eat";

    // Animation Clipの再生時間(必要なものだけ)
    float howl_clip_length; float attack_clip_length; public float eat_clip_length;

    // その他の変数
    [SerializeField] GameObject model;
    [SerializeField] float idol_speed;
    GameObject prey;
    bool move_able = true;
    string[] prey_tags = {"Player", "Rabbit"};

    [SerializeField, Range(1, 5)] float model_scale;

    // モデルのサイズに依存する要素
    // NavMeshAgent stoppingDistance
    [SerializeField] float walk_speed;
    [SerializeField] float run_speed;



    // I:Idol W:Walk R:Run
    // オブジェクトの移動速度によって３つのアニメーションを遷移
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



    // 具体的な処理を記述
    // (動物間で似たような処理になるならAI_Baseに入れることも考える)
    void Idol()
    {
        navi.speed = idol_speed;
        float random_timer = Random.Range(3.0f, 8.0f);
        float howl_rate = 0.2f;
        float k = Random.Range(0, 1.0f);
        if(0 <= k && k < howl_rate)
        {
            DelayCall(random_timer, () => current_state = howl);
        }
        else
        {
            DelayCall(random_timer, () => current_state = walk);
        }
    }
    void Walk()
    {
        navi.speed = walk_speed;
        // 50,40はScaleによって変更の可能性あり
        SetDestination2(NaviRandomPos(50, 45));
    }
    void Howl()
    {
        move_able = false;
        anim.SetInteger("State", 2);
        DelayCall(howl_clip_length, () => current_state = idol, () => move_able = true);
    }    
    void Chase()
    {
        navi.speed = run_speed;
    }
    void Attack()
    {
        move_able = false;
        anim.SetInteger("State", 3);
        DelayCall(attack_clip_length, () => current_state = eat);
    }
    void Eat()
    {
        anim.SetInteger("State", 4);
        LookAtInOneCall(prey.transform.position - model.transform.position, 0.5f);
        DelayCall(eat_clip_length, () => current_state = idol, () => move_able = true, () => prey = null);
    }



    void OnValidate()
    {
        transform.localScale = new Vector3(model_scale, model_scale, model_scale);
    }



    void OnTriggerStay(Collider col)
    {
        if(IsContaining(col.tag, prey_tags))
        {
            // preyを見つけた瞬間のみ呼ばれる
            if(current_state != chase && current_state != attack && current_state != eat 
                && current_state != howl && col.transform.root.GetComponent<Public_Status>().status != "Dead")
            {
                current_state = chase;
                prey = col.transform.root.gameObject;
            }

            // 追いかけている間は呼び続ける
            if(prey != null && current_state == chase)
            {
                // 飛んでいる相手用に、地面に向かってraycastする
                Ray ray = new Ray(prey.transform.position, Vector3.down);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    target_pos = hit.point;
                    SetDestination2(target_pos);
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(IsContaining(col.tag, prey_tags) && col.transform.root.gameObject == prey && current_state == chase)
        {
            current_state = idol;
            navi.ResetPath();
            prey = null;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        navi = GetComponent<NavMeshAgent>();

        before_stateName = "";
        current_state = idol;
        public_status = GetComponent<Public_Status>();

        // デリゲートに各々のメソッドを登録
        idol.execution = Idol; walk.execution = Walk; howl.execution = Howl;
        chase.execution = Chase; attack.execution = Attack; eat.execution = Eat;

        // 必要なAnimation Clipの再生時間を取得
        howl_clip_length = GetClipLength(howl_clip_name);
        attack_clip_length = GetClipLength(attack_clip_name);
        eat_clip_length = GetClipLength(eat_clip_name);

        // model_scaleに応じて値を修正
        walk_speed *= model_scale;
        run_speed *= model_scale;
        navi.stoppingDistance *= model_scale;
    }

    void Update()
    {
        // State遷移時に一度だけ呼ばれる
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
            // 飛んでいる相手用にSqrComparisonは必要(Arrivedでは不十分)
            else if(current_state == chase && SqrComparison(prey.transform.position - model.transform.position, navi.stoppingDistance))
            {
                navi.speed = 0;
                current_state = attack;
            }
            else
            {
                return;
            }
        }

        if(public_status.prey != prey)
        {
            public_status.prey = prey;
        }
    }
}