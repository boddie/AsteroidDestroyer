using AsteroidDestroyer.Particles.Systems;
using Microsoft.Xna.Framework;

namespace AsteroidDestroyer.Particles
{
    class ParticleSystems
    {
        private static Game _game;
        public static ParticleSystem TorpedoInstance
        {
            get
            {
                var tps = new TorpedoParticleSystem(_game, _game.Content); 
                tps.DrawOrder = 999;
                _game.Components.Add(tps);
                return tps;
            } 
        }

        public static ParticleSystem WhiteHoleInstance
        {
            get
            {
                var whps = new WhiteHoleParticleSystem(_game, _game.Content);
                whps.DrawOrder = 999;
                _game.Components.Add(whps);
                return whps;
            }
        }

        public static ParticleSystem BlackHoleInstance
        {
            get
            {
                var bhps = new BlackHoleParticleSystem(_game, _game.Content);
                bhps.DrawOrder = 999;
                _game.Components.Add(bhps);
                return bhps;
            }
        }

        public static void Init(Game game)
        {
            _game = game;
        }
    }
}
