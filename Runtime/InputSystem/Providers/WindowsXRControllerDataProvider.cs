// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Attributes;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.Interfaces.InputSystem.Controllers;
using RealityToolkit.Services.InputSystem.Controllers;
using RealityToolkit.Windows.XR.InputSystem.Controllers;
using RealityToolkit.Windows.XR.InputSystem.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Windows.XR.InputSystem.Providers
{
    /// <summary>
    /// Manages active controllers when running on the <see cref="WindowsXRPlatform"/>.
    /// </summary>
    [RuntimePlatform(typeof(WindowsXRPlatform))]
    [System.Runtime.InteropServices.Guid("80764933-e115-4d1e-beda-26383c32c327")]
    public class WindowsXRControllerDataProvider : BaseControllerDataProvider
    {
        /// <inheritdoc />
        public WindowsXRControllerDataProvider(string name, uint priority, WindowsXRControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService) { }

        private readonly Dictionary<Handedness, WindowsXRHandController> activeControllers = new Dictionary<Handedness, WindowsXRHandController>();

        /// <inheritdoc />
        public override IReadOnlyList<IMixedRealityController> ActiveControllers => activeControllers.Values.ToList();

        /// <inheritdoc />
        public override void Update()
        {
            var leftHandInputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            if (leftHandInputDevice != default &&
                TryGetControllerType(leftHandInputDevice, out var leftHandControllerType) &&
                TryGetOrAddController(Handedness.Left, leftHandControllerType, out var leftController))
            {
                leftController.UpdateController();
            }
            else
            {
                RemoveController(Handedness.Left);
            }

            var rightHandInputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (rightHandInputDevice != default &&
                TryGetControllerType(rightHandInputDevice, out var rightHandControllerType) &&
                TryGetOrAddController(Handedness.Right, rightHandControllerType, out var rightController))
            {
                rightController.UpdateController();
            }
            else
            {
                RemoveController(Handedness.Right);
            }
        }

        /// <inheritdoc />
        public override void Disable() => RemoveAllControllers();

        private void RemoveAllControllers()
        {
            foreach (var activeController in activeControllers)
            {
                RemoveController(activeController.Key, false);
            }

            activeControllers.Clear();
        }

        private bool TryGetController(Handedness handedness, out WindowsXRHandController controller)
        {
            if (activeControllers.ContainsKey(handedness))
            {
                var existingController = activeControllers[handedness];
                Debug.Assert(existingController != null, $"{nameof(WindowsXRHandController)} {handedness} has been destroyed but remains in the active controller registry.");
                controller = existingController;
                return true;
            }

            controller = null;
            return false;
        }

        private bool TryGetOrAddController(Handedness handedness, Type controllerType, out WindowsXRHandController controller)
        {
            if (TryGetController(handedness, out controller))
            {
                return true;
            }

            try
            {
                controller = (WindowsXRHandController)Activator.CreateInstance(controllerType, this, TrackingState.NotTracked, handedness, GetControllerMappingProfile(controllerType, handedness));
                controller.TryRenderControllerModel();
                activeControllers.Add(handedness, controller);
                InputSystem?.RaiseSourceDetected(controller.InputSource, controller);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create {controllerType.Name}!");
                Debug.LogException(ex);
                controller = null;
                return false;
            }
        }

        private bool TryGetControllerType(InputDevice inputDevice, out Type controllerType)
        {
            if ((inputDevice.characteristics & InputDeviceCharacteristics.HandTracking) != 0)
            {
                controllerType = typeof(WindowsXRHandController);
                return true;
            }
            if ((inputDevice.characteristics & InputDeviceCharacteristics.HeldInHand) != 0)
            {
                // TODO: Implement support for WMR controllers.
            }

            controllerType = null;
            return false;
        }

        private void RemoveController(Handedness handedness, bool removeFromRegistry = true)
        {
            if (TryGetController(handedness, out var controller))
            {
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);

                if (removeFromRegistry)
                {
                    activeControllers.Remove(handedness);
                }
            }
        }
    }
}
