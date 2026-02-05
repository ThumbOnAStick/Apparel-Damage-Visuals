using ApparelDamageVisuals.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApparelDamageVisuals.Patches
{
    [StaticConstructorOnStartup]
    internal class MetaPatches
    {
        public static Harmony harmony;

        static MetaPatches()
        {
            Harmony harmony;
            if ((harmony = MetaPatches.harmony) == null)
            {
                harmony = (MetaPatches.harmony = new Harmony("thumb.APV"));
            }
            try
            {
                MetaPatches.harmony = harmony;
                ApparelGraphicPatch.PatchHarmony();
            }
            catch (Exception e)
            {
                Dialog_MessageBox box = new Dialog_MessageBox($"APV: an error occured while trying to patch harmony, you can report this bug on bug report thread. {e}");
                Find.WindowStack.Add(box);
            }

            ADVLogger.Message("Harmony patches were successful");
        }
    }
}
