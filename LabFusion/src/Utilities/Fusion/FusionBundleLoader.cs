﻿using LabFusion.Data;

using UnityEngine;

namespace LabFusion.Utilities;

public static class FusionBundleLoader
{
    public static T LoadPersistentAsset<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
    {
        var asset = bundle.LoadAsset(name);

        if (asset != null)
        {
            asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return asset.TryCast<T>();
        }

        return null;
    }


    public static void LoadPersistentAssetAsync<T>(this AssetBundle bundle, string name, Action<T> onLoaded) where T : UnityEngine.Object
    {
        var request = bundle.LoadAssetAsync<T>(name);

        request.add_completed((Il2CppSystem.Action<AsyncOperation>)((a) =>
        {
            // Make sure the asset exists
            if (request.asset == null)
                return;
            var result = request.asset.TryCast<T>();
            result.hideFlags = HideFlags.DontUnloadUnusedAsset;
            onLoaded(result);
        }));
    }

    public static AssetBundle LoadAssetBundle(string name)
    {
        // Android
        if (PlatformHelper.IsAndroid)
        {
            return EmbeddedAssetBundle.LoadFromAssembly(FusionMod.FusionAssembly, ResourcePaths.AndroidBundlePrefix + name);
        }
        // Windows
        else
        {
            return EmbeddedAssetBundle.LoadFromAssembly(FusionMod.FusionAssembly, ResourcePaths.WindowsBundlePrefix + name);
        }
    }

    public static AssetBundleCreateRequest LoadAssetBundleAsync(string name)
    {
        // Android
        if (PlatformHelper.IsAndroid)
        {
            return EmbeddedAssetBundle.LoadFromAssemblyAsync(FusionMod.FusionAssembly, ResourcePaths.AndroidBundlePrefix + name);
        }
        // Windows
        else
        {
            return EmbeddedAssetBundle.LoadFromAssemblyAsync(FusionMod.FusionAssembly, ResourcePaths.WindowsBundlePrefix + name);
        }
    }

    public static void OnBundleLoad()
    {
        FusionContentLoader.OnBundleLoad();
        FusionPointItemLoader.OnBundleLoad();
        FusionAchievementLoader.OnBundleLoad();
    }

    /*
    public static void OnBundleUnloaded()
    {
        FusionContentLoader.OnBundleUnloaded();
        FusionPointItemLoader.OnBundleUnloaded();
        FusionAchievementLoader.OnBundleUnloaded();
    } */

    public static void OnBundleUnloaded()
    {
        try
        {
            FusionLogger.Log("Calling FusionContentLoader.OnBundleUnloaded");
            FusionContentLoader.OnBundleUnloaded();
            FusionLogger.Log("Finished FusionContentLoader.OnBundleUnloaded");

            FusionLogger.Log("Calling FusionPointItemLoader.OnBundleUnloaded");
            FusionPointItemLoader.OnBundleUnloaded();
            FusionLogger.Log("Finished FusionPointItemLoader.OnBundleUnloaded");

            FusionLogger.Log("Calling FusionAchievementLoader.OnBundleUnloaded");
            FusionAchievementLoader.OnBundleUnloaded();
            FusionLogger.Log("Finished FusionAchievementLoader.OnBundleUnloaded");
        }
        catch (Exception ex)
        {
            Task faultedTask = Task.FromException(ex);
            FusionLogger.LogException("unloading bundles in OnBundleUnloaded", ex);
        }
    }

}