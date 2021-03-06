﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNearByObstacle : MonoBehaviour {

    //public static GetNearByObstacle _instance;
    public List<CoverObstacle> obstacles = new List<CoverObstacle>();
    public CoverObstacle destObstcle;
    private EnemyBrain enemyAI;


    public float changeObstacle=15f;
    private int index;
    private Transform player;

    private float changeObstacleTimer = 0f;
    private void Awake()
    {
        //_instance = this;
        index = 0;
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        enemyAI = GetComponent<EnemyBrain>();
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

        if (enemyAI.currentState==EnemyState.Cover)
        {
            changeObstacleTimer -= Time.deltaTime;
            if (changeObstacleTimer < 0f)
            {
                index++;
                changeObstacleTimer = changeObstacle;


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
