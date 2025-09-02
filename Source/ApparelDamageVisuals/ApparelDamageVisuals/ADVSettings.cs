using CameraPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals
{
    public class ADVSettings : Verse.ModSettings
    {
        private int maxCameraZoom;  
        private bool allowColonists;
        private bool allowAlive;

        public float MaxCameraZoom => (float)maxCameraZoom;
        public bool AllowColonists => allowColonists;
        public bool AllowAlive => allowAlive;

        public ADVSettings()
        {
            maxCameraZoom = 15;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref maxCameraZoom, "minCameraZoom", 5);
            Scribe_Values.Look(ref allowColonists, "allowColonists", true);
            Scribe_Values.Look(ref allowAlive, "allowAlive", true);

        }

        public void DrawWindowsSettings(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            maxCameraZoom = (int)listing_Standard.SliderLabeled("ADV.MaxCameraZoom".Translate(maxCameraZoom), maxCameraZoom, 3, 20, 0.5f , "ADV.MaxCameraZoom.Tooltip".Translate());
            listing_Standard.CheckboxLabeled("ADV.AllowColonists".Translate(), ref this.allowColonists, 1);
            listing_Standard.CheckboxLabeled("ADV.AllowAlive".Translate(), ref this.allowAlive, 1);
            listing_Standard.End();
            this.Write();

        }

    }
}
