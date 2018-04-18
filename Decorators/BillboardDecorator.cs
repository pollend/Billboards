using Custom_Scenery.CustomScenery;
using Parkitect.Mods.AssetPacks;
using UnityEngine;

namespace Billboards.Decorators
{
    public class BillboardDecorator: IDecorator
    {
        public void Decorate(GameObject go, Asset asset, AssetBundle assetBundle)
        {
            BillBoard deco = go.GetComponent<BillBoard>() ?? go.AddComponent<BillBoard>();
            deco.buildOnLayerMask = 4096;
            deco.buildOnGrid = asset.BuildOnGrid;
            deco.heightChangeDelta = asset.HeightDelta;
            deco.defaultGridSubdivision = asset.GridSubdivision;
            deco.defaultSnapToGridCenter = asset.SnapCenter;
            if (string.IsNullOrEmpty(asset.SubCategory))
            {
                deco.categoryTag = asset.Category;
            }
            else
            {
                deco.categoryTag = asset.Category + "/" + asset.SubCategory;
            }

            deco.canSeeThrough = asset.SeeThrough;
            deco.canBlockRain = asset.BlocksRain;
            if (asset.IsResizable)
            {
                CustomSize customSize = go.AddComponent<CustomSize>();
                customSize.minSize = asset.MinSize;
                customSize.maxSize = asset.MaxSize;
            }
        }
    }

}