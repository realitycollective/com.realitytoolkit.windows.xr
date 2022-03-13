// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.Providers.Controllers.Hands;

namespace RealityToolkit.Windows.XR.InputSystem.Profiles
{
    /// <summary>
    /// Configuration profile for <see cref="Providers.WindowsXRControllerDataProvider"/>.
    /// </summary>
    public class WindowsXRControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        /// <inheritdoc />
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(MixedRealityHandController), Handedness.Left),
                new ControllerDefinition(typeof(MixedRealityHandController), Handedness.Right)
            };
        }
    }
}