using UnityEngine;
using UnityEditor;

namespace Smart
{
    using GL = GUILayout;
    using static EditorGUILayout;

    public partial class LevelEditor
    {
        void DRAW_SELECTED()
        {
            string title = string.Format("Selected ({0})", selected.Count);
            
            DrawHeader(title, "GameObject Icon");
            DRAW_SELECTED_CONTENTS();
        }
        
        void DRAW_SELECTED_CONTENTS()
        {

            if (selected.Count == 0)
            {
                BeginVertical("helpbox");
                GL.FlexibleSpace();
                GL.Label("Nothing Selected", "CenteredLabel");
                GL.FlexibleSpace();
                EndVertical();

                return;
            }

            int maxColumns = (int)(position.width / imgWidth1);
            int rowElements = 0;
            bool rowOpen = false;
            
            for (int i = 0; i < selected.Count; i++)
            {
                if (rowElements == 0) { BeginHorizontal(); rowOpen = true; }
                RemoveButton(selected[i], buttonOps80);
                rowElements++;
                if (rowElements == maxColumns) { EndHorizontal(); rowOpen = false; rowElements = 0; }
            }
            if (rowOpen) { EndHorizontal(); }
        }
    }
}
