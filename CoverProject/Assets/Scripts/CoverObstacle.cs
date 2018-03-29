using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverObstacle : MonoBehaviour 
{
    public Transform[] preCoverPostion;
    public Vector3 coverWithWall;
    private Transform player;

    // Use this for initialization
    void Awake () 
    {
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
    }

    void Start () 
    {
        StartCoroutine(CheckCover());
    }
    private Vector3 GetCoverPosition()
    {
        Vector3 coverPositon =new Vector3();
        if (player!=null)
        {
            Vector3 faceWithWall = (transform.position - player.position).normalized;
            int normalTag = Mathf.RoundToInt(Vector3.Dot(transform.forward.normalized, faceWithWall));
            if (normalTag >= 0)
            {
                coverPositon = preCoverPostion[0].position;
            }
            else if (normalTag < 0)
            {
                coverPositon = preCoverPostion[1].position;
            }
        }

        return coverPositon;
    }

    IEnumerator CheckCover()
    {
        while(true)
        {
            coverWithWall=GetCoverPosition();
            yield return new WaitForSeconds(2f);
        }
    }
}
