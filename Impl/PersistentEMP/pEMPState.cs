namespace EOSExt.EMP.Impl.PersistentEMP
{
    public struct pEMPState
    {
        public ActiveState status;

        public pEMPState() { }

        public pEMPState(ActiveState status) { this.status = status; }

        public pEMPState(pEMPState o)
        {
            status = o.status;
        }
    }
}
