using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;

namespace RAK
{

    public class SpeedMath
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct intfloatunion
        {
            [FieldOffset(0)]
            public float f;
            [FieldOffset(0)]
            public int i;
        }
        public static intfloatunion ifu = new intfloatunion();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float val)
        {
            ifu.i = 0;
            ifu.f = val;
            ifu.i = (1 << 29) + (ifu.i >> 1) - (1 << 22);

            return ifu.f;
        }
        public static float SqrtGPT(float x)
        {
            int i = BitConverter.ToInt32(BitConverter.GetBytes(x), 0);
            i = 0x5f375a86 - (i >> 1);
            float y = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
            y = y * (1.5f - (0.5f * x * y * y));
            return y;
        }
    }
}
