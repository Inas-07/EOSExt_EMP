using ExtraObjectiveSetup.Utils;
using GTFO.API;
using LevelGeneration;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace EOSExt.EMP.Impl.Handlers
{
    public class EMPLightHandler : EMPHandler
    {
        private static Dictionary<IntPtr, EMPLightHandler> handlers = new();

        public static IEnumerable<EMPLightHandler> Instances => handlers.Values;

        internal static EMPLightHandler GetHandler(LG_Light light) 
        {
            return handlers.TryGetValue(light.Pointer, out var handler) ? handler : null;
        }


        private static void Clear()
        {
            handlers.Clear();
        }

        static EMPLightHandler()
        {
            LevelAPI.OnBuildStart += Clear;
            LevelAPI.OnLevelCleanup += Clear;
        }

        private LG_Light _light;
        private float _originalIntensity;
        private Color _originalColor;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            base.Setup(gameObject, controller);

            _light = gameObject.GetComponent<LG_Light>();
            if (_light == null)
            {
                EOSLogger.Warning("No Light!");
            }
            else
            {
                // FIXME: this impl. turns light color into white, if it's not flickering
                _originalIntensity = _light.GetIntensity();
                _originalColor = new Color(_light.m_color.r, _light.m_color.g, _light.m_color.b, _light.m_color.a);
                State = EMPState.On;
            }
            handlers[_light.Pointer] = this;
        }

        public void SetOriginalColor(Color color) => _originalColor = new(color.r, color.g, color.b, color.a);

        public void SetOriginalIntensity(float intensity) => _originalIntensity = intensity;

        protected override void OnTick(bool isEMPD)
        {
            //base.OnTick(isEMPD);
            //if(isEMPD)
        }

        protected override void FlickerDevice()
        {
            if (_light == null)
                return;
            _light.ChangeIntensity(Random.GetRandom01() * _originalIntensity);
        }

        protected override void DeviceOn()
        {
            if (_light == null)
                return;
            _light.ChangeIntensity(_originalIntensity);
            _light.ChangeColor(_originalColor);
        }

        protected override void DeviceOff()
        {
            if (_light == null)
                return;
            _light.ChangeIntensity(0.0f);
            _light.ChangeColor(Color.black);
        }
    }
}
