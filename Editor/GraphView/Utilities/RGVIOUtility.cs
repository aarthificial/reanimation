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

        private static Dictionary<Type, string> folderByType = new Dictionary<Type, string>()
        {
            { typeof(SwitchNode), baseSwitchesFolder},
            { typeof(SimpleAnimationNode), baseSimpleAnimationsFolder}
        };

        static RGVIOUtility()
        {
            CreateFolder(baseReanimationPath);
            CreateFolder(Path.Combine(baseReanimationPath, baseSwitchesFolder));
            CreateFolder(Path.Combine(baseReanimationPath, baseSimpleAnimationsFolder));
        }
        private static void CreateFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parentFolder = Path.GetRelativePath(Directory.GetParent(basePath).FullName, Directory.GetParent(path).FullName);
            if (new DirectoryInfo(path) != new DirectoryInfo(basePath))
            {
                CreateFolder(parentFolder);
            }
            string thisFolderName = Path.GetRelativePath(parentFolder, path);
            AssetDatabase.CreateFolder(parentFolder, thisFolderName);
        }
        private static string GetNodeFolder(ReanimatorNode rootNode, ReanimatorNode node)
        {
            string path = AssetDatabase.GetAssetPath(rootNode);
            path = Directory.GetParent(path).Parent.FullName.Contains("Assets") ? Path.GetRelativePath(Directory.GetParent(basePath).FullName, Directory.GetParent(path).Parent.FullName)
                : Path.GetRelativePath(Directory.GetParent(basePath).FullName, Directory.GetParent(path).FullName);
            path = Path.Combine(path, folderByType[node.GetType()]);
            return path;
        }
        public static string CombineFolderWithNodeFolder(string folder, ReanimatorNode node)
        {
            return Path.Combine(folder, folderByType[node.GetType()]);
        }
        public static string FolderInBase(string path)
        {
            return Path.Combine(baseReanimationPath, path);
        }
        public static void CreateNode(string rootFolderPath, ReanimatorNode node)
        {
            string path = rootFolderPath;
            CreateFolder(rootFolderPath);
            Debug.Log(rootFolderPath + " "+Path.GetRelativePath(baseReanimationPath, rootFolderPath));
            string nodeName = node.name;
            for (int i = 0; !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(Path.Combine(path, ToAssetName(nodeName)))); i++)
            {
                nodeName = node.name + "_" + i;
            }
            AssetDatabase.CreateAsset(node, Path.Combine(path, ToAssetName(nodeName)));
        }
        public static void CreateNode(ReanimatorNode rootNode, ReanimatorNode node)
        {
            string path = GetNodeFolder(rootNode, node);
            CreateNode(path, node);
        }
        public static bool RenameNode(ReanimatorNode rootNode, ReanimatorNode node, string newName)
        {
            string path = AssetDatabase.GetAssetPath(node);
            if(path == null) path = Path.Combine(GetNodeFolder(rootNode, node), ToAssetName(node.name));
            
            string s = AssetDatabase.RenameAsset(path, ToAssetName(newName));
            if (s.Length > 0)
            {
                Debug.LogError(s);
                return false;
            }
            return true;
        }
        public static void DeleteNode(ReanimatorNode rootNode, ReanimatorNode node)
        {
            string path = GetNodeFolder(rootNode, node);
            AssetDatabase.DeleteAsset(Path.Combine(path, ToAssetName(node.name)));
        }
        private static string ToAssetName(string name)
        {
            return name + ".asset";
        }
    }
}