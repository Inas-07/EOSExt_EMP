﻿using LevelGeneration;
using GameData;
using System.Collections;
using LEGACY.Utilities;
using Player;
using BepInEx.IL2CPP.Utils.Collections;
using SNetwork;
using AIGraph;
using AK;

namespace LEGACY.Patch.ExtraEventsConfig.SpawnEnemyWave_Custom
{

    sealed class ExtraEventsConfig_SpawnEnemyWave_Custom
    {
        private static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ushort>> WaveEventsMap = null;

        public static void StopSpecifiedWave(WardenObjectiveEventData eventToTrigger, bool ignoreTrigger, float currentDuration)
        {
            UnityEngine.Coroutine coroutine = CoroutineManager.StartCoroutine(StopWave(eventToTrigger, currentDuration).WrapToIl2Cpp(), null);
            WardenObjectiveManager.m_wardenObjectiveEventCoroutines.Add(coroutine);
        }

        private static IEnumerator StopWave(WardenObjectiveEventData eventToTrigger, float currentDuration)
        {
            WardenObjectiveEventData e = eventToTrigger;

            float delay = UnityEngine.Mathf.Max(e.Delay - currentDuration, 0f);
            if (delay > 0f)
            {
                yield return new UnityEngine.WaitForSeconds(delay);
            }

            WardenObjectiveManager.DisplayWardenIntel(e.Layer, e.WardenIntel);
            if (e.DialogueID > 0u)
            {
                PlayerDialogManager.WantToStartDialog(e.DialogueID, -1, false, false);
            }
            if (e.SoundID > 0u)
            {
                WardenObjectiveManager.Current.m_sound.Post(e.SoundID, true);
                var line = e.SoundSubtitle.ToString();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    GuiManager.PlayerLayer.ShowMultiLineSubtitle(line);
                }
            }

            if (!SNet.IsMaster) yield break;

            if (string.IsNullOrEmpty(e.WorldEventObjectFilter))
            {
                Logger.Error("WorldEventObjectFilter is empty. Aborted stop wave event.");
                yield break;
            }

            if (!WaveEventsMap.ContainsKey(e.WorldEventObjectFilter))
            {
                Logger.Error("Wave Filter {0} is unregistered, cannot stop wave.", e.WorldEventObjectFilter);
                yield break;
            }

            WaveEventsMap.TryGetValue(e.WorldEventObjectFilter, out var eventIDList);
            WaveEventsMap.Remove(e.WorldEventObjectFilter);

            Mastermind.MastermindEvent masterMindEvent_StopWave;
            foreach(ushort eventID in eventIDList)
            {
                if (Mastermind.Current.TryGetEvent(eventID, out masterMindEvent_StopWave))
                {
                    masterMindEvent_StopWave.StopEvent();
                }
            }

            Logger.Debug("Wave(s) with filter {0} stopped.", e.WorldEventObjectFilter);
        }

        public static void OnStopAllWave()
        {
            WaveEventsMap.Clear();
        }

        public static bool SpawnWave(WardenObjectiveEventData eventToTrigger, bool ignoreTrigger, float currentDuration)
        {
            if (!string.IsNullOrEmpty(eventToTrigger.WorldEventObjectFilter))
            {
                if (!WaveEventsMap.ContainsKey(eventToTrigger.WorldEventObjectFilter))
                {
                    System.Collections.Generic.List<ushort> eventIDList = new();
                    WaveEventsMap.Add(eventToTrigger.WorldEventObjectFilter, eventIDList);
                    Logger.Debug("Registering Wave(s) with filter {0}", eventToTrigger.WorldEventObjectFilter);

                    // added list here instead of in coroutine to get around with issue caused by concurrency.
                    // the WaveEventsMap will be cleaned OnApplicationQuit and AfterLevel
                }
            }

            UnityEngine.Coroutine coroutine = CoroutineManager.StartCoroutine(SpawnWave(eventToTrigger, currentDuration).WrapToIl2Cpp(), null);
            WardenObjectiveManager.m_wardenObjectiveEventCoroutines.Add(coroutine);

            return false;
        }

        private static IEnumerator SpawnWave(WardenObjectiveEventData eventToTrigger, float currentDuration)
        {
            WardenObjectiveEventData e = eventToTrigger;

            float delay = UnityEngine.Mathf.Max(e.Delay - currentDuration, 0f);
            if (delay > 0f)
            {
                yield return new UnityEngine.WaitForSeconds(delay);
            }

            WardenObjectiveManager.DisplayWardenIntel(e.Layer, e.WardenIntel);
            if (e.DialogueID > 0u)
            {
                PlayerDialogManager.WantToStartDialog(e.DialogueID, -1, false, false);
            }
            if (e.SoundID > 0u)
            {
                WardenObjectiveManager.Current.m_sound.Post(e.SoundID, true);
                var line = e.SoundSubtitle.ToString();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    GuiManager.PlayerLayer.ShowMultiLineSubtitle(line);
                }
            }
            
