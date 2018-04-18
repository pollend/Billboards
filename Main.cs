using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Billboards.Decorators;
using Parkitect.Mods.AssetPacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Custom_Scenery
{
    public class Main : IMod
    {
        private AssetPack assetPack;

        private List<SerializedMonoBehaviour> assetSMBs = new List<SerializedMonoBehaviour>();
        private GameObject hider;

        public void onEnabled()
        {
            String BannerPath = FilePaths.getFolderPath("Banners");

            
            if (!Directory.Exists(BannerPath))
            {
                char dsc = System.IO.Path.DirectorySeparatorChar;

                Directory.CreateDirectory(BannerPath);
                Directory.CreateDirectory(BannerPath + dsc + "long");
                Directory.CreateDirectory(BannerPath + dsc + "wide");
                Directory.CreateDirectory(BannerPath + dsc + "square");

                File.Copy(Path + dsc + "Banners" + dsc + "long" + dsc + "long.png", BannerPath + dsc + "long" + dsc + "long.png");
                File.Copy(Path + dsc + "Banners" + dsc + "wide" + dsc + "wide.png", BannerPath + dsc + "wide" + dsc + "wide.png");
                File.Copy(Path + dsc + "Banners" + dsc + "square" + dsc + "square.png", BannerPath + dsc + "square" + dsc + "square.png");
            }
            
            AssetPack assetPack = JsonUtility.FromJson<AssetPack>(File.ReadAllText(System.IO.Path.Combine(Path,"billboard.json")));
            AssetBundle bundle =  AssetBundle.LoadFromFile(System.IO.Path.Combine(Path, "assetPack"));

            if (bundle == null)
            {
                throw new Exception("Failed to load AssetBundle!");
            }
            hider = new GameObject("Hider");
            Object.DontDestroyOnLoad(hider);
            
            foreach (var asset in assetPack.Assets)
            {
                if (asset.Type == AssetType.Deco)
                {
                    try
                    {
                        GameObject gameObject =
                            Object.Instantiate(
                                bundle.LoadAsset<GameObject>($"Assets/Resources/AssetPack/{asset.Guid}.prefab"));


                        new BillboardDecorator().Decorate(gameObject, asset, bundle);
                        new MaterialDecorator().Decorate(gameObject, asset, bundle);
                        new CustomColorDecorator().Decorate(gameObject, asset, bundle);
                        new LightEffectsDecorator().Decorate(gameObject, asset, bundle);
                        Object.DontDestroyOnLoad(gameObject);
                        BuildableObject component = gameObject.GetComponent<BuildableObject>();
                        component.name = asset.Guid;
                        component.setDisplayName(asset.Name);
                        component.price = asset.Price;
                        component.dontSerialize = true;
                        component.isPreview = true;
                        component.isStatic = true;
                        Object.DontDestroyOnLoad(component.gameObject);
                        assetSMBs.Add(component);
                        component.transform.SetParent(hider.transform);
                        ScriptableSingleton<AssetManager>.Instance.registerObject(component);
                    }
                    catch (Exception message)
                    {
                        Debug.LogError(message);
                    }
                }
            }
            hider.SetActive(false);
            bundle.Unload(false);

        }

        public void onDisabled()
        {
            foreach (SerializedMonoBehaviour assetSMB in this.assetSMBs)
            {
                ScriptableSingleton<AssetManager>.Instance.unregisterObject(assetSMB);
            }
            if ((UnityEngine.Object)this.hider != (UnityEngine.Object)null)
            {
                UnityEngine.Object.Destroy(this.hider);
            }
        }

        public string Name => "Billboards";
        public string Description => "Billboard Pack";

        public string Path
        {
            get { return ModManager.Instance.getModEntries().First(x => x.mod == this).path; }
        }

        string IMod.Identifier => "Billboards";

    }
}
