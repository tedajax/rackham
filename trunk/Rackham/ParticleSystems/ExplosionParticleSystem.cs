#region File Description
//-----------------------------------------------------------------------------
// ExplosionParticleSystem.cs
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
    /// Custom particle system for creating the fiery part of the explosions.
    /// </summary>
    public class ExplosionParticleSystem : ParticleSystem
    {
        public ExplosionParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "explosion";

            settings.MaxParticles = 6000;

            settings.Duration = TimeSpan.FromSeconds(2);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 2;
            settings.MaxHorizontalVelocity = 3;

            settings.MinVerticalVelocity = -2;
            settings.MaxVerticalVelocity = 29;

            settings.EndVelocity = 0;

            settings.MinColor = Color.DarkGray;
            settings.MaxColor = Color.Gray;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1;
            settings.MaxStartSize = 1;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 20;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}
