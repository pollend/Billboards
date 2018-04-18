// Parkitect.Mods.AssetPacks.LightEffectsDecorator
using Parkitect.Mods.AssetPacks;
using UnityEngine;

internal class LightEffectsDecorator : IDecorator
{
    public void Decorate(GameObject assetGO, Asset asset, AssetBundle assetBundle)
    {
        if (asset.LightsTurnOnAtNight)
        {
            LightController lightController = assetGO.AddComponent<LightController>();
            if (asset.LightsUseCustomColors)
            {
                lightController.useCustomColors = asset.LightsUseCustomColors;
                lightController.customColorSlot = asset.LightsCustomColorSlot;
            }
        }
    }
}