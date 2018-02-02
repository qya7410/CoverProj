using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyVisualSystem))]
public class EnemyVisualSystemEditor : Editor 
{
    private void OnSceneGUI()
    {
        EnemyVisualSystem fov = (EnemyVisualSystem)target;//这里需要强转 因为objet是泛型？
        Handles.color = Color.white;
        //此方法画一个圆，从玩家的位置开始，绕着y轴，从forward方向开始，画一个半径为多少，角度为360的圆
        Handles.DrawWireArc(fov.transform.position,Vector3.up,Vector3.forward,360,fov.viewRadius);

        Vector3 viewAngleA = fov.DirectionFormAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirectionFormAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, viewAngleA * fov.viewRadius + fov.transform.position);
        Handles.DrawLine(fov.transform.position, viewAngleB * fov.viewRadius + fov.transform.position);


        Handles.color = Color.blue;
        foreach (Transform player in fov.visibleTarget)
        {
            Handles.DrawLine(fov.transform.position,player.position);

        }
    }
}
