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

namespace AsteroidDestroyer.Particles.Systems
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
            settings.TextureName = @"Textures\explosion";

            settings.MaxParticles = 100;

            settings.Duration = TimeSpan.FromSeconds(2);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 4;
            settings.MaxHorizontalVelocity = 6;

            settings.MinVerticalVelocity = -4;
            settings.MaxVerticalVelocity = 6;

            settings.EndVelocity = 0;

            settings.MinColor = Color.DarkGray;
            settings.MaxColor = Color.Gray;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 3;
            settings.MaxStartSize = 8;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 20;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
