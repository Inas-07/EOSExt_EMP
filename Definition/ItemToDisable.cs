using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool Map { set; get; } = true;

        public override bool Equals(object obj)
        {
            return obj is ItemToDisable disable &&
                   BioTracker == disable.BioTracker &&
                   PlayerHUD == disable.PlayerHUD &&
                   PlayerFlashLight == disable.PlayerFlashLight &&
                   EnvLight == disable.EnvLight &&
                   GunSight == disable.GunSight &&
                   Sentry == disable.Sentry &&
                   Map == disable.Map;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BioTracker, PlayerHUD, PlayerFlashLight, EnvLight, GunSight, Sentry, Map);
        }

        public override string ToString()
        {
            return $"ItemToDisable:\n" +
                $"BioTracker: {BioTracker}, PlayerHUD: {PlayerHUD}, PlayerFlashLight: {PlayerFlashLight},\n" +
                $"EnvLight: {EnvLight}, GunSight: {GunSight}, Sentry: {Sentry}, Map: {Map}";
        }
    }
}
