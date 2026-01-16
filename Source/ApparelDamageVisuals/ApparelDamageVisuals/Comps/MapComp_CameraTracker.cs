using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using Verse;

namespace ApparelDamageVisuals.Comps
{
    internal class MapComp_CameraTracker : MapComponent
    {
        readonly string cameraP = "brrainz.cameraplus";
        readonly string cameraS = "ray1203.SimpleCameraSetting";

        public MapComp_CameraTracker(Map map) : base(map)
        {

        }

        bool AreCameraModsLoaded()
        {
            return ModsConfig.IsActive(cameraP) || ModsConfig.IsActive(cameraS);
        }

        public override void MapGenerated()
        {
            base.MapGenerated();
            if (!AreCameraModsLoaded())
            {
                Find.WindowStack.Add(new Dialog_MessageBox("ADV.Warning1".Translate(cameraP, cameraS)));
            }
        }
    }
}
