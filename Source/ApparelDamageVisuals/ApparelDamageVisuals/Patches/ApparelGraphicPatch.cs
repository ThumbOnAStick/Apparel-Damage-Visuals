using ApparelDamageVisuals.ADVGraphics;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApparelDamageVisuals.Patches
{
    public class ApparelGraphicPatch
    {
        public static void PatchHarmony()
        {
            MethodInfo original = AccessTools.Method(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel");
            HarmonyMethod postfix = new HarmonyMethod(typeof(ApparelGraphicPatch).GetMethod("Postfix"));
            MetaPatches.harmony.Patch(original, null, postfix);
        }

        static bool ValidateApparel(Apparel apparel)
        {
            List<BodyPartGroupDef> defs = apparel.def.apparel.bodyPartGroups;
            bool validateParts = defs.Contains(BodyPartGroupDefOf.Torso) || defs.Contains(BodyPartGroupDefOf.FullHead);
            return apparel.Wearer != null && apparel.def.useHitPoints && validateParts; 
        }

        public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref bool __result, ref ApparelGraphicRecord rec)
        {
            if (!ValidateApparel(apparel))
                return;
            if (apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso))
            {
                rec.graphic = new Graphic_TornWrapper(rec.graphic, apparel);
            }
        }
    }
}
