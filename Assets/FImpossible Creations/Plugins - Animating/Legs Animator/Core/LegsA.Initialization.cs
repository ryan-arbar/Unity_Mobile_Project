using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{

    public partial class LegsAnimator
    {
        public bool LegsInitialized { get; private set; }
        bool _wasInstantTriggered = true;

        protected virtual void Initialize()
        {
            if (LegsInitialized) return;

            GroundedTime = 0f;
            MovingTime = 0f;
            IsMovingBlend = 0f;
            IsGroundedBlend = 1f;
            RagdolledDisablerBlend = 1f;
            DeltaTime = 0.05f;

            RagdolledDisablerBlend = 1f;
            RagdolledTime = -100f;

            Initialize_BaseTransform();
            RefreshMatrices();
            _wasInstantTriggered = true;

            Legs_RefreshLegsOwner();
            User_RefreshHelperVariablesOnParametersChange();
            Controll_DefineHashes();
            Initialize_Stability();

            finalScaleReferenceSqrt = ScaleReference * ScaleReference;

            HipsSetup.Initialize(this, Hips, BaseTransform);
            _LastAppliedHipsFinalPosition = Hips.position;
            HipsHubs_Init();

            for (int i = 0; i < Legs.Count; i++) Legs[i].InitLegBasics(this, i, (i + 1) < Legs.Count ? Legs[i + 1] : null);
            IK_Initialize();

            LegsInitialized = true;
            AllowUpdate = true;

            InitializeGetStepInfoReceiver();
            if (Mecanim) AnimatePhysics = Mecanim.updateMode == AnimatorUpdateMode.AnimatePhysics;

            //StepHeatmap_Setup();

            InitializeModules();
            PrepareValues(Time.deltaTime);

            User_UpdateParametersAfterManualChange();
            User_RefreshHelperVariablesOnParametersChange();
        }

        public void InitializeGetStepInfoReceiver()
        {
            if (StepInfoReceiver != null) _StepReceiver = StepInfoReceiver.GetComponent<ILegStepReceiver>();
        }

        public void Initialize_BaseTransform()
        {
            if (baseTransform == null) baseTransform = transform;

            InitialBaseScale = baseTransform.lossyScale;
            if (InitialBaseScale.y == 0f) InitialBaseScale = Vector3.one;

            User_RefreshHelperVariablesOnParametersChange();
            MotionInfluence_Init();
        }

        public bool IsSetupValid()
        {
            if (Legs.Count == 0) return false;
            if (Hips == null) return false;

            bool allSet = true;
            for (int i = 0; i < Legs.Count; i++)
            {
                if (Legs[i].BoneStart == null) { allSet = false; break; }
                if (Legs[i].BoneEnd == null) { allSet = false; break; }
            }

            return allSet;
        }


        protected virtual void OnLegsReactivate()
        {
            RefreshMatrices();
            MotionInfluence.Reset();
            Modules_OnReInitialize();

            HipsSetup.HipsMuscle.OverrideProceduralPosition(Vector3.zero);
            HipsSetup.HipsRotMuscle.OverrideProceduralRotation(Quaternion.identity);

            _LastAppliedHipsStabilityOffset = Vector3.zero;
            _LastAppliedHipsFinalPosition = Hips.position;

            _Hips_StabilityLocalOffset = Vector3.zero;
            _Hips_FinalStabilityOffset = Vector3.zero;
            //_Hips_StabilityLocalAdjustement = Vector3.zero;
            //_Hips_sd_StabilAdjustm = Vector3.zero;
            //_Hips_PushLocalOffset = Vector3.zero;
            //_Hips_sd_PushOffset = Vector3.zero;

            HipsSetup.Reset();

            _glueModeExecuted = EGlueMode.Moving;

            for (int i = 0; i < Legs.Count; i++)
            {
                Legs[i].Reset();
            }
        }

    }
}