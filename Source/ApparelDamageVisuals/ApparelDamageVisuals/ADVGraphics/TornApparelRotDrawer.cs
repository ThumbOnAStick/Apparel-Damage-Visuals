using ApparelDamageVisuals.ADVShader;
using ApparelDamageVisuals.Utils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace ApparelDamageVisuals.ADVGraphics
{
    /// <summary>
    /// Drawer for broken clothes
    /// </summary>
    internal class TornApparelRotDrawer
    {
        float durabilityCached;
        Material materialCached;
        protected enum EdgeDirection {North, South, East, West}

        public TornApparelRotDrawer()
        {
            durabilityCached = 0;
        }

        bool CanRenderNow()
        {
            return ApparelDamageVisualsMod.Settings.MaxCameraZoom > Find.Camera.orthographicSize;
        }

        public Material GetMaterial(Material baseMat, float durability)
        {

            // Use cached material as piority
            if (this.materialCached != null && durabilityCached == durability)
            {
                return this.materialCached;
            }

            // Avoid calling in main thread or when camera is too far away
            if (!UnityData.IsInMainThread || !CanRenderNow())
            {
                return baseMat;
            }


            this.durabilityCached = durability;
            // Render Torn Material on main thread only
            return this.materialCached = BuildTornMaterial(baseMat, durability);
        }

        protected void CopyIfPresent(Material src, Material dst, string prop)
        {
            if (src.HasProperty(prop)) dst.SetColor(prop, src.GetColor(prop));
        }

        protected Material BuildTornMaterial(Material baseMat, float durability)
        {
            try
            {
                if (durability > 0.7f) return baseMat;
                var newMat = new Material(baseMat)
                {
                    shader = ADVContentDatabase.TestUnlitShader
                };
                Texture2D mask = ADVContentDatabase.MaskLv1;
                if (durability < 0.5 && durability > 0.2)
                {
                    mask = ADVContentDatabase.MaskLv2;
                }
                else if (durability <= 0.2)
                {
                    mask = ADVContentDatabase.MaskLv3;
                }
                newMat.SetTexture("_Mask", mask);
                return newMat;
            }
            catch (Exception ex)
            {
                ADVLogger.ADVError(ex.ToString());
            }

            return baseMat;
        }

        protected FloatRange HoleSize(float scale)
        {
            var minSize = (1 - this.durabilityCached) * (scale / 7);
            var maxSize = minSize * 1.5f;
            return new FloatRange(minSize, maxSize);
        }
    }
}
