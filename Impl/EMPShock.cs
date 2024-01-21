using EOSExt.EMP.Definition;
using FloLib.Networks.Replications;
using UnityEngine;

namespace EOSExt.EMP.Impl
{
    public class EMPShock
    {
        public readonly ItemToDisable DISABLE_NOTHING = new()
        {
            BioTracker = false,
            PlayerFlashLight = false,
            PlayerHUD = false,
            Sentry = false,
            EnvLight = false,
            GunSight = false,
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
    }
}
