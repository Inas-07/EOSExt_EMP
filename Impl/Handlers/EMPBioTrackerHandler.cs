using AK;
using Gear;
using ExtraObjectiveSetup.Utils;
using UnityEngine;

namespace EOSExt.EMP.Impl.Handlers
{
    public class EMPBioTrackerHandler : EMPHandler
    {
        public static EMPBioTrackerHandler Instance { get; private set; }

        static EMPBioTrackerHandler()
        {

        }

        private EnemyScanner _scanner;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            if(Instance != null)
            {
                EOSLogger.Warning("EMP: re-setup EMPPlayerFlashLightHandler");
                Instance.OnDespawn();
            }

            base.Setup(gameObject, controller);
            _scanner = gameObject.GetComponent<EnemyScanner>();
            if (_scanner == null)
            {
                EOSLogger.Error("Couldn't get bio-tracker component!");
            }

            Instance = this;
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            Instance = null;
        }

        protected override void DeviceOff()
        {
            _scanner.Sound.Post(EVENTS.BIOTRACKER_TOOL_LOOP_STOP);
            _scanner.m_graphics.m_display.enabled = false;
        }

        protected override void DeviceOn() => _scanner.m_graphics.m_display.enabled = true;

        protected override void FlickerDevice() => _scanner.enabled = Random.FlickerUtil();
    }
}
