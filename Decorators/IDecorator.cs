using Parkitect.Mods.AssetPacks;
using UnityEngine;

internal interface IDecorator
{
    void Decorate(GameObject assetGO, Asset asset, AssetBundle assetBundle);
}
