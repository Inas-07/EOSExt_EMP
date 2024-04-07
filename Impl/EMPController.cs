using Il2CppInterop.Runtime.Attributes;
using ExtraObjectiveSetup.Utils;
using UnityEngine;

namespace EOSExt.EMP.Impl
{
    public sealed class EMPController : MonoBehaviour
    {
        public EMPHandler Handler { get; private set; }

        private bool _hasHandler;

        private bool _setup;

        public float endTime { get; private set; }

        public Vector3 Position => transform.position;

        private float next_remove_time = float.NaN;

        private const float UPDATE_INTERVAL = 1.0f;

        //private void OnEnable()
        //{
        //    if (GameStateManager.CurrentStateName != eGameStateName.InLevel || !_setup)
        //        return;

        //    if (endTime > Clock.Time)// emp is not ended, still in effect
        //    {
        //        Handler.ForceState(EMPState.Off);
        //    }
        //    else
        //    {
        //        Handler.ForceState(EMPState.On);
        //    }
        //}

        private void Update()
        {
            if (!_hasHandler)
                return;
            float time = Clock.Time;
            Handler.Tick();

            if(float.IsNaN(next_remove_time) || next_remove_time < time)
            {
                Handler.RemoveEndedEMPs();
                next_remove_time = time + UPDATE_INTERVAL;
            }
        }

        [HideFromIl2Cpp]
        public void AssignHandler(EMPHandler handler)
        {
            if (Handler != null)
            {
                EOSLogger.Warning("Tried to assign a handler to a controller that already had one!");
            }
            else
            {
                Handler = handler;
                Handler.Setup(gameObject, this);
                _hasHandler = true;
                _setup = true;
            }
        }

        [HideFromIl2Cpp]
        public void ForceState(EMPState state)
        {
            if (Handler == null)
                return;
            Handler.ForceState(state);
        }

        private void OnDestroy()
        {
            Handler.OnDespawn();
            Handler = null;
        }
    }
}