            // spawn code rewrite
            PlayerAgent localPlayer = PlayerManager.GetLocalPlayerAgent();
            if (!SNet.IsMaster || localPlayer == null)
                yield break;

            var waveData = eventToTrigger.EnemyWaveData;

            if ((waveData.WaveSettings > 0U && waveData.WavePopulation > 0U && (currentDuration == 0.0 || waveData.SpawnDelay >= currentDuration)) == false)
                yield break;

            SurvivalWaveSettingsDataBlock waveSettingDB = SurvivalWaveSettingsDataBlock.GetBlock(waveData.WaveSettings);

            bool abort_spawn = false;
            AIG_CourseNode spawnNode = localPlayer.CourseNode;
            SurvivalWaveSpawnType spawnType = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer;

            if (waveSettingDB.m_overrideWaveSpawnType == true)
            {
                switch (waveSettingDB.m_survivalWaveSpawnType)
                {
                    case SurvivalWaveSpawnType.InSuppliedCourseNodeZone:
                        LG_Zone specified_zone = null;
                        Builder.CurrentFloor.TryGetZoneByLocalIndex(eventToTrigger.DimensionIndex, eventToTrigger.Layer, eventToTrigger.LocalIndex, out specified_zone);
                        if (specified_zone == null)
                        {
                            Logger.Error("SpawnSurvialWave_InSuppliedCourseNodeZone - Failed to find LG_Zone.");
                            Logger.Error("DimensionIndex: {0}, Layer: {1}, LocalIndex: {2}", eventToTrigger.DimensionIndex, eventToTrigger.Layer, eventToTrigger.LocalIndex);
                            break;
                        }

                        LG_SecurityDoor door = null;
                        Utils.TryGetZoneEntranceSecDoor(specified_zone, out door);
                        if (door.m_sync.GetCurrentSyncState().status != eDoorStatus.Open && door.LinkedToZoneData.ActiveEnemyWave.HasActiveEnemyWave == false)
                        {
                            Logger.Warning("The LG_SecurityDoor to the supplied zone is inaccessible, and the door has no active enemy wave!");
                            Logger.Warning("Aborted wave spawn.");
                            abort_spawn = true;
                            break;
                        }

                        Logger.Warning("Starting wave with spawn type InSuppliedCourseNodeZone!");
                        Logger.Warning("DimensionIndex: {0}, Layer: {1}, LocalIndex: {2}", eventToTrigger.DimensionIndex, eventToTrigger.Layer, eventToTrigger.LocalIndex);
                        spawnNode = specified_zone.m_courseNodes[0];
                        spawnType = SurvivalWaveSpawnType.InSuppliedCourseNodeZone;

                        break;
                    default: break;
                }
            }

            if (abort_spawn) yield break;

