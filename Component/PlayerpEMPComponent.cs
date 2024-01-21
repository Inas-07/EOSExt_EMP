using EOSExt.EMP.Definition;
using EOSExt.EMP.Impl.Handlers;
using Player;
using UnityEngine;
using EOSExt.EMP.Impl;
using System.Collections.Generic;

namespace EOSExt.EMP.EMPComponent
{
    public class PlayerpEMPComponent : MonoBehaviour
    {
        private float nextUpdateTime = float.NaN;

        public const float UPDATE_INTERVAL = 0.2f;

        public static PlayerpEMPComponent Current { get; internal set; } = null;

        public bool InAnypEMP { get; private set; } = false;

        public PlayerAgent player { get; internal set; }

        private void CheckSetup()
        {
            if (EMPManager.Current.LocalPlayerAgent == null) return;

            EMPManager.Current.SetupHUDAndFlashlight();
            EMPManager.Current.SetupToolHandler();
        }

        void Update()
        {
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel) return;

            if (!float.IsNaN(nextUpdateTime) && Clock.Time < nextUpdateTime) return;

            nextUpdateTime = Clock.Time + UPDATE_INTERVAL;

            CheckSetup();

            //var player = EMPManager.Current.LocalPlayerAgent;
            if (player == null)
            {
                return;
            }

            ItemToDisable itemToDisable = new()
            {
                BioTracker = false,
                PlayerFlashLight = false,
                PlayerHUD = false,
                EnvLight = false,
                GunSight = false,
                Sentry = false
            };

            InAnypEMP = false;
            foreach (var EMP in EMPManager.Current.ActiveEMPs)
            {
                if (EMP.State != ActiveState.ENABLED) continue;

                if (EMP.InRange(player.Position))
                {
                    itemToDisable.BioTracker = itemToDisable.BioTracker || EMP.ItemToDisable.BioTracker;
                    itemToDisable.PlayerFlashLight = itemToDisable.PlayerFlashLight || EMP.ItemToDisable.PlayerFlashLight;
                    itemToDisable.PlayerHUD = itemToDisable.PlayerHUD || EMP.ItemToDisable.PlayerHUD;
                    itemToDisable.GunSight = itemToDisable.GunSight || EMP.ItemToDisable.GunSight;
                    itemToDisable.Sentry = itemToDisable.Sentry || EMP.ItemToDisable.Sentry;
                    InAnypEMP = true;
                }
            }

            void Handle(IEnumerable<EMPHandler> handlers, bool enabled)
            {
                foreach (var handler in handlers)
                {
                    if(enabled)
                    {
                        handler?.controller?.AddTime(float.PositiveInfinity);
                    }
                    else
                    {
                        handler?.controller?.ClearTime();
                    }
                }
            }

            Handle(EMPBioTrackerHandler.Handlers, itemToDisable.BioTracker);
            Handle(EMPPlayerFlashLightHandler.Handlers, itemToDisable.PlayerFlashLight);
            Handle(EMPPlayerHudHandler.Handlers, itemToDisable.PlayerHUD);
            Handle(EMPGunSightHandler.Handlers, itemToDisable.GunSight);
            Handle(EMPSentryHandler.Handlers, itemToDisable.Sentry);
        }

        void OnDestroy()
        {
            EMPManager.Current.OnLocalPlayerAgentDestroy();
            player = null;
        }

        static PlayerpEMPComponent()
        {

        }
    }
}
