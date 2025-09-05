using ApparelDamageVisuals.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals.ADVGraphics
{
    public class Graphic_TornWrapper : Graphic
    {
        private readonly Graphic inner;
        private readonly Thing targetThing;
        private readonly Dictionary<Rot4, TornApparelRotDrawer> drawers = new Dictionary<Rot4, TornApparelRotDrawer>();
        private readonly int seed;
        private float durabilityCached;
        private bool isArmor;

        public Graphic_TornWrapper(Graphic inner, Thing targetThing, bool isVertical = false, bool drawLine = false)
        {
            if (inner == null)
            {
                return;
            }

            if (targetThing == null)
            {
                Log.Warning("ADV: Graphic_TornWrapper received null targetThing. Skipping wrapper creation.");
                return;
            }

            this.inner = inner;
            this.targetThing = targetThing;
            this.seed = targetThing.thingIDNumber;
            this.data = inner.data;
            this.color = inner.color;
            this.colorTwo = inner.colorTwo;
            if (targetThing!= null && targetThing.def.tradeTags != null && targetThing.def.tradeTags.Contains("Armor"))
            {
                this.isArmor = true;
            }
        }

        public override string ToString() => $"Graphic_TornWrapper({inner})";

        public override Material MatSingle => inner?.MatSingle;

        float Durability => (float)this.targetThing.HitPoints/(float)this.targetThing.MaxHitPoints;

        SimpleCurve MaxHoleCountCurve()
        {
            var curve = new SimpleCurve();
            curve.Add(0f, 20f);
            curve.Add(1f, 0f);
            return curve;
        }

        IntRange HoleCount()
        {
            var max = (int)(MaxHoleCountCurve().Evaluate(this.durabilityCached));
            var min = Math.Max(max - 5, 0);
            return new IntRange(min, max);
        }

    

        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            if(thing == null || inner == null)
            {
                return base.MatAt(rot, thing);
            }
            try
            {
                durabilityCached = Durability;
                if (!drawers.TryGetValue(rot, out TornApparelRotDrawer drawer))
                {
                    if (isArmor)
                        drawers[rot] = drawer = new TornArmorRotDrawer();
                    else
                        drawers[rot] = drawer = new TornApparelRotDrawer();
                }
                var baseMat = inner.MatAt(rot, thing);
                return drawer.GetMaterial(baseMat, seed, this.HoleCount(), Durability);
            }
            catch (Exception e)
            {
                string thingName = thing != null ? thing.ThingID : "None";
                Log.Error($"ADV: failed to draw torn apparel mat for {thingName}, stacktrace: {e}");
            }

            return base.MatAt(rot, thing);
        }

        protected override void DrawMeshInt(Mesh mesh, Vector3 loc, Quaternion quat, Material mat)
        {
            base.DrawMeshInt(mesh, loc, quat, mat);
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            inner?.DrawWorker(loc, rot, thingDef, thing, extraRotation);
        }
    }
}
