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

        private readonly Dictionary<Handedness, UnityXRHandController> activeControllers = new Dictionary<Handedness, UnityXRHandController>();

        /// <inheritdoc />
        public override void Update()
        {
            if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand) != default && TryGetController(Handedness.Left, out var leftController))
            {
                leftController.UpdateController();
            }
            else
            {
                RemoveController(Handedness.Left);
            }

            if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand) != default && TryGetController(Handedness.Left, out var rightController))
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

        private bool TryGetController(Handedness handedness, out UnityXRHandController controller)
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

        private bool TryGetOrAddController(Handedness handedness, out UnityXRHandController controller)
        {
            if (TryGetController(handedness, out controller))
            {
                return true;
            }

            try
            {
                controller = new UnityXRHandController(this, TrackingState.NotTracked, handedness, GetControllerMappingProfile(typeof(UnityXRHandController), handedness));
                controller.TryRenderControllerModel();
                AddController(controller);
                activeControllers.Add(handedness, controller);
                InputSystem?.RaiseSourceDetected(controller.InputSource, controller);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create {nameof(UnityXRHandController)}!");
                Debug.LogException(ex);
                controller = null;
                return false;
            }
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
