// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.OpenXR;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using RealityToolkit.Services;
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
        public WindowsXRHandJointDataProvider(RealityCollective.Definitions.Utilities.Handedness handedness)
        {
            handTracker = handedness == RealityCollective.Definitions.Utilities.Handedness.Left
                ? HandTracker.Left
                : HandTracker.Right;
        }

        private static readonly HandJoint[] handJoints = Enum.GetValues(typeof(HandJoint)) as HandJoint[];
        private readonly HandTracker handTracker = null;
        private readonly HandJointLocation[] handJointLocations = new HandJointLocation[HandTracker.JointCount];
        private IMixedRealityCameraRig cameraRig;

        /// <inheritdoc />
        public void UpdateHandJoints(InputDevice inputDevice, ref MixedRealityPose[] jointPoses, ref Dictionary<XRHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (cameraRig == null)
            {
                FindCameraRig();
            }

            if (handTracker != null && handTracker.TryLocateHandJoints(FrameTime.OnUpdate, handJointLocations))
            {
                for (var i = 0; i < handJoints.Length; i++)
                {
                    var handJoint = handJoints[i];
                    var handJointLocation = handJointLocations[(int)handJoint];
                    var position = handJointLocation.Pose.position;
                    var rotation = handJointLocation.Pose.rotation;

                    var xrHandJoint = ConvertToXRHandJoint(handJoint);
                    var pose = new MixedRealityPose(position, rotation);
                    jointPosesDictionary[xrHandJoint] = pose;
                    jointPoses[(int)xrHandJoint] = pose;
                }
            }
        }

        private XRHandJoint ConvertToXRHandJoint(HandJoint handJoint)
        {
            switch (handJoint)
            {
                case HandJoint.Palm: return XRHandJoint.Palm;
                case HandJoint.Wrist: return XRHandJoint.Wrist;

                case HandJoint.ThumbMetacarpal: return XRHandJoint.ThumbMetacarpal;
                case HandJoint.ThumbProximal: return XRHandJoint.ThumbProximal;
                case HandJoint.ThumbDistal: return XRHandJoint.ThumbDistal;
                case HandJoint.ThumbTip: return XRHandJoint.ThumbTip;

                case HandJoint.IndexMetacarpal: return XRHandJoint.IndexMetacarpal;
                case HandJoint.IndexProximal: return XRHandJoint.IndexProximal;
                case HandJoint.IndexIntermediate: return XRHandJoint.IndexIntermediate;
                case HandJoint.IndexDistal: return XRHandJoint.IndexDistal;
                case HandJoint.IndexTip: return XRHandJoint.IndexTip;

                case HandJoint.MiddleMetacarpal: return XRHandJoint.MiddleMetacarpal;
                case HandJoint.MiddleProximal: return XRHandJoint.MiddleProximal;
                case HandJoint.MiddleIntermediate: return XRHandJoint.MiddleIntermediate;
                case HandJoint.MiddleDistal: return XRHandJoint.MiddleDistal;
                case HandJoint.MiddleTip: return XRHandJoint.MiddleTip;

                case HandJoint.RingMetacarpal: return XRHandJoint.RingMetacarpal;
                case HandJoint.RingProximal: return XRHandJoint.RingProximal;
                case HandJoint.RingIntermediate: return XRHandJoint.RingIntermediate;
                case HandJoint.RingDistal: return XRHandJoint.RingDistal;
                case HandJoint.RingTip: return XRHandJoint.RingTip;

                case HandJoint.LittleMetacarpal: return XRHandJoint.LittleMetacarpal;
                case HandJoint.LittleProximal: return XRHandJoint.LittleProximal;
                case HandJoint.LittleIntermediate: return XRHandJoint.LittleIntermediate;
                case HandJoint.LittleDistal: return XRHandJoint.LittleDistal;
                case HandJoint.LittleTip: return XRHandJoint.LittleTip;

                default: return XRHandJoint.Unknown;
            }
        }

        private void FindCameraRig()
        {
            if (!MixedRealityToolkit.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem))
            {
                Debug.LogError($"{nameof(WindowsXRHandJointDataProvider)} needs the {nameof(IMixedRealityCameraSystem)} to work.");
                return;
            }

            cameraRig = cameraSystem.MainCameraRig;
        }
    }
}