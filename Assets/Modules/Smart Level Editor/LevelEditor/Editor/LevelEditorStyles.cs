using UnityEngine;

namespace Smart
{
    public partial class LevelEditor
    {
        public static class Styles
        {
            public static RectOffset zero
            {
                get => new RectOffset(0, 0, 0, 0);
            }

            public static GUIStyle radio
            {
                get
                {
                    GUIStyle style = new GUIStyle("radio");
                    style.overflow = zero;

                    return style;
                }
            }

            // Scene label style name
            public static GUIStyle sceneLabel
            {
                get
                {
                    GUIStyle style = new GUIStyle("profilerbadge");
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 12;
                    style.fixedHeight = 12;
                    style.stretchHeight = true;
                    style.margin = new RectOffset(5, 5, 5, 5);

                    return style;
                }
            }

            public static GUIStyle boldlabel
            {
                get
                {
                    GUIStyle style = new GUIStyle("boldlabel");
                    style.overflow = new RectOffset(0, 0, 0, 0);
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.padding = new RectOffset(0, 0, 0, 0);
                    style.alignment = TextAnchor.MiddleLeft;

                    return style;
                }
            }

            public static GUIStyle textField
            {
                get
                {
                    GUIStyle style = new GUIStyle("textField");
                    style.overflow = new RectOffset(0, 0, 0, 0);
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.stretchHeight = true;
                    style.alignment = TextAnchor.MiddleLeft;

                    return style;
                }
            }

            public static GUIStyle progressBar
            {
                get
                {
                    GUIStyle style = new GUIStyle("ProgressBarBar");
                    style.margin = new RectOffset(0, 0, 0, 0);
                    style.alignment = TextAnchor.MiddleLeft;
                    style.normal.textColor = Color.white;

                    return style;
                }
            }

            public static GUIStyle CenteredHelpbox
            {
                get
                {
                    GUIStyle style = new GUIStyle("Helpbox");
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 12;
                    return style;
                }
            }
        }
    }
}
