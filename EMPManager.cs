using UnityEngine;
using System.Collections.Generic;
using GTFO.API;
using Il2CppInterop.Runtime.Injection;
using ExtraObjectiveSetup.Utils;
using Player;
using EOSExt.EMP.Impl;
using EOSExt.EMP.EMPComponent;
using EOSExt.EMP.Definition;
using ExtraObjectiveSetup.BaseClasses;

namespace EOSExt.EMP
{
    public partial class EMPManager : GenericExpeditionDefinitionManager<pEMPDefinition>
    {
        private List<EMPController> _empTargets { get; } = new List<EMPController>();

        private List<EMPShock> _activeEMPShock { get; } = new List<EMPShock>();

        public static EMPManager Current { get; private set; } = new();

        public PlayerAgent LocalPlayerAgent { get; private set; } = null;

        public void SetLocalPlayerAgent(PlayerAgent localPlayerAgent)
        {
            this.LocalPlayerAgent = localPlayerAgent;
            if (PlayerpEMPComponent.Current != null)
            {
                LevelAPI.OnBuildStart -= PlayerpEMPComponent.Current.Reset;
                LevelAPI.OnLevelCleanup -= PlayerpEMPComponent.Current.Reset;
            }

            PlayerpEMPComponent.Current = localPlayerAgent.gameObject.AddComponent<PlayerpEMPComponent>();
            LevelAPI.OnBuildStart += PlayerpEMPComponent.Current.Reset;
            LevelAPI.OnLevelCleanup += PlayerpEMPComponent.Current.Reset;
            EOSLogger.Debug("LocalPlayerAgent setup completed");
        }

        public void AddTarget(EMPController target) => _empTargets.Add(target);

        public void RemoveTarget(EMPController target) => _empTargets.Remove(target);

        public void ActivateEMPShock(Vector3 position, float range, float duration)
        {
            if (!GameStateManager.IsInExpedition)
            {
                EOSLogger.Error("Tried to activate an EMP when not in level, this shouldn't happen!");
            }
            else
            {
                _activeEMPShock.Add(new EMPShock(position, range, duration));
                foreach (EMPController empTarget in _empTargets)
                {
                    if (Vector3.Distance(position, empTarget.Position) < range)
                        empTarget.AddTime(duration);
                }
            }
        }

        public float DurationFromPosition(Vector3 position)
        {
            _activeEMPShock.RemoveAll(e => Mathf.Round(e.RemainingTime) <= 0);
            float totalDurationForPosition = 0;
            foreach (EMPShock active in _activeEMPShock)
            {
                if (active.InRange(position))
                {
                    totalDurationForPosition += active.RemainingTime;
                }
            }
            return totalDurationForPosition;
        }

        private void Clear()
        {
            _empTargets.Clear();
            _activeEMPShock.Clear();
            EMPHandler.Cleanup();
        }

        public override void Init()
        {
            EMPWardenEvents.Init();
            LevelAPI.OnBuildStart += Clear;
            LevelAPI.OnLevelCleanup += Clear;
            pEMPInit();
        }

        public IEnumerable<EMPController> EMPTargets => _empTargets;

        private EMPManager()
        {

        }

        static EMPManager()
        {
            ClassInjector.RegisterTypeInIl2Cpp<EMPController>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerpEMPComponent>();
        }
    }
}
