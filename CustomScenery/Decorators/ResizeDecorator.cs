using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Custom_Scenery.Decorators;
using UnityEngine;

namespace Custom_Scenery.CustomScenery.Decorators
{
    class ResizeDecorator : IDecorator
    {
        public void Decorate(GameObject go, Dictionary<string, object> options, AssetBundle assetBundle)
        {
            CustomSize cs = go.AddComponent<CustomSize>();

            cs.minSize = 1;
            cs.maxSize = 4;
        }
    }
}
