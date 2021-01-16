using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Aarthificial.Reanimation.KeyFrames;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor
{
    internal enum GlobalDriverApplyMode
    {
        Add,
        Update,
        Remove,
    }

    [CustomEditor(typeof(SimpleAnimationNode))]
    public class SimpleAnimationNodeEditor : UnityEditor.Editor
    {
        private static float _fps = 10;
        private SerializedProperty _driver;
        private SerializedProperty _ai;
        private SerializedProperty _percentage;
        private SerializedProperty _drivers;
        private SerializedProperty _frames;
        private string _globalDriverName = "globalDriver";
        private int _globalDriverValue;

        private void OnEnable()
        {
            _driver = serializedObject.FindProperty("driver");
            _ai = serializedObject.FindProperty("autoIncrement");
            _percentage = serializedObject.FindProperty("percentageBased");
            _drivers = serializedObject.FindProperty("drivers");
            _frames = serializedObject.FindProperty("frames");
        }

        private readonly List<Sprite> _sprites = new List<Sprite>();

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_driver);
            EditorGUILayout.PropertyField(_drivers);
            EditorGUILayout.PropertyField(_frames);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            if (_ai.boolValue)
                _percentage.boolValue = false;
            EditorGUI.BeginDisabledGroup(_percentage.boolValue);
            EditorGUILayout.PropertyField(_ai);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(_ai.boolValue);
            EditorGUILayout.PropertyField(_percentage);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

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

            _sprites.Clear();
            for (var i = 0; i < _frames.arraySize; i++)
            {
                var frameProp = _frames.GetArrayElementAtIndex(i);
                var sprite = frameProp.FindPropertyRelative("sprite").objectReferenceValue as Sprite;
                if (sprite != null)
                    _sprites.Add(sprite);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override bool RequiresConstantRepaint()
        {
            return _fps > 0 && _ai.boolValue;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            _fps = EditorGUILayout.FloatField("Frames per seconds", _fps);

            if (_sprites.Count == 0) return;
            int index = _ai.boolValue
                ? (int) (EditorApplication.timeSinceStartup * _fps % _sprites.Count)
                : 0;

            Helpers.DrawTexturePreview(r, _sprites[index]);
        }

        private void ApplyGlobalDriver(GlobalDriverApplyMode mode)
        {
            if (_globalDriverName == "") return;

            for (var i = 0; i < _frames.arraySize; i++)
            {
                var frameProp = _frames.GetArrayElementAtIndex(i);
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

        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Textures)")]
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
                    .Select(sprite => new KeyFrame(sprite))
                    .ToArray()
            );
            string baseName = trailingNumbersRegex.Replace(textures[0].name, "");
            asset.name = baseName + "_animation";
            
            string assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(textures[0]));
            AssetDatabase.CreateAsset(asset, Path.Combine(assetPath ?? Application.dataPath, asset.name + ".asset"));
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Textures)", true)]
        private static bool CreateFromTexturesValidation()
        {
            return Selection.GetFiltered<Texture2D>(SelectionMode.Assets).Length > 0;
        }
    }
}