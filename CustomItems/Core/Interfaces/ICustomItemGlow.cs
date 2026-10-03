using UnityEngine;

// This code is derived from SnivysUltimatePackage
// Copyright © SnivyFilms
// Licensed under the GNU Affero General Public License v3.0
//
// Original source:
// https://github.com/SnivyFilms/SnivysUltimatePackage
namespace CustomItems.Core.Interfaces
{
    public interface ICustomItemGlow
    {
        public bool HasCustomItemGlow { get; set; }
        
        public Color CustomItemGlowColor { get; set; }
        
        public float GlowRange { get; set; }
        
        public float GlowIntensity { get; set; }
        
        public GlowShadowType ShadowType { get; set; }
        
        public Vector3 GlowOffset { get; set; }
        
        public enum GlowShadowType
        {
            None,
            Hard,
            Soft
        }
    }
}