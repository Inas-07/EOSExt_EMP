﻿using UnityEngine;

namespace EOSExt.EMP.Impl
{
    public abstract class EMPHandler
    {
        protected DeviceState _deviceState;
        protected float _stateTimer;
        private static bool _isLocalPlayerDisabled;
        private float _delayTimer;
        private bool _destroyed;

        public EMPState State { get; protected set; }

        public EMPController controller { get; protected set; }

        public GameObject gameObject { get; protected set; }

        public static bool IsLocalPlayerDisabled => _isLocalPlayerDisabled && GameStateManager.CurrentStateName == eGameStateName.InLevel;

        protected virtual float FlickerDuration => 0.2f;

        protected virtual float MinDelay => 0.0f;

        protected virtual float MaxDelay => 1.5f;

        protected virtual bool IsDeviceOnPlayer => false;

        public virtual void Setup(GameObject gameObject, EMPController controller)
        {
            this.gameObject = gameObject;
            this.controller = controller;
        }

        public static void Cleanup() => _isLocalPlayerDisabled = false;

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

        public void Tick(bool isEMPD)
        {
            if (_destroyed)
                return;

            if (isEMPD && State == EMPState.On)
            {
                float randomDelay = GetRandomDelay(MinDelay, MaxDelay);
                State = EMPState.FlickerOff;
                _delayTimer = Clock.Time + randomDelay;
                _stateTimer = Clock.Time + randomDelay + FlickerDuration;
            }
            if (!isEMPD && State == EMPState.Off)
            {
                float randomDelay = GetRandomDelay(MinDelay, MaxDelay);
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
                    if (!IsDeviceOnPlayer)
                        break;
                    _isLocalPlayerDisabled = false; // TODO: EMP effect is separated on different parts, so this field should be deprecated
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
                    if (!IsDeviceOnPlayer)
                        break;
                    _isLocalPlayerDisabled = true;
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
        }

        public void OnDespawn() => _destroyed = true;

        protected abstract void FlickerDevice();

        protected abstract void DeviceOn();

        protected abstract void DeviceOff();

        protected enum DeviceState
        {
            On,
            Off,
            Unknown,
        }

        // EEC.Utils.Rand
        protected static System.Random _rand { get; private set; } = new System.Random();

        protected static float GetRandomDelay(float min, float max) => min + GetRandom01() * (max - min);

        protected static float GetRandom01() => (float)_rand.NextDouble();

        protected static int GetRandomRange(int min, int maxPlusOne) => _rand.Next(min, maxPlusOne);

        protected static int Index(int length) => _rand.Next(0, length);

        protected static bool FlickerUtil(int oneInX = 2) => Index(oneInX) == 0;

    }
}
