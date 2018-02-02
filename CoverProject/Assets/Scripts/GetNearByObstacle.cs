using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNearByObstacle : MonoBehaviour {

    public static GetNearByObstacle _instance;
    public List<CoverObstacle> obstacles = new List<CoverObstacle>();
    public CoverObstacle destObstcle;
    private EnemyAI enemyAI;
    public float changeObstacleTimer=15f;
    private int index;
    private Transform player;

    private void Awake()
    {
        _instance = this;
        index = 0;
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        enemyAI = GetComponent<EnemyAI>();
        destObstcle = obstacles[index];
    }
    // Use this for initialization
    void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
        //destObstcle = obstacles[index];

        if (enemyAI.currentState==EnemyState.TakeCover)
        {
            changeObstacleTimer -= Time.deltaTime;
            if (Vector3.Distance(transform.position,player.position)<=3&&changeObstacleTimer < 0f)
            {
                    index++;
                    changeObstacleTimer = 5f;


            }
            if (index>=obstacles.Count)
            {
                index = 0;
            }
           
        }
        destObstcle = obstacles[index];
        //Debug.Log(index);
    }
}
