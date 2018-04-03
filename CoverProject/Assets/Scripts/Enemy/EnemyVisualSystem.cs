using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualSystem : MonoBehaviour
{
    public bool isFindpalyer;
    //public float findPlayer
    public float viewRadius;//视觉圆的半径

    public float alertTimer=5f;
    
    [Range(0, 360)]
    public float viewAngle;//视觉角度
    public LayerMask obstcleMask;
    public LayerMask playerMask;

    public List<Transform> visibleTarget = new List<Transform>();
    public float meshResolution;//决定了网格需要发射几多射线
    public int edgeFindIteration;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter = new MeshFilter();
    private Mesh viewMesh;
    private Transform palyer;
    private Animator anim;

    float alert=0f;
    bool hightHit;
    bool lowHit;
    public struct ViewCastinfo //检测需要的信息，把它作为一个结构体
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastinfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }


    private void Start()
    {
        anim = GetComponent<Animator>();
        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetWithDelay", 0.5f);

        palyer = GameObject.FindWithTag(Tags.player).transform;
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
        FindVisiblePlayer();

         if ((hightHit||lowHit)||(hightHit&&lowHit))
        {
                isFindpalyer = true;
            }
            else
            {
                alert+=Time.deltaTime;
                if(alert>=alertTimer)
                {
                    isFindpalyer = false;
                    alert=0f;
                }
                
        }
            Debug.Log("hightHit"+hightHit.ToString());
            Debug.Log("lowHit"+lowHit.ToString());
            // Debug.Log("警报时间"+alert.ToString());
    }

    void DrawEnemyVisualisation()//画扇形
    {
        int stepCount = Mathf.RoundToInt(meshResolution * viewAngle);//总共的步长
        float stepAngleSize = viewAngle / stepCount;//单个步长的角度
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastinfo oldViewCast = new ViewCastinfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastinfo newViewCast = ViewCast(angle);
            if (i>0)
            {
                bool isEdgeDstThreshold = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit||(oldViewCast.hit&&newViewCast.hit&&isEdgeDstThreshold))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
           
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCont = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCont];
        int[] triangle = new int[(vertexCont - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCont - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            float globalAngle = transform.eulerAngles.y + stepAngleSize * i;
            if (i < vertexCont - 2)
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
        Vector3 dir = DirectionFormAngle(globleAngle, true);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstcleMask))
        {
            return new ViewCastinfo(true, hit.point, hit.distance, globleAngle);
        }
        else
        {
            return new ViewCastinfo(false, transform.position + dir * viewRadius, viewRadius, globleAngle);
        }
    }


    void FindVisiblePlayer()
    {

        Vector3 dirWithPalyer = palyer.position - transform.position;
        //dirWithPalyer = new Vector3(dirWithPalyer.x, 0, dirWithPalyer.z);

        if(Vector3.Angle(dirWithPalyer,transform.forward)<viewAngle&&Vector3.Distance(transform.position,palyer.position)<viewRadius)
        {
           
            //isFindpalyer = true;
            RaycastHit hitHigh;
            Ray ray = new Ray(transform.position + Vector3.up*1.3f, dirWithPalyer);
            if (Physics.Raycast(ray,out hitHigh,viewRadius))
            {
                if(hitHigh.collider.tag==Tags.player)
                {
                    hightHit=true;
                }
                else{
                    hightHit=false;
                }
            }
           

            RaycastHit hitLow;
            Ray ray2 = new Ray(transform.position + Vector3.up *0.5f, dirWithPalyer);
            if (Physics.Raycast(ray2, out hitLow, viewRadius))
            {
                if(hitLow.collider.tag==Tags.player)
                {
                    lowHit=true;
                }
                else
                {
                    lowHit=false;
                }
            }
            Debug.DrawLine(ray.origin,hitHigh.point, Color.white);
            Debug.DrawLine(ray2.origin,hitLow.point, Color.green);
        }
    }

    //这里定义的是通过角度映射回一个在某平面上的位置信息，此信息通过三角函数可画一个圆
    public Vector3 DirectionFormAngle(float angleInDegrees, bool isGlobleAngle)
    {
        if (!isGlobleAngle)
        {
            //如果是世界空间，加上局部坐标的欧拉角.y
            angleInDegrees += transform.eulerAngles.y;
        }
        //这里是弧度，不是角度！！！角度是同一个圆心两直线的角度。弧度是外围走过的路径。
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    EdgeInfo FindEdge(ViewCastinfo minViewCast,ViewCastinfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeFindIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastinfo newViewCast = ViewCast(angle);
            bool isEdgeDstThreshold = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit&&!isEdgeDstThreshold)
            {
                minAngle = newViewCast.angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = newViewCast.angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA,Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
