using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAK;
using UnityEditor;

namespace RAK
{
    public static class RayCastExtender 
    {
        public static bool DoesRayHit(this Camera cam, Vector2 screenPos,out RaycastHit hit, LayerMask layers,  float maxDist = 100)
        {
            Ray ray = cam.ScreenPointToRay(screenPos);
            return Physics.Raycast(ray,out hit, maxDist, layers);
        }

        public static bool DoesMousePositionRayHit(this Camera cam, out RaycastHit hit, LayerMask layers, float maxDist = 100)
        {
            return cam.DoesRayHit(Input.mousePosition, out hit,layers,maxDist);
        }

        public static bool IsMousePressedAndHitting(this Camera cam, out RaycastHit hit, LayerMask layers, float maxDist = 100)
        {
            if (!Input.GetMouseButton(0))
            {
                hit = default(RaycastHit);
                return false;
            }
            else
            {
                return cam.DoesMousePositionRayHit(out hit, layers, maxDist);
            }
        }


        public static T IsMousePressedAndHittingType<T>(this Camera cam,  LayerMask layers, float maxDist = 100)
        {
            RaycastHit hit;
            if (cam.IsMousePressedAndHitting(out hit, layers, maxDist))
            {
                T t =  hit.transform.GetComponent<T>();

            }
            return default(T);
        }






        public static bool DoesForwardRayHit(this Transform transform, out RaycastHit hit, LayerMask layers, float maxDist = 100)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            //Debug.DrawRay(ray.origin,ray.direction*maxDist,Color.yellow,3);
            return Physics.Raycast(ray, out hit, maxDist, layers);
        }
        public static T DoesForwardRayHit<T>(this Transform transform, LayerMask layers, float maxDist = 100)
        {
            RaycastHit hit;
            if (transform.DoesForwardRayHit(out hit, layers, maxDist))
            {
                //Debug.Log(hit.collider);
                T t = hit.collider.transform.GetHigherComponent<T>();
                return t;
            }
            return default(T);
        }


        public static bool IsMousePressedAndHitting(this Transform transform, out RaycastHit hit, LayerMask layers, float maxDist = 100)
        {
            if (!Input.GetMouseButton(0))
            {
                hit = default(RaycastHit);
                return false;
            }
            else
            {
                return transform.DoesForwardRayHit(out hit, layers, maxDist);
            }
        }
        public static T IsMousePressedAndHittingType<T>(this Transform transform, LayerMask layers, float maxDist = 100)
        {
            RaycastHit hit;
            if (transform.IsMousePressedAndHitting(out hit, layers, maxDist))
            {
                T t = hit.collider.transform.GetHigherComponent<T>();
                return t;

            }
            return default(T);
        }

        public static Vector3 GroundV3(this Vector3 v3, LayerMask layers, float maxRange = 100,float upOffset = 0)
        {
            RaycastHit hit;
            Vector3 origin = v3 + upOffset * Vector3.up;
            if (Physics.Raycast(origin, Vector3.down, out hit, maxRange, layers))
            {

                Debug.DrawLine(origin, hit.point, Color.green, 3);
                return hit.point;
            }
            else
            {
                Debug.DrawRay(origin, Vector3.down * maxRange, Color.red, 3);
                return v3;
            }
        }
        public static (Vector3,Vector3) GroundV4_IgnoreW(this Vector4 v4, LayerMask layers, float maxRange = 100, float upOffset = 0)
        {
            Vector3 v3 = v4;
            RaycastHit hit;
            Vector3 origin = v3 + upOffset * Vector3.up;
            if (Physics.Raycast(origin, Vector3.down, out hit, maxRange, layers))
            {

                Debug.DrawLine(origin, hit.point, Color.green, 3);
                return (hit.point,hit.normal);
            }
            else
            {
                Debug.DrawRay(origin, Vector3.down * maxRange, Color.red, 3);
                return (v3,Vector3.up);
            }
        }
        public static (Vector3, float) GroundV4_IgnoreW_normalOffset(this Vector4 v4,Vector3 forward, LayerMask layers, float maxRange = 100, float upOffset = 0)
        {
            (Vector3 p,Vector3 d) = v4.GroundV4_IgnoreW(layers, maxRange, upOffset);

            float angle = VectorMathExtender.AngleOffAroundAxis(d, Vector3.up, forward);
            return (p,angle);
        }

    }
}