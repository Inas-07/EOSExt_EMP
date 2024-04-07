using ExtraObjectiveSetup.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace EOSExt.EMP.Impl
{
    public abstract partial class EMPHandler
    {
        protected static Dictionary<long, EMPHandler> Handlers { get; } = new();

        private static long next_handler_id = 0;

        public static IEnumerable<EMPHandler> AllHandlers => Handlers.Values;

        protected long HandlerId { get; private set; }

        public EMPState State { get; protected set; }

        protected HashSet<EMPShock> AffectedBy { get; } = new();

        public GameObject go { get; protected set; }

        public Vector3 position => go?.transform.position ?? Vector3.zero;

        public virtual bool IsEMPed()
        {
            foreach(var emp in AffectedBy)
            {
                if (Clock.Time < emp.endTime) return true;
            }

            return false;
        }

        public virtual void Setup(GameObject gameObject, EMPController controller)
        {
            go = gameObject;

            foreach (var emp in EMPManager.Current.ActiveEMPs)
            {
                if(Vector3.Distance(go.transform.position, emp.position) < emp.range)
                {
                    AddAffectedBy(emp);
                }
            }

            HandlerId = next_handler_id++;
            if (Handlers.ContainsKey(HandlerId))
            {
                EOSLogger.Warning("What the hell we got a duplicate EMPHandler ID!?");
            }

            Handlers[HandlerId] = this;
        }

        public virtual void OnDespawn()
        {
            _destroyed = true;
            AffectedBy.Clear();
            Handlers.Remove(HandlerId);
            go = null;
        }

        public void AddAffectedBy(EMPShock empShock) => AffectedBy.Add(empShock);

        public void RemoveAffectedBy(EMPShock empShock) => AffectedBy.Remove(empShock);

        internal void RemoveEndedEMPs()
        {
            float time = Clock.Time;
            AffectedBy.RemoveWhere(emp => emp.endTime < time);
        } 
    }
}
