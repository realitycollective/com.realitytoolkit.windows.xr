// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Windows.XR.InputSystem
{
    /// <summary>
    /// Extends Unity's default <see cref="CommonUsages"/> by custom usages to the <see cref="WindowsXRPlatform"/>.
    /// https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.4/manual/features/microsofthandinteraction.html
    /// </summary>
    public class WindowsXRInputFeatureUsages
    {
        private const string pointerPositionUsageName = "PointerPosition";
        private const string pointerRotationUsageName = "PointerRotation";

        /// <summary>
        /// Gets the world space <see cref="Vector3"/> representing the <see cref="InputDevice"/> spatial pointer position.
        /// </summary>
        public static readonly InputFeatureUsage<Vector3> PointerPosition = new InputFeatureUsage<Vector3>(pointerPositionUsageName);

        /// <summary>
        /// Gets the world space <see cref="Quaternion"/> representing the <see cref="InputDevice"/> spatial pointer rotation.
        /// </summary>
        public static readonly InputFeatureUsage<Quaternion> PointerRotation = new InputFeatureUsage<Quaternion>(pointerRotationUsageName);
    }
}
