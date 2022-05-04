// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Windows.XR.CameraSystem.Profiles;
using UnityEngine;
using RealityToolkit.Attributes;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Services.CameraSystem.Providers;

namespace RealityToolkit.Windows.XR.CameraSystem.Providers
{
    /// <summary>
    /// <see cref="IMixedRealityCameraSystem"/> data provider used when running on the
    /// <see cref="WindowsXRPlatform"/>.
    /// </summary>
    [RuntimePlatform(typeof(WindowsXRPlatform))]
    [System.Runtime.InteropServices.Guid("b332b5fb-c7ed-4348-9261-ad563eb621c6")]
    public class WindowsXRCameraDataProvider : BaseCameraDataProvider
    {
        /// <inheritdoc />
        public WindowsXRCameraDataProvider(string name, uint priority, WindowsXRCameraDataProviderProfile profile, IMixedRealityCameraSystem parentService)
            : base(name, priority, profile, parentService)
        {

        }
    }
}
