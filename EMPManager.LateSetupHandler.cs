﻿using System.Collections.Generic;
using ExtraObjectiveSetup.Utils;
using Player;
using Gear;
using EOSExt.EMP.Impl;
using EOSExt.EMP.Impl.Handlers;
using ExtraObjectiveSetup.BaseClasses;
using EOSExt.EMP.Definition;

namespace EOSExt.EMP
{
    public partial class EMPManager: GenericExpeditionDefinitionManager<pEMPDefinition>
    {
        internal void SetupHUDAndFlashlight()
        {
            if(LocalPlayerAgent == null)
            {
                EOSLogger.Error($"{nameof(SetupHUDAndFlashlight): LocalPlayerAgent is not set!}"); 
                return;
            }

            if (LocalPlayerAgent.gameObject.GetComponent<EMPController>() != null)
            {
                return;
            }

            LocalPlayerAgent.gameObject.AddComponent<EMPController>().AssignHandler(new EMPPlayerHudHandler());
            LocalPlayerAgent.gameObject.AddComponent<EMPController>().AssignHandler(new EMPPlayerFlashLightHandler());
            EOSLogger.Log($"pEMP: PlayerHUD & flashlight setup completed");
        }

        internal void SetupAmmoWeaponHandlers(InventorySlot slot)
        {
            //EOSLogger.Error("SetupAmmoWeaponHandlers");

            void SetupAmmoWeaponHandlerForSlot(InventorySlot slot)
            {
                var backpack = PlayerBackpackManager.LocalBackpack;
                if (backpack.TryGetBackpackItem(slot, out var backpackItem))
                {
                    if (backpackItem.Instance.gameObject.GetComponent<EMPController>() != null)
                    {
                        //EOSLogger.Debug("Item already has controller, skipping...");
                        return;
                    }

                    backpackItem.Instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPGunSightHandler());
                }
                else
                {
                    EOSLogger.Warning($"Couldn't get item for slot {slot}!");
                }
            }

            switch (slot)
            {
                case InventorySlot.GearStandard:
                case InventorySlot.GearSpecial:
                    SetupAmmoWeaponHandlerForSlot(slot);
                    break;
                default: return;
            }
            EOSLogger.Log($"pEMP: Backpack {slot} setup completed");
        }

        internal void SetupToolHandler()
        {
            var backpack = PlayerBackpackManager.LocalBackpack;
            if (backpack.TryGetBackpackItem(InventorySlot.GearClass, out var backpackItem))
            {
                if (backpackItem.Instance.gameObject.GetComponent<EMPController>() != null)
                {
                    //EOSLogger.Debug("Item already has controller, skipping...");
                    return;
                }

                if (backpackItem.Instance.GetComponent<EnemyScanner>() != null)
                {
                    backpackItem.Instance.gameObject.AddComponent<EMPController>().AssignHandler(new EMPBioTrackerHandler());
                    EOSLogger.Warning($"pEMP: Backpack {InventorySlot.GearClass} setup completed");
                }
            }
            else
            {
                EOSLogger.Warning($"Couldn't get item for slot {InventorySlot.GearClass}!");
            }
        }

    }
}
