using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureExtension 
{
    public static string ToBase64 (this Texture2D tex)
    {
        byte[] texByte = tex.EncodeToPNG ();
        return System.Convert.ToBase64String (texByte);
    }

    public static Texture2D FromBase64 (string base64Tex)
    {
        if (string.IsNullOrEmpty(base64Tex)) return null;
        var texByte = System.Convert.FromBase64String (base64Tex);
        Texture2D tex = new Texture2D (2, 2);
        return tex.LoadImage (texByte) ? tex : null;
    }
}
