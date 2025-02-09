﻿using LabFusion.Network;
using LabFusion.SDK.Achievements;
using LabFusion.Senders;
using LabFusion.Utilities;
using LabFusion.Player;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;

using UnityEngine;

namespace LabFusion.Data;

public struct DescentIntroEvent
{
    public int selectionNumber;
    public DescentIntroType type;

    public DescentIntroEvent(int selectionNumber, DescentIntroType type)
    {
        this.selectionNumber = selectionNumber;
        this.type = type;
    }
}

public struct DescentElevatorEvent
{
    public DescentElevatorType type;

    public DescentElevatorEvent(DescentElevatorType type)
    {
        this.type = type;
    }
}

public struct DescentNooseEvent
{
    public byte smallId;
    public DescentNooseType type;

    public DescentNooseEvent(byte smallId, DescentNooseType type)
    {
        this.smallId = smallId;
        this.type = type;
    }
}

public class DescentData : LevelDataHandler
{
    public override string LevelTitle => "01 - Descent";

    public static DescentData Instance { get; private set; }

    public static NooseBonelabIntro Noose;
    public static TutorialElevator Elevator;
    public static GameControl_Descent GameController;
    public static Control_UI_BodyMeasurements BodyMeasurementsUI;

    public static Grip KnifeGrip;

    private static readonly List<DescentIntroEvent> _introEvents = new();
    private static readonly List<DescentNooseEvent> _nooseEvents = new();
    private static readonly List<DescentElevatorEvent> _elevatorEvents = new();

    public static DescentIntroEvent CreateIntroEvent(int selectionNumber, DescentIntroType type)
    {
        var value = new DescentIntroEvent(selectionNumber, type);

        if (NetworkInfo.IsServer)
            _introEvents.Add(value);

        return value;
    }

    public static DescentNooseEvent CreateNooseEvent(byte smallId, DescentNooseType type)
    {
        var value = new DescentNooseEvent(smallId, type);

        if (NetworkInfo.IsServer)
            _nooseEvents.Add(value);

        return value;
    }

    public static DescentElevatorEvent CreateElevatorEvent(DescentElevatorType type)
    {
        var value = new DescentElevatorEvent(type);

        if (NetworkInfo.IsServer)
            _elevatorEvents.Add(value);

        return value;
    }

    public static void CheckAchievement()
    {
        if (KnifeGrip == null && !FindKnife())
            return;

        // Check if we were holding the knife and we weren't attached to the noose
        if (!Noose.rM.IsLocalPlayer())
        {
            foreach (var hand in KnifeGrip.attachedHands)
            {
                // Make sure this is our hand
                if (hand.manager.IsLocalPlayer())
                {
                    AchievementManager.TryGetAchievement<Betrayal>(out var achievement);
                    achievement?.IncrementTask();
                    break;
                }
            }
        }
    }

    protected override void MainSceneInitialized()
    {
        Instance = this;
        _introEvents.Clear();

        Noose = GameObject.FindObjectOfType<NooseBonelabIntro>(true);
        Elevator = GameObject.FindObjectOfType<TutorialElevator>(true);
        GameController = GameObject.FindObjectOfType<GameControl_Descent>(true);
        BodyMeasurementsUI = GameObject.FindObjectOfType<Control_UI_BodyMeasurements>(true);
        FindKnife();
    }

    private static bool FindKnife()
    {
        var knife = GameObject.Find("SEQUENCE_EFFECTS/Dagger_A");
        if (knife != null)
        {
            KnifeGrip = knife.GetComponentInChildren<Grip>(true);
            return true;
        }

        return false;
    }

    public void CacheValues() => MainSceneInitialized();

    protected override void PlayerCatchup(PlayerId playerId)
    {
        // Send all intro events
        foreach (var intro in _introEvents)
        {
            CampaignSender.SendDescentIntro(intro, playerId);
        }

        // Send all noose events
        foreach (var noose in _nooseEvents)
        {
            CampaignSender.SendDescentNoose(noose, playerId);
        }

        // Send all elevator events
        foreach (var elevator in _elevatorEvents)
        {
            CampaignSender.SendDescentElevator(elevator, playerId);
        }
    }
}