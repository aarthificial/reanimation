using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor
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
        protected static int CurrentFrame = 0;
        protected static bool ShouldPlay = true;
        protected readonly List<Sprite> Sprites = new List<Sprite>();
        protected SerializedProperty Frames;

        private string _globalDriverName = "globalDriver";
        private int _globalDriverValue;

        protected void OnEnable()
        {
            AddCustomProperty("controlDriver");
            AddCustomProperty("drivers");
            Frames = AddCustomProperty("frames");
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
                ApplyGlobalDriver(GlobalDriverApplyMode.Add, _globalDriverName, _globalDriverValue);
            if (GUILayout.Button(
                new GUIContent("Update", "Updates the driver in every keyframe (Only if the driver DOES already exist)")
            ))
                ApplyGlobalDriver(GlobalDriverApplyMode.Update, _globalDriverName, _globalDriverValue);
            if (GUILayout.Button(new GUIContent("Remove", "Removes the driver from every keyframe")))
                ApplyGlobalDriver(GlobalDriverApplyMode.Remove, _globalDriverName, _globalDriverValue);
            EditorGUILayout.EndHorizontal();
        }
        
        protected virtual void UpdateSpritesCache()
        {
            Sprites.Clear();
            for (var i = 0; i < Frames.arraySize; i++)
            {
                var frameProp = Frames.GetArrayElementAtIndex(i);
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

        protected virtual void DrawPlaybackControls()
        {
            EditorGUILayout.BeginHorizontal();
            ShouldPlay = EditorGUILayout.ToggleLeft("Play", ShouldPlay, GUILayout.MaxWidth(100));
            if (ShouldPlay)
                FPS = EditorGUILayout.FloatField("Frames per seconds", FPS);
            else
                CurrentFrame = EditorGUILayout.IntSlider(CurrentFrame, 0, Sprites.Count - 1);
            EditorGUILayout.EndHorizontal();
        }

        protected void ApplyGlobalDriver(GlobalDriverApplyMode mode, string driverName, int driverValue)
        {
            if (driverName == "") return;

            for (var i = 0; i < Frames.arraySize; i++)
            {
                var frameProp = Frames.GetArrayElementAtIndex(i);
                var driversProp = frameProp.FindPropertyRelative("drivers");
                var keysProp = driversProp.FindPropertyRelative("keys");
                var valuesProp = driversProp.FindPropertyRelative("values");

                int existingIndex = -1;
                for (var j = 0; j < keysProp.arraySize; j++)
                {
                    var keyProp = keysProp.GetArrayElementAtIndex(j);
                    if (keyProp.stringValue == driverName)
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

                            keyProp.stringValue = driverName;
                            valueProp.intValue = driverValue;
                        }

                        break;
                    case GlobalDriverApplyMode.Update:
                        if (existingIndex > -1)
                        {
                            var valueProp = valuesProp.GetArrayElementAtIndex(existingIndex);
                            valueProp.intValue = driverValue;
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

        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Textures)", false, 400)]
        private static void CreateFromTextures()
        {
            var trailingNumbersRegex = new Regex(@"(\d+$)");

            var frames = new List<Sprite>();
            var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            foreach (var texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                frames.AddRange(AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>());
            }

            var asset = SimpleAnimationNode.Create<SimpleAnimationNode>(
                frames: frames
                    .OrderBy(
                        sprite =>
                        {
                            var match = trailingNumbersRegex.Match(sprite.name);
                            return match.Success ? int.Parse(match.Groups[0].Captures[0].ToString()) : 0;
                        }
                    )
                    .Select(sprite => new SimpleCel(sprite))
                    .ToArray()
            );
            string baseName = trailingNumbersRegex.Replace(textures[0].name, "");
            asset.name = baseName + "_animation";

            string assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(textures[0]));
            AssetDatabase.CreateAsset(asset, Path.Combine(assetPath ?? Application.dataPath, asset.name + ".asset"));
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Textures)", true, 400)]
        private static bool CreateFromTexturesValidation()
        {
            return Selection.GetFiltered<Texture2D>(SelectionMode.Assets).Length > 0;
        }
    }
}