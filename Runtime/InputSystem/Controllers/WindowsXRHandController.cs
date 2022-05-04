// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Services.InputSystem.Controllers.UnityXR;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;

namespace RealityToolkit.Windows.XR.InputSystem.Controllers
{
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
    }
}
