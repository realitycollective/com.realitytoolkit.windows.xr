// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.OpenXR;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Extensions;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using RealityToolkit.Services;
using RealityToolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Windows.XR.InputSystem.Controllers
{
    /// <summary>
    /// Provides hand joints data for use with <see cref="WindowsXRHandController"/>.
    /// </summary>
    public sealed class WindowsXRHandJointDataProvider : IUnityXRHandJointDataProvider
    {
        public WindowsXRHandJointDataProvider(Definitions.Utilities.Handedness handedness)
        {
            handTracker = handedness == Definitions.Utilities.Handedness.Left
                ? HandTracker.Left
                : HandTracker.Right;
        }

        ~WindowsXRHandJointDataProvider()
        {
            if (!conversionProxyRootTransform.IsNull())
            {
                conversionProxyTransforms.Clear();
                conversionProxyRootTransform.Destroy();
            }
        }

        private Transform conversionProxyRootTransform;

        private readonly Dictionary<TrackedHandJoint, Transform> conversionProxyTransforms =
            new Dictionary<TrackedHandJoint, Transform>();

        private static readonly HandJoint[] handJoints = Enum.GetValues(typeof(HandJoint)) as HandJoint[];
        private readonly HandTracker handTracker = null;
        private readonly HandJointLocation[] locations = new HandJointLocation[HandTracker.JointCount];
        private Transform cameraRigTransform;

        /// <inheritdoc />
        public void UpdateHandJoints(InputDevice inputDevice, Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            if (cameraRigTransform.IsNull())
            {
                FindCameraRig();
            }

            if (handTracker != null && handTracker.TryLocateHandJoints(FrameTime.OnUpdate, locations))
            {
                for (var i = 0; i < handJoints.Length; i++)
                {
                    var handJoint = handJoints[i];
                    var handJointLocation = locations[(int)handJoint];
                    var position = cameraRigTransform.TransformPoint(handJointLocation.Pose.position);
                    var rotation = cameraRigTransform.rotation * handJointLocation.Pose.rotation;

                    jointPoses[ConvertToTrackedHandJoint(handJoint)] = new MixedRealityPose(position, rotation);
                }
            }
         
            // ConvertJointPoses(jointPoses, GetHandRootPose(jointPoses));
        }
        
        private void ConvertJointPoses(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses, MixedRealityPose handRootPose)
        {
            foreach (var handJoint in jointPoses)
            {
                jointPoses[handJoint.Key] = ConvertJointPose(handJoint.Key, handRootPose, handJoint.Value);
            }
        }
        
        private MixedRealityPose ConvertJointPose(TrackedHandJoint trackedHandJoint, MixedRealityPose handRootPose, MixedRealityPose jointPose)
        {
            var jointTransform = GetProxyTransform(trackedHandJoint);

            if (trackedHandJoint == TrackedHandJoint.Wrist)
            {
                jointTransform.localPosition = handRootPose.Position;
                jointTransform.localRotation = handRootPose.Rotation;
            }
            else
            {
                jointTransform.parent = cameraRigTransform;
                jointTransform.localPosition = cameraRigTransform.InverseTransformPoint(cameraRigTransform.position + cameraRigTransform.rotation * jointPose.Position);
                jointTransform.localRotation = Quaternion.Inverse(cameraRigTransform.rotation) * cameraRigTransform.rotation * jointPose.Rotation;
                jointTransform.parent = conversionProxyRootTransform;
            }

            return new MixedRealityPose(
                conversionProxyRootTransform.InverseTransformPoint(jointTransform.position),
                Quaternion.Inverse(conversionProxyRootTransform.rotation) * jointTransform.rotation);
        }
        
        private MixedRealityPose GetHandRootPose(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            // We use the wrist pose as the hand root pose.
            var wristPose = jointPoses[TrackedHandJoint.Wrist];
            var wristProxyTransform = GetProxyTransform(TrackedHandJoint.Wrist);

            // Convert to camera rig's local coordinate space.
            wristProxyTransform.position = cameraRigTransform.InverseTransformPoint(cameraRigTransform.position + cameraRigTransform.rotation * wristPose.Position);
            wristProxyTransform.rotation = Quaternion.Inverse(cameraRigTransform.rotation) * cameraRigTransform.rotation * wristPose.Rotation;

            return new MixedRealityPose(wristProxyTransform.position, wristProxyTransform.rotation);
        }

        private TrackedHandJoint ConvertToTrackedHandJoint(HandJoint handJoint)
        {
            switch (handJoint)
            {
                case HandJoint.Palm: return TrackedHandJoint.Palm;
                case HandJoint.Wrist: return TrackedHandJoint.Wrist;

                case HandJoint.ThumbMetacarpal: return TrackedHandJoint.ThumbMetacarpal;
                case HandJoint.ThumbProximal: return TrackedHandJoint.ThumbProximal;
                case HandJoint.ThumbDistal: return TrackedHandJoint.ThumbDistal;
                case HandJoint.ThumbTip: return TrackedHandJoint.ThumbTip;

                case HandJoint.IndexMetacarpal: return TrackedHandJoint.IndexMetacarpal;
                case HandJoint.IndexProximal: return TrackedHandJoint.IndexProximal;
                case HandJoint.IndexIntermediate: return TrackedHandJoint.IndexIntermediate;
                case HandJoint.IndexDistal: return TrackedHandJoint.IndexDistal;
                case HandJoint.IndexTip: return TrackedHandJoint.IndexTip;

                case HandJoint.MiddleMetacarpal: return TrackedHandJoint.MiddleMetacarpal;
                case HandJoint.MiddleProximal: return TrackedHandJoint.MiddleProximal;
                case HandJoint.MiddleIntermediate: return TrackedHandJoint.MiddleIntermediate;
                case HandJoint.MiddleDistal: return TrackedHandJoint.MiddleDistal;
                case HandJoint.MiddleTip: return TrackedHandJoint.MiddleTip;

                case HandJoint.RingMetacarpal: return TrackedHandJoint.RingMetacarpal;
                case HandJoint.RingProximal: return TrackedHandJoint.RingProximal;
                case HandJoint.RingIntermediate: return TrackedHandJoint.RingIntermediate;
                case HandJoint.RingDistal: return TrackedHandJoint.RingDistal;
                case HandJoint.RingTip: return TrackedHandJoint.RingTip;

                case HandJoint.LittleMetacarpal: return TrackedHandJoint.LittleMetacarpal;
                case HandJoint.LittleProximal: return TrackedHandJoint.LittleProximal;
                case HandJoint.LittleIntermediate: return TrackedHandJoint.LittleIntermediate;
                case HandJoint.LittleDistal: return TrackedHandJoint.LittleDistal;
                case HandJoint.LittleTip: return TrackedHandJoint.LittleTip;

                default: return TrackedHandJoint.None;
            }
        }

        private void FindCameraRig()
        {
            if (MixedRealityToolkit.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem))
            {
                cameraRigTransform = cameraSystem.MainCameraRig.RigTransform;
            }
            else
            {
                var cameraTransform = CameraCache.Main.transform;
                Debug.Assert(cameraTransform.parent.IsNotNull(), "The camera must be parented.");
                cameraRigTransform = CameraCache.Main.transform.parent;
            }
        }

        private Transform GetProxyTransform(TrackedHandJoint handJointKind)
        {
            if (conversionProxyRootTransform.IsNull())
            {
                conversionProxyRootTransform =
                    new GameObject($"{nameof(WindowsXRHandJointDataProvider)}.HandJointConversionProxy").transform;
                conversionProxyRootTransform.transform.SetParent(cameraRigTransform, false);
                conversionProxyRootTransform.gameObject.SetActive(false);
            }

            if (handJointKind == TrackedHandJoint.Wrist)
            {
                return conversionProxyRootTransform;
            }

            if (conversionProxyTransforms.ContainsKey(handJointKind))
            {
                return conversionProxyTransforms[handJointKind];
            }

            var transform = new GameObject($"{handJointKind} Proxy").transform;
            transform.SetParent(cameraRigTransform, false);
            conversionProxyTransforms.Add(handJointKind, transform);

            return transform;
        }
    }
}