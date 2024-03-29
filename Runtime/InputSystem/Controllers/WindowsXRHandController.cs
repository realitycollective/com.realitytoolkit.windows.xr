// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using RealityToolkit.Services.InputSystem.Controllers.UnityXR;

namespace RealityToolkit.Windows.XR.InputSystem.Controllers
{
    /// <summary>
    /// Hand tracking based controller for the <see cref="WindowsXRPlatform"/> platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("dac6f2b1-5375-40ac-a033-7f73f0a39e1d")]
    public class WindowsXRHandController : UnityXRHandController
    {
        /// <inheritdoc />
        public WindowsXRHandController() { }

        /// <inheritdoc />
        public WindowsXRHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
            handJointDataProvider = new WindowsXRHandJointDataProvider(controllerHandedness);
            handMeshDataProvider = new WindowsXRHandMeshDataProvider(controllerHandedness);
        }

        /// <inheritdoc />
        protected override void UpdateSpatialPointerPose()
        {
            var position = SpatialPointerPose.Position;
            if (InputDevice.TryGetFeatureValue(WindowsXRInputFeatureUsages.PointerPosition, out var pointerPosition))
            {
                position = cameraRig.RigTransform.TransformPoint(pointerPosition);
            }

            var rotation = SpatialPointerPose.Rotation;
            if (InputDevice.TryGetFeatureValue(WindowsXRInputFeatureUsages.PointerRotation, out var pointerRotation))
            {
                rotation = cameraRig.RigTransform.rotation * pointerRotation;
            }

            SpatialPointerPose = new MixedRealityPose(position, rotation);
        }
    }
}
