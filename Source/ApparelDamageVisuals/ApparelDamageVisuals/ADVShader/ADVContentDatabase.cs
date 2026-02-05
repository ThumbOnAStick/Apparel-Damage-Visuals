using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ApparelDamageVisuals.ADVShader
{
    [StaticConstructorOnStartup]
    internal class ADVContentDatabase
    {
        private static AssetBundle bundleInt;
        private static Dictionary<string, Shader> _lookupShaders;
        public static readonly Shader TestUnlitShader = LoadShader(Path.Combine("Assets", "testunlit.shader"));
        public static readonly Texture2D MaskLv1 = ContentFinder<Texture2D>.Get("ADVMaskSlightlyWornout1");
        public static readonly Texture2D MaskLv2 = ContentFinder<Texture2D>.Get("ADVMaskHeavilyWornout1");
        public static readonly Texture2D MaskLv3 = ContentFinder<Texture2D>.Get("ADVMaskBearlyholding1");
        public static AssetBundle CbBundle
        {
            get
            {
                if (bundleInt == null)
                {
                    bundleInt = ApparelDamageVisualsMod.Instance.MainBundle;
                }
                return bundleInt;
            }
        }

        private static Shader LoadShader(string shaderName)
        {
            if(_lookupShaders == null) _lookupShaders = new Dictionary<string, Shader>();

            if (!_lookupShaders.ContainsKey(shaderName))
            {
                _lookupShaders[shaderName] = CbBundle.LoadAsset<Shader>(shaderName);
            }
            Shader shader = _lookupShaders[shaderName];

            if (shader != null) return shader;
            Log.Warning("[ADV] Could not load shader: " + shaderName);
            return ShaderDatabase.DefaultShader;
        }
    }

    
       
    }
