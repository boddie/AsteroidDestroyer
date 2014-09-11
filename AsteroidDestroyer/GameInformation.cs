using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidDestroyer
{
    public class GameInformation
    {
        private static GameInformation _active;

        public Random Rand { get; private set; }

        private GameInformation()
        {
            Rand = new Random();
        }

        public static GameInformation Instance
        {
            get
            {
                if (_active == null)
                {
                    _active = new GameInformation();
                }
                return _active;
            }
        }
    }
}
