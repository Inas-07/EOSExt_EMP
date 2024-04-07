using ExtraObjectiveSetup.Utils;
using Player;
using System.Collections.Generic;
using UnityEngine;
using CellMenu;
namespace EOSExt.EMP.Impl.Handlers
{
    public class EMPPlayerHudHandler : EMPHandler
    {
        public static EMPPlayerHudHandler Instance { get; private set; }

        static EMPPlayerHudHandler()
        {

        }

        private readonly List<RectTransformComp> _targets = new List<RectTransformComp>();

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            if(Instance != null) 
            {
                EOSLogger.Warning("EMP: re-setup EMPPlayerHudHandler");
                Instance.OnDespawn();
            }

            _targets.Clear();
            base.Setup(gameObject, controller);

            _targets.Add(GuiManager.PlayerLayer.m_compass);
            _targets.Add(GuiManager.PlayerLayer.m_wardenObjective);
            _targets.Add(GuiManager.PlayerLayer.Inventory);
            _targets.Add(GuiManager.PlayerLayer.m_playerStatus);
            Instance = this;
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            _targets.Clear();
            Instance = null;
        }

        private void ForAllComponents(bool enabled)
        {
            foreach (Component t in _targets)
            {
                t.gameObject.SetActive(enabled);
            }

            foreach (var p in PlayerManager.PlayerAgentsInLevel)
            {
                if (p.IsLocallyOwned) continue;

                p.NavMarker.SetMarkerVisible(enabled);
            }

            CellSettingsApply.ApplyPlayerGhostOpacity(enabled ?
                CellSettingsManager.SettingsData.HUD.Player_GhostOpacity.Value : 0.0f);
        }

        protected override void DeviceOff()
        {
            ForAllComponents(false);
        }

        protected override void DeviceOn()
        {
            ForAllComponents(true);
        }

        protected override void FlickerDevice()
        {
            ForAllComponents(Random.FlickerUtil());
        }
    }
}
