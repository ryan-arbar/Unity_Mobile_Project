using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FIMSpace.FProceduralAnimation
{
    //[CreateAssetMenu(fileName = "LAM_InsectLegsAnimatingHelper", menuName = "FImpossible Creations/Legs Animator/Control Module - Insect Legs Animating Helper", order = 2)]
    public class LAM_InsectLegsHelper : LegsAnimatorControlModuleBase
    {
        LegsAnimator.Variable _onOneSideV;
        LegsAnimator.Variable _onStepCulldownV;
        LegsAnimator.Variable _legSideCulldownV;
        LegsAnimator.Variable _afterFullCulldownV;

        readonly string minSideS = "Minimum Standing Legs On One Side";
        readonly string stepculldS = "On Step Culldown";
        readonly string sideculldV = "Leg Side Culldown";
        readonly string waitV = "On Full Attach Culldown";

        float mainCulldown = 0f;
        float sideLCulldown = 0f;
        float sideRCulldown = 0f;


        private List<LegHelper> legHelpers = null;
        public override void OnInit(LegsAnimator.LegsAnimatorCustomModuleHelper hlp)
        {
            _onOneSideV = hlp.RequestVariable(minSideS, 2);
            _onStepCulldownV = hlp.RequestVariable(stepculldS, 0.025f);
            _legSideCulldownV = hlp.RequestVariable(sideculldV, 0.015f);
            _afterFullCulldownV = hlp.RequestVariable(waitV, 0f);

            legHelpers = new List<LegHelper>();

            for (int l = 0; l < LA.Legs.Count; l++)
            {
                LegHelper helper = new LegHelper(LA.Legs[l]);
                legHelpers.Add(helper);
            }

            if (_onOneSideV.GetInt() >= LA.Legs.Count) _onOneSideV.SetValue(LA.Legs.Count / 2);
        }

        #region Leg Helper

        class LegHelper
        {
            public bool WasAttaching = false;
            public LegsAnimator.Leg legRef { get; private set; }
            public float FullyAttachedAt = -1f;

            public LegHelper(LegsAnimator.Leg leg)
            {
                legRef = leg;
                WasAttaching = false;
                FullyAttachedAt = -1f;
            }
        }

        bool AllowDetach(LegHelper leg)
        {
            if (mainCulldown > 0) return false;

            if (leg.legRef.Side == LegsAnimator.ELegSide.Left) { if (sideLCulldown > 0) return false; }
            else if (leg.legRef.Side == LegsAnimator.ELegSide.Right) { if (sideRCulldown > 0) return false; }

            if (_onOneSideV.GetFloat() > 0)
            {
                int standing = 0;

                for (int l = 0; l < legHelpers.Count; l++)
                {
                    var ol = legHelpers[l].legRef;

                    if (ol.Side != leg.legRef.Side) continue;
                    if ((!ol.G_DuringAttaching || ol.G_Attached)
                        /*|| ol.G_AttachPossible*/ /* preventing nervous legs on slopes without ground detected beneath */ )
                        standing += 1;
                }

                if (standing < _onOneSideV.GetFloat()) return false;
            }

            if (Time.time - leg.FullyAttachedAt < _afterFullCulldownV.GetFloat()) return false;

            return true;
        }

        #endregion


        public override void OnPreLateUpdate(LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {
            if (legHelpers == null) return;

            mainCulldown -= LA.DeltaTime;
            sideLCulldown -= LA.DeltaTime;
            sideRCulldown -= LA.DeltaTime;

        }

        public override void Leg_LateUpdate(LegsAnimator.LegsAnimatorCustomModuleHelper hlp, LegsAnimator.Leg leg)
        {
            LegHelper helper = legHelpers[leg.PlaymodeIndex];

            if (leg.G_DuringAttaching)
            {
                // On leg step up
                if (helper.WasAttaching == false)
                {
                    mainCulldown = _onStepCulldownV.GetFloat();
                    if (leg.Side == LegsAnimator.ELegSide.Left) sideRCulldown = _legSideCulldownV.GetFloat();
                    else if (leg.Side == LegsAnimator.ELegSide.Right) sideLCulldown = _legSideCulldownV.GetFloat();
                }
            }

            if (leg.G_Attached) { if (helper.FullyAttachedAt == -1) helper.FullyAttachedAt = Time.time; }
            else helper.FullyAttachedAt = -1f;

            helper.legRef.G_CustomForceNOTDetach = !AllowDetach(helper);
            helper.WasAttaching = leg.G_DuringAttaching;
        }


        #region Editor Code

#if UNITY_EDITOR

        public override void Editor_OnSceneGUI(LegsAnimator legsAnimator, LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {
            if (!Application.isPlaying) return;
            if (legHelpers == null) return;

            for (int l = 0; l < legHelpers.Count; l++)
            {
                if (AllowDetach(legHelpers[l])) UnityEditor.Handles.color = Color.red;
                else UnityEditor.Handles.color = Color.green;

                UnityEditor.Handles.SphereHandleCap(0, legHelpers[l].legRef._PreviousFinalIKPos, Quaternion.identity, legsAnimator.ScaleReference * 0.07f, EventType.Repaint);
            }
        }

        public override void Editor_InspectorGUI(LegsAnimator legsAnimator, LegsAnimator.LegsAnimatorCustomModuleHelper helper)
        {

            EditorGUIUtility.labelWidth = 220;
            EditorGUILayout.HelpBox("Better leg controll for multiple legs creatures.", MessageType.Info);
            GUILayout.Space(5);

            var legsOnSideV = helper.RequestVariable(minSideS, 2);
            legsOnSideV.Editor_DisplayVariableGUI();
            EditorGUIUtility.labelWidth = 0;

            var OnStepCulldowneV = helper.RequestVariable(stepculldS, 0.025f);
            OnStepCulldowneV.SetMinMaxSlider(0f, 0.15f);
            OnStepCulldowneV.Editor_DisplayVariableGUI();

            var LegSideCulldownV = helper.RequestVariable(sideculldV, 0.015f);
            LegSideCulldownV.SetMinMaxSlider(0f, 0.15f);
            LegSideCulldownV.Editor_DisplayVariableGUI();

            var waitAfterFull = helper.RequestVariable(waitV, 0.0f);
            waitAfterFull.SetMinMaxSlider(0f, 0.3f);
            if (!waitAfterFull.TooltipAssigned) waitAfterFull.AssignTooltip("Culldown measured since last full attach for single leg happened. Can fix sudden leg re-adjusting on being pushed/long creature rotating.");
            waitAfterFull.Editor_DisplayVariableGUI();

            GUILayout.Space(5);

            if (legHelpers == null) return;

            for (int l = 0; l < legHelpers.Count; l++)
            {
                UnityEditor.EditorGUILayout.LabelField("  [" + l + "] " + (AllowDetach(legHelpers[l]) ? " ALLOW " : " STOP "));
            }
        }

#endif
        #endregion


    }
}