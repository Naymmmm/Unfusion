﻿#if MELONLOADER
using MelonLoader;

using LabFusion.SDK.Gamemodes;

using Il2CppInterop.Runtime.Attributes;

using Il2CppUltEvents;
#else
using UltEvents;
using UnityEngine;
#endif

namespace LabFusion.MarrowIntegration
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("BONELAB Fusion/Gamemodes/Gamemode End Event")]
    [RequireComponent(typeof(UltEventHolder))]
    [DisallowMultipleComponent]
#endif
    public sealed class GamemodeEndEvent : FusionMarrowBehaviour
    {
#if MELONLOADER
        public GamemodeEndEvent(IntPtr intPtr) : base(intPtr) { }

        private void Awake()
        {
            GamemodeManager.OnGamemodeChanged += OnGamemodeChanged;
        }

        private void OnDestroy()
        {
            GamemodeManager.OnGamemodeChanged -= OnGamemodeChanged;
        }

        [HideFromIl2Cpp]
        private void OnGamemodeChanged(Gamemode gamemode)
        {
            if (gamemode == null)
            {
                var holder = GetComponent<UltEventHolder>();

                if (holder != null)
                    holder.Invoke();
            }
        }
#else
        public override string Comment => "Executes the UltEventHolder attached to this GameObject when a gamemode ends.";
#endif
    }
}
