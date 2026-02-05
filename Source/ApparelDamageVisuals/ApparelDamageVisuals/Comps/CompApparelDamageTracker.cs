using ApparelDamageVisuals.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals.Comps
{
    public class CompApparelDamageTracker : ThingComp
    {
        private DamageDef mostRecentDamageDef;

        public CompApparelDamageTracker()
        {
            mostRecentDamageDef = DamageDefOf.Cut;
        }
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            if (dinfo.Def == DamageDefOf.Deterioration) return;
            mostRecentDamageDef = dinfo.Def;
        }

        public DamageDef mostRecentDamage
        {
            get
            {
                return this.mostRecentDamage;
            }
        }


        public Color ApparelDamageColor
        {
            get
            {
                if (mostRecentDamageDef == DamageDefOf.AcidBurn) return Color.green;
                if (mostRecentDamageDef == DamageDefOf.Bullet ||
                    mostRecentDamageDef == DamageDefOf.Bomb ||
                    mostRecentDamageDef == DamageDefOf.Crush ||
                    mostRecentDamageDef == DamageDefOf.Burn ||
                    mostRecentDamageDef == DamageDefOf.Flame) return Color.black;
                return Color.grey;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref mostRecentDamageDef, "mostRecentDamageDef");
        }
    }
}
