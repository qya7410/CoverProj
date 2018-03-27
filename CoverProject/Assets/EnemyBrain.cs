using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour 
{
    public Transform player;
	public Transform[] patrolPath;
	public bool isPatroling;
	public bool isShooting;
	public bool isCover;
	public bool isStooed =false;
	public float patrolStayTimer =5;
//	public float targetStayTimer
    public float beteewnShot =2f;
    private float beteewnShotTimer = 0f;

    public Transform players;

	private NavMeshAgent agent;
	private Animator anim;

	private float agentSpeed;

	private int pathIndex = 0;

	private float moveSpeed;
    private float speed =0f;
    private float moveToStopTimer=0f;
    //public float moveToStop;
    // Use this for initialization







    private Vector3 desPos;


	void Start () {
        
		agent = GetComponent<NavMeshAgent> ();
        //agent.updateUpAxis = false;
		anim = GetComponent<Animator> ();
		agentSpeed =agent.speed*0.5f;
        anim.SetFloat(HashIDs.enmeyMoveSpeedHash, agentSpeed);
        desPos = patrolPath[pathIndex].position;
	}
	
	// Update is called once per frame
	void Update () 
	{
        

		if (isPatroling) 
		{
			Patroling ();
			
		}
		if(isCover)
		{
			Cover();
            Shooting();
		}
        //moveSpeed = MovingSpeed();
	}

	//这个方法更新敌人巡逻
	void Patroling()
	{
        if (agent.remainingDistance<agent.stoppingDistance)
        {
            //开启导航的条件应该是
            patrolStayTimer -= Time.deltaTime;
            if (patrolStayTimer<0f)
            {
                agent.isStopped = false;
                if (pathIndex==patrolPath.Length)
                {
                    pathIndex = 0;
                    patrolStayTimer = 5f;
                }
                else
                {
                    pathIndex++;
                    patrolStayTimer = 5f;
                }
            }
            else
            {
                agent.isStopped = true;
            }
        }
        agent.SetDestination(patrolPath[pathIndex].position);
        Debug.Log(agent.isStopped);
        //isCover =false;
  //      anim.SetBool(HashIDs.enmeyCoverHash, false);
		//if (patrolPath.Length>0) 
		//{
			
		//	if (Vector3.Distance(transform.position,agent.destination)<agent.stoppingDistance)
  //          {
		//		anim.SetBool(HashIDs.enmeyMoveHash,false);
  //              agent.speed = MovingSpeed();
  //              //agent.enabled = false;
  //              patrolStayTimer -= Time.deltaTime;
  //              if (patrolStayTimer<=0&&pathIndex<patrolPath.Length)
  //              {
  //                  //agent.enabled = true;
		//			anim.SetBool(HashIDs.enmeyMoveHash,true);
  //                  pathIndex++;
  //                  patrolStayTimer= 5f;                   
  //                  if (pathIndex == patrolPath.Length)
  //                  {
  //                      pathIndex = 0;
  //                  }
  //              }
  //          }

		//agent.SetDestination (patrolPath [pathIndex].position);	
        //}
	}

	void Cover()
	{
        isPatroling = false;	
		CoverObstacle destWall = GetNearByObstacle._instance.destObstcle;
		//Vector3 desPos =destWall.GetCoverPosition();
		agent.SetDestination(desPos);
		if(Vector3.Distance(transform.position,agent.destination)<agent.stoppingDistance)
		{
			transform.position =agent.destination;
            anim.SetBool(HashIDs.enmeyCoverHash,true);
		}else
		{
            anim.SetBool(HashIDs.enmeyCoverHash,false);
		}
	}


	//这个方法更新敌人的走动速度，1是跑，0.5是走，0是停
	float MovingSpeed()
	{
		if (isPatroling) 
        {
            agent.speed = 0.5f;
            if (agent.remainingDistance<agent.stoppingDistance+0.5f)
            {
                moveToStopTimer += Time.deltaTime;
                agent.speed= Mathf.Lerp(0.5f, 0f, moveToStopTimer);
                if(agent.speed<0.1)
				 {
                    moveToStopTimer = 0f;
                    agent.speed= 0f;
				 }
                else
                {
                    return agent.speed; 
                }
                Debug.Log(moveToStopTimer);
            }
            return agent.speed;
        }
		else if(isCover)
		{
            agent.speed=2f;
			if (agent.remainingDistance<agent.stoppingDistance+0.5f)
            {
                //float timer = 0;
                moveToStopTimer += Time.deltaTime;
                agent.speed = Mathf.Lerp(2, 0, moveToStopTimer);
                if(agent.speed<0.1)
                 {
                    moveToStopTimer = 0f;
                    agent.speed= 0f;
                 }
                else
                {
                    return agent.speed; 
                }
            }
		}
        return agent.speed;
       
	}

    private void Shooting()
    {
            
        beteewnShotTimer-= Time.deltaTime;
        if (beteewnShotTimer<=0f&&agent.isStopped==false)
        {
            agent.isStopped = true;
            anim.SetTrigger(HashIDs.enmeyShotHash);
            beteewnShotTimer = beteewnShot;
        }
            agent.isStopped = false;
    }
}
