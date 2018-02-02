using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualSystem : MonoBehaviour {
    //public float findPlayer
    public float viewRadius;//视觉圆的半径

     
    [Range(0,360)]
    public float viewAngle;//视觉角度
    public LayerMask obstcleMask;
    public LayerMask playerMask;

    public List<Transform> visibleTarget = new List<Transform>();
    public float meshResolution;//决定了网格需要发射几多射线


    public MeshFilter viewMeshFilter = new MeshFilter();
    private Mesh viewMesh;
    public struct ViewCastinfo //检测需要的信息，把它作为一个结构体
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastinfo(bool _hit,Vector3 _point,float _dst,float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }


    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetWithDelay",0.2f);
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisiblePlayer();
        }

    }

    private void Update()
    {
        DrawEnemyVisualisation();
    }

    void DrawEnemyVisualisation()//画扇形
    {
        int stepCount = Mathf.RoundToInt(meshResolution * viewAngle);//总共的步长
        float stepAngleSize = viewAngle / stepCount;//单个步长的角度
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastinfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCont = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCont];
        int[] triangle = new int[(vertexCont - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCont-1; i++)
        {
            vertices[i + 1] =transform.InverseTransformPoint(viewPoints[i]);
            float globalAngle = transform.eulerAngles.y + stepAngleSize * i;
            if (i<vertexCont - 2)
            {
                triangle[i * 3] = 0;
                triangle[i * 3 + 1] = i + 1;
                triangle[i * 3 + 2] = i + 2;
            }

        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangle;
        viewMesh.RecalculateNormals();
    }

    ViewCastinfo ViewCast(float globleAngle)
    {
        Vector3 dir = DirectionFormAngle(globleAngle,true);

        RaycastHit hit;
        if (Physics.Raycast(transform.position,dir,out hit,viewRadius,obstcleMask))
        {
            return new ViewCastinfo(true, hit.point, hit.distance, globleAngle);
        }
        else
        {
            return new ViewCastinfo(false, transform.position+dir*viewRadius, viewRadius, globleAngle);
        }
    }


    void FindVisiblePlayer()
    {
        visibleTarget.Clear();//保证列表的实时更新
        //教程上是一个数组，去实现玩家对多个敌人的查找，我这里玩家只有一个，因此直接取第一个索引即可
        Collider[] player = Physics.OverlapSphere(transform.position,viewRadius,playerMask);


        for (int i = 0; i < player.Length; i++)
        {
            Transform playerTrans = player[i].transform;
            //敌人与玩家的方向
            Vector3 enemyToPlayerDir = (playerTrans.transform.position - transform.position).normalized;
            //如果敌人到玩家的夹角，小于iewAngle/2，此时应该发射射线。。
            if (Vector3.Angle(transform.forward, enemyToPlayerDir) < viewAngle / 2)
            {
                //记录敌人到玩家的距离
                float distanceWithPlayer = Vector3.Distance(transform.position, player[0].transform.position);
                //射线开始发射。
                if (!Physics.Raycast(transform.position, enemyToPlayerDir, distanceWithPlayer,obstcleMask))
                {
                    visibleTarget.Add(playerTrans);
                }
            }
        }



    }
    //这里定义的是通过角度映射回一个在某平面上的位置信息，此信息通过三角函数可画一个圆
    public Vector3 DirectionFormAngle(float angleInDegrees,bool isGlobleAngle)
    {
        if (!isGlobleAngle)
        {
            //如果是世界空间，加上局部坐标的欧拉角.y
            angleInDegrees += transform.eulerAngles.y;
        }
        //这里是弧度，不是角度！！！角度是同一个圆心两直线的角度。弧度是外围走过的路径。
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
