using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour 
{
	public Transform[] patrolPath;
	public bool isPatroling;
	public bool isShooting;
	public bool isCover;
	public bool isStooed =false;
	public float patrolStayTimer =5;
//	public float targetStayTimer
	public float beteewnShotTimer =0.5f;

	private NavMeshAgent agent;
	private Animator anim;

	private float agentSpeed;

	private int pathIndex = 0;

	private float t =10f;
	// Use this for initialization
	void Start () {
		
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		agentSpeed =agent.speed*0.5f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float mov = MovingSpeed ();
		anim.SetFloat ("MovingSpeed",mov);
		if (isPatroling) 
		{
			Patroling ();
			
		}
		if(isCover)
		{
			Cover();
		}
		// Shooting ();
	}
	void LateUpdate()
	{
		
	}
	//这个方法更新敌人巡逻
	void Patroling()
	{
		isCover =false;
		if (patrolPath.Length>0) 
		{
			
			if (Vector3.Distance(transform.position,agent.destination)<agent.stoppingDistance)
            {
				anim.SetBool(HashIDs.enmeyMoveHash,false);
                patrolStayTimer -= Time.deltaTime;
                if (patrolStayTimer<=0&&pathIndex<patrolPath.Length)
                {
					anim.SetBool(HashIDs.enmeyMoveHash,true);
                    pathIndex++;
                    patrolStayTimer= 3f;                   
                    if (pathIndex == patrolPath.Length)
                    {
                        pathIndex = 0;
                    }
                }
            }
			else
			{
				anim.SetBool(HashIDs.enmeyMoveHash,true);				
			}
		agent.SetDestination (patrolPath [pathIndex].position);	
        }
	}

	void Cover()
	{
		isPatroling =false;
		CoverObstacle destWall = GetNearByObstacle._instance.destObstcle;
		Vector3 desPos =destWall.GetCoverPosition();
		agent.SetDestination(desPos);
		if(Vector3.Distance(transform.position,agent.destination)<agent.stoppingDistance)
		{
			transform.position =agent.destination;
			anim.SetBool("Cover",true);
		}else
		{
			anim.SetBool("Cover",false);
		}
	}


	//这个方法更新敌人的走动速度，1是跑，0.5是走，0是停
	float MovingSpeed()
	{
		float speed =0;
		if (isPatroling) 
        {	
			 speed=0.5f;
            if (agent.remainingDistance<agent.stoppingDistance+0.5f)
            {
				t+=Mathf.Clamp(Time.deltaTime,0f,1f);
				speed = Mathf.Lerp(agentSpeed,0, t*3f);
				// if(speed<0.1)
				// {
				// 	speed=0f;
				// }
            }
			else
			{
				t=0f;
				speed=agentSpeed;
			}
        }
		else if(isCover)
		{
			speed =1f;
			agent.speed=2f;
			if (agent.remainingDistance<agent.stoppingDistance+0.5f)
            {
				t+=Mathf.Clamp(Time.deltaTime,0f,1f);
				speed = Mathf.Lerp(speed,0, t*3f);
            }
		}
		return speed;
		// Debug.Log(speed);
	}
}
