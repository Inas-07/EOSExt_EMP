using ExtraObjectiveSetup.Utils;

namespace EOSExt.EMP.Definition
{
    public class ItemToDisable
    {
        public bool BioTracker { set; get; } = true;
        public bool PlayerHUD { set; get; } = true;
        public bool PlayerFlashLight { set; get; } = true;
        public bool EnvLight { set; get; } = true;
        public bool GunSight { set; get; } = true;
        public bool Sentry { set; get; } = true;
    }

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
                Sentry = other.ItemToDisable.Sentry
            };
        }
    }
}
