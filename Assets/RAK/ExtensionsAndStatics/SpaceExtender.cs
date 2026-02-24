using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RAK;

public static class SpaceExtender
{
    //static bool canvasStandardadized = false;
    //static float reverseScaleFactor;
    //static Vector2 canvasCentreDifference;

    //public static void StandardizeCanvasForSpaceTransformation(this CanvasScaler canvasScaler)
    //{

    //    reverseScaleFactor = canvasScaler.referenceResolution.y / Screen.height;
    //    canvasCentreDifference = new Vector2(Screen.width, Screen.height) * reverseScaleFactor / 2;
    //    canvasStandardadized = true;
    //}
    //public static Vector2 ConvertUISpaceToMouseSpace(this Vector2 v)
    //{
    //    if (!canvasStandardadized) throw new Exception("Canvas not standardized in this playthrough");

    //    return (v + canvasCentreDifference) / reverseScaleFactor;
    //}

    //public static Vector2 ConvertMouseSpaceToUISpace(this Vector2 v)
    //{
    //    if (!canvasStandardadized) throw new Exception("Canvas not standardized in this playthrough");

    //    return v * reverseScaleFactor - canvasCentreDifference;
    //}

    //public static Vector2 ConvertMouseSpaceToUISpace(this Vector3 v)
    //{
    //    return ConvertMouseSpaceToUISpace((Vector2)v);
    //}
}
