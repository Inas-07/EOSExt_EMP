using UnityEngine;

namespace EOSExt.EMP.Impl
{
    public class EMPShock
    {
        public Vector3 position { get; internal set; }
        
        public float range { get; internal set; }

        public float endTime { get; internal set; }

        public float RemainingTime => endTime - Clock.Time;

        public EMPShock(Vector3 position, float range, float endTime)
        {
            this.position = position;
            this.range = range;
            this.endTime = endTime;
        }

        public bool InRange(Vector3 position) => Vector3.Distance(position, this.position) < range;
    }
}
