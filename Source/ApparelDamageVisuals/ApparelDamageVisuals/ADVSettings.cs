using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using Verse;

namespace ApparelDamageVisuals
{
    public class ADVSettings : Verse.ModSettings
    {
        private int maxCameraZoom;
        private bool allowAlive;
        private float holeSize = 1;
        private float threshold1 = 0.7f;
        private float threshold2 = 0.5f;
        private float threshold3 = 0.2f;

        public float MaxCameraZoom => (float)maxCameraZoom;
        public bool AllowAlive => allowAlive;

        public float HoleSize => holeSize;

        public ADVSettings()
        {
            maxCameraZoom = 15;
            allowAlive = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref maxCameraZoom, "minCameraZoom", 5);
            Scribe_Values.Look(ref allowAlive, "allowAlive", true);


        }

        public void DrawWindowsSettings(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            maxCameraZoom = (int)listing_Standard.SliderLabeled("ADV.MaxCameraZoom".Translate(maxCameraZoom), maxCameraZoom, 3, 20, 0.5f, "ADV.MaxCameraZoom.Tooltip".Translate());
            listing_Standard.CheckboxLabeled("ADV.AllowAlive".Translate(), ref this.allowAlive, 1);
            this.holeSize = (float)Math.Round((double)listing_Standard.SliderLabeled("ADV.Zize".Translate(this.holeSize.ToString()), this.holeSize, 0.1f, 1f), 2);  
            listing_Standard.End();
            this.Write();

        }

    }
}
