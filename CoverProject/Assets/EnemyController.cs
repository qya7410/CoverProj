using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState
{
   Patrol,
}

public class EnemyController : MonoBehaviour {
    public EnemyState currentState = EnemyState.Patrol;
    public Transform[] patrolPath;

    NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (currentState==EnemyState.Patrol)
        {
            agent.SetDestination(GetNextPatrolPosition());
        }
	}

    Vector3 GetNextPatrolPosition()
    {
        
        int index = 0;
        Vector3 nextPos = patrolPath[index].position;
        if (Vector3.Distance(transform.position,patrolPath[index].position)<=0.5f)
        {
            index++;
            if (index==patrolPath.Length)
            {
                index = 0;
            }
        }
        return nextPos;
    }
}
