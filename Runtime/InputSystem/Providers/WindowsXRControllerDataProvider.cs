// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Windows.XR.InputSystem.Profiles;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Interfaces.InputSystem;
using XRTK.Services.InputSystem.Controllers;

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
    }
}
