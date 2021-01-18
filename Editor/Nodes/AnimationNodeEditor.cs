using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Nodes
{
    public enum GlobalDriverApplyMode
    {
        Add,
        Update,
        Remove,
    }

    public class AnimationNodeEditor : ReanimatorNodeEditor
    {
        protected static float FPS = 10;
        protected static int CurrentFrame;
        protected static bool ShouldPlay = true;
        protected readonly List<Sprite> Sprites = new List<Sprite>();
        protected SerializedProperty Cels;

        private string _globalDriverName = "globalDriver";
        private int _globalDriverValue;

        protected virtual void OnEnable()
        {
            AddCustomProperty("controlDriver");
            AddCustomProperty("drivers");
            Cels = AddCustomProperty("cels");
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawCustomProperties();
            EditorGUILayout.Separator();
            DrawGlobalDriverEditor();
            UpdateSpritesCache();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawGlobalDriverEditor()
        {
            EditorGUILayout.LabelField("Global Driver Editor", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _globalDriverName = EditorGUILayout.TextField(_globalDriverName);
            _globalDriverValue = EditorGUILayout.IntField(_globalDriverValue);
            if (GUILayout.Button(
                new GUIContent("Add", "Adds the driver to every keyframe (Only if the driver does NOT already exist)")
            ))
                ApplyGlobalDriver(GlobalDriverApplyMode.Add);
            if (GUILayout.Button(
                new GUIContent("Update", "Updates the driver in every keyframe (Only if the driver DOES already exist)")
            ))
                ApplyGlobalDriver(GlobalDriverApplyMode.Update);
            if (GUILayout.Button(new GUIContent("Remove", "Removes the driver from every keyframe")))
                ApplyGlobalDriver(GlobalDriverApplyMode.Remove);
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void UpdateSpritesCache()
        {
            Sprites.Clear();
            for (var i = 0; i < Cels.arraySize; i++)
            {
                var frameProp = Cels.GetArrayElementAtIndex(i);
                var sprite = frameProp.FindPropertyRelative("sprite").objectReferenceValue as Sprite;
                if (sprite != null)
                    Sprites.Add(sprite);
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return FPS > 0 && ShouldPlay;
        }

        public override void OnPreviewGUI(Rect position, GUIStyle background)
        {
            DrawPlaybackControls();

            if (Sprites.Count == 0) return;
            int index = ShouldPlay
                ? (int) (EditorApplication.timeSinceStartup * FPS % Sprites.Count)
                : CurrentFrame;

            Helpers.DrawTexturePreview(position, Sprites[index]);
        }

        protected void DrawPlaybackControls()
        {
            EditorGUILayout.BeginHorizontal();
            ShouldPlay = EditorGUILayout.ToggleLeft("Play", ShouldPlay, GUILayout.MaxWidth(100));
            if (ShouldPlay)
                FPS = EditorGUILayout.FloatField("Frames per seconds", FPS);
            else
                CurrentFrame = EditorGUILayout.IntSlider(CurrentFrame, 0, Sprites.Count - 1);
            EditorGUILayout.EndHorizontal();
        }

        private void ApplyGlobalDriver(GlobalDriverApplyMode mode)
        {
            if (_globalDriverName == "") return;

            for (var i = 0; i < Cels.arraySize; i++)
            {
                var frameProp = Cels.GetArrayElementAtIndex(i);
                var driversProp = frameProp.FindPropertyRelative("drivers");
                var keysProp = driversProp.FindPropertyRelative("keys");
                var valuesProp = driversProp.FindPropertyRelative("values");

                int existingIndex = -1;
                for (var j = 0; j < keysProp.arraySize; j++)
                {
                    var keyProp = keysProp.GetArrayElementAtIndex(j);
                    if (keyProp.stringValue == _globalDriverName)
                    {
                        existingIndex = j;
                        break;
                    }
                }

                switch (mode)
                {
                    case GlobalDriverApplyMode.Add:
                        if (existingIndex < 0)
                        {
                            keysProp.InsertArrayElementAtIndex(0);
                            valuesProp.InsertArrayElementAtIndex(0);

                            var keyProp = keysProp.GetArrayElementAtIndex(0);
                            var valueProp = valuesProp.GetArrayElementAtIndex(0);

                            keyProp.stringValue = _globalDriverName;
                            valueProp.intValue = _globalDriverValue;
                        }

                        break;
                    case GlobalDriverApplyMode.Update:
                        if (existingIndex > -1)
                        {
                            var valueProp = valuesProp.GetArrayElementAtIndex(existingIndex);
                            valueProp.intValue = _globalDriverValue;
                        }

                        break;
                    case GlobalDriverApplyMode.Remove:
                        if (existingIndex > -1)
                        {
                            keysProp.DeleteArrayElementAtIndex(existingIndex);
                            valuesProp.DeleteArrayElementAtIndex(existingIndex);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }
        }
    }
}