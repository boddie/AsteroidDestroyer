
using BEPUutilities;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Audio;

namespace AsteroidDestroyer
{
    /// <summary>
    /// Basic camera class supporting mouse/keyboard/gamepad-based movement.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector3 Position { get; set; }
        float yaw;
        float pitch;
        SoundEffectInstance _moveSound;

        /// <summary>
        /// Gets or sets the yaw rotation of the camera.
        /// </summary>
        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = MathHelper.WrapAngle(value);
            }
        }
        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                pitch = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix ViewMatrix { get; private set; }
        /// <summary>
        /// Gets or sets the projection matrix of the camera.
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }

        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix { get; private set; }

        /// <summary>
        /// Gets the game owning the camera.
        /// </summary>
        public AsteroidDestroyer Game { get; private set; }

        /// <summary>
        /// Constructs a new camera.
        /// </summary>
        /// <param name="game">Game that this camera belongs to.</param>
        /// <param name="position">Initial position of the camera.</param>
        /// <param name="speed">Initial movement speed of the camera.</param>
        public Camera(AsteroidDestroyer game, Vector3 position, float speed)
        {
            Game = game;
            Position = position;
            Speed = speed;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfViewRH(MathHelper.PiOver4, 4f / 3f, .1f, 10000.0f);
            Mouse.SetPosition(200, 200);
        }

        public Vector3 Forward
        {
            get
            {
                return WorldMatrix.Forward;
            }

        }

        /// <summary>
        /// Moves the camera forward using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        public void MoveForward(float dt)
        {
            Position += WorldMatrix.Forward * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera right using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveRight(float dt)
        {
            Position += WorldMatrix.Right * (dt * Speed);
        }
        /// <summary>
        /// Moves the camera up using its speed.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        /// 
        public void MoveUp(float dt)
        {
            Position += new Vector3(0, (dt * Speed), 0);
        }

        /// <summary>
        /// Updates the camera's view matrix.
        /// </summary>
        /// <param name="dt">Timestep duration.</param>
        public void Update(float dt)
        {
            if (!Resources.Instance.GameOver)
            {
                if (_moveSound == null)
                {
                    _moveSound = Resources.Instance.GetSound("move").CreateInstance();
                    _moveSound.IsLooped = true;
                    _moveSound.Volume = 0.1f;
                }

                //Turn based on mouse input.
                Yaw += (200 - Game.MouseState.X) * dt * .12f;
                Pitch += (200 - Game.MouseState.Y) * dt * .12f;

                Mouse.SetPosition(200, 200);

                WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);


                float distance = Speed * dt;

                this.Position = Resources.Instance.Ship.Position;

                bool playSound = false;

                //Scoot the camera around depending on what keys are pressed.
                if (Game.KeyboardState.IsKeyDown(Keys.W))
                {
                    Resources.Instance.Ship.LinearVelocity += 2 * (WorldMatrix.Forward * (dt * Speed));
                    playSound = true;
                }
                if (Game.KeyboardState.IsKeyDown(Keys.S))
                {
                    Resources.Instance.Ship.LinearVelocity += -2 * (WorldMatrix.Forward * (dt * Speed));
                    playSound = true;
                }
                if (Game.KeyboardState.IsKeyDown(Keys.A))
                {
                    Resources.Instance.Ship.LinearVelocity += 2 * (Vector3.Cross(WorldMatrix.Up, WorldMatrix.Forward) * (dt * Speed));
                    playSound = true;
                }
                if (Game.KeyboardState.IsKeyDown(Keys.D))
                {
                    Resources.Instance.Ship.LinearVelocity += -2 * (Vector3.Cross(WorldMatrix.Up, WorldMatrix.Forward) * (dt * Speed));
                    playSound = true;
                }
                if (Game.KeyboardState.IsKeyDown(Keys.Q))
                {
                    float x = (Resources.Instance.Ship.LinearVelocity.X + 0) * 0.99f;
                    float y = (Resources.Instance.Ship.LinearVelocity.Y + 0) * 0.99f;
                    float z = (Resources.Instance.Ship.LinearVelocity.Z + 0) * 0.99f;

                    Resources.Instance.Ship.LinearVelocity = new Vector3(x, y, z);
                    playSound = true;
                }

                if (playSound)
                    _moveSound.Play();
                else if (_moveSound.State == SoundState.Playing)
                    _moveSound.Stop();

                WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);
                ViewMatrix = Matrix.Invert(WorldMatrix);
            }
        }
    }
}
