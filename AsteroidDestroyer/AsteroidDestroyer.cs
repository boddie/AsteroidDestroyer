using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BEPUphysics.Entities;
using BEPUphysics;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Vector3 = BEPUutilities.Vector3;
using Matrix = BEPUutilities.Matrix;
using System.Collections.Generic;
using AsteroidDestoyer.Components;
using Microsoft.Xna.Framework.Audio;


namespace AsteroidDestroyer
{
    public class AsteroidDestroyer : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;

        private ScreenManager screenManager;
        private SpriteBatch spriteBatch;

        public KeyboardState KeyboardState;
        public MouseState MouseState;

        public AsteroidDestroyer()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);

            // Add the screen manager component used to control the 
            // flow of screens throughout the game.
            screenManager = new ScreenManager(this);
            this.Components.Add(screenManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Resources.Instance.AddFont("TitleFont", Content.Load<SpriteFont>(@"Fonts/TitleFont"));
            Resources.Instance.AddFont("GameFont", Content.Load<SpriteFont>(@"Fonts/GameFont"));
            Resources.Instance.AddFont("ScrollFont", Content.Load<SpriteFont>(@"Fonts/ScrollFont"));

            Resources.Instance.AddTexture("stars", Content.Load<Texture2D>(@"Textures\SkyboxMap"));
            Resources.Instance.AddTexture("crosshair", Content.Load<Texture2D>(@"Textures\crosshair"));
            Resources.Instance.AddTexture("crack1", Content.Load<Texture2D>(@"Textures\crack1"));
            Resources.Instance.AddTexture("crack2", Content.Load<Texture2D>(@"Textures\crack2"));
            Resources.Instance.AddTexture("crack3", Content.Load<Texture2D>(@"Textures\crack3"));

            Resources.Instance.AddModel("asteroid1", Content.Load<Model>(@"Models\Asteroid1"));
            Resources.Instance.AddModel("asteroid2", Content.Load<Model>(@"Models\Asteroid2"));
            Resources.Instance.AddModel("asteroid3", Content.Load<Model>(@"Models\Asteroid3"));
            Resources.Instance.AddModel("asteroid4", Content.Load<Model>(@"Models\Asteroid4"));
            Resources.Instance.AddModel("asteroid5", Content.Load<Model>(@"Models\Asteroid5"));
            Resources.Instance.AddModel("asteroid6", Content.Load<Model>(@"Models\Asteroid6"));
            Resources.Instance.AddModel("skybox", Content.Load<Model>(@"Models\Skybox"));
            Resources.Instance.AddModel("ship", Content.Load<Model>(@"Models\sphere"));
            Resources.Instance.AddModel("boundary", Content.Load<Model>(@"Models\border"));

            Resources.Instance.AddSound("explosion", Content.Load<SoundEffect>(@"Sounds/explosion"));
            Resources.Instance.AddSound("game", Content.Load<SoundEffect>(@"Sounds/game"));
            Resources.Instance.AddSound("theme", Content.Load<SoundEffect>(@"Sounds/theme"));
            Resources.Instance.AddSound("move", Content.Load<SoundEffect>(@"Sounds/move"));
            Resources.Instance.AddSound("laser", Content.Load<SoundEffect>(@"Sounds/laser"));
            Resources.Instance.AddSound("glass", Content.Load<SoundEffect>(@"Sounds/glass"));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, null);
            base.Draw(gameTime);
            spriteBatch.End();

            // Set states ready for 3D  
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap; 
        }
    }
}
