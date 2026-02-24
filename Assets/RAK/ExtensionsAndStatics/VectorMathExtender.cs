using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RAK
{
    public static class VectorMathExtender
    {
        public static Vector3 SetZ(this Vector2 vector,float z)
        {
            return new Vector3(vector.x, vector.y,z);
        }
        public static Vector2 XY2(this Vector3 vector)
        {
            return new Vector2(vector.x,vector.y);
        }
        public static Vector2 XZ2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 CleanY(this Vector3 vector, float newY=0)
        {
            vector.y = newY;
            return vector ;
        }
        public static Vector3 CleanX(this Vector3 vector, float newX = 0)
        {
            vector.x = newX;
            return vector;
        }
        public static Vector3 CleanZ(this Vector3 vector, float newZ = 0)
        {
            vector.z = newZ;
            return vector;
        }
        public static Vector2 ReduceMagnitudeByUnits(this Vector2 v, float reduction, float limit)
        {
            float m = v.magnitude;
            Vector2 dir = v.normalized;
            m = Mathf.Clamp(m-reduction,limit,m);
            return dir*m;
        }
        public static Vector3 ClampMagnitude(this Vector3 v, float minMag, float MaxMag)
        {
            float m = v.magnitude;
            Vector3 dir = v.normalized;
            m = Mathf.Clamp(m, minMag, MaxMag);
            return dir * m;
        }
        public static float AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis, bool clockwise = false)
        {
            Vector3 right;
            if (clockwise)
            {
                right = Vector3.Cross(forward, axis);
                forward = Vector3.Cross(axis, right);
            }
            else
            {
                right = Vector3.Cross(axis, forward);
                forward = Vector3.Cross(right, axis);
            }
            return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * 180 / Mathf.PI;
        }

    }
}