﻿using System;
using System.Globalization;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public partial class LegsAnimator
    {
        public LegStepAnimatingParameters BaseLegAnimating = new LegStepAnimatingParameters();
        /// <summary> For future leg motion presets </summary>
        public LegStepAnimatingParameters LegAnimatingSettings { get { return BaseLegAnimating; } }


        /// <summary>
        /// Step Animating is used by leg movement during gluing process
        /// </summary>
        [System.Serializable]
        public class LegStepAnimatingParameters
        {
            [Tooltip("Average duration of the automatic leg animation")]
            [Range(0.1f, 1f)] public float StepMoveDuration = .375f;
            [Tooltip("Curve of ik point going towards desired position (just XZ movement, to Y - no leg rise curve)")]
            [FPD_FixedCurveWindow(0f, 0f, 1f, 1.25f, .4f, .5f, 1f)]
            public AnimationCurve MoveToGoalCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            [Tooltip("Making foot motion move towards target not in full linear straight towards target motion but adding a bit curve back (positive value) or forward (negative values) making movement a bit more natural")]
            [FPD_FixedCurveWindow(0f, -1f, 1f, 1f, .4f, .6f, .9f)] public AnimationCurve SpherizeTrack = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f);
            [Range(0f,2f)] public float SpherizePower = 0.3f;


            [Tooltip("Minimum leg raise height. If distance of target step animation is small, then foot raise is smaller - down to this minimum raise value.")]
            [Range(0f, 1f)] public float MinFootRaise = .1f;
            [Tooltip("Maximum leg raise height. If distance of target step animation is very big, then foot raise is bigger - up to this maximum raise value.")]
            [Range(0f, 1f)] public float MaxFootRaise = .4f;
            [Tooltip("Raise height step animation curve evaluated on step animation duration.")]
            [FPD_FixedCurveWindow(0f, 0f, 1f, 1f, .5f, 1f, .5f)]
            public AnimationCurve RaiseYAxisCurve;


            [Space(3)]
            [Tooltip("Allowing to speed up leg adjusting animation when leg is getting stretched, when opposite leg is requesting adjustement or when main character is rotating in place quickly")]
            [Range(0f, 1f)] public float AllowSpeedups = 0.4f;
            [Tooltip("You can allow to use opposite leg before idle glue leg adjustement finishes")]
            [Range(0.1f,1f)] public float AllowDetachBefore = 1f;


            // Expert Curves

            [Tooltip("Extra hips push power animation curve evaluated on step animation duration.")]
            [FPD_FixedCurveWindow(0f, 0f, 1f, 1f, 1f, .6f, .6f)]
            public AnimationCurve PushHipsOnMoveCurve;

            [Tooltip("Extra foot ankle rotation animation curve evaluated on step animation duration.")]
            [FPD_FixedCurveWindow(0f, -1f, 1f, 1f)]
            public AnimationCurve FootRotationCurve;



            #region Curves Definition


            public void RefreshDefaultCurves()
            {
                //LogCurve("FootRotationCurve", FootRotationCurve); return;
                Curves_RefreshMoveToGoalCurve();
                Curves_RefreshRaiseYAxisCurve();
                Curves_RefreshSpherizeTrack();
                Curves_RefreshFootRotationCurve();
                Curves_RefreshPushHipsOnMoveCurve();
            }

            public void Curves_RefreshRaiseYAxisCurve()
            {
                RaiseYAxisCurve = new AnimationCurve();
                RaiseYAxisCurve.AddKey(new Keyframe(0f, 0f, 0.8504464f, 0.8504464f, 0f, 0.6517575f));
                RaiseYAxisCurve.AddKey(new Keyframe(0.2731183f, 0.45f, 0.9770691f, 0.9770691f, 0.3333333f, 0.3387407f));
                RaiseYAxisCurve.AddKey(new Keyframe(0.505118f, 0.5f, -0.2710344f, -0.2710344f, 0.3333333f, 0.3333333f));
                RaiseYAxisCurve.AddKey(new Keyframe(0.9110107f, 0f, -0.1500788f, -0.1500788f, 0.5409704f, 0f));
            }

            public void Curves_RefreshRaiseYAxisCurveSpiderPreset()
            {
                RaiseYAxisCurve = new AnimationCurve();
                RaiseYAxisCurve.AddKey(new Keyframe(0f, 0f, 0.8504464f, 0.8504464f, 0f, 0.6517575f));
                RaiseYAxisCurve.AddKey(new Keyframe(0.2731183f, 0.45f, 0.9770691f, 0.9770691f, 0.3333333f, 0.3387407f));
                RaiseYAxisCurve.AddKey(new Keyframe(0.5943514f, 0.7946472f, -0.2710344f, -0.2710344f, 0.3333333f, 0.3333333f));
                RaiseYAxisCurve.AddKey(new Keyframe(1f, 0f, -0.1500788f, -0.1500788f, 0.5409704f, 0f));
            }

            public void Curves_RefreshMoveToGoalCurve()
            {
                MoveToGoalCurve = new AnimationCurve();
                MoveToGoalCurve.AddKey(new Keyframe(0, 0, 0, 0, 0, 0.1842105f));
                MoveToGoalCurve.AddKey(new Keyframe(0.4885197f, 0.8972011f, 1.38764f, 1.38764f, 0.3333333f, 0.3333333f));
                MoveToGoalCurve.AddKey(new Keyframe(1, 1, 0, 0, 0, 0));
            }

            public void Curves_RefreshFootRotationCurve()
            {
                FootRotationCurve = new AnimationCurve();
                FootRotationCurve.AddKey(new Keyframe(0f, 0f, 0.5764588f, 0.5764588f, 0f, 0.4956417f));
                FootRotationCurve.AddKey(new Keyframe(0.4378169f, 0.2035736f, -0.2411275f, -0.2411275f, 0.3333333f, 0.4033037f));
                FootRotationCurve.AddKey(new Keyframe(0.7841034f, -0.1339308f, 0.3331003f, 0.3331003f, 0.3333333f, 0.3333333f));
                FootRotationCurve.AddKey(new Keyframe(1f, 0f, 0.3498169f, 0.3498169f, 0.5534658f, 0f));
            }

            public void Curves_RefreshPushHipsOnMoveCurve()
            {
                PushHipsOnMoveCurve = new AnimationCurve();

                PushHipsOnMoveCurve.AddKey(new Keyframe(0f, 0f, 5.630541f, 5.630541f, 0f, 0.198735f));
                PushHipsOnMoveCurve.AddKey(new Keyframe(0.383f, 0.3733972f, -0.1664574f, -0.1664574f, 0.333f, 0.2940554f));
                PushHipsOnMoveCurve.AddKey(new Keyframe(0.7075226f, 0.1460427f, -1.565806f, -1.565806f, 0.3605607f, 0.3446763f));
                PushHipsOnMoveCurve.AddKey(new Keyframe(1f, 0f, 0f, 0f, 0.09374858f, 0f));
            }
                                    
            public void Curves_RefreshPushHipsOnMoveCurveSpiderPreset()
            {
                PushHipsOnMoveCurve = new AnimationCurve();

                PushHipsOnMoveCurve.AddKey(new Keyframe(0f, 0f, 5.630541f, 5.630541f, 0f, 0.198735f));
                PushHipsOnMoveCurve.AddKey(new Keyframe(0.320017f, 0.654645f, -0.1664574f, -0.1664574f, 0.333f, 0.2940554f));
                PushHipsOnMoveCurve.AddKey(new Keyframe(0.6681702f, 0.2174691f, -1.565806f, -1.565806f, 0.3605607f, 0.3446763f));
                PushHipsOnMoveCurve.AddKey(new Keyframe(1f, 0f, 0f, 0f, 0.09374858f, 0f));
            }
            
            public void Curves_RefreshSpherizeTrack()
            {
                SpherizeTrack = new AnimationCurve();
                SpherizeTrack.AddKey(new Keyframe(0f, 0f, 0.6958197f, 0.6958197f, 0f, 0.460011f));
                SpherizeTrack.AddKey(new Keyframe(0.4f, 0.3f, -0.04204308f, -0.04204308f, 0.333f, 0.3410656f));
                SpherizeTrack.AddKey(new Keyframe(0.85f, 0f, -0.2721428f, -0.2721428f, 0.3953607f, 0f));
            }


            public void LogCurve(string name, AnimationCurve c)
            {
                string log = "";
                IFormatProvider prov = CultureInfo.InvariantCulture;
                for (int i = 0; i < c.keys.Length; i++)
                {
                    var key = c.keys[i];
                    log += "\n"+ name+".AddKey(new Keyframe(" + key.time.ToString(prov) + "f, " + key.value.ToString(prov) + "f, " + key.inTangent.ToString(prov) + "f, " + key.outTangent.ToString(prov) + "f, " + key.inWeight.ToString(prov) + "f, " + key.outWeight.ToString(prov) + "f));";
                }

                Debug.Log(log);
            }

            #endregion


        }

    }
}