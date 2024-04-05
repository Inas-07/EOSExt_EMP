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
using System.Collections.Concurrent;

namespace EOSExt.EMP
{
    public partial class EMPManager : GenericExpeditionDefinitionManager<pEMPDefinition>
    {
        private List<EMPController> _empTargets { get; } = new List<EMPController>();

        private ConcurrentBag<EMPShock> _activeEMPs { get; } = new();
        
        public IEnumerable<EMPShock> ActiveEMPs => _activeEMPs;

        public static EMPManager Current { get; private set; } = new();

        public PlayerAgent LocalPlayerAgent { get; private set; } = null;


        internal void SetLocalPlayerAgent(PlayerAgent localPlayerAgent)
        {
            this.LocalPlayerAgent = localPlayerAgent;

            PlayerpEMPComponent.Current = localPlayerAgent.gameObject.AddComponent<PlayerpEMPComponent>();
            PlayerpEMPComponent.Current.player = localPlayerAgent;
            EOSLogger.Debug("LocalPlayerAgent setup completed");
        }

        internal void OnLocalPlayerAgentDestroy()
        {
            PlayerpEMPComponent.Current = null;
            EOSLogger.Debug("LocalPlayerAgent Destroyed");
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
                var endTime = Clock.Time + duration;
                _activeEMPs.Add(new EMPShock(position, range, endTime));
                foreach (EMPController empTarget in _empTargets)
                {
                    if (Vector3.Distance(position, empTarget.Position) < range)
                        empTarget.AddTime(duration);
                }
            }
        }

        public float DurationFromPosition(Vector3 position)
        {
            //_activeEMPs.RemoveAll(e => Mathf.Round(e.RemainingTime) <= 0);
            float totalDurationForPosition = 0;
            foreach (EMPShock emp in _activeEMPs)
            {
                if (emp.InRange(position))
                {
                    totalDurationForPosition += emp.RemainingTime;
                }
            }
            return totalDurationForPosition;
        }

        private void Clear()
        {
            _empTargets.Clear();
            _activeEMPs.Clear();
            EMPHandler.Cleanup();
        }

        public override void Init()
        {
            EMPWardenEvents.Init();
            LevelAPI.OnBuildStart += Clear;
            LevelAPI.OnLevelCleanup += Clear;

            Events.InventoryWielded += SetupAmmoWeaponHandlers;
            //EventAPI.OnManagersSetup += () =>
            //{
            //    _activeEMPCheckCoroutine = CoroutineManager.StartPersistantCoroutine();
            //};
            pEMPInit();
        }

        public IEnumerable<EMPController> EMPTargets => _empTargets;

        private Coroutine _activeEMPCheckCoroutine = null;

        private System.Collections.IEnumerator activeEMPCheck()
        {
            while(true)
            {
                
            }
        }

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
