using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour 
{
	public Transform[] patrolPath;
	public bool isPatroling;
	public bool isShooting;
//	public float targetStayTimer

	private NavMeshAgent agent;
	private Animator anim;
	private float patrolStayTimer =0f;

	private int PathIndex = 0;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isPatroling) {
			Patroling ();
		}
		Shooting ();
	}

	void Patroling()
	{
		float mov = MovingSpeed ();
		anim.SetFloat ("MovingSpeed",mov);
		if (patrolPath.Length>0) 
		{
			if (agent.remainingDistance<=0.1f) 
			{
				agent.isStopped = true;
				transform.position = agent.destination;
				patrolStayTimer += Time.deltaTime;
				if (patrolStayTimer>3f) 
				{
					PathIndex++;
					patrolStayTimer = 0f;
					agent.isStopped = false;
					if (PathIndex==patrolPath.Length) 
					{
						PathIndex = 0;
					}
				}

			}
		}
		agent.SetDestination (patrolPath [PathIndex].position);

		Debug.DrawRay (patrolPath [PathIndex].position, Vector3.up * 10);
	}

	void Shooting()
	{
		if (isShooting) {
			agent.isStopped = true;
			anim.SetTrigger ("Shooting");
		} else {
			agent.isStopped = false;
		}
	}

	float MovingSpeed()
	{
		float speed;
		if (isPatroling) 
        {
			speed = agent.speed * 0.5f;
            if (agent.remainingDistance<0.5f)
            {
                speed = Mathf.Lerp(speed, 0, Time.deltaTime * 5f);
            }
        } else 
		{
			speed = 0f;
		}
		return speed;
	}
}
