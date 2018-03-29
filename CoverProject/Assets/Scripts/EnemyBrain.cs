using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patroling,
    Cover
}
public class EnemyBrain : MonoBehaviour 
{
    public GameObject bullet;
    public Transform firePosition;

    public EnemyState currentState;

    //public Transform player;
	public Transform[] patrolPath;
	public bool isPatroling;
	public bool isShooting;
	public bool isCover;
	public bool isStooed =false;

    public float patrolStay = 5f;
    private float patrolStayTimer;

    public float beteewnShot =2f;
    public float smoothRotateDamping = 6f;
    private float beteewnShotTimer;
    private EnemyVisualSystem enemyEyes;
    private Transform player;

	private NavMeshAgent agent;
	private Animator anim;

	//private float agentSpeed;

	private int pathIndex;

	private float moveSpeed;
    public float speed;
    public float moveToStopTimer=0f;
    private Vector3 desPos;
    private GetNearByObstacle obs;
    public float enmeyAlertTime;
    private float alert = 0;

	void Start () 
    {
        alert = enmeyAlertTime;
        patrolStayTimer = patrolStay;
        player = GameObject.FindWithTag(Tags.player).transform;
        enemyEyes = GetComponent<EnemyVisualSystem>();
        obs = GetComponent<GetNearByObstacle>();
        currentState = EnemyState.Patroling;
        beteewnShotTimer = 2;
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
        //desPos = patrolPath[pathIndex].position;
	}
	
	// Update is called once per frame
	void Update () 
	{
        //Debug.Log(patrolStayTimer);
        switch (currentState)
        {
            case EnemyState.Patroling:
                Patroling();
                break;
            case EnemyState.Cover:
                Cover();
                AlertMoment();
                break;
        }
        if(enemyEyes.isFindpalyer)
		{
            currentState = EnemyState.Cover;

        }
        else
        {
            currentState = EnemyState.Patroling; 
        }
        if(isShooting)
        {
            ShootingAnimation();
        }
        //Debug.Log("EnmeyState=" + currentState);
	}

	//这个方法更新敌人巡逻
	void Patroling()
	{
        isShooting = false;
        anim.SetBool(HashIDs.enmeyCoverHash, false);
        if (agent.remainingDistance<agent.stoppingDistance)
        {
            agent.isStopped = true;
            //开启导航的条件在stopdistance之内
            patrolStayTimer -= Time.deltaTime;
            if (patrolStayTimer<0f)
            {
                //agent.isStopped = false;
                if (pathIndex>=patrolPath.Length)
                {
                    pathIndex = 0;
                    patrolStayTimer = patrolStay;
                }
                else
                {
                    ++pathIndex;
                    patrolStayTimer = patrolStay;
                }
            }
        }
        //划重点，其他情况开启导航
        else
        {
            agent.isStopped = false;
        }
        anim.SetFloat(HashIDs.enmeyMoveSpeedHash, MovingSpeed());
        agent.SetDestination(patrolPath[pathIndex].position);
    }

	void Cover()
	{
        isShooting = true;
        LookForPlayer();
        //transform
        anim.SetFloat(HashIDs.enmeyMoveSpeedHash, CoverSpeed());
        CoverObstacle destWall = obs.destObstcle;
        desPos =destWall.coverWithWall;
		agent.SetDestination(desPos);
		if(Vector3.Distance(transform.position,agent.destination)<agent.stoppingDistance)
		{
			transform.position =agent.destination;
            anim.SetBool(HashIDs.enmeyCoverHash,true);
		}else
		{
            anim.SetBool(HashIDs.enmeyCoverHash,false);
		}

        Debug.DrawRay(desPos, Vector3.up * 30, Color.red);
	}


	//更新敌人的走动速度
	float MovingSpeed()
	{
        agent.speed = 1f;
        if (agent.remainingDistance>agent.stoppingDistance)
        {
                moveSpeed=0.2f; 
        }
        else
        {
            moveToStopTimer += Time.deltaTime;
            moveSpeed = Mathf.Lerp(moveSpeed, 0f, moveToStopTimer);

            if (moveSpeed<= 0f)
            {
                moveToStopTimer = 0f;
                moveSpeed = 0f;
            }
        }
        return moveSpeed;
       
	}

    //更新敌人在Cover状态下的的跑步速度
    float CoverSpeed()
    {
        agent.speed = 2f;
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            moveSpeed = 1f;
        }
        else
        {
            moveToStopTimer += Time.deltaTime;
            moveSpeed = Mathf.Lerp(moveSpeed, 0f, moveToStopTimer);

            if (moveSpeed <= 0f)
            {
                moveToStopTimer = 0f;
                moveSpeed = 0f;
            }
        }
        return moveSpeed;
    }
    //射击
    private void ShootingAnimation()
    {
        LookForPlayer();
        //间隔时间发送射击状态
        beteewnShotTimer += Time.deltaTime;
        if (beteewnShotTimer>=beteewnShot)
        {
            beteewnShotTimer = 0f;
            anim.SetTrigger(HashIDs.enmeyShotHash);
        }
        //动画不妨期间禁用导航器
        AnimatorStateInfo Info = anim.GetCurrentAnimatorStateInfo(0);
        if (Info.IsName("Base Layer.Shooting")&&Info.normalizedTime<1.0f)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
    }


    private void LookForPlayer()
    {
        //盯向目标
        Vector3 relPosition = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(relPosition.x, 0f, relPosition.z));
        transform.rotation = Quaternion.Lerp(transform.rotation,targetRotation,smoothRotateDamping*Time.deltaTime);
    }

    void AlertMoment()
    {
        if (Vector3.Distance(transform.position, player.position) > enemyEyes.viewRadius)
        {
            alert -= Time.deltaTime;
            if (alert <=0)
            {
                agent.isStopped = true;
                enemyEyes.isFindpalyer = false;
                alert = enmeyAlertTime;
                anim.SetTrigger(HashIDs.enmeyLookingHash);
            }
        }

        AnimatorStateInfo Info = anim.GetCurrentAnimatorStateInfo(0);
        //if (Info.IsName("Base Layer.Looking") && Info.normalizedTime < 1.0f)
        //{
        //    agent.isStopped = true;
        //}
        //else
        //{
        //    agent.enabled = false;
        //}
    }

    public void GunShot()
    {
        Instantiate(bullet,firePosition.position,transform.rotation);
    }
}
