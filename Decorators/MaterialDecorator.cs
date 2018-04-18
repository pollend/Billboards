
// Parkitect.Mods.AssetPacks.MaterialDecorator
using Parkitect.Mods.AssetPacks;
using UnityEngine;

internal class MaterialDecorator : IDecorator
{
	private Material standard;

	private Material diffuse;

	private Material colorDiffuse;

	private Material specular;

	private Material colorSpecular;

	private Material colorDiffuseIllum;

	private Material colorSpecularIllum;

	public MaterialDecorator()
	{
		Material[] objectMaterials = ScriptableSingleton<AssetManager>.Instance.objectMaterials;
		foreach (Material material in objectMaterials)
		{
			switch (material.name)
			{
			case "Diffuse":
				this.diffuse = material;
				break;
			case "CustomColorsDiffuse":
				this.colorDiffuse = material;
				break;
			case "Specular":
				this.specular = material;
				break;
			case "CustomColorsSpecular":
				this.colorSpecular = material;
				break;
			case "CustomColorsIllum":
				this.colorDiffuseIllum = material;
				break;
			case "CustomColorsIllumSpecular":
				this.colorSpecularIllum = material;
				break;
			}
		}
	}

	public void Decorate(GameObject assetGO, Asset asset, AssetBundle assetBundle)
	{
		if (asset.Type == AssetType.Fence)
		{
			Fence component = assetGO.GetComponent<Fence>();
			this.replaceMaterials(component.flatGO, asset);
			if ((Object)component.postGO != (Object)null)
			{
				this.replaceMaterials(component.postGO, asset);
			}
		}
		else
		{
			this.replaceMaterials(assetGO, asset);
		}
	}

	private void replaceMaterials(GameObject go, Asset asset)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				Material material = sharedMaterials[j];
				if (!((Object)material == (Object)null))
				{
					if (material.name.StartsWith("Diffuse"))
					{
						sharedMaterials[j] = this.diffuse;
					}
					else if (material.name.StartsWith("CustomColorsDiffuseIllum"))
					{
						sharedMaterials[j] = this.colorDiffuseIllum;
					}
					else if (material.name.StartsWith("CustomColorsDiffuse"))
					{
						sharedMaterials[j] = this.colorDiffuse;
					}
					else if (material.name.StartsWith("Specular"))
					{
						sharedMaterials[j] = this.specular;
					}
					else if (material.name.StartsWith("CustomColorsSpecularIllum"))
					{
						sharedMaterials[j] = this.colorSpecularIllum;
					}
					else if (material.name.StartsWith("CustomColorsSpecular"))
					{
						sharedMaterials[j] = this.colorSpecular;
					}
					else if (!material.name.StartsWith("SignTextMaterial") && !material.name.StartsWith("tv_image"))
					{
						sharedMaterials[j] = material;
						sharedMaterials[j].shader = ScriptableSingleton<AssetManager>.Instance.standardShader;
					}
				}
			}
			renderer.sharedMaterials = sharedMaterials;
		}
	}
}