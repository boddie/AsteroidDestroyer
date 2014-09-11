
/**********************************************************/
/**                                                      **/
/**                Author: James Boddie                  **/
/**                Date: 5/5/2014                        **/
/**                                                      **/
/**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using BEPUphysics.Entities.Prefabs;

namespace AsteroidDestroyer
{
    public class Resources
    {
        #region Class Member Variables

        public enum LevelSize
        {
            SMALL = 1,
            MEDIUM = 2,
            LARGE = 3
        }
        public LevelSize GameSize { get; set; }

        public enum AsteroidCount
        {
            TWENTY_FIVE = 25,
            FIFTY = 50,
            SEVENTY = 75
        }
        public AsteroidCount GameCount { get; set; }

        private static Resources _active;

        private Dictionary<string, Texture2D> _textures;
        private Dictionary<string, SoundEffect> _sounds;
        private Dictionary<string, SpriteFont> _fonts;
        private Dictionary<string, Model> _models;

        public int Score { get; set; }
        public bool GameOver { get; set; }
        public Random Rand { get; private set; }
        public int Lives { get; set; }
        public bool shipHit { get; set; }
        public int RemainingAsteroids;
        public bool PlayerWins { get; set; }

        public MobileMesh Ship;

        #endregion

        #region Public Methods

        // I feel most of these are pretty self explanatory. If I saw 
        // a need for comments I put them in.

        public void AddTexture(string key, Texture2D texture)
        {
            _textures.Add(key, texture);
        }

        public Texture2D GetTexture(string key)
        {
            if (!_textures.ContainsKey(key))
                return null;
            return _textures[key];
        }

        public void AddSound(string key, SoundEffect sound)
        {
            _sounds.Add(key, sound);
        }

        public SoundEffect GetSound(string key)
        {
            if (!_sounds.ContainsKey(key))
                return null;
            return _sounds[key];
        }

        public void AddFont(string key, SpriteFont font)
        {
            _fonts.Add(key, font);
        }

        public SpriteFont GetFont(string key)
        {
            if (!_fonts.ContainsKey(key))
                return null;
            return _fonts[key];
        }

        public void AddModel(string key, Model model)
        {
            _models.Add(key, model);
        }

        public Model GetModel(string key)
        {
            if (!_models.ContainsKey(key))
                return null;
            return _models[key];
        }

        public void IterateSize()
        {
            switch (GameSize)
            {
                case LevelSize.SMALL:
                    GameSize = LevelSize.MEDIUM;
                    break;
                case LevelSize.MEDIUM:
                    GameSize = LevelSize.LARGE;
                    break;
                case LevelSize.LARGE:
                    GameSize = LevelSize.SMALL;
                    break;
            }
        }

        public void IterateCount()
        {
            switch (GameCount)
            {
                case AsteroidCount.TWENTY_FIVE:
                    GameCount = AsteroidCount.FIFTY;
                    break;
                case AsteroidCount.FIFTY:
                    GameCount = AsteroidCount.SEVENTY;
                    break;
                case AsteroidCount.SEVENTY:
                    GameCount = AsteroidCount.TWENTY_FIVE;
                    break;
            }
            RemainingAsteroids = (int)GameCount;
        }

        /// <summary>
        /// Used to initialize this singleton class
        /// </summary>
        public static Resources Instance
        {
            get
            {
                if (_active == null)
                {
                    _active = new Resources();
                }
                return _active;
            }
        }

        public bool inSphere(Vector3 position)
        {
            int radius = 0;
            switch (GameSize)
            {
                case LevelSize.SMALL:
                    radius = 300;
                    break;
                case LevelSize.MEDIUM:
                    radius = 450;
                    break;
                case LevelSize.LARGE:
                    radius = 600;
                    break;
            }
            if (position.X * position.X + position.Y * position.Y + position.Z * position.Z <= radius * 20)
                return true;
            return false;
        }

        #endregion

        #region Private Singleton Constructor

        private Resources()
        {
            _textures = new Dictionary<string, Texture2D>();
            _sounds = new Dictionary<string, SoundEffect>();
            _fonts = new Dictionary<string, SpriteFont>();
            _models = new Dictionary<string, Model>();
            Rand = new Random();

            GameSize = LevelSize.MEDIUM;
            GameCount = AsteroidCount.FIFTY;
            RemainingAsteroids = (int)GameCount;
        }

        #endregion
    }
}
