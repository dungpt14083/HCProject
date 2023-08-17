using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Config/BallConfig")]
public class BallPhysicConfig : ScriptableObject
{
    public float mass = 1f;
    public float drag = 0.1f;
    public float angularDrag = 0.1f;
    public float maxAngularVelocity = 7f;
    public CollisionDetectionMode collisionMode;
}
