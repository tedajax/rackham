#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Tanks
{
    /// <summary>
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    public class SmokePlumeParticleSystem : ParticleSystem
    {
        public SmokePlumeParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "fancysmoke";

            settings.MaxParticles = 600;

            settings.Duration = TimeSpan.FromSeconds(1);
            
            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1.5f;

            settings.MinVerticalVelocity = 1;
            settings.MaxVerticalVelocity = 2;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(0, 0, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 5;
            settings.MaxStartSize = 5;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 20;
        }
    }
}
