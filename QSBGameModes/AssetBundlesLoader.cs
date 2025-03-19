using OWML.Common;
using UnityEngine;

namespace QSBGameModes;

public static class AssetBundlesLoader
{
    public static Material SeekerMaterial;
    public static void LoadBundles(IModHelper modHelper)
    {
        AssetBundle seekerBundle = modHelper.Assets.LoadBundle("assetbundles/seeker_bundle");
        SeekerMaterial = seekerBundle.LoadAsset<Material>("SeekerMaterial.mat");
    }
}