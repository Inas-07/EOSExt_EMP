using UnityEngine;

namespace EOSExt.EMP.Impl
{
    public abstract partial class EMPHandler
    {
        protected DeviceState _deviceState;
        
        protected float _stateTimer;

        private float _delayTimer;

        private bool _destroyed;

        protected virtual float FlickerDuration => 0.2f;

        protected virtual float OnToOffMinDelay => 0.0f;

        protected virtual float OnToOffMaxDelay => 0.75f;

        protected virtual float OffToOnMinDelay => 0.85f;

        protected virtual float OffToOnMaxDelay => 1.5f;


        protected virtual bool IsDeviceOnPlayer => false;

        public void ForceState(EMPState state)
        {
            if (State == state)
                return;
            State = state;
            _delayTimer = Clock.Time - 1f;
            _stateTimer = Clock.Time - 1f;
            if (state != EMPState.On)
            {
                if (state == EMPState.Off)
                {
                    _deviceState = DeviceState.Off;
                    DeviceOff();
                }
                else // state == FlickerOn or FlickerOff
                {
                    _deviceState = DeviceState.Unknown;
                }
            }
            else
            {
                _deviceState = DeviceState.On;
                DeviceOn();
            }
        }

        public void Tick()
        {
            if (_destroyed)
                return;

            bool isEMPD = IsEMPed();

            if (isEMPD && State == EMPState.On)
            {
                float randomDelay = Random.GetRandomDelay(OnToOffMinDelay, OnToOffMaxDelay);
                State = EMPState.FlickerOff;
                _delayTimer = Clock.Time + randomDelay;
                _stateTimer = Clock.Time + randomDelay + FlickerDuration;
            }
            if (!isEMPD && State == EMPState.Off)
            {
                float randomDelay = Random.GetRandomDelay(OffToOnMinDelay, OffToOnMaxDelay);
                State = EMPState.FlickerOn;
                _delayTimer = Clock.Time + randomDelay;
                _stateTimer = Clock.Time + randomDelay + FlickerDuration;
            }

            switch (State)
            {
                case EMPState.On:
                    if (_deviceState == DeviceState.On)
                        break;
                    DeviceOn();
                    _deviceState = DeviceState.On;
                    break;

                case EMPState.FlickerOff:
                    if (_delayTimer > Clock.Time)
                        break;
                    if (Clock.Time < _stateTimer)
                    {
                        FlickerDevice();
                        break;
                    }
                    State = EMPState.Off;
                    break;

                case EMPState.Off:
                    if (_deviceState == DeviceState.Off)
                        break;
                    DeviceOff();
                    _deviceState = DeviceState.Off;
                    break;

                case EMPState.FlickerOn:
                    if (_delayTimer > Clock.Time)
                        break;
                    if (Clock.Time < _stateTimer)
                    {
                        FlickerDevice();
                        break;
                    }
                    State = EMPState.On;
                    break;
            }

            OnTick(isEMPD);
        }

        protected virtual void OnTick(bool isEMPD)
        {

        }

        protected abstract void FlickerDevice();

        protected abstract void DeviceOn();

        protected abstract void DeviceOff();

        protected enum DeviceState
        {
            On,
            Off,
            Unknown,
        }

    }
}
