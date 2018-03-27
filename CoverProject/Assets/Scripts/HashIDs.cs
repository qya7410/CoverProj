using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashIDs : MonoBehaviour 
{
    public static int enmeyMoveHash = Animator.StringToHash("Walk");
    public static int enmeyRunHash = Animator.StringToHash("Run");
    public static int enmeyIdelHash = Animator.StringToHash("Idel");
    public static int enmeyShotHash = Animator.StringToHash("Shooting");
    public static int enmeyMoveSpeedHash = Animator.StringToHash("MovingSpeed");
    public static int enmeyCoverHash = Animator.StringToHash("Cover");

}
