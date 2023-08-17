using System.Collections;
using System.Collections.Generic;
using HcGames;
using UnityEngine;

public static class ProtobufModelExtension {

    public static Vector3 ToVector3(this HCVector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }

    public static HCVector3 ToHCVector3(this Vector3 vector3)
    {
        return new HCVector3()
        {
            X = vector3.x,
            Y = vector3.y,
            Z = vector3.z
        };
    }

    public static Vector3 ToVector3(this HCVector2 vector2)
    {
        return new Vector3(vector2.X, 0.0f, vector2.Y);
    }

    public static Quaternion ToQuaternion(this HCQuaternion quaternion)
    {
        return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    }

    public static HCQuaternion ToHCQuaternion(this Quaternion quaternion)
    {
        return new HCQuaternion()
        {
            X = quaternion.x,
            Y = quaternion.y,
            Z = quaternion.z,
            W = quaternion.w
        };
    }

    public static NetworkObjectPhysicData ToNetworkPhysicData(this NetworkPhysicsObject networkPhysicsObject)
    {
        return new NetworkObjectPhysicData()
        {
            Id = networkPhysicsObject.ballID,
            Position = networkPhysicsObject.Rigidbody.position.ToHCVector3(),
            Rotation = networkPhysicsObject.Rigidbody.rotation.ToHCQuaternion(),
            Velocity = networkPhysicsObject.Rigidbody.velocity.ToHCVector3(),
            AngularVelocity = networkPhysicsObject.Rigidbody.angularVelocity.ToHCVector3()
        };
    }
    
    
}

