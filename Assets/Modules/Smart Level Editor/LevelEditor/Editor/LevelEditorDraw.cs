using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;
    using static EditorGUIUtility;

    public partial class LevelEditor
    {

        GUILayoutOption[] buttonOps80 = new GUILayoutOption[]
        {
            GL.Width(imgWidth1),
            GL.Height(imgHeight1)
        };

        GUILayoutOption[] buttonOps100 = new GUILayoutOption[]
        {
            GL.Width(imgWidth2),
            GL.Height(imgHeight2)
        };

        void DRAW_CREATE_DATA()
        {
            if (GL.Button("Create Data", "LargeButton"))
            {
                string path = EditorUtility.SaveFilePanelInProject("", "", "asset", "");

                data = CreateInstance<LevelEditorData>();

                AssetDatabase.CreateAsset(data, path);

                AssetDatabase.Refresh();
            }
        }

        void BeginIconSize()
        {
            defaultIconSize = GetIconSize();
        }

        void EndIconSize()
        {
            SetIconSize(defaultIconSize);
        }

        const float headerIconSize = 32;

        void DrawHeader(string name, string icon)
        {
            BeginHorizontal(headerStyle);
            BeginIconSize();
            SetIconSize(new Vector2(headerIconSize, headerIconSize));

            GUIContent content = IconContent(icon);
            content.text = name;

            GL.Box(content, titleStyle);
            EndIconSize();
            EndHorizontal();
        }
        
        void RemoveButton(Object value, params GUILayoutOption[] ops)
        {
            Texture2D tex = GetPreview(value);

            if (GL.Button(tex, "label", ops))
            {
                selected.Remove(value);

                CreatePreviews();

                if (view)
                    view.Repaint();
                
                Repaint();
            }
        }

        void AddButton(Object value, params GUILayoutOption[] ops)
        {
            Texture2D tex = GetPreview(value);

            if (GL.Button(tex, "label", ops))
            {
                selected.Add(value);

                if (view)
                    view.Repaint();

                Repaint();
            }
        }

        void Separator()
        {
            GL.Space(standardVerticalSpacing * 4);
        }

        void SceneGUIButtons()
        {
            Vector3 x = new Vector3(1, 0, 0);
            Vector3 y = new Vector3(0, 1, 0);
            Vector3 z = new Vector3(0, 0, 1);

            Handles.BeginGUI();
            GL.FlexibleSpace();
            BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            GL.Toggle(rotationAxis == x, "X", "Button");
            if (EditorGUI.EndChangeCheck())
            {
                rotationAxis = x;
            }

            EditorGUI.BeginChangeCheck();
            GL.Toggle(rotationAxis == y, "Y", "Button");
            if (EditorGUI.EndChangeCheck())
            {
                rotationAxis = y;
            }


            EditorGUI.BeginChangeCheck();
            GL.Toggle(rotationAxis == z, "Z", "Button");
            if (EditorGUI.EndChangeCheck())
            {
                rotationAxis = z;
            }

            HelpBox("Press (x) to rotate", MessageType.None);
            
            GL.FlexibleSpace();
            EndHorizontal();
            GL.Space(25);
            Handles.EndGUI();
        }
    }
}
