using Parkitect.Mods.AssetPacks;
using System.Collections.Generic;
using UnityEngine;

class CustomColorDecorator : IDecorator
{
    public void Decorate(GameObject assetGO, Asset asset, AssetBundle assetBundle)
    {
        if (asset.HasCustomColors)
        {
            CustomColors customColors = assetGO.AddComponent<CustomColors>();
            List<Color> list = new List<Color>();
            for (int i = 0; i < asset.ColorCount; i++)
            {
                CustomColor customColor = asset.CustomColors[i];
                list.Add(new Color(customColor.Red, customColor.Green, customColor.Blue, customColor.Alpha));
            }
            customColors.setColors(list.ToArray());
        }
    }
}