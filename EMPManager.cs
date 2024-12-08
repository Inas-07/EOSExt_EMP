using UnityEngine;
using Il2CppInterop.Runtime.Injection;
using ExtraObjectiveSetup.Utils;
using Player;
using EOSExt.EMP.Impl;
using EOSExt.EMP.EMPComponent;
using EOSExt.EMP.Definition;
using ExtraObjectiveSetup.BaseClasses;
using System.Collections.Generic;
using EOSExt.EMP.Impl.PersistentEMP;
using GTFO.API.Utilities;
using Il2CppSystem.Runtime.InteropServices;
using SNetwork;

namespace EOSExt.EMP
{
    public partial class EMPManager : GenericExpeditionDefinitionManager<pEMPDefinition>
    {
        public static EMPManager Current { get; } = new();

        //public PlayerAgent Player { get; private set; }
        public PlayerAgent Player => SNet.HasLocalPlayer && SNet.LocalPlayer.HasPlayerAgent? SNet.LocalPlayer.PlayerAgent.Cast<PlayerAgent>() : null; 

        internal List<EMPShock> ActiveEMPs { get; } = new();

        internal void SetLocalPlayerAgent(PlayerAgent localPlayerAgent)
        {
            if(PlayerpEMPComponent.Current == null)
            {
                localPlayerAgent.gameObject.AddComponent<PlayerpEMPComponent>();
            }
            //EOSLogger.Debug("LocalPlayerAgent setup completed");
        }

        public void ActivateEMPShock(Vector3 position, float range, float duration)
        {
            if (!GameStateManager.IsInExpedition)
            {
                EOSLogger.Error("Tried to activate an EMP when not in level, this shouldn't happen!");
                return;
            }

            var endTime = Clock.Time + duration;
            var empshock = new EMPShock(position, range, endTime);
            foreach (var h in EMPHandler.AllHandlers)
            {
                if (Vector3.Distance(position, h.position) < range)
                {
                    h.AddAffectedBy(empshock);
                }
            }

            ActiveEMPs.Add(empshock);
        }

        internal void RemoveInactiveEMPs()
        {
            float time = Clock.Time;
            ActiveEMPs.RemoveAll(emp => emp.endTime < time);
        }

        public bool IsPlayerMapEMPD()
        {
            if (Player == null || !GameStateManager.IsInExpedition) return false;

            var p = Player.Position;
            foreach(var emp in ActiveEMPs)
            {
                if(Vector3.Distance(p, emp.position) < emp.range)
                {
                    return true;
                }
            }

            foreach (var pemp in pEMPs)
            {
                if (pemp.State != ActiveState.ENABLED) continue;
                if (!pemp.ItemToDisable.Map) continue;

                if (Vector3.Distance(p, pemp.position) < pemp.range)
                {
                    EOSLogger.Warning("MapEMPD: by pEMP");
                    return true;
                }
            }

            return false;
        }

        protected override void FileChanged(LiveEditEventArgs e)
        {
            base.FileChanged(e);
            if(definitions.TryGetValue(CurrentMainLevelLayout, out var defs))
            {
                foreach(var def in defs.Definitions)
                {
                    EOSLogger.Warning(def.ItemToDisable.ToString());
                }
            }
        }

        public override void Init()
        {
            EMPWardenEvents.Init();
            Events.InventoryWielded += SetupAmmoWeaponHandlers;
            pEMPInit();
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
