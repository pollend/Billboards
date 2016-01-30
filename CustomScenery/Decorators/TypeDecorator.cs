using System.Collections.Generic;
using Custom_Scenery.Decorators.Type;
using UnityEngine;

namespace Custom_Scenery.Decorators
{
    class TypeDecorator : IDecorator
    {
        private string _type;

        public TypeDecorator(string type)
        {
            _type = type;
        }

        public void Decorate(GameObject go, Dictionary<string, object> options, AssetBundle assetBundle)
        {
            switch (_type)
            {
                case "billboard":
                    (new BillboardDecorator()).Decorate(go, options, assetBundle);
                    break;
            }
        }

        public GameObject Decorate(Dictionary<string, object> options, AssetBundle bundle)
        {
            GameObject asset = null;

            switch (_type)
            {
                case "billboard":
                    asset = Object.Instantiate(bundle.LoadAsset((string) options["model"])) as GameObject;
                    break;
            }

            Decorate(asset, options, bundle);

            return asset;
        }
    }
}
