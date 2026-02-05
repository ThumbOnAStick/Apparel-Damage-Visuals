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

        // Add this property to check if wrapper initialized correctly
        public bool IsValid => inner != null && targetThing != null;

        public Graphic_TornWrapper(Graphic inner, Thing targetThing, bool isVertical = false, bool drawLine = false)
        {
            // Store inner even if null so we can check IsValid
            this.inner = inner;
            this.targetThing = targetThing;

            if (inner == null || targetThing == null)
            {
                return;
            }

            this.seed = targetThing.thingIDNumber;
            this.data = inner.data;
            this.color = inner.color;
            this.colorTwo = inner.colorTwo;
            if (targetThing.def.tradeTags != null && targetThing.def.tradeTags.Contains("Armor"))
            {
                this.isArmor = true;
            }
        }

        public override string ToString() => $"Graphic_TornWrapper({inner})";

        // Return inner's material, fallback to base only if inner exists
        public override Material MatSingle => inner != null ? inner.MatSingle : null;

        float Durability => targetThing != null && targetThing.MaxHitPoints > 0
            ? (float)targetThing.HitPoints / targetThing.MaxHitPoints
            : 1f;

  
        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            if (thing == null || inner == null)
            {
                return base.MatAt(rot, thing);
            }
            try
            {
                durabilityCached = Durability;
                if (!drawers.TryGetValue(rot, out TornApparelRotDrawer drawer))
                {
                    drawers[rot] = drawer = new TornApparelRotDrawer();
                }
                var baseMat = inner.MatAt(rot, thing);
                return drawer.GetMaterial(baseMat, Durability);
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