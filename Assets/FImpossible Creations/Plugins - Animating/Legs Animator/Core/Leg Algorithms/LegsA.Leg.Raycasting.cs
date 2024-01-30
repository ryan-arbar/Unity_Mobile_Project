using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{
    public partial class LegsAnimator
    {
        public partial class Leg
        {
            public bool RaycastHitted { get; private set; }
            public RaycastHit LastGroundHit { get { return legGroundHit; } }
            public RaycastHit legGroundHit;

            public Vector3 groundHitRootSpacePos { get; private set; }

            Vector3 lastRaycastingOrigin;
            Vector3 previousAnkleAlignedOnGroundHitWorldPos;
            public Vector3 ankleAlignedOnGroundHitWorldPos { get; private set; }
            Vector3 ankleAlignedOnGroundHitRootLocal;
            Quaternion ankleAlignedOnGroundHitRotation;

            #region Custom Raycast

            bool _UsingEmptyRaycast = false;
            bool _UsingCustomRaycast = false;
            float _CustomRaycastBlendIn = 0f;
            RaycastHit _CustomRaycastHit;
            Vector3 _PreviousCustomRaycastingStartIKPos;
            Vector3 _PreviousCustomRaycastingIKPos;

            public void User_OverrideRaycastHit(Transform tr)
            {
                if (!_UsingCustomRaycast)
                {
                    _CustomRaycastBlendIn = 0f;
                    _PreviousCustomRaycastingStartIKPos = C_LastHeelWorldPos;
                }

                _UsingCustomRaycast = true;
                RaycastHit hit = new RaycastHit();
                hit.point = tr.position;
                hit.normal = tr.up;
                _CustomRaycastOnBlendIn(hit);
            }

            public void User_OverrideRaycastHit(RaycastHit hit)
            {
                if (!_UsingCustomRaycast)
                {
                    _CustomRaycastBlendIn = 0f;
                    _PreviousCustomRaycastingStartIKPos = C_LastHeelWorldPos;
                }

                _UsingCustomRaycast = true;
                _CustomRaycastOnBlendIn(hit);
            }

            public void User_RestoreRaycasting()
            {
                if (_UsingCustomRaycast) _CustomRaycastBlendIn = 1f;
                _UsingCustomRaycast = false;
            }

            void _CustomRaycastOnBlendIn(RaycastHit hit)
            {
                _CustomRaycastBlendIn += Owner.DeltaTime * 6f;
                if (_CustomRaycastBlendIn > 1f) _CustomRaycastBlendIn = 1f;

                hit.point = Vector3.LerpUnclamped(_PreviousCustomRaycastingStartIKPos, hit.point, _CustomRaycastBlendIn);
                _CustomRaycastHit = hit;
                _PreviousCustomRaycastingIKPos = hit.point;
            }

            void _CustomRaycastOnBlendOut()
            {
                if (_UsingCustomRaycast) return;
                if (_CustomRaycastBlendIn <= 0f) return;

                _CustomRaycastBlendIn -= Owner.DeltaTime * 8f;
                if (_CustomRaycastBlendIn < 0f) _CustomRaycastBlendIn = 0f;

                if (RaycastHitted == false) return;

                RaycastHit hit = legGroundHit;
                hit.point = Vector3.LerpUnclamped(hit.point, _PreviousCustomRaycastingIKPos, _CustomRaycastBlendIn);
                hit.normal = Vector3.SlerpUnclamped(hit.normal, _CustomRaycastHit.normal, _CustomRaycastBlendIn);
                legGroundHit = hit;
            }

            #endregion


            public float raycastSlopeAngle { get; private set; }

            void Raycasting_Init()
            {
                ankleAlignedOnGroundHitWorldPos = BoneEnd.position;
                raycastSlopeAngle = 0f;
            }

            public void OverrideControlPositionsWithCurrentIKState()
            {
                AnkleH.LastKeyframeRootPos = ToRootLocalSpace(_FinalIKPos);
            }

            public void OverrideSourceIKPos()
            {
                OverrideSourceIKPos(_FinalIKPos);
            }

            public void OverrideSourceIKPos(Vector3 newSrc)
            {
                _SourceIKPos = newSrc;
            }

            bool _noRaycast_skipFeetCalcs = false;
            public void Raycasting_PreLateUpdate()
            {
                RaycastHitted = false;
                _noRaycast_skipFeetCalcs = false;

                if (_UsingCustomRaycast)
                {
                    RaycastHitted = true;
                    legGroundHit = _CustomRaycastHit;
                    groundHitRootSpacePos = ToRootLocalSpace(legGroundHit.point);
                    _UsingEmptyRaycast = true;
                    _noRaycast_skipFeetCalcs = true;
                    _Raycasting_CalculateBasis();
                    ankleAlignedOnGroundHitRotation = GetAlignedOnGroundHitRot(_SourceIKRot, legGroundHit.normal);
                }
                else
                {
                    if (Owner.RaycastStyle == ERaycastStyle.NoRaycasting)
                    {
                        GenerateZeroFloorRaycastHit();
                        CustomRaycastValidate();
                        _noRaycast_skipFeetCalcs = true;
                        _UsingEmptyRaycast = true;
                        ankleAlignedOnGroundHitRotation = _SourceIKRot;
                    }
                    else
                    {
                        _UsingEmptyRaycast = false;

                        if (Owner.RaycastStyle == ERaycastStyle.StraightDown)
                        {
                            Raycast_StraightDown();
                        }
                        else if (Owner.RaycastStyle == ERaycastStyle.OriginToFoot)
                        {
                            Raycast_OriginToFoot();
                        }
                        else if (Owner.RaycastStyle == ERaycastStyle.OriginToFoot_DownOnNeed)
                        {
                            Raycast_OriginToFoot();
                            if (!RaycastHitted) Raycast_StraightDown();
                        }
                        else if (Owner.RaycastStyle == ERaycastStyle.AlongBones)
                        {
                            Raycast_AlongBones();
                            if (!RaycastHitted) { Raycast_StraightDown(); }
                        }

                        if (!RaycastHitted)
                        {
                            if (Owner.ZeroStepsOnNoRaycast)
                            {
                                _noRaycast_skipFeetCalcs = true;
                                _UsingEmptyRaycast = true;
                                GenerateZeroFloorRaycastHit();
                                ankleAlignedOnGroundHitRotation = _SourceIKRot;
                            }
                        }
                    }

                    _CustomRaycastOnBlendOut();
                }


                if (_noRaycast_skipFeetCalcs)
                {
                    return;
                }

                // Foot rotation on raycast hit
                if (RaycastHitted)
                {
                    ankleAlignedOnGroundHitRotation = GetAlignedOnGroundHitRot(_SourceIKRot, legGroundHit.normal);
                }
                else
                    ankleAlignedOnGroundHitRotation = _SourceIKRot;

            }


            void GenerateZeroFloorRaycastHit()
            {
                RaycastHit hit = new RaycastHit();

                Vector3 fakehitRootSpace = ToRootLocalSpace(_SourceIKPos);
                ankleAlignedOnGroundHitRootLocal = fakehitRootSpace;
                fakehitRootSpace.y = 0f;
                Vector3 fakeHit = RootSpaceToWorld(fakehitRootSpace);

                hit.point = fakeHit;
                hit.normal = Owner.Up;

                legGroundHit = hit;
                RaycastHitted = true;

                groundHitRootSpacePos = fakehitRootSpace;
            }


            void CustomRaycastValidate()
            {
                _Raycasting_CalculateBasis();
                raycastSlopeAngle = 0f;

                A_WasAligning = true;
                A_WasAligningFrameBack = true;

                A_LastTargetAlignRot = _SourceIKRot;
                A_LastApppliedAlignRot = _SourceIKRot;

                A_PreviousRelevantAnklePos = _SourceIKPos;
                A_LastAlignHeightDiff = C_Local_MidFootPosVsGroundHit.y;
                A_LastAlignHeightCompareValue = ScaleRef * 0.002f + ParentHub._Hips_StepHeightAdjustOffset;
            }


            Vector3 Raycast_RefreshOrigin()
            {
                Vector3 origin = ParentHub.LastRootLocalPos;
                origin = RootSpaceToWorld(origin);

                lastRaycastingOrigin = origin;
                return origin;
            }


            void Raycast_OriginToFoot()
            {
                Vector3 origin = Raycast_RefreshOrigin();

                Vector3 castEndPoint = RootSpaceToWorld(AnkleH.LastKeyframeRootPos) - Owner.Up * C_AnkleToHeelWorldHeight;
                Vector3 direction = castEndPoint - origin;

                float toGround = direction.magnitude * 1.05f;
                direction.Normalize();

                Vector3 rayGroundPos = origin + direction * toGround;

                if (Physics.Linecast(origin, rayGroundPos, out legGroundHit, Owner.GroundMask, Owner.RaycastHitTrigger))
                {
                    CaptureRaycastHitForLeg();
                }
                else
                {
                    ankleAlignedOnGroundHitWorldPos = AnkleIK.srcPosition;
                }
            }

            void Raycast_AlongBones()
            {
                Raycast_RefreshOrigin();

                if (DoRaycasting(_AnimatorStartBonePos, _AnimatorMidBonePos))
                {
                    CaptureRaycastHitForLeg();
                }
                else
                {
                    Vector3 endPos = _AnimatorEndBonePos + (_AnimatorEndBonePos - _AnimatorMidBonePos) * 0.1f;

                    if (DoRaycasting(_AnimatorMidBonePos, endPos))
                    {
                        CaptureRaycastHitForLeg();
                    }
                    else
                        ankleAlignedOnGroundHitWorldPos = AnkleIK.srcPosition;
                }
            }

            void Raycast_StraightDown()
            {
                Vector3 castStartPointLocal = AnkleH.LastKeyframeRootPos;

                Vector3 origin = ParentHub.LastRootLocalPos;
                float toGround;

                if (Owner.RaycastStartHeight == ERaycastStartHeight.FirstBone)
                {
                    origin = BoneStart.position;
                    toGround = IKProcessor.fullLength;
                }
                else 
                {
                    origin.x = castStartPointLocal.x;
                    //origin.x = Ankle.LastKeyframeRootPos.x;
                    origin.z = castStartPointLocal.z;

                    toGround = Owner.ScaleReference * (Owner.RaycastStartHeightMul / Root.lossyScale.y);

                    if (Owner.RaycastStartHeight == ERaycastStartHeight.StaticScaleReference)
                        origin.y = toGround;

                    origin = RootSpaceToWorld(origin);
                }

                lastRaycastingOrigin = origin;

                Vector3 direction = -Owner.Up;

                Vector3 rayGroundPos = origin + direction * toGround;
                float extraRaycastingDistance = ScaleRef * Owner.CastDistance;

                Vector3 rayEnd = rayGroundPos + direction * extraRaycastingDistance;

                //UnityEngine.Debug.DrawLine(origin, rayEnd, Color.green, 0.11f);

                if (DoRaycasting(origin, rayEnd))
                {
                    CaptureRaycastHitForLeg();
                }
                else
                {
                    ankleAlignedOnGroundHitWorldPos = AnkleIK.srcPosition;
                }
            }

            internal bool DoRaycasting(Vector3 origin, Vector3 rayEnd)
            {
                bool hitted;

                if (Owner.RaycastShape == ERaycastMode.Linecast)
                {
                    hitted = Physics.Linecast(origin, rayEnd, out legGroundHit, Owner.GroundMask, Owner.RaycastHitTrigger);
                }
                else
                {
                    float radius = Owner.ScaleReference * 0.065f;
                    Vector3 castDir = rayEnd - origin;
                    float castDistance = castDir.magnitude - radius;
                    hitted = Physics.SphereCast(origin, radius, castDir.normalized, out legGroundHit, castDistance - radius, Owner.GroundMask, Owner.RaycastHitTrigger);
                    if (hitted)
                    {
                        if (Owner.SpherecastRealign > 0f)
                        {
                            Vector3 ppos = ToRootLocalSpace(legGroundHit.point);
                            ppos.x = Mathf.LerpUnclamped(ppos.x, AnkleH.LastKeyframeRootPos.x, Owner.SpherecastRealign);
                            ppos.z = Mathf.LerpUnclamped(ppos.z, AnkleH.LastKeyframeRootPos.z, Owner.SpherecastRealign);
                            legGroundHit.point = RootSpaceToWorld(ppos);
                        }
                    }
                }

                return hitted;
            }

            void CaptureRaycastHitForLeg()
            {
                RaycastHitted = true;
                groundHitRootSpacePos = ToRootLocalSpace(legGroundHit.point);

                #region Prevent sudden angle changes - smooth on big angle changes

                raycastSlopeAngle = Vector3.Angle(Owner.Up, legGroundHit.normal);

                if (raycastSlopeAngle > 45f)
                {
                    var hit = legGroundHit;
                    hit.normal = Vector3.Slerp(legGroundHit.normal, Owner.Up, Mathf.InverseLerp(45f, 90f, raycastSlopeAngle) * 0.5f);
                    legGroundHit = hit;
                }

                #endregion

                _Raycasting_CalculateBasis();
            }


            void _Raycasting_CalculateBasis()
            {
                previousAnkleAlignedOnGroundHitWorldPos = ankleAlignedOnGroundHitWorldPos;
                ankleAlignedOnGroundHitWorldPos = GetAlignedOnGroundHitPos(groundHitRootSpacePos, legGroundHit.point, legGroundHit.normal);
                ankleAlignedOnGroundHitRootLocal = ToRootLocalSpace(ankleAlignedOnGroundHitWorldPos);
            }


            Vector3 GetAlignedOnGroundHitPos(Vector3 rootSpaceHitPos, Vector3 worldHit, Vector3 normal)
            {
                Vector3 andjustedLocalAnklePos = rootSpaceHitPos;
                andjustedLocalAnklePos.y = ToRootLocalSpace(worldHit + normal * C_AnkleToHeelWorldHeight).y;
                return RootSpaceToWorld(andjustedLocalAnklePos);
            }

            Quaternion GetAlignedOnGroundHitRot(Quaternion sourceRotation, Vector3 normal)
            {
                Quaternion alignedRot = Quaternion.FromToRotation(sourceRotation * AnkleIK.up, normal);
                alignedRot *= sourceRotation;
                return alignedRot;
            }

        }
    }
}