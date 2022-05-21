// Copyright (c) Reality Collective. All rights reserved.
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.OpenXR;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using RealityToolkit.Definitions.Controllers.Hands;

namespace RealityToolkit.Windows.XR.InputSystem.Controllers
{
    /// <summary>
    /// Provides hand mesh data for use with <see cref="WindowsXRHandController"/>.
    /// </summary>
    public sealed class WindowsXRHandMeshDataProvider : IUnityXRHandMeshDataProvider
    {
        public WindowsXRHandMeshDataProvider(Definitions.Utilities.Handedness handedness)
        {
            handMeshTracker = handedness == Definitions.Utilities.Handedness.Left ? HandMeshTracker.Left : HandMeshTracker.Right;
        }

        private readonly HandMeshTracker handMeshTracker;
        private readonly Mesh mesh;
        private readonly Mesh neutralPoseMesh;
        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<int> triangles = new List<int>();
        private Vector2[] handMeshUVs;

        /// <inheritdoc />
        public HandMeshData UpdateHandMesh(InputDevice inputDevice)
        {
            if (handMeshUVs == null && handMeshTracker.TryGetHandMesh(FrameTime.OnUpdate, neutralPoseMesh, HandPoseType.ReferenceOpenPalm))
            {
                handMeshUVs = InitializeUVs(neutralPoseMesh.vertices);
            }

            if (handMeshTracker.TryGetHandMesh(FrameTime.OnUpdate, mesh) && handMeshTracker.TryLocateHandMesh(FrameTime.OnUpdate, out _))
            {
                mesh.GetVertices(vertices);
                mesh.GetNormals(normals);
                mesh.GetTriangles(triangles, 0);

                return new HandMeshData(
                    vertices.ToArray(),
                    triangles.ToArray(),
                    normals.ToArray(),
                    handMeshUVs);
            }

            return HandMeshData.Empty;
        }

        private Vector2[] InitializeUVs(Vector3[] neutralPoseVertices)
        {
            if (neutralPoseVertices.Length == 0)
            {
                Debug.LogError("Loaded 0 vertices for neutralPoseVertices");
                return System.Array.Empty<Vector2>();
            }

            var minY = neutralPoseVertices[0].y;
            var maxY = minY;

            for (int ix = 1; ix < neutralPoseVertices.Length; ix++)
            {
                Vector3 p = neutralPoseVertices[ix];

                if (p.y < minY)
                {
                    minY = p.y;
                }
                else if (p.y > maxY)
                {
                    maxY = p.y;
                }
            }

            var scale = 1.0f / (maxY - minY);
            var uvs = new Vector2[neutralPoseVertices.Length];

            for (int ix = 0; ix < neutralPoseVertices.Length; ix++)
            {
                var p = neutralPoseVertices[ix];
                uvs[ix] = new Vector2(p.x * scale + 0.5f, (p.y - minY) * scale);
            }

            return uvs;
        }
    }
}
