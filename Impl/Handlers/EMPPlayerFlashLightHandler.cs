using GameData;
using Gear;
using ExtraObjectiveSetup.Utils;
using Player;
using UnityEngine;

namespace EOSExt.EMP.Impl.Handlers
{
    public class EMPPlayerFlashLightHandler : EMPHandler
    {
        public static EMPPlayerFlashLightHandler Instance { get; private set; }

        static EMPPlayerFlashLightHandler()
        {

        }

        private PlayerInventoryBase _inventory;
        private float _originalIntensity;
        private bool _originalFlashlightState;

        protected override bool IsDeviceOnPlayer => true;

        private bool FlashlightEnabled => _inventory.FlashlightEnabled;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            if (Instance != null)
            {
                EOSLogger.Warning("EMP: re-setup EMPPlayerFlashLightHandler");
                Instance.OnDespawn();
            }

            base.Setup(gameObject, controller);

            _inventory = gameObject.GetComponent<PlayerAgent>().Inventory;
            if (_inventory == null)
            {
                EOSLogger.Warning("Player inventory was null!");
            }
            else
            {
                State = EMPState.On;
                Events.FlashLightWielded += InventoryEvents_ItemWielded;
            }
            Instance = this;
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            Events.FlashLightWielded -= Instance.InventoryEvents_ItemWielded;
            Instance = null;
        }

        private void InventoryEvents_ItemWielded(GearPartFlashlight flashlight) => _originalIntensity = GameDataBlockBase<FlashlightSettingsDataBlock>.GetBlock(flashlight.m_settingsID).intensity;

        protected override void DeviceOff()
        {
            _originalFlashlightState = FlashlightEnabled;
            if (!_originalFlashlightState)
                return;
            _inventory.Owner.Sync.WantsToSetFlashlightEnabled(false);
        }

        protected override void DeviceOn()
        {
            if (_originalFlashlightState != FlashlightEnabled)
                _inventory.Owner.Sync.WantsToSetFlashlightEnabled(_originalFlashlightState);
            _inventory.m_flashlight.intensity = _originalIntensity;
        }

        protected override void FlickerDevice()
        {
            if (!FlashlightEnabled)
                return;
            _inventory.m_flashlight.intensity = Random.GetRandom01() * _originalIntensity;
        }
    }

}
