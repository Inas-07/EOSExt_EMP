//using System;
//using EOSExt.EMP.Definition;
//using ExtraObjectiveSetup.Utils;

//namespace EOSExt.EMP.Impl
//{
//    public enum ActiveState
//    {
//        DISABLED,
//        ENABLED,
//    }

//    public struct pEMPState
//    {
//        public ActiveState status;

//        public pEMPState() { }

//        public pEMPState(ActiveState status) { this.status = status; }

//        public pEMPState(pEMPState o)
//        {
//            status = o.status;
//        }
//    }

//    public class pEMP : EMPShock
//    {
//        public pEMPDefinition def { get; private set; }

//        public ActiveState State { get; private set; } = ActiveState.DISABLED;

//        public void ChangeState(ActiveState newState)
//        {
//            EOSLogger.Debug($"pEMP_{def.pEMPIndex} Change state: {State} -> {newState}");
//            State = newState;
//            switch (newState)
//            {
//                case ActiveState.DISABLED:
//                    duration = float.NegativeInfinity; break;
//                case ActiveState.ENABLED:
//                    duration = float.PositiveInfinity; break;
//                default: throw new NotImplementedException();
//            }
//        }

//        //public void ChangeState(ActiveState newState) => StateReplicator.SetState(new pEMPState() { status = newState });

//        //internal void SetupReplicator(uint replicatorID)
//        //{
//        //    StateReplicator = StateReplicator<pEMPState>.Create(replicatorID, new pEMPState() { status = ActiveState.DISABLED }, LifeTimeType.Level);
//        //    StateReplicator.OnStateChanged += OnStateChanged;
//        //}

//        public pEMP(pEMPDefinition def)
//            : base(def.Position.ToVector3(), def.Range, float.NegativeInfinity)
//        {
//            this.def = def;
//        }
//    }
//}
