using UnityEditor;
using UnityEngine;
using UnityEditor.EditorTools;

namespace Smart
{
    public partial class LevelEditor
    {

        void EditUpdate()
        {
            // Prepare to pick
            pick.Set(height, e, view.camera);

            switch (mode)
            {
                case Mode.Create:
                    EditModeCreate();
                    break;
                case Mode.Delete:
                    EditModeDelete();
                    break;
                case Mode.Move:
                    EditModeMove();
                    break;
                case Mode.Rotate:
                    EditModeRotate();
                    break;
                case Mode.Scale:
                    EditModeScale();
                    break;
            }

            // Label
            SceneLabel(pickPosition + labelMouseOffset, "{0} : {1}", mode, paint);
            // Repain
            view.Repaint();
        }

        //
        // Edit Create
        //

        void EditModeCreate()
        {
            Tools.current = Tool.None;
            // Don't allow selection
            PassiveControls();
            // Pick update
            pick.Update();
            // Get position
            pickPosition = pick.Position;
            pickNormal = pick.HitNormal;

            switch(paint)
            {
                case Paint.Brush:
                    EditModeCreateBrush();
                    break;
                case Paint.Single:
                    EditModeCreateSingle();
                    break;
            }

            RotatePreviews();
        }

        void EditModeCreateSingle()
        {
            // Snap if we didn't hit other objects
            pickPosition = pick.Hit ? Snap(pick.Point, move) : Snap(pickPosition, move);
#if MESH_DRAW
            // Prepare mesh draw
            meshDraw.Set(GetFirst() as GameObject);
            // Draw mesh
            meshDraw.Draw(GetPoints());
#endif
            UpdatePreviews(GetPoints(), pickPosition, pickNormal);
            // Wire cube
            DrawBrush(pickPosition,  pickNormal, Color.green);
            // Left click event
            LeftClick(Create);
        }

        void EditModeCreateBrush()
        {
            // Snap when we are brush painting
            pickPosition = Snap(pickPosition, move);
#if MESH_DRAW
            // Prepare mesh draw
            meshDraw.Set(GetFirst() as GameObject);
            // Draw mesh
            meshDraw.Draw(GetPoints());
#endif
            UpdatePreviews(GetPoints(), pickPosition, pickNormal);
            // Wire cube
            DrawBrush(pickPosition, pickNormal, Color.green);
            // Left click event
            LeftClick(Create);
        }

        //
        // Delete
        //

        void EditModeDelete()
        {
            Tools.current = Tool.None;
            // Don't allow selection
            PassiveControls();
            // Raycast
            pick.Update();
            // Get position
            pickPosition = Snap(pick.Position, move);

            switch (paint)
            {
                case Paint.Brush:
                    EditModeDeleteBrush();
                    break;
                case Paint.Single:
                    EditModeDeleteSingle();
                    break;
            }
        }

        void EditModeDeleteSingle()
        {
            if (e.type == EventType.Used)
            {
                // Get GameObject
                pick.GetGameObject(layer);
            }

            if (pick.Target)
            {
                // hit target
                pickPosition = pick.TargetPosition;
            }

            // Wire cube
            DrawBrush(pickPosition, pickNormal, Color.red);

            LeftClick(Delete);
        }

        void EditModeDeleteBrush()
        {
            // Wire cube
            DrawBrush(pickPosition, pickNormal, Color.red);

            LeftClick(Delete);
        }

        //
        // Position
        //

        void EditModeMove()
        {
            Tools.current = Tool.Move;

            //transforms = Selection.transforms;

            RotateTransforms();

            for (int i = 0; i < transforms.Length; i++)
            {
                if (null == transforms[i]) { continue; }

                if (transforms[i].hasChanged)
                {
                    Undo.RecordObject(transforms[i], "sle.move");

                    transforms[i].localPosition = Snap(transforms[i].localPosition, move);

                    transforms[i].hasChanged = false;
                }

                SceneLabel(transforms[i].position + labelObjectOffset, transforms[i].localPosition.ToString());
            }

            pick.UpdateRay();
            // Update position
            pick.UpdatePosition();
            // Get position
            pickPosition = pick.Position;
            
        }

        //
        // Rotation
        //

        void EditModeRotate()
        {
            Tools.current = Tool.Rotate;
            //transforms = Selection.transforms;

            RotateTransforms();

            for (int i = 0; i < transforms.Length; i++)
            {
                if (null == transforms[i]) { continue; }

                Tools.handleRotation = transforms[i].localRotation;

                if (transforms[i].hasChanged)
                {
                    //Undo.RecordObject(transforms[i], "sle.rotate");
                    
                    transforms[i].localEulerAngles = Snap(Tools.handleRotation.eulerAngles, rotate);

                    transforms[i].hasChanged = false;
                }

                SceneLabel(transforms[i].position + labelObjectOffset, transforms[i].eulerAngles.ToString());
            }

            pick.UpdateRay();
            // Update position
            pick.UpdatePosition();
            // Get position
            pickPosition = pick.Position;
        }

        //
        // Scale
        //

        void EditModeScale()
        {
            Tools.current = Tool.Scale;

            //transforms = Selection.transforms;

            RotateTransforms();

            for (int i = 0; i < transforms.Length; i++)
            {
                if (null == transforms[i]) { continue; }

                if (transforms[i].hasChanged)
                {
                    Undo.RecordObject(transforms[i], "sle.scale");

                    transforms[i].localScale = Snap(transforms[i].localScale, scale);

                    transforms[i].hasChanged = false;
                }

                SceneLabel(transforms[i].position + labelObjectOffset, transforms[i].localScale.ToString());
            }

            pick.UpdateRay();
            // Update position
            pick.UpdatePosition();
            // Get position
            pickPosition = pick.Position;
        }

        //
        //
        //

        void RotatePreviews()
        {
            if (e.control) { return; }

            if (KeyDown(KeyCode.X))
            {
                Transform t;

                Vector3 value = Vector3.zero;
                for (int i = 0; i < previews.Count; i++)
                {
                    t = previews[i].transform;

                    value.x = rotationAxis.x * rotate.x;
                    value.y = rotationAxis.y * rotate.y;
                    value.z = rotationAxis.z * rotate.z;

                    t.Rotate(value);

                    angle.x = ClampAngle(angle.x, value.x);
                    angle.y = ClampAngle(angle.y, value.y);
                    angle.z = ClampAngle(angle.z, value.z);
                }
            }
        }

        void RotateTransforms()
        {
            if (e.control) { return; }

            if (KeyDown(KeyCode.X))
            {
                Transform t;

                Vector3 value = Vector3.zero;
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (null == transforms[i]) { continue; }

                    t = transforms[i];

                    value.x = rotationAxis.x * rotate.x;
                    value.y = rotationAxis.y * rotate.y;
                    value.z = rotationAxis.z * rotate.z;

                    t.Rotate(value);

                    angle.x = ClampAngle(angle.x, value.x);
                    angle.y = ClampAngle(angle.y, value.y);
                    angle.z = ClampAngle(angle.z, value.z);
                }
            }
        }

        float ClampAngle(float angle, float value)
        {
            angle += value;

            if (angle < 0) { angle = 360; }

            if (angle >= 360) { angle = 0; }

            return angle;
        }
    }
}