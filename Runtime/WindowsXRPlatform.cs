// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XRTK.Definitions.Platforms;

namespace RealityToolkit.Windows.XR
{
    /// <summary>
    /// Used by the toolkit to signal that a feature is available on the Windows XR platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("575619ae-25e7-4f73-af33-c1604e1dd858")]
    public class WindowsXRPlatform : BasePlatform
    {
        private const string xrInputSubsystemDescriptorId = "OpenXR Input Extension";
        private const string xrMeshSubsystemDescriptorId = "OpenXR Mesh Extension";

        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
                var meshSubsystems = new List<XRMeshSubsystem>();
                SubsystemManager.GetSubsystems(meshSubsystems);
                var xrMeshSubsystemDescriptorFound = false;

                for (var i = 0; i < meshSubsystems.Count; i++)
                {
                    var meshSubsystem = meshSubsystems[i];
                    if (meshSubsystem.SubsystemDescriptor.id.Equals(xrMeshSubsystemDescriptorId) &&
                        meshSubsystem.running)
                    {
                        xrMeshSubsystemDescriptorFound = true;
                    }
                }

                // The XR Mesh Subsystem is not available / running,
                // the platform doesn't seem to be available.
                if (!xrMeshSubsystemDescriptorFound)
                {
                    return false;
                }

                var inputSubsystems = new List<XRInputSubsystem>();
                SubsystemManager.GetSubsystems(inputSubsystems);
                var xrInputSubsystemDescriptorFound = false;

                for (var i = 0; i < inputSubsystems.Count; i++)
                {
                    var inputSubsystem = inputSubsystems[i];
                    if (inputSubsystem.SubsystemDescriptor.id.Equals(xrInputSubsystemDescriptorId) &&
                        inputSubsystem.running)
                    {
                        xrInputSubsystemDescriptorFound = true;
                    }
                }

                // The XR Input Subsystem is not available / running,
                // the platform doesn't seem to be available.
                if (!xrInputSubsystemDescriptorFound)
                {
                    return false;
                }

                // Only if both, Mesh and Input XR Subsystems are available
                // and running, the platform is considered available.
                return true;
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
