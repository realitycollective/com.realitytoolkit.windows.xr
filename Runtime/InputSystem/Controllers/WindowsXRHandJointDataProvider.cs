// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.OpenXR;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Services;
using XRTK.Utilities;

namespace RealityToolkit.Windows.XR.InputSystem.Controllers
{
    /// <summary>
    /// Provides hand joints data for use with <see cref="WindowsXRHandController"/>.
    /// </summary>
    public sealed class WindowsXRHandJointDataProvider : IUnityXRHandJointDataProvider
    {
        public WindowsXRHandJointDataProvider(XRTK.Definitions.Utilities.Handedness handedness)
        {
            handTracker = handedness == XRTK.Definitions.Utilities.Handedness.Left ? HandTracker.Left : HandTracker.Right;
        }

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
    }
}