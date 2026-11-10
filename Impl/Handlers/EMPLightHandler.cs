using ExtraObjectiveSetup.Utils;
using GTFO.API;
using AmorLib.API;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace EOSExt.EMP.Impl.Handlers
{
    public class EMPLightHandler : EMPHandler
    {
        private static Dictionary<IntPtr, EMPLightHandler> handlers = new();

        public static IEnumerable<EMPLightHandler> Instances => handlers.Values;

        private readonly LightWorker _worker;
        private ILightModifier _mod;

        static EMPLightHandler()
        {
            LevelAPI.OnBuildStart += Clear;
            LevelAPI.OnLevelCleanup += Clear;
        }

        private static void Clear()
        {
            foreach (var handler in handlers.Values)
            {
                handler._mod?.Remove();
                handler._mod = null;
            }
            handlers.Clear();
        }

        public EMPLightHandler(LightWorker worker)
        {
            _worker = worker;
        }

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            base.Setup(gameObject, controller);
            if (_worker?.Light == null)
            {
                EOSLogger.Warning("EMPLightHandler: no light worker");
                return;
            }

            State = EMPState.On;
            handlers[_worker.Light.Pointer] = this;
        }

        protected override void OnTick(bool isEMPD)
        {
            base.OnTick(isEMPD);
            if (State == EMPState.Off)
            {
                DeviceOff();
            }
            else if (State == EMPState.On)
            {
                DeviceOn();
            }
        }

        protected override void FlickerDevice()
        {
            if (_worker?.Light == null) return;
            _mod ??= _worker.AddModifier(_worker.CurrentColor, _worker.CurrentIntensity, _worker.CurrentEnabled, LightPriority.EMP);
            _mod.Intensity = Random.GetRandom01() * _worker.OrigIntensity;
        }

        protected override void DeviceOn()
        {
            if (_mod == null) return;
            _mod.Remove();
            _mod = null;
        }

        protected override void DeviceOff()
        {
            if (_worker?.Light == null) return;
            _mod ??= _worker.AddModifier(_worker.CurrentColor, _worker.CurrentIntensity, _worker.CurrentEnabled, LightPriority.EMP);
            _mod.Color = Color.black;
            _mod.Intensity = 0f;
        }
    }
}
