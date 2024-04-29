using ExtraObjectiveSetup.Utils;
using System;
using System.Collections.Generic;

namespace EOSExt.EMP.Definition
{
    public class pEMPDefinition
    {
        public uint pEMPIndex { get; set; } = 0u;

        public Vec3 Position { set; get; } = new();

        public float Range { set; get; } = 0.0f;

        public ItemToDisable ItemToDisable { get; set; } = new();

        public pEMPDefinition() { }

        public pEMPDefinition(pEMPDefinition other)
        {
            pEMPIndex = other.pEMPIndex;
            Position = new() { 
                x = other.Position.x,
                y = other.Position.y,
                z = other.Position.z
            };
            Range = other.Range;
            ItemToDisable = new() {
                BioTracker = other.ItemToDisable.BioTracker,
                PlayerHUD = other.ItemToDisable.PlayerHUD,
                PlayerFlashLight = other.ItemToDisable.PlayerFlashLight,
                EnvLight = other.ItemToDisable.EnvLight,
                GunSight = other.ItemToDisable.GunSight,
                Sentry = other.ItemToDisable.Sentry,
                Map = other.ItemToDisable.Map,
            };
        }

        public override bool Equals(object obj)
        {
            return obj is pEMPDefinition definition &&
                   pEMPIndex == definition.pEMPIndex &&
                   Position.ToVector3() == definition.Position.ToVector3() && 
                   Range == definition.Range &&
                   ItemToDisable.Equals(definition.ItemToDisable);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(pEMPIndex, Position, Range, ItemToDisable);
        }
    }
}
