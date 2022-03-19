// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Windows.XR.InputSystem.Profiles;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XRTK.Attributes;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Services.InputSystem.Controllers;
using XRTK.Services.InputSystem.Controllers.UnityXR;

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

        private readonly Dictionary<Handedness, UnityXRController> activeControllers = new Dictionary<Handedness, UnityXRController>();

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

        private bool TryGetController(Handedness handedness, out UnityXRController controller)
        {
            if (activeControllers.ContainsKey(handedness))
            {
                var existingController = activeControllers[handedness];
                Debug.Assert(existingController != null, $"{nameof(UnityXRHandController)} {handedness} has been destroyed but remains in the active controller registry.");
                controller = existingController;
                return true;
            }

            controller = null;
            return false;
        }

        private bool TryGetOrAddController(Handedness handedness, Type controllerType, out UnityXRController controller)
        {
            if (TryGetController(handedness, out controller))
            {
                return true;
            }

            try
            {
                controller = (UnityXRController)Activator.CreateInstance(controllerType, this, TrackingState.NotTracked, handedness, GetControllerMappingProfile(controllerType, handedness));
                controller.TryRenderControllerModel();
                AddController(controller);
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
                controllerType = typeof(UnityXRHandController);
                return true;
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
                    RemoveController(controller);
                    activeControllers.Remove(handedness);
                }
            }
        }
    }
}
