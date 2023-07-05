using Aarthificial.Reanimation.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.GraphView
{
    public static class RGVIOUtility
    {
        private static readonly string baseFolder = "Reanimator";
        private static readonly string basePath = "Assets";
        private static readonly string baseSwitchesFolder = "Switches";
        private static readonly string baseSimpleAnimationsFolder = "Animations";

        private static readonly string baseReanimationPath = Path.Combine(basePath, baseFolder);
        private static readonly string baseSimpleAnimationsPath = Path.Combine(baseReanimationPath, baseSimpleAnimationsFolder);
        private static readonly string baseSwitchesPath = Path.Combine(baseReanimationPath, baseSwitchesFolder);

        private static Dictionary<Type, string> pathByType = new Dictionary<Type, string>()
        {
            { typeof(SwitchNode), baseSwitchesPath},
            { typeof(SimpleAnimationNode), baseSimpleAnimationsPath}
        };

        static RGVIOUtility()
        {
            CreateFolder(basePath, baseFolder);
            CreateFolder(baseReanimationPath, baseSwitchesFolder);
            CreateFolder(baseReanimationPath, baseSimpleAnimationsFolder);
        }
        private static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(path, folderName))) return;
            AssetDatabase.CreateFolder(path, folderName);
        }
        public static void SaveNode<T>(ReanimatorNode node) where T : ReanimatorNode
        {
            string path = pathByType[typeof(T)];
            string nodeName = node.name;
            for (int i = 0; !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(Path.Combine(baseSwitchesPath, ToAssetName(nodeName)))); i++)
            {
                nodeName = node.name + "_" + i;
            }
            AssetDatabase.CreateAsset(node, Path.Combine(path, ToAssetName(nodeName)));
        }
        public static void SaveNode(ReanimatorNode node)
        {
            string path = pathByType[node.GetType()];
            string nodeName = node.name;
            for (int i = 0; !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(Path.Combine(baseSwitchesPath, ToAssetName(nodeName)))); i++)
            {
                nodeName = node.name + "_" + i;
            }
            AssetDatabase.CreateAsset(node, Path.Combine(path, ToAssetName(nodeName)));
        }
        public static bool RenameNode<T>(ReanimatorNode node, string newName) where T : ReanimatorNode
        {
            string path = AssetDatabase.GetAssetPath(node);
            if(path == null) path = Path.Combine(pathByType[typeof(T)], ToAssetName(node.name));
            
            string s = AssetDatabase.RenameAsset(path, ToAssetName(newName));
            if (s.Length > 0)
            {
                Debug.LogError(s);
                return false;
            }
            return true;
        }
        public static void DeleteNode<T>(string name) where T : ReanimatorNode
        {
            string path = pathByType[typeof(T)];
            AssetDatabase.DeleteAsset(Path.Combine(path, name));
        }
        private static string ToAssetName(string name)
        {
            return name + ".asset";
        }
    }
}