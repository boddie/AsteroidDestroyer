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
    public class GameScreen : Screen
    {
        const int MAX_TIME = 100;
        GameManager _manager;
        float _currentTime;
        SoundEffectInstance _ambientSound;
        float _hitTimer;
        float _hitResetGoal = 3000;

        public GameScreen(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            game.IsMouseVisible = false;

            _manager = new GameManager(game);
            game.Components.Add(_manager);
            _currentTime = 0;
            Resources.Instance.RemainingAsteroids = (int)Resources.Instance.GameSize;
        }

        public override void Draw()
        {
            if (Resources.Instance.RemainingAsteroids == 0)
            {
                Resources.Instance.PlayerWins = true;
                Resources.Instance.GameOver = true;
            }

            Rectangle backgroundRect = new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);
            switch (Resources.Instance.Lives)
            {
                case 2:
                    _spriteBatch.Draw(Resources.Instance.GetTexture("crack1"), backgroundRect, Color.White);
                    break;
                case 1:
                    _spriteBatch.Draw(Resources.Instance.GetTexture("crack2"), backgroundRect, Color.White);
                    break;
                case 0:
                    _spriteBatch.Draw(Resources.Instance.GetTexture("crack3"), backgroundRect, Color.White);
                    Resources.Instance.PlayerWins = false;
                    Resources.Instance.GameOver = true;
                    break;
                default:
                    break;
            }

            Rectangle crosshairRect = new Rectangle(_game.GraphicsDevice.Viewport.Width / 2 - 50, 
                _game.GraphicsDevice.Viewport.Height / 2 - 50, 100, 100);
            if(!Resources.Instance.GameOver)
                _spriteBatch.Draw(Resources.Instance.GetTexture("crosshair"), crosshairRect, Color.White);

            int remainingTime = Math.Max(MAX_TIME - (int)_currentTime, 0);

            _spriteBatch.DrawString(Resources.Instance.GetFont("TitleFont"),
                    remainingTime.ToString(), new Vector2(600, 10), 
                    Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);

            _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Hits Left: " + Resources.Instance.Lives, 
                    new Vector2(50, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);

            _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Score: " + Resources.Instance.Score, 
                    new Vector2(1050, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);

            _spriteBatch.DrawString(Resources.Instance.GetFont("GameFont"),
                    "Remaining Asteroids: " + Resources.Instance.RemainingAsteroids,
                    new Vector2(50, _game.GraphicsDevice.Viewport.Height - 50), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);

            if (Resources.Instance.GameOver && Resources.Instance.PlayerWins)
            {
                Vector2 fontSize = Resources.Instance.GetFont("TitleFont").MeasureString("You Survived!");
                Vector2 position = new Vector2(_game.GraphicsDevice.Viewport.Width / 2 - fontSize.X / 2,
                    _game.GraphicsDevice.Viewport.Height / 2 - fontSize.Y / 2);
                _spriteBatch.DrawString(Resources.Instance.GetFont("TitleFont"),
                    "You Survived!", position, Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            else if (Resources.Instance.GameOver && !Resources.Instance.PlayerWins)
            {
                Vector2 fontSize = Resources.Instance.GetFont("TitleFont").MeasureString("You Died!");
                Vector2 position = new Vector2(_game.GraphicsDevice.Viewport.Width / 2 - fontSize.X / 2,
                    _game.GraphicsDevice.Viewport.Height / 2 - fontSize.Y / 2);
                _spriteBatch.DrawString(Resources.Instance.GetFont("TitleFont"),
                    "You Died!", position, Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
        }

        public override void Update(ref Screen activeScreen, GameTime gameTime)
        {
            if (Resources.Instance.shipHit && _hitTimer == 0)
            {
                Resources.Instance.Lives = Math.Max(Resources.Instance.Lives - 1, 0);
                Resources.Instance.GetSound("glass").Play();
            }
            if (Resources.Instance.shipHit && _hitTimer < _hitResetGoal)
            {
                _hitTimer += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                _hitTimer = 0;
                Resources.Instance.shipHit = false;
            }

            if (_ambientSound == null)
            {
                _ambientSound = Resources.Instance.GetSound("game").CreateInstance();
                _ambientSound.IsLooped = true;
                _ambientSound.Volume = 0.5f;
                _ambientSound.Play();
            }

            if(!Resources.Instance.GameOver)
                _currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_currentTime >= MAX_TIME)
            {
                Resources.Instance.PlayerWins = false;
                Resources.Instance.GameOver = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Resources.Instance.Score = 0;

                foreach (KeyValuePair<int, MobileMeshModel> model in _manager.modelLookup)
                {
                    _game.Components.Remove(model.Value);
                }
                _manager.modelLookup.Clear();

                foreach (Emitter e in _manager.emitters)
                {
                    _game.Components.Remove(e.System);
                }
                _game.Components.Remove(_manager.skybox);
                _game.Components.Remove(_manager.GameBoundary);
                _game.Components.Remove(_manager.Ship);
                _game.Components.Remove(_manager);
                _ambientSound.Stop();
                activeScreen = new StartScreen(_game, _spriteBatch);
            }
        }
    }
}
