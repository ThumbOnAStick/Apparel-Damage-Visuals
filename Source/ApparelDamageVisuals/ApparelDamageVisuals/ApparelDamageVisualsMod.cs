using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals
{
    public class ApparelDamageVisualsMod : Mod
    {

        private readonly ADVSettings settings;

        public static ApparelDamageVisualsMod Instance { get; private set; }

        // Safe global accessor even if called before the ctor ran.
        public static ADVSettings Settings =>
            Instance != null
                ? Instance.settings
                : LoadedModManager.GetMod<ApparelDamageVisualsMod>().GetSettings<ADVSettings>();

        public ApparelDamageVisualsMod(ModContentPack content) : base(content)
        {
            Instance = this;
            settings = GetSettings<ADVSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DrawWindowsSettings(inRect);
        }

        public override string SettingsCategory()
        {
            return "ADV.ADVMod".Translate();
        }


    }
}
