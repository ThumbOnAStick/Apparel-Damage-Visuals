using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ApparelDamageVisuals.Utils
{
    public static class ADVLogger
    {
        static readonly string tag = "[ADV]";
        public static void Message(string message)
        {
            Log.Message(tag + message);
        }

        public static void Error(string message)
        {
            Log.Error(tag + message);
        }
    }
}
