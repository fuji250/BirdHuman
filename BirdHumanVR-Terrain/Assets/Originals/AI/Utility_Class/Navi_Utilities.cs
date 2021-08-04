using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public abstract class Navi_Utilities : Anim_Utilities
{
    protected abstract NavMeshAgent navi {get; set;}
    protected Vector3 target_pos {get; set;}

    // NavMesh上のランダムな位置を返す
    protected Vector3 NaviRandomPos(float max_distance, float search_radius)
    {
        NavMeshHit hit;
        Vector3 rand_dir = Random.insideUnitSphere * max_distance;
        Vector3 source_pos = transform.position + rand_dir;
        if(NavMesh.SamplePosition(source_pos, out hit, search_radius, NavMesh.AllAreas))
        {
            target_pos = hit.position;
            return hit.position;
        }
        else
        {
            target_pos = transform.position;
            return transform.position;
        }
    }

    // Agentが目的地に到着したかをbool値で返す
    protected bool Arrived()
    {
        return SqrComparison(target_pos - transform.position, navi.stoppingDistance);
    }

    // pathPendingを考慮したSetDestination()
    protected void SetDestination2(Vector3 destination)
    {
        if(!navi.pathPending)
        {
            navi.SetDestination(destination);
        }
    }
}