            UnityEngine.Coroutine coroutine = CoroutineManager.StartCoroutine(TriggerEnemyWaveDataAndRegisterWaveEventID(waveData, spawnNode, spawnType, currentDuration, eventToTrigger.WorldEventObjectFilter).WrapToIl2Cpp(), null);
            WardenObjectiveManager.m_wardenObjectiveWaveCoroutines.Add(coroutine);
        }

        internal static void OnMastermindBuildDone()
        {
            WaveEventsMap = new();
        }

        internal static void OnLevelCleanup()
        {
            WaveEventsMap.Clear();
            WaveEventsMap = null;
        }

        private static IEnumerator TriggerEnemyWaveDataAndRegisterWaveEventID(
          GenericEnemyWaveData data,
          AIG_CourseNode spawnNode,
          SurvivalWaveSpawnType waveSpawnType,
          float currentDuration = 0.0f,
          string worldEventObjectFilter = "")
        {
            UnityEngine.Debug.Log("WardenObjectiveManager.TriggerEnemyWaveData Data: " + data + " spawnNode: " + spawnNode + " waveSpawnType: " + waveSpawnType);
            yield return new UnityEngine.WaitForSeconds(data.SpawnDelay - currentDuration);
            if (spawnNode != null)
            {
                ushort eventID;
                if (SNet.IsMaster && Mastermind.Current.TriggerSurvivalWave(spawnNode, data.WaveSettings, data.WavePopulation, out eventID, waveSpawnType, 2f))
                {
                    UnityEngine.Debug.Log("WardenObjectiveManager.TriggerEnemyWaveData - Enemy wave spawned (" + waveSpawnType + ") with edventID " + eventID);
                    if (!string.IsNullOrEmpty(worldEventObjectFilter))
                    {
                        if(WaveEventsMap.ContainsKey(worldEventObjectFilter))
                        {
                            System.Collections.Generic.List<ushort> eventIDList;
                            WaveEventsMap.TryGetValue(worldEventObjectFilter, out eventIDList);
                            eventIDList.Add(eventID);
                        }
                        else
                        {
                            Logger.Error("We should have instanitiated a List before call to coroutine, WTF???");
                            yield break;
                        }

                        Logger.Debug("Registered wave with filter {0}", worldEventObjectFilter);
                    }
                }
                else
                    UnityEngine.Debug.Log("WardenObjectiveManager.TriggerEnemyWaveData : WARNING : Failed spawning enemy wave");
            }

            if (!string.IsNullOrEmpty(data.IntelMessage))
                GuiManager.PlayerLayer.m_wardenIntel.ShowSubObjectiveMessage("", data.IntelMessage);
            
            if (spawnNode != null)
            {
                if (data.TriggerAlarm)
                {
                    // Not pretty sure if 'this' can be replaced by WardenObjectiveManager.Current
                    int num1 = (int)WardenObjectiveManager.Current.m_sound.UpdatePosition(spawnNode.Position);
                    int num2 = (int)WardenObjectiveManager.Current.m_sound.Post(EVENTS.APEX_PUZZLE_START_ALARM);
                    UnityEngine.Debug.Log("WardenObjectiveManager.TriggerEnemyWaveData - Alarm");
                }
                yield return new UnityEngine.WaitForSeconds(2f);
            }
            else
                UnityEngine.Debug.LogError("WardenobjectiveManager.TriggerEnemyWaveData got NO SPAWNNODE for the enemy wave!");
        }

        //private static void SpawnSurvialWave_InSuppliedCourseNodeZone_Custom(WardenObjectiveEventData eventToTrigger, float currentDuration)
        //{
        //    var waveData = eventToTrigger.EnemyWaveData;

        //    if (waveData.WaveSettings > 0U && waveData.WavePopulation > 0U && (currentDuration == 0.0 || waveData.SpawnDelay >= currentDuration))
        //    {
        //        AIG_CourseNode suppliedCourseNode = null;
        //        LG_Zone specified_zone;
        //        Builder.CurrentFloor.TryGetZoneByLocalIndex(eventToTrigger.DimensionIndex, eventToTrigger.Layer, eventToTrigger.LocalIndex, out specified_zone);
        //        if (specified_zone == null)
        //        {
        //            Logger.Error("SpawnSurvialWave_InSuppliedCourseNodeZone - Failed to find LG_Zone.");
        //            Logger.Error("DimensionIndex: {0}, Layer: {1}, LocalIndex: {2}", eventToTrigger.DimensionIndex, eventToTrigger.Layer, eventToTrigger.LocalIndex);
        //            return;
        //        }

        //        LG_SecurityDoor door = null;
        //        Utils.TryGetZoneEntranceSecDoor(specified_zone, out door);
        //        if (door.m_sync.GetCurrentSyncState().status != eDoorStatus.Open && door.LinkedToZoneData.ActiveEnemyWave.HasActiveEnemyWave == false)
        //        {
        //            Logger.Warning("The LG_SecurityDoor to the supplied zone is inaccessible, and the door has no active enemy wave!");
        //            Logger.Warning("Aborted wave spawn.");
        //            return;
        //        }

        //        suppliedCourseNode = specified_zone.m_courseNodes[0];
        //        Logger.Warning("Starting wave with spawn type InSuppliedCourseNodeZone!");
        //        Logger.Warning("DimensionIndex: {0}, Layer: {1}, LocalIndex: {2}", eventToTrigger.DimensionIndex, eventToTrigger.Layer, eventToTrigger.LocalIndex);

        //        UnityEngine.Coroutine coroutine = CoroutineManager.StartCoroutine(WardenObjectiveManager.Current.TriggerEnemyWaveData(waveData, suppliedCourseNode, SurvivalWaveSpawnType.InSuppliedCourseNodeZone, currentDuration), null);
        //        WardenObjectiveManager.m_wardenObjectiveWaveCoroutines.Add(coroutine);
        //    }
        //}

        public static void OnApplicationQuit()
        {
            WaveEventsMap = null;
        }

        private ExtraEventsConfig_SpawnEnemyWave_Custom() { }
    }
}