
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

namespace RAK
{
    public static class TransformExtender
    {
        //public static string GetPath(this Transform current)
        //{
        //    if (current.parent == null)
        //        return "/" + current.name;
        //    return current.parent.GetPath() + "/" + current.name;
        //}
        public static string GetFullHierarchyName(this Transform transform)
        {
            string name = transform.name;
            Transform current = transform.parent;

            while (current != null)
            {
                name = current.name + "/" + name;
                current = current.parent;
            }

            return name;
        }
        public static IEnumerator RotateTowards(this Transform transform, Vector3 lookToPoint, float timeBudget, Action onComplete=null)
        {
            Vector3 directionVec = (lookToPoint - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookToPoint - transform.position, Vector3.up);
            float startTime = Time.time;
            Quaternion startRotation = transform.rotation;
            while (Time.time < startTime + timeBudget)
            {
                float p = Mathf.Clamp01((Time.time - startTime) / timeBudget);
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, p);
                yield return null;
            }
            transform.rotation = targetRotation; 
            onComplete?.Invoke();
        }

        public static IEnumerator MoveToPoint(this Transform transform, Vector3 moveToPoint, float maxSpeed, float acctime_0toMax = .25f,float startSpeed=0, float rotRate=-1, Action onComplete=null, Action<float> onSpeedSet=null)
        {
            float accelration = maxSpeed/acctime_0toMax;
            float speed = startSpeed;
            Vector3 currentDistVec = moveToPoint - transform.position;
            Vector3 directionVec = currentDistVec.normalized;
            Quaternion targetRotation = transform.rotation;
            if (directionVec.sqrMagnitude != 0)
            {
                targetRotation = Quaternion.LookRotation(directionVec, Vector3.up); 
            }
            Debug.DrawRay(transform.position, directionVec, Color.red, 5);
            //Vector3 currentDistVec;
            //do
            //{

            //    if(rotRate>0) transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotRate * Time.deltaTime);

            //    currentDistVec = moveToPoint - transform.position;

            //    speed += accelration * Time.deltaTime;
            //    speed = Mathf.Clamp(speed, 0, maxSpeed);

            //    transform.Translate(directionVec * speed * Time.deltaTime, Space.World);
            //    onSpeedSet?.Invoke(speed);
            //    yield return null;

            //} while (currentDistVec.magnitude > 0.01f && Vector3.Dot(currentDistVec, directionVec) > 0);
            while (true)
            {
                if (rotRate > 0) transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotRate * Time.deltaTime);


                speed += accelration * Time.deltaTime;
                speed = Mathf.Clamp(speed, 0, maxSpeed);

                transform.Translate(directionVec * speed * Time.deltaTime, Space.World);
                onSpeedSet?.Invoke(speed);
                currentDistVec = moveToPoint - transform.position;
                if (Vector3.Dot(currentDistVec, directionVec) > 0)
                {
                    yield return null;
                }
                else
                {
                    transform.position = moveToPoint;
                    break;
                }
            }
            speed = 0;
            onSpeedSet?.Invoke(speed);
            onComplete?.Invoke();
        }

        public static IEnumerator SteadyWalk(this Transform transform, Vector3 pointToMove, float speed, float rotationLerpRate = -1)
        {
            Vector3 directionVec = (pointToMove - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionVec, Vector3.up);
            Vector3 currentDistVec;
            do
            {

                if(rotationLerpRate>0)transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationLerpRate * Time.deltaTime);

                currentDistVec = pointToMove - transform.position;

                transform.Translate(directionVec * speed * Time.deltaTime, Space.World);
                yield return null;

            } while (currentDistVec.magnitude > 0.2f && Vector3.Dot(currentDistVec, directionVec) > 0);
        }

        /// <summary>
        /// Change all aspects of this transform to a different reference transform
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="target"></param>
        /// <param name="timeBudget"></param>
        /// <param name="doPosition"></param>
        /// <param name="doRotation"></param>
        /// <param name="doScale"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static IEnumerator ReTransform(this Transform transform, Transform target, float timeBudget, bool doPosition =true, bool doRotation = true, bool doScale = false, Action onComplete = null)
        {
            Quaternion startRotation = transform.rotation;
            Vector3 startPosition = transform.position;
            Vector3 startScale = transform.localScale;
            
            float startTime = Time.time;
            while (Time.time < startTime + timeBudget)
            {
                float p = Mathf.Clamp01((Time.time - startTime) / timeBudget);
                if(doRotation)transform.rotation = Quaternion.Lerp(startRotation, target.rotation, p);
                if (doPosition) transform.position = Vector3.Lerp(startPosition, target.position, p);
                if (doScale) transform.localScale = Vector3.Lerp(startScale, target.localScale, p);
                yield return null;
            }
            if (doRotation) transform.rotation = target.rotation;
            if (doPosition) transform.position = target.position;
            if (doScale) transform.localScale = target.localScale;
            onComplete?.Invoke();
        }
        public static IEnumerator ReTransformRealTime(this Transform transform, Transform target, float timeBudget, bool doPosition = true, bool doRotation = true, bool doScale = false, Action onComplete = null)
        {
            Quaternion startRotation = transform.rotation;
            Vector3 startPosition = transform.position;
            Vector3 startScale = transform.localScale;

            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < startTime + timeBudget)
            {
                float p = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / timeBudget);
                if (doRotation) transform.rotation = Quaternion.Lerp(startRotation, target.rotation, p);
                if (doPosition) transform.position = Vector3.Lerp(startPosition, target.position, p);
                if (doScale) transform.localScale = Vector3.Lerp(startScale, target.localScale, p);
                yield return null;
            }
            if (doRotation) transform.rotation = target.rotation;
            if (doPosition) transform.position = target.position;
            if (doScale) transform.localScale = target.localScale;
            onComplete?.Invoke();
        }


        public static IEnumerator ReOrient(this Transform transform, Quaternion finalRot, float timeBudget, bool isLocal = false, Action onComplete = null)
        {
            Quaternion startRotation = isLocal ? transform.localRotation : transform.rotation;

            float startTime = Time.time;
            while (Time.time < startTime + timeBudget)
            {
                float p = Mathf.Clamp01((Time.time - startTime) / timeBudget);
                if (isLocal)
                {
                    transform.localRotation = Quaternion.Lerp(startRotation, finalRot, p);
                }
                else
                {

                    transform.rotation = Quaternion.Lerp(startRotation, finalRot, p);
                }
                yield return null;
            }
            if (isLocal)
            {
                transform.localRotation = finalRot;
            }
            else
            {

                transform.rotation = finalRot;
            }
            onComplete?.Invoke();
        }

        public static IEnumerator GetGrabbed(this Transform transform, Transform grabPoint, float maxTime, float lerpRate, bool doRotation = true, float scaleRatio = 1, Action onComplete = null)
        {
            Vector3 startScale = transform.localScale;
            Vector3 endScale = transform.localScale*scaleRatio;

            float startTime = Time.time;
            while (Time.time < startTime + maxTime)
            {
                //float p = Mathf.Clamp01((Time.time - startTime) / maxTime);
                float p = (Time.time - startTime) * lerpRate;
                if (doRotation) transform.rotation = Quaternion.Lerp(transform.rotation, grabPoint.rotation, p);
                transform.position = Vector3.Lerp(transform.position, grabPoint.position, p);
                transform.localScale = Vector3.Lerp(startScale, endScale, p);
                yield return null;
            }
            if (doRotation) transform.rotation = grabPoint.rotation;
            transform.position = grabPoint.position;
            transform.localScale = endScale;
            transform.SetParent(grabPoint);
            onComplete?.Invoke();
        }

        public static IEnumerator GetCustomGrabbed(this Transform transform, Transform grabPoint, float maxTime, float lerpRate, float scaleRatio = 1, Vector3 offset =default(Vector3),Vector3 eulRot= default(Vector3), Action onComplete = null)
        {



            transform.SetParent(grabPoint);
            Vector3 startScale = transform.localScale;
            Vector3 endScale = transform.localScale * scaleRatio;
            //Debug.Log($"gettingGrabbed {startScale}->{endScale}");
            Quaternion finalLocalRot = Quaternion.Euler(eulRot);

            float startTime = Time.time;
            while (Time.time < startTime + maxTime)
            {
                //float p = Mathf.Clamp01((Time.time - startTime) / maxTime);
                float p = (Time.time - startTime) * lerpRate;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, grabPoint.rotation, p);
                transform.localPosition = Vector3.Lerp(transform.localPosition, offset, p);
                transform.localScale = Vector3.Lerp(startScale, endScale, p);
                yield return null;
            }
            transform.localRotation = finalLocalRot;
            transform.localPosition = offset;
            transform.localScale = endScale;
            onComplete?.Invoke();
        }

        public static float AngleFromForward_Degrees(this Transform obj, Transform target)
        {
            Vector3 objForward = obj.forward;
            Vector3 directionToTarget = (target.position - obj.position).normalized;
            float angle = Vector3.Angle(objForward, directionToTarget);
            return angle;
        }
        public static float AngleFromForward_HorizontalDegrees(this Transform obj, Transform target)
        {
            Vector3 objForward = obj.forward.CleanY().normalized;
            Vector3 directionToTarget = (target.position - obj.position).CleanY().normalized;
            float angle = Vector3.Angle(objForward, directionToTarget);
            return angle;
        }
        public static float AngleFromForward_VerticalDegrees(this Transform obj, Transform target)
        {
            Vector3 objForward = obj.forward.CleanX().normalized;
            Vector3 directionToTarget = (target.position - obj.position).CleanX().normalized;
            float angle = Vector3.Angle(objForward, directionToTarget);
            return angle;
        }

        public static bool IsTargetWithinAngles(this Transform obj, Transform target, Vector2 angleBounds)
        {
            float x = obj.AngleFromForward_HorizontalDegrees(target);
            float y = obj.AngleFromForward_VerticalDegrees(target);

            //$"{x} _ {y}".Debug();
            return  x <= angleBounds.x && y <= angleBounds.y;
        }

        public static Transform CreateTrackerTransform(this Transform reference, Transform parent, string name)
        {
            Transform tracker = new GameObject(name).transform;
            tracker.SetParent(parent);
            tracker.position = reference.position;
            tracker.rotation = reference.rotation;
            tracker.localScale = reference.localScale;
            return tracker;
        }
        public static IEnumerator Rescale(this Transform target, Vector3 targetScale, float tSpan, Func<float, float> curve=null, Action onComplete=null)
        {
            Vector3 startScale = target.localScale;
            float startTime = Time.time;
            while (Time.time < startTime + tSpan)
            {
                float pp = Mathf.Clamp01((Time.time - startTime) / tSpan);
                float p = curve?.Invoke(pp) ?? pp;
                target.localScale = Vector3.Lerp(startScale, targetScale, p);
                yield return null;
            }
            target.localScale = targetScale;
            onComplete?.Invoke();
        }
        public static IEnumerator LocalRepos(this Transform target, Vector3 targetPos, float tSpan, Func<float,float> curve=null, Action onComplete=null)
        {
            Vector3 startPos = target.localPosition;
            float startTime = Time.time;
            Vector3 dir = (targetPos - startPos).normalized;
            float mag = (targetPos - startPos).magnitude;
            while (Time.time < startTime + tSpan)
            {
                float pp = Mathf.Clamp01((Time.time - startTime) / tSpan);
                float p = curve?.Invoke(pp) ?? pp;
                target.localPosition = startPos + dir * mag * p;// Vector3.Lerp(startPos, targetPos, p);
                yield return null;
            }
            target.localPosition = targetPos;
            onComplete?.Invoke();
        }
        public static IEnumerator ReOrient(this Transform target, Quaternion targetRotation, float tSpan, Func<float, float> curve, Action onComplete = null)
        {
            Quaternion startVal = target.rotation;
            float startTime = Time.time;
            while (Time.time < startTime + tSpan)
            {
                float pp = Mathf.Clamp01((Time.time - startTime) / tSpan);
                float p = curve?.Invoke(pp) ?? pp;
                target.rotation = Quaternion.Slerp(startVal, targetRotation, p);
                yield return null;
            }
            target.rotation = targetRotation;
            onComplete?.Invoke();
        }
    }


}