using System.Collections.Generic;
using Aarthificial.Reanimation.Cels;
using UnityEditor;

namespace Aarthificial.Reanimation.Editor
{
    [CustomPropertyDrawer(typeof(SimpleCel))]
    public class SimpleCelPropertyDrawer : CelPropertyDrawer
    {
        protected override IEnumerable<string> PropertiesToDraw => new[] {"sprite", "drivers"};
    }
}