using EOSExt.EMP.Impl.Handlers;
using Player;
using UnityEngine;
using EOSExt.EMP.Impl.PersistentEMP;
using SNetwork;


namespace EOSExt.EMP.EMPComponent
{
    public class PlayerpEMPComponent : MonoBehaviour
    {
        private float nextUpdateTime = float.NaN;

        public const float UPDATE_INTERVAL = 0.2f;

        public static PlayerpEMPComponent Current => SNet.HasLocalPlayer && SNet.LocalPlayer.HasPlayerAgent ? SNet.LocalPlayer.PlayerAgent.Cast<PlayerAgent>().GetComponent<PlayerpEMPComponent>() : null; //{ get; internal set; } = null;

        public PlayerAgent player => gameObject.GetComponent<PlayerAgent>();

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

            foreach (var EMP in EMPManager.Current.pEMPs)
            {
                var itd = EMP.ItemToDisable;

                if (EMP.State == ActiveState.ENABLED && EMP.InRange(player.Position))
                {
                    if (itd.BioTracker) EMPBioTrackerHandler.Instance?.AddAffectedBy(EMP);
                    if (itd.PlayerFlashLight) EMPPlayerFlashLightHandler.Instance?.AddAffectedBy(EMP);
                    if (itd.PlayerHUD) EMPPlayerHudHandler.Instance?.AddAffectedBy(EMP);
                    if (itd.Sentry) EMPSentryHandler.Instance?.AddAffectedBy(EMP);
                    if (itd.GunSight)
                    {
                        foreach (var h in EMPGunSightHandler.Instances)
                            h.AddAffectedBy(EMP);
                    }
                }
                else
                {
                    EMPBioTrackerHandler.Instance?.RemoveAffectedBy(EMP);
                    EMPPlayerFlashLightHandler.Instance?.RemoveAffectedBy(EMP);
                    EMPPlayerHudHandler.Instance?.RemoveAffectedBy(EMP);
                    EMPSentryHandler.Instance?.RemoveAffectedBy(EMP);
                    foreach (var h in EMPGunSightHandler.Instances)
                        h.RemoveAffectedBy(EMP);
                }
            }
        }

        void OnDestroy()
        {

        }

        static PlayerpEMPComponent()
        {

        }
    }
}
