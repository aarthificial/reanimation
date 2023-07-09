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
            var sprites = new List<Sprite>();
            var textures = GetFilteredSelection<Texture2D>();
            foreach (var texture in textures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                sprites.AddRange(AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>());
            }

            CreateFromSpritesList(sprites, textures);
        }

        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Textures)", true, 400)]
        private static bool CreateFromTexturesValidation()
        {
            
            return GetFilteredSelection<Texture2D>().Count > 0;
        }

        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Sprites)", false, 400)]
        private static void CreateFromSprites()
        {

            var sprites = GetFilteredSelection<Sprite>();
            var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

            CreateFromSpritesList(sprites, textures.ToList());
        }

        [MenuItem("Assets/Create/Reanimator/Simple Animation (From Sprites)", true, 400)]
        private static bool CreateFromSpritesValidation()
        {
            return GetFilteredSelection<Sprite>().Count > 0;
        }

        private static void CreateFromSpritesList(List<Sprite> sprites, List<Texture2D> textures)
        {
            var trailingNumbersRegex = new Regex(@"(\d+$)");

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

        //Selection.GetFiltered gets Texture2D parent of Sprite so instead we write our function to avoid this
        private static List<T> GetFilteredSelection<T>()
        {
            List<T> filtered = new List<T>();
            foreach(var obj in Selection.objects)
            {
                if (obj is T t)
                    filtered.Add(t);
            }
            return filtered;
        }
    }
}