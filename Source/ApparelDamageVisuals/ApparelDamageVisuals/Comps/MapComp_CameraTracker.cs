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
        public MapComp_CameraTracker(Map map) : base(map)
        {
        }

        public override void MapComponentOnGUI()
        {
            // Handle F11 key press to set camera zoom
            if (Event.current.type == EventType.Used && Event.current.keyCode == KeyCode.F11)
            {
                Find.CameraDriver.SetRootPosAndSize(Find.CameraDriver.MapPosition.ToVector3(), 20f);
                Event.current.Use(); // Consume the event
            }
        }
    }
}
