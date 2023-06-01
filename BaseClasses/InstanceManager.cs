﻿using System.Collections.Generic;
using GameData;
using LevelGeneration;
using ExtraObjectiveSetup.Utils;
using GTFO.API;

namespace ExtraObjectiveSetup.BaseClasses
{
    public abstract class InstanceManager<T> where T : Il2CppSystem.Object
    {
        protected Dictionary<(eDimensionIndex, LG_LayerType, eLocalZoneIndex), Dictionary<System.IntPtr, uint>> instances2Index = new();
        protected Dictionary<(eDimensionIndex, LG_LayerType, eLocalZoneIndex), List<T>> index2Instance = new();

        public const uint INVALID_INSTANCE_INDEX = uint.MaxValue;

        public virtual uint Register((eDimensionIndex, LG_LayerType, eLocalZoneIndex) globalZoneIndex, T instance)
        {
            if (instance == null) return INVALID_INSTANCE_INDEX;

            Dictionary<System.IntPtr, uint> instancesInZone = null;
            List<T> instanceIndexInZone = null;
            if (!instances2Index.ContainsKey(globalZoneIndex))
            {
                instancesInZone = new();
                instanceIndexInZone = new();
                instances2Index[globalZoneIndex] = instancesInZone;
                index2Instance[globalZoneIndex] = instanceIndexInZone;
            }
            else
            {
                instancesInZone = instances2Index[globalZoneIndex];
                instanceIndexInZone = index2Instance[globalZoneIndex];
            }

            if (instancesInZone.ContainsKey(instance.Pointer))
            {
                EOSLogger.Error($"InstanceManager<{typeof(T)}>: trying to register duplicate instance! Skipped....");
                return INVALID_INSTANCE_INDEX;
            }

            uint instanceIndex = (uint)instancesInZone.Count; // valid index starts from 0

            instancesInZone[instance.Pointer] = instanceIndex;
            instanceIndexInZone.Add(instance);

            return instanceIndex;
        }

        public virtual uint Register(T instance) => Register(GetGlobalZoneIndex(instance), instance);

        /// <summary>
        /// Return the zone instance index for this 'instance'. 
        /// The zone will be located via global zone index of the instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public uint GetZoneInstanceIndex(T instance)
        {
            var globalZoneIndex = GetGlobalZoneIndex(instance);

            if (!instances2Index.ContainsKey(globalZoneIndex)) return INVALID_INSTANCE_INDEX;

            var zoneInstanceIndices = instances2Index[globalZoneIndex];
            return zoneInstanceIndices.ContainsKey(instance.Pointer) ? zoneInstanceIndices[instance.Pointer] : INVALID_INSTANCE_INDEX;
        }

        public T GetInstance((eDimensionIndex, LG_LayerType, eLocalZoneIndex) globalZoneIndex, uint instanceIndex)
        {
            if (!index2Instance.ContainsKey(globalZoneIndex)) return default;

            var zoneInstanceIndices = index2Instance[globalZoneIndex];

            return instanceIndex < zoneInstanceIndices.Count ? zoneInstanceIndices[(int)instanceIndex] : default;
        }

        public T GetInstance(eDimensionIndex dimensionIndex, LG_LayerType layerType, eLocalZoneIndex localIndex, uint instanceIndex) => GetInstance((dimensionIndex, layerType, localIndex), instanceIndex);

        public List<T> GetInstancesInZone((eDimensionIndex, LG_LayerType, eLocalZoneIndex) globalZoneIndex) => index2Instance.ContainsKey(globalZoneIndex) ? index2Instance[globalZoneIndex] : null;

        public List<T> GetInstancesInZone(eDimensionIndex dimensionIndex, LG_LayerType layerType, eLocalZoneIndex localIndex) => GetInstancesInZone((dimensionIndex, layerType, localIndex));

        public bool IsRegistered(T instance)
        {
            var globalZoneIndex = GetGlobalZoneIndex(instance);
            if (!instances2Index.ContainsKey(globalZoneIndex)) return false;

            return instances2Index[globalZoneIndex].ContainsKey(instance.Pointer);
        }

        public IEnumerable<(eDimensionIndex, LG_LayerType, eLocalZoneIndex)> RegisteredZones() => index2Instance.Keys;

        /// <summary>
        /// Clear all registered instances.
        /// This method should only be invoked upon OnLevelCleanup
        /// </summary>
        private void Clear()
        {
            index2Instance.Clear();
            instances2Index.Clear();
        }

        public abstract (eDimensionIndex, LG_LayerType, eLocalZoneIndex) GetGlobalZoneIndex(T instance);

        public InstanceManager()
        {
            LevelAPI.OnLevelCleanup += Clear;
        }
    }
}
