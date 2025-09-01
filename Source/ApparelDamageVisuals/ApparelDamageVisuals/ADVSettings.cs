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

        public float MaxCameraZoom => (float)maxCameraZoom;

        private int maxCameraZoom;  

        public ADVSettings()
        {
            maxCameraZoom = 7;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref maxCameraZoom, "minCameraZoom", 5);
        }

        public void DrawWindowsSettings(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            maxCameraZoom = (int)listing_Standard.SliderLabeled("ADV.MaxCameraZoom".Translate(maxCameraZoom), maxCameraZoom, 3, 10, 0.5f , "ADV.MaxCameraZoom.Tooltip".Translate());
            listing_Standard.End();
            this.Write();

        }

    }
}
