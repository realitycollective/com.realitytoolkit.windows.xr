// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.OpenXR;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Interfaces.InputSystem.Providers.Controllers.Hands;

namespace RealityToolkit.Windows.XR.InputSystem.Controllers
{
    /// <summary>
    /// Provides hand joints data for use with <see cref="WindowsXRHandController"/>.
    /// </summary>
    public sealed class WindowsXRHandJointDataProvider : IUnityXRHandJointDataProvider
    {
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
    }
}