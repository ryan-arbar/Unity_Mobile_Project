﻿using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    [CreateAssetMenu(fileName = "LA Leg Motion Settings", menuName = "FImpossible Creations/Legs Animator/Leg Motion Preset", order = 0)]
    public class LegMotionSettingsPreset : ScriptableObject
    {
        [Header("Settings for single leg - leg animation style")]
        public LegsAnimator.LegStepAnimatingParameters Settings;

        private void Reset()
        {
            Settings = new LegsAnimator.LegStepAnimatingParameters();
            Settings.RefreshDefaultCurves();
        }
    }
}