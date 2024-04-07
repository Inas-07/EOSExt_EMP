using System.Collections.Generic;
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
            if(Player == null)
            {
                EOSLogger.Error($"{nameof(SetupHUDAndFlashlight): LocalPlayerAgent is not set!}"); 
                return;
            }

            if(EMPPlayerHudHandler.Instance == null)
            {
                Player.gameObject.AddComponent<EMPController>().AssignHandler(new EMPPlayerHudHandler());
                EOSLogger.Log("pEMP: PlayerHud setup completed");
            }
            if (EMPPlayerFlashLightHandler.Instance == null)
            {
                Player.gameObject.AddComponent<EMPController>().AssignHandler(new EMPPlayerFlashLightHandler());
                EOSLogger.Log("pEMP: PlayerFlashlight setup completed");
            }
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
                    EOSLogger.Log($"pEMP: Backpack {slot} setup completed");
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
                    EOSLogger.Log($"pEMP: Backpack {InventorySlot.GearClass} setup completed");
                }
            }
            else
            {
                EOSLogger.Warning($"Couldn't get item for slot {InventorySlot.GearClass}!");
            }
        }
    }
}
