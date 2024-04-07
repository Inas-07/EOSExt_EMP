using ExtraObjectiveSetup.Utils;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using GTFO.API;

namespace EOSExt.EMP.Impl.Handlers
{
    public class EMPGunSightHandler : EMPHandler
    {
        public static IEnumerable<EMPGunSightHandler> Instances => handlers;

        private static List<EMPGunSightHandler> handlers = new();

        static EMPGunSightHandler()
        {
            LevelAPI.OnLevelCleanup += Clear;
            LevelAPI.OnBuildStart += Clear;
        }

        public GameObject[] _sightPictures;

        private void SetupGunSights()
        {
            var componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
            if (componentsInChildren != null)
            {
                _sightPictures = componentsInChildren
                    .Where(x => x.sharedMaterial != null && x.sharedMaterial.shader != null)
                    .Where(x => x.sharedMaterial.shader.name.Contains("HolographicSight"))
                    .Select(x => x.gameObject)
                    .ToArray();
            }

            if (_sightPictures == null || _sightPictures.Length < 1)
            {
                EOSLogger.Error($"Unable to find sight on {go.name}!");
            }
        }

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            base.Setup(gameObject, controller);
            SetupGunSights();
            handlers.Add(this);
        }

        protected override void DeviceOff() => ForEachSights(x => x.SetActive(false));

        protected override void DeviceOn() => ForEachSights(x => x.SetActive(true));

        protected override void FlickerDevice() => ForEachSights(x => x.SetActive(Random.FlickerUtil()));

        private void ForEachSights(Action<GameObject> action)
        {
            foreach (GameObject sightPicture in _sightPictures)
            {
                if (sightPicture != null && action != null)
                    action(sightPicture);
            }
        }

        private static void Clear()
        {
            handlers.Clear();
        }
    }
}
