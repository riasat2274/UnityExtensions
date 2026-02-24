using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAK
{
    public static class UnityMiscExtender
    {
        public static Texture2D GetStaticCopy(this RenderTexture rendTex)
        {
            Texture2D tex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.RGBA32, false);
            RenderTexture.active = rendTex;
            tex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            return tex;
        }
    }
}
