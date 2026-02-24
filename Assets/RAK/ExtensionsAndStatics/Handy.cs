using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Handy {

    public static void SetHorizontalSpinByVelocity(this Rigidbody rgbd, float mult = 1)
    {
        Vector3 a = new Vector3(rgbd.linearVelocity.z*mult, rgbd.angularVelocity.y, rgbd.linearVelocity.x*mult);
        rgbd.angularVelocity = a;
        //rgbd.AddTorque(a-rgbd.angularVelocity,ForceMode.VelocityChange);
    }


 


    public static Transform Duplicate_WithLocalVals(this Transform sample, Transform nuparent, bool isEmpty)
    {
        GameObject go;
        if (!isEmpty)
        {
            go = MonoBehaviour.Instantiate(sample.gameObject);// new GameObject(target.name);
        }
        else
        {
            go = new GameObject(sample.name);
        }
        go.name = sample.name;
        go.transform.parent = nuparent;
        go.transform.localPosition = sample.localPosition;
        go.transform.localRotation = sample.localRotation;
        return go.transform;
    }
    public static void SetParent_WithSameLocals(this Transform target, Transform nuparent)
    {
        Vector3 locPos = target.localPosition;
        Quaternion locRot = target.localRotation;
        target.parent = nuparent;
        target.localPosition = locPos;
        target.localRotation = locRot;
    }
    public static void SetParentWithLocalsLikeAnother(this Transform target, Transform nuparent, Transform sample)
    {
        Vector3 locPos = sample.localPosition;
        Quaternion locRot = sample.localRotation;
        target.parent = nuparent;
        target.localPosition = locPos;
        target.localRotation = locRot;
    }





    public static void DrawArcYaxis(Vector3 position, Vector3 direction, float radius, float angle,  int divisions, Color borderColor, Color fillColor)
    {
        divisions = Mathf.Clamp (divisions,1,100);
        Vector3 fwd = direction;
        float halfAngle = angle / 2;
        float sightRange = radius;

        float f;
        int N = divisions + 1;
        Mesh m = new Mesh();
        Vector3[] vertArr = new Vector3[N+1];
        int[] trisArr = new int[(N-1)*3];
        Vector3[] normArr = new Vector3[N+1];


        vertArr [0] = position;   
        normArr [0] = Vector3.up;


        for (int i = 0; i < N; i++) {
            f = -halfAngle + (2*halfAngle*i)/(N-1);
            Vector3 pos = position + (Quaternion.Euler (0, f, 0) * fwd)*sightRange;
            vertArr [i + 1] = pos;
            if (i < N-1) {
                trisArr [3 * i + 0] = 0;
                trisArr [3 * i + 1] = i+1;
                trisArr [3 * i + 2] = i+2;
            }
            normArr [i + 1] = Vector3.up;
        }
        m.vertices = vertArr;
        m.triangles = trisArr;
        m.normals = normArr;

        for (int i = 0; i < vertArr.Length -2 ; i++)
        {
            Gizmos.color = borderColor;

            if(i!=vertArr.Length-1)
                Gizmos.DrawLine(vertArr[i+1], vertArr[i+2]);
            //else if(i==vertArr.Length-2)
                
            else
                Gizmos.DrawLine(vertArr[1], vertArr[vertArr.Length-1]);
        }

        Gizmos.color = fillColor;
        Gizmos.DrawMesh(m);
    }
    public static void DrawDiscYaxis(Vector3 position, float radius, int divisions, Color borderColor, Color fillColor)
    {
        DrawArcYaxis(position,Vector3.forward,radius,360,divisions,borderColor,fillColor);
    }
	public static void  DrawSight(float angle, float range, int divisions, Transform thisTransform,Color color)
	{
        DrawArcYaxis(thisTransform.position,thisTransform.forward, range,angle,divisions, new Color(0,0,0,0) ,color);
	}

}