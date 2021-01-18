using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Nodes
{
    [CustomEditor(typeof(SimpleAnimationNode))]
    public class SimpleAnimationNodeEditor : AnimationNodeEditor
    {
        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Textures)", false, 400)]
        private static void CreateFromTextures()
        {
            var trailingNumbersRegex = new Regex(@"(\d+$)");

            var sprites = new List<Sprite>();
            var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            foreach (var texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                sprites.AddRange(AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>());
            }

            var cels = sprites
                .OrderBy(
                    sprite =>
                    {
                        var match = trailingNumbersRegex.Match(sprite.name);
                        return match.Success ? int.Parse(match.Groups[0].Captures[0].ToString()) : 0;
                    }
                )
                .Select(sprite => new SimpleCel(sprite))
                .ToArray();

            var asset = SimpleAnimationNode.Create<SimpleAnimationNode>(
                cels: cels
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