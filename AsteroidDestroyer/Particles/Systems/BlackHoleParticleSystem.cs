#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace AsteroidDestroyer.Particles.Systems
{
    /// <summary>
    /// Custom particle system for creating a flame effect.
    /// </summary>
    class BlackHoleParticleSystem : ParticleSystem
    {
        public BlackHoleParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = @"Particles\Textures\blackhole";

            settings.MaxParticles = 2400;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = -1;
            settings.MaxHorizontalVelocity = 1;
            settings.EndVelocity = 0;

            settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = 1;

            settings.Gravity = new Vector3(0, 0, 0);

            settings.MinColor = new Color(255, 255, 255, 120);
            settings.MaxColor = new Color(255, 255, 255, 240);

            settings.MinStartSize = 2;
            settings.MaxStartSize = 6;

            settings.MinEndSize = 2;
            settings.MaxEndSize = 20;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}
