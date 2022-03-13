// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Platforms;

namespace RealityToolkit.Windows.XR
{
    /// <summary>
    /// Used by the toolkit to signal that a feature is available on the Windows XR platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("575619ae-25e7-4f73-af33-c1604e1dd858")]
    public class WindowsXRPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
#if WINDOWS_UWP
                return !UnityEngine.Application.isEditor;
#else
                return false;
#endif
            }
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override UnityEditor.BuildTarget[] ValidBuildTargets { get; } =
        {
            UnityEditor.BuildTarget.WSAPlayer
        };
#endif
    }
}
