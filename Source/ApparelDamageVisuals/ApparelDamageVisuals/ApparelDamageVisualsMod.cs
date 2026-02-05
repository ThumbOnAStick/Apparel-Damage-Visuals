using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        public static ADVSettings Settings =>
            Instance != null
                ? Instance.settings
                : LoadedModManager.GetMod<ApparelDamageVisualsMod>().GetSettings<ADVSettings>();

        public AssetBundle MainBundle
        {
            get
            {
                string text = "";

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    text = "StandaloneOSX";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    text = "StandaloneWindows64";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    text = "StandaloneLinux64";
                }

                string bundlePath = Path.Combine(base.Content.RootDir, "Materials\\Bundles\\" + text + "\\testunlit");
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

                if (bundle == null)
                {
                    Log.Error("[ADV] Failed to load bundle at path: " + bundlePath);
                }
                return bundle;
            }
        }

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
