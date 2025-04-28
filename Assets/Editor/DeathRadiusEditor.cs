using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityBehaviour))]
public class DeathRadiusEditor : Editor
{
    private void OnSceneGUI()
    {
        EntityBehaviour entityBehaviour = (EntityBehaviour)target;

        // Set the color for the arc
        Handles.color = Color.red;
        // Draw a wire arc (full circle) representing the death radius
        Handles.DrawWireArc(
            entityBehaviour.transform.position, // center position
            Vector3.up,                          // normal axis (upwards)
            Vector3.forward,                     // starting direction
            360,                                 // angle (360° for full circle)
            entityBehaviour.deathRadius          // radius length (your variable)
        );

    }
    
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
