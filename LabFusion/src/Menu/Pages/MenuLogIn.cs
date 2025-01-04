﻿using LabFusion.Marrow.Proxies;
using LabFusion.Network;
using LabFusion.Preferences.Client;

using UnityEngine;

namespace LabFusion.Menu;

public static class MenuLogIn
{
    private static int _lastLayerIndex = 0;

    public static void PopulateLogIn(GameObject logInGameObject)
    {
        // Layer Panel
        var layerPanel = logInGameObject.transform.Find("panel_Layer");

        var layoutOptions = layerPanel.Find("layout_Options");

        var targetLayerLabel = layoutOptions.Find("label_TargetLayer").GetComponent<LabelElement>()
            .WithTitle($"Target Layer: {ClientSettings.NetworkLayerTitle.Value}");

        var cycleLayerElement = layoutOptions.Find("button_CycleLayer").GetComponent<FunctionElement>()
            .WithTitle("Cycle")
            .Do(() =>
            {
                int count = NetworkLayer.SupportedLayers.Count;

                if (count <= 0)
                {
                    return;
                }

                _lastLayerIndex++;

                if (count <= _lastLayerIndex)
                {
                    _lastLayerIndex = 0;
                }

                ClientSettings.NetworkLayerTitle.Value = NetworkLayer.SupportedLayers[_lastLayerIndex].Title;
            });

        ClientSettings.NetworkLayerTitle.OnValueChanged += (v) =>
        {
            targetLayerLabel.Title = $"Target Layer: {v}";
        };

        var logInElement = layoutOptions.Find("button_LogIn").GetComponent<FunctionElement>()
            .WithTitle("Log In (illegally)")
            .Do(() =>
            {
                var layer = NetworkLayerManager.GetTargetLayer();

                if (layer != null)
                {
                    NetworkLayerManager.LogIn(layer);
                }
            });

        // Connecting Panel
        var connectingPanel = logInGameObject.transform.Find("panel_Connecting");

        connectingPanel.Find("label_Connecting").GetComponent<LabelElement>()
            .WithTitle("Connecting");

        // Failed Panel
        var failedPanel = logInGameObject.transform.Find("panel_Failed");

        failedPanel.Find("label_Failed").GetComponent<LabelElement>()
            .WithTitle("Connection Failed");

        // Success Panel
        var successPanel = logInGameObject.transform.Find("panel_Success");

        successPanel.Find("label_Success").GetComponent<LabelElement>()
            .WithTitle("Connection Succeeded");
    }
}
