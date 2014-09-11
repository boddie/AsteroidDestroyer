using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace AsteroidDestroyer.Components
{
    public class StartScreen : Screen
    {
        ButtonState _prevState;
        bool _loadScene, _startLoad;
        SoundEffectInstance _ambientSound;
        float _scrollStartY;

        public StartScreen(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            game.IsMouseVisible = true;
            Resources.Instance.GameOver = false;
            _prevState = Mouse.GetState().LeftButton;
            _loadScene = false;
            _startLoad = false;
            _scrollStartY = _game.GraphicsDevice.Viewport.Height  + 20;
            Resources.Instance.Lives = 3;
            Resources.Instance.Score = 0;
        }

        public override void Draw()
        {
            DrawScrollingText();

            Rectangle backgroundRect = new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);
            _spriteBatch.Draw(Resources.Instance.GetTexture("stars"), backgroundRect, Color.White);
            _spriteBatch.DrawString(Resources.Instance.GetFont("TitleFont"), 
                "Asteroid Destroyer", new Vector2(60, 50), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);

            ButtonState currentState = Mouse.GetState().LeftButton;

            Rectangle controlRect = new Rectangle(60, 125, 130, 50);
            if (controlRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Controls", new Vector2(60, 125), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);

                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    "W - Forward Thrust", new Vector2(60, 165), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    "S - Backward Thrust", new Vector2(60, 185), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    "A - Left Thrust", new Vector2(60, 205), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    "D - Right Thrust", new Vector2(60, 225), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    "Q - Brake", new Vector2(60, 245), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    "Left Mouse - Select / Fire Laser", new Vector2(60, 265), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    "Escape - Back", new Vector2(60, 285), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            else
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Controls", new Vector2(60, 125), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }

            Rectangle sizeRect = new Rectangle(60, 550, 300, 50);
            if (sizeRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                if (_prevState != currentState && currentState == ButtonState.Pressed)
                    Resources.Instance.IterateSize();
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Level Size: " + Resources.Instance.GameSize.ToString(), new Vector2(60, 550), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            else
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Level Size: " + Resources.Instance.GameSize.ToString(), new Vector2(60, 550), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }

            Rectangle countRect = new Rectangle(60, 600, 400, 50);
            if (countRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                if (_prevState != currentState && currentState == ButtonState.Pressed)
                    Resources.Instance.IterateCount();
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Asteroid Count: " + Resources.Instance.GameCount.ToString(), new Vector2(60, 600), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            else
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Asteroid Count: " + Resources.Instance.GameCount.ToString(), new Vector2(60, 600), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }

            Rectangle startRect = new Rectangle(60, 650, 300, 50);
            if (startRect.Contains(Mouse.GetState().X, Mouse.GetState().Y))
            {
                if (_prevState != currentState && currentState == ButtonState.Pressed)
                    _startLoad = true;
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Start Game", new Vector2(60, 650), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            else
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Start Game", new Vector2(60, 650), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }

            if (_startLoad)
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Loading...", new Vector2(1100, 650), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _loadScene = true;
            }

            _prevState = Mouse.GetState().LeftButton;
        }

        public override void Update(ref Screen activeScreen, GameTime gameTime)
        {
            _scrollStartY -= (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.02f;

            if (_ambientSound == null)
            {
                _ambientSound = Resources.Instance.GetSound("theme").CreateInstance();
                _ambientSound.IsLooped = true;
                _ambientSound.Volume = 0.25f;
                _ambientSound.Play();
            }

            if (_loadScene)
            {
                _ambientSound.Stop();
                activeScreen = new GameScreen(_game, _spriteBatch);
            }
        }

        private void DrawScrollingText()
        {
            string[] toDisplay = new string[] 
                { "Not so long ago...",
                  " ",
                  "in the galaxy in which you currently reside...",
                  "You somehow ended up in a random sphere in",
                  "outer space. Luckily you are alive and in a",
                  "space ship of some sort... But unluckily...",
                  "you are surrounded by Asteroids that are going",
                  "to hit you unless you destroy them with your",
                  "lasers. Your ship can only take a few hits",
                  "before it cracks open and throws you into space.",
                  "That or you get impaled by it... either way",
                  "it does not sound very delightful. At least ",
                  "somehow there is sound in space all of a sudden!",
                  "In all seriousness though... You must destroy all",
                  "of the asteroids in 100 seconds! After 100 seconds",
                  "your remaining oxygen will be depleted!",
                  " ",
                  "In other words... Good Luck!" };

            float offset = _scrollStartY;
            foreach (string s in toDisplay)
            {
                Vector2 fontSize = Resources.Instance.GetFont("ScrollFont").MeasureString(s);
                Vector2 location = new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f + (_game.GraphicsDevice.Viewport.Width * 0.5f / 2 - fontSize.X / 2), offset);
                _spriteBatch.DrawString(Resources.Instance.GetFont("ScrollFont"),
                    s, location, Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                offset += fontSize.Y * 1.5f;
            }
            Resources.Instance.GetFont("GameFont");
        }
    }
}
