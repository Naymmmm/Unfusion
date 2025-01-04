using LabFusion.Network.Proxy;
using LabFusion.Preferences.Client;
using LabFusion.Utilities;

namespace LabFusion.Network;

public static class NetworkLayerDeterminer
{
    public static NetworkLayer LoadedLayer { get; private set; }
    public static string LoadedTitle { get; private set; }

    public static NetworkLayer GetDefaultLayer()
    {
        if (PlatformHelper.IsAndroid)
        {
            return NetworkLayer.GetLayer<ProxySteamVRNetworkLayer>();
        }

        // Substitute SteamVRNetworkLayer for ProxySteamVRNetworkLayer so we can hopefully 127.0.0.1 that shit
        //return NetworkLayer.GetLayer<SteamVRNetworkLayer>();
        FusionLogger.Log("Substituting NetworkLayer...");
        return NetworkLayer.GetLayer<ProxySteamVRNetworkLayer>();
    }

    public static NetworkLayer VerifyLayer(NetworkLayer layer)
    {
        if (layer.CheckValidation())
        {
            return layer;
        }
        else if (layer.TryGetFallback(out var fallback))
        {
            return VerifyLayer(fallback);
        }
        else
        {
            return NetworkLayer.GetLayer<EmptyNetworkLayer>();
        }
    }

    public static void LoadLayer()
    {
        var title = ClientSettings.NetworkLayerTitle.Value;

        if (!NetworkLayer.LayerLookup.TryGetValue(title, out var layer))
        {
            layer = GetDefaultLayer();
        }

        layer = VerifyLayer(layer);

        LoadedLayer = layer;
        LoadedTitle = layer.Title;
    }
}