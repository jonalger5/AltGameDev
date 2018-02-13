//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(GuardController))]
//public class GuardViewEditor : Editor
//{

//    void OnSceneGUI()
//    {
//        GuardController guard = (GuardController)target;
//        Handles.color = Color.red;
//        Handles.DrawWireArc(guard.transform.position, Vector3.up, Vector3.forward, 360, guard.viewRadius);
//        Vector3 viewAngleA = guard.DirFromAngle(-guard.viewAngle / 2, false);
//        Vector3 viewAngleB = guard.DirFromAngle(guard.viewAngle / 2, false);

//        Handles.DrawLine(guard.transform.position, guard.transform.position + viewAngleA * guard.viewRadius);
//        Handles.DrawLine(guard.transform.position, guard.transform.position + viewAngleB * guard.viewRadius);
//    }
//}
