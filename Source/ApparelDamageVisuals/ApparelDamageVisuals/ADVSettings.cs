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
        private float size;
        private int thickness;
        private bool allowColonists;
        private bool allowAlive;
        private bool antialiasing;
        private bool outline;

        public float Size => size;
        public float MaxCameraZoom => (float)maxCameraZoom;
        public int Thickness => thickness;
        public bool AllowColonists => allowColonists;
        public bool AllowAlive => allowAlive;
        public bool Antialiasing => antialiasing;
        public bool Outline => outline;

        public ADVSettings()
        {
            maxCameraZoom = 15;
            thickness = 3;
            size = 1;
            allowAlive = true;
            allowColonists = true;
            antialiasing = true;
            outline = true;

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref maxCameraZoom, "minCameraZoom", 5);
            Scribe_Values.Look(ref thickness, "thickness", 3);
            Scribe_Values.Look(ref size, "size", 1.0f);
            Scribe_Values.Look(ref allowColonists, "allowColonists", true);
            Scribe_Values.Look(ref allowAlive, "allowAlive", true);
            Scribe_Values.Look(ref antialiasing, "antialiasing", true);
            Scribe_Values.Look(ref outline, "outline", true);


        }

        public void DrawWindowsSettings(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            maxCameraZoom = (int)listing_Standard.SliderLabeled("ADV.MaxCameraZoom".Translate(maxCameraZoom), maxCameraZoom, 3, 20, 0.5f, "ADV.MaxCameraZoom.Tooltip".Translate());
            var sizeUnNormalized = listing_Standard.SliderLabeled("ADV.Zize".Translate(((double)size).ToString()), size, 0.5f, 3.0f, 0.5f);
            size = Mathf.Round(sizeUnNormalized / 0.25f) * 0.25f;   // Snap to 0.25 
            listing_Standard.GapLine();
            listing_Standard.CheckboxLabeled("ADV.AllowColonists".Translate(), ref this.allowColonists, 1);
            listing_Standard.CheckboxLabeled("ADV.AllowAlive".Translate(), ref this.allowAlive, 1);
            listing_Standard.GapLine();
            listing_Standard.CheckboxLabeled("ADV.Antialiasing".Translate(), ref this.antialiasing, 1);
            if (this.antialiasing)
            {
                listing_Standard.CheckboxLabeled("ADV.Outline".Translate(), ref this.outline, 1);
                thickness = (int)listing_Standard.SliderLabeled("ADV.Thickness".Translate(thickness), thickness, 1, 5, 0.5f);

            }

            listing_Standard.End();
            this.Write();

        }

    }
}
