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
            if (apparel.Wearer == null) return false;
            if (!ApparelDamageVisualsMod.Settings.AllowColonists && apparel.Wearer.IsColonist) return false;
            if (!ApparelDamageVisualsMod.Settings.AllowAlive && !apparel.Wearer.Dead) return false;
            List<BodyPartGroupDef> defs = apparel.def.apparel.bodyPartGroups;
            bool validateParts = defs.Contains(BodyPartGroupDefOf.Torso) || defs.Contains(BodyPartGroupDefOf.FullHead) ||
                defs.Contains(BodyPartGroupDefOf.Legs);
            return apparel.def.useHitPoints && validateParts;
        }

        public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref bool __result, ref ApparelGraphicRecord rec)
        {
            if (ValidateApparel(apparel))
            {
                rec.graphic = new Graphic_TornWrapper(rec.graphic, apparel);
            }
             
  
        }
    }
}
