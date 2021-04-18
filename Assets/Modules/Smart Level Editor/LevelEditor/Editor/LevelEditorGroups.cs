using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;

    public partial class LevelEditor
    {
        Vector2 mainScroll;

        void DRAW_GROUPS()
        {
            DrawHeader("Groups", "LODGroup Icon");
            
            Rect rect = BeginVertical("Helpbox");
            DRAW_GROUP_CONTENTS();
            DragAndDropGroupMessage();
            EndVertical();

            GROUP_EVENTS(rect);
        }

        void DragAndDropGroupMessage()
        {
            if (createGroup) { return; }

            if (null == data.selected) { return; }

            GL.FlexibleSpace();
            EditorGUISmart.BeginCenterArea();
            GL.Label("Drag & Drop", Styles.CenteredHelpbox, GL.Width(200));
            EditorGUISmart.EndCenterArea();
            GL.FlexibleSpace();
        }

        void DRAW_GROUP_CONTENTS()
        {
            // Separator
            Separator();
            // Search bar
            DrawSearchBar();

            // Separator
            Separator();
            DRAW_GROUP_SETTINGS();
            DRAW_GROUP_TITLE();
            // Separator
            Separator();

            DRAW_GROUP_OBJECTS();
        }
        
        void DrawSearchBar()
        {
            BeginHorizontal();
            DrawSearchBarField();
            EndHorizontal();
        }

        void DrawSearchBarField()
        {
            filter = GL.TextField(filter, "SearchTextField");
            if (GL.Button("", "SearchCancelButton")) { filter = ""; }
        }
        
        bool createGroup;
        string newGroupName;

        void DRAW_GROUP_SETTINGS()
        {
            BeginHorizontal();
            DRAW_GROUP_BUTTONS();
            DRAW_NEW_GROUP_BUTTON();
            EndHorizontal();

            if (createGroup)
            {
                Separator();
                BeginHorizontal();
                newGroupName = TextField("New Group Name", newGroupName);
                if (GL.Button("Create", "MiniButtonRight", GL.ExpandWidth(false)))
                {
                    data.Add(new LevelEditorGroup(newGroupName));
                    data.selected = data[data.count - 1];
                    createGroup = false;

                    EditorUtility.SetDirty(data);
                }
                EndHorizontal();
            }
        }

        void DRAW_GROUP_BUTTONS()
        {
            for (int i = 0; i < data.count; i++)
            {
                EditorGUI.BeginChangeCheck();
                GL.Toggle(data.selected == data[i], data[i].name, i == 0 ? "ButtonLeft" : "ButtonMid");
                if (EditorGUI.EndChangeCheck())
                {
                    createGroup = false;
                    data.selected = data[i];
                    Repaint();
                }
            }
        }

        void DRAW_NEW_GROUP_BUTTON()
        {
            if(0 == data.count)
            {
                if (GL.Button("Add", "Button"))
                {
                    data.selected = null;
                    createGroup = !createGroup;
                }
            }
            else
            {
                if (GL.Button("+", "ButtonRight", GL.ExpandWidth(false)))
                {
                    data.selected = null;
                    createGroup = !createGroup;
                }
            }
        }

        void DRAW_GROUP_TITLE()
        {
            if (createGroup) { return; }

            if (null == data.selected) { return; }

            if (!data.Contains(data.selected)) { return; }

            Separator();

            BeginHorizontal();
            GL.Label(string.Format("Name: {0} - Objects: {1}", data.selected.name, data.selected.Count), titleStyle);
            if (GL.Button("Delete", "MiniButtonRight", GL.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Level Editor", "Delete Group?", "Yes", "No"))
                {
                    data.Remove(data.selected);
                    data.selected = null;
                }
            }
            EndHorizontal();
        }

        void DRAW_GROUP_OBJECTS()
        {
            if (createGroup) { return; }

            if (null == data.selected) { return; }

            if (!data.Contains(data.selected)) { return; }

            Object value;
            int maxColumns = (int)(position.width / imgWidth2);
            int rowElements = 0;
            bool rowOpen = false;
            bool isSelected = false;

            for (int i = 0; i < data.selected.Count; i++)
            {
                deleteElement = false;
                value = data.selected[i];

                if (!value) { continue; }

                if (!value.name.Contains(filter)) { continue; }

                isSelected = selected.Contains(value);


                if (rowElements == 0) { BeginHorizontal(); rowOpen = true; }

                Rect rect = BeginVertical(isSelected ? Styles.progressBar : GUIStyle.none, GL.MaxWidth(80), GL.MaxHeight(80));
                DrawElement(value);
                EndVertical();

                ObjectEvent(value, rect);

                rowElements++;
                if (rowElements == maxColumns) { EndHorizontal(); rowOpen = false; rowElements = 0; }

                if (deleteElement) { break; }
            }

            if (rowOpen) { EndHorizontal(); }
        }

        bool deleteElement;

        void DrawElement(Object value)
        {
            GUIStyle label = new GUIStyle("label");
            label.wordWrap = true;
            

            Texture2D tex = GetPreview(value);
            GL.Label(tex, buttonOps100);

            BeginHorizontal();
            GL.Label(value.name, label);
            if (GL.Button("x", "MiniButtonRight", GL.ExpandWidth(false)))
            {
                data.selected.Remove(value);
                selected.Remove(value);

                deleteElement = true;
            }
            EndHorizontal();
        }
    }
}
