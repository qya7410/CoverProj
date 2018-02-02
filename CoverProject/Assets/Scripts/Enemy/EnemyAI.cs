using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState
{
    Pratrol,
    TakeCover
}
public class EnemyAI : MonoBehaviour 
{
    //public Transform dest;

    private CoverObstacle nearByObstacle;
    public float distanceWithPlayer = 100f;
    public Transform[] pratrolWayPoints;
    public EnemyState currentState = EnemyState.Pratrol;
    public float stayTimer = 10f;


    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;

    private int wayIndex = 0;
    //private int randomIdle;

    public bool findPlayer = false;
    public float vigilantTimer=10f;

    public float betweenShotTime = 0.5f;
    public float resumeFindCoverTimer = 0.5f;
    //public Vector3 coverPos = new Vector3();
	// Use this for initialization
	void Start () 
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        //if (nearByObstacle==null)
        //{
        //nearByObstacle = GetNearByObstacle._instance.destObstcle;
        //}
    }

    // Update is called once per frame
    void Update () 
    {
       
        Debug.Log(agent.destination);
        UpdateAnimation();
        switch (currentState)
        {
            case EnemyState.Pratrol:
                UpdatePratrol();
                break;
            case EnemyState.TakeCover:
                UpdateTakeCover();
                break;
        }
        Debug.DrawRay(agent.destination, Vector3.up * 5, Color.red);

	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.player)
        {
            findPlayer = true;
            currentState = EnemyState.TakeCover;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag==Tags.player)
        {
            findPlayer = false;
        }
    }



    private void UpdateTakeCover()
    {
     
        nearByObstacle = GetNearByObstacle._instance.destObstcle;
        vigilantTimer -= Time.deltaTime;//警惕计时器

        agent.isStopped = false;
        agent.speed = 2;
        transform.LookAt(player);
        //Vector3 coverPos = nearByObstacle.GetCoverPosition();

        //到达目标点后
        if (Vector3.Distance(transform.position,agent.destination)<0.1f)
        {
            betweenShotTime -= Time.deltaTime;

        }
        //else
        //{
        //    betweenShotTime = 0.5f;
        //}
        //警惕时间过后，重制巡逻状态。
        if (vigilantTimer<=0f&&!findPlayer)
        {
            
            //Debug.Log(vigilantTimer);
            currentState = EnemyState.Pratrol;
            vigilantTimer = 10f;
        }

        agent.SetDestination(nearByObstacle.GetCoverPosition());
    }

    private void UpdatePratrol()
    {
        //nearByObstacle.enabled=false;
        agent.speed = 1;
        if (pratrolWayPoints.Length >= 0)
        {

            if (Vector3.Distance(transform.position, pratrolWayPoints[wayIndex].position) <= agent.stoppingDistance)
            {

                agent.isStopped = true;
                stayTimer -= Time.deltaTime;
                if (stayTimer <= 0 && wayIndex < pratrolWayPoints.Length)
                {
                    agent.isStopped = false;
                    wayIndex++;
                    stayTimer = 10f;
                    if (wayIndex >= pratrolWayPoints.Length)
                    {
                        wayIndex = 0;
                    }
                }

            }
            agent.SetDestination(pratrolWayPoints[wayIndex].position);
        }
    }

    //agent.speed决定了敌人是否在走动，敌人只有发现玩家以后才会跑起来。speed会设置成2，恢复巡逻模式的speed=1
    void UpdateAnimation()
    {

        //当前离目标点很靠近的时候，认为达到目标点。
        if (agent.remainingDistance<0.1f)
        {

            //停止移动
            agent.isStopped = true;
            anim.SetBool(HashIDs.enmeyRunHash, false);
            anim.SetBool(HashIDs.enmeyMoveHash, false);
            //在Cover下
            if (currentState==EnemyState.TakeCover)
            {
                //每隔0.5s，射击一次
                if (betweenShotTime <= 0)
                {
                    betweenShotTime = 0.5f;
                    anim.SetBool(HashIDs.enmeyShotHash, true);

                }
            }
            else//在巡逻下，如果后期有其他状态，这里需要更新
            {
                agent.isStopped = false;
                anim.SetInteger(HashIDs.enmeyIdelHash, Random.Range(0, 2));
                anim.SetBool(HashIDs.enmeyShotHash, false);
            }

        }else if (agent.speed>1)//发现玩家，跑步
        {
            agent.isStopped = false;
            anim.SetBool(HashIDs.enmeyShotHash, false);
            anim.SetBool(HashIDs.enmeyRunHash, true);
        }
        else
        {
            agent.isStopped = false;
            anim.SetBool(HashIDs.enmeyRunHash, false);
            anim.SetBool(HashIDs.enmeyMoveHash, true);
        }
    }

}
