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


        public override void MapComponentOnGUI()
        {
            // Handle F11 key press to set camera zoom
            if (Event.current.type == EventType.Used && Event.current.keyCode == KeyCode.F11)
            {
                Find.CameraDriver.SetRootPosAndSize(Find.CameraDriver.MapPosition.ToVector3(), 11f);
                Event.current.Use(); // Consume the event
            }
        }
    }
}
