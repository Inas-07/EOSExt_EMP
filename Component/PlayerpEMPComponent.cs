using EOSExt.EMP.Impl.Handlers;
using Player;
using UnityEngine;
using EOSExt.EMP.Impl.PersistentEMP;

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
            if (EMPManager.Current.Player == null) return;

            EMPManager.Current.SetupHUDAndFlashlight();
            EMPManager.Current.SetupToolHandler();
        }

        void Update()
        {
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel) return;

            if (!float.IsNaN(nextUpdateTime) && Clock.Time < nextUpdateTime) return;

            nextUpdateTime = Clock.Time + UPDATE_INTERVAL;

            CheckSetup();

            EMPManager.Current.RemoveInactiveEMPs();

            if (player == null)
            {
                return;
            }

            InAnypEMP = false;
            foreach (var EMP in EMPManager.Current.pEMPs)
            {
                if (EMP.State != ActiveState.ENABLED) continue;

                var itd = EMP.ItemToDisable;
                if (EMP.InRange(player.Position))
                {
                    if(itd.BioTracker) EMPBioTrackerHandler.Instance?.AddAffectedBy(EMP);
                    else EMPBioTrackerHandler.Instance?.RemoveAffectedBy(EMP);
                    
                    if (itd.PlayerFlashLight) EMPPlayerFlashLightHandler.Instance?.AddAffectedBy(EMP);
                    else EMPPlayerFlashLightHandler.Instance?.RemoveAffectedBy(EMP);
                    
                    if (itd.PlayerHUD) EMPPlayerHudHandler.Instance?.AddAffectedBy(EMP);
                    else EMPPlayerHudHandler.Instance?.RemoveAffectedBy(EMP);
                    
                    if (itd.Sentry) EMPSentryHandler.Instance?.AddAffectedBy(EMP);
                    else EMPSentryHandler.Instance?.RemoveAffectedBy(EMP);

                    if (itd.GunSight)
                    {
                        foreach (var h in EMPGunSightHandler.Instances)
                            h.AddAffectedBy(EMP);
                    }
                    else
                    {
                        foreach (var h in EMPGunSightHandler.Instances)
                            h.RemoveAffectedBy(EMP);
                    }

                    InAnypEMP = true;
                }
            }
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
