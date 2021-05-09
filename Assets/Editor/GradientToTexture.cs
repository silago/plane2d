/// http://answers.unity3d.com/questions/461958/generate-a-gradient-texture-from-editor-script.html
/// http://answers.unity3d.com/questions/436295/how-to-have-a-gradient-editor-in-an-editor-script.html
#region
using System.IO;
using UnityEditor;
using UnityEngine;
#endregion
public class GradientToTexture : EditorWindow
{
    public Gradient gradient;

    private int width;
    private void OnGUI()
    {
        if (gradient == null) gradient = new Gradient();
        EditorGUI.BeginChangeCheck();
        var serializedGradient = new SerializedObject(this);
        var colorGradient = serializedGradient.FindProperty("gradient");
        EditorGUILayout.PropertyField(colorGradient, true, null);
        //if(EditorGUI.EndChangeCheck()) {
        serializedGradient.ApplyModifiedProperties();
        //}
        width = Mathf.Clamp(EditorGUILayout.IntField("Width", width), 1, 4096);
        if (gradient != null)
        {
            var tex = new Texture2D(width, 1);
            for (var i = 0; i < width; i++) tex.SetPixel(i, 0, gradient.Evaluate(i / (float)width));
            if (GUILayout.Button("Gen"))
            {
                var path = EditorUtility.SaveFilePanel("Save texture as PNG", "", "foo.png", "png");
                if (path.Length != 0) GenTexture(tex, path);
            }
        }


    }
    [MenuItem("Window/GradientToTexture")]
    // Use this for initialization
    private static void Init()
    {
        var window = (GradientToTexture)GetWindow(typeof(GradientToTexture));
        window.maxSize = new Vector2(300, 200);
    }
    private void GenTexture(Texture2D tex, string path)
    {
        var pngData = tex.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(path, pngData);
    }
}
