using ApparelDamageVisuals.ADVShader;
using ApparelDamageVisuals.Comps;
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

        public Material GetMaterial(Material baseMat, float durability, Thing apparel)
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
            return this.materialCached = BuildTornMaterial(baseMat, durability, apparel);
        }

        protected void CopyIfPresent(Material src, Material dst, string prop)
        {
            if (src.HasProperty(prop)) dst.SetColor(prop, src.GetColor(prop));
        }

        protected Material BuildTornMaterial(Material baseMat, float durability, Thing apparel)
        {
            try
            {
                if (durability > 0.7f) return baseMat;
                var newMat = new Material(baseMat)
                {
                    shader = ADVContentDatabase.TestUnlitShader
                };

                // Decide mask
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

                // Decide damage layer color
                Color damageLayerColor = Color.grey;
                CompApparelDamageTracker apparelDamageComp = null;
                if (apparel != null)
                apparelDamageComp = apparel.TryGetComp<CompApparelDamageTracker>();
                if (apparelDamageComp != null)
                {
                    damageLayerColor = apparelDamageComp.ApparelDamageColor;
                    //ADVLogger.Message($"Current damge: {apparelDamageComp.mostRecentDamage}");
                }
                newMat.SetColor("_DamageLayerColor", damageLayerColor);

                return newMat;
            }
            catch (Exception ex)
            {
                ADVLogger.Error(ex.ToString());
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
