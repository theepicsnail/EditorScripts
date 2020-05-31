using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextureToMaterial
{
    [MenuItem("Assets/TextureToMaterial")]
    private static void CreateMaterial()
    {
        foreach (Object o in Selection.objects)
        {
            Texture2D selected = o as Texture2D;
            if (selected)
            {
                var mat = new Material(Shader.Find("Standard"));
                mat.SetTexture("_MainTex", selected);
                if (selected.alphaIsTransparency)
                {
                    mat.SetFloat("_Mode", 3);//transparent.
                }
				string path = AssetDatabase.GetAssetPath(selected);
				path = path.Substring(0,path.LastIndexOf("."));
                AssetDatabase.CreateAsset(mat, path + ".mat");
            }
        }


    }

    [MenuItem("Assets/TextureToMaterial", true)]
    private static bool NewMenuOptionValidation()
    {
        if(Selection.activeObject)
            return Selection.activeObject.GetType() == typeof(Texture2D);
        return false;
    }
}
