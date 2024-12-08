using EOSExt.EMP.Definition;
using EOSExt.EMP.Impl.PersistentEMP;
using System;
using UnityEngine;

namespace EOSExt.EMP.Impl
{
    public class EMPShock
    {
        public static readonly ItemToDisable DISABLE_NOTHING = new()
        {
            BioTracker = false,
            PlayerFlashLight = false,
            PlayerHUD = false,
            Sentry = false,
            EnvLight = false,
            GunSight = false,
            Map = false,
        };

        public virtual ActiveState State => RemainingTime > 0f ? ActiveState.ENABLED : ActiveState.DISABLED;

        public Vector3 position { get; protected set; }
        
        public float range { get; protected set; }

        public float endTime { get; protected set; }

        public virtual float RemainingTime => endTime - Clock.Time;

        public virtual ItemToDisable ItemToDisable => new() { 
            // default: disable all
        };

        public EMPShock(Vector3 position, float range, float endTime)
        {
            this.position = position;
            this.range = range;
            this.endTime = endTime;
        }

        public bool InRange(Vector3 position) => Vector3.Distance(position, this.position) < range;

        public override bool Equals(object obj)
        {
            return obj is EMPShock shock &&
                   position == shock.position &&
                   range == shock.range;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State, position, range, endTime, RemainingTime, ItemToDisable);
        }
    }
}
