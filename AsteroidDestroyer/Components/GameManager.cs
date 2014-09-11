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
using System;
using AsteroidDestroyer.Particles;
using AsteroidDestroyer.Particles.Systems;
using ConversionHelper;

namespace AsteroidDestroyer.Components
{
    public class GameManager : DrawableGameComponent
    {

        private Space space;
        public Camera Camera;
        public Skybox skybox;
        public Dictionary<int, MobileMeshModel> modelLookup;
        public Boundary GameBoundary;
        public MobileMeshModel Ship;

        public KeyboardState KeyboardState;
        public MouseState MouseState;

        private AsteroidSpawner spawner;

        private Game game;

        ButtonState _prevState;
        BasicEffect basicEffect;

        public List<Emitter> emitters;

        public GameManager(Game game)
            : base(game)
        {
            this.game = game;
        }

        public override void Initialize()
        {
            Camera = new Camera((AsteroidDestroyer)game, new Vector3(0, 3, 10), 5f);

            skybox = new Skybox(game, Resources.Instance.GetModel("skybox"), Resources.Instance.GetTexture("stars"), this);
            game.Components.Add(skybox);

            spawner = new AsteroidSpawner((int)Resources.Instance.GameSize, (int)Resources.Instance.GameCount);

            modelLookup = new Dictionary<int, MobileMeshModel>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            space = new Space();

            CreateAsteroids();
            LoadShip();
            LoadBoundary();

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Microsoft.Xna.Framework.Matrix.CreateOrthographicOffCenter
            (0, GraphicsDevice.Viewport.Width,     // left, right
            GraphicsDevice.Viewport.Height, 0,    // bottom, top
            0, 1);

            emitters = new List<Emitter>();

            base.LoadContent();
        }

        bool _shoot = false;

        public override void Draw(GameTime gameTime)
        {
            if (_shoot)
            {
                shoot();
            }

            base.Draw(gameTime);
        }

        private void shoot()
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();
            var vertices = new VertexPositionColor[4];
            vertices[0].Position = new Microsoft.Xna.Framework.Vector3(50, GraphicsDevice.Viewport.Height, 0);
            vertices[0].Color = Color.Transparent;
            vertices[1].Position = new Microsoft.Xna.Framework.Vector3(GraphicsDevice.Viewport.Width / 2 - 20, GraphicsDevice.Viewport.Height / 2, 0);
            vertices[1].Color = Color.White;
            vertices[2].Position = new Microsoft.Xna.Framework.Vector3(GraphicsDevice.Viewport.Width - 50, GraphicsDevice.Viewport.Height, 0);
            vertices[2].Color = Color.Transparent;
            vertices[3].Position = new Microsoft.Xna.Framework.Vector3(GraphicsDevice.Viewport.Width / 2 + 20, GraphicsDevice.Viewport.Height / 2, 0);
            vertices[3].Color = Color.White;

            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 2);

            _shoot = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Stack<Emitter> toRemove = new Stack<Emitter>();
            foreach (Emitter e in emitters)
            {
                e.Update(gameTime, Camera);
                if (e.Remove)
                {
                    toRemove.Push(e);
                }
            }
            while (toRemove.Count > 0)
                emitters.Remove(toRemove.Pop());

            Resources.Instance.RemainingAsteroids = modelLookup.Count;

            if (!Resources.Instance.GameOver)
            {
                ButtonState currentState = Mouse.GetState().LeftButton;

                Camera.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                RayCastResult rayCastResult;
                Vector3 rayStart = Camera.Position + Camera.Forward * 5;
                Vector3 rayDirection = Camera.Forward;
                space.RayCast(new BEPUutilities.Ray(rayStart, rayDirection), out rayCastResult);

                if (currentState != _prevState && _prevState == ButtonState.Pressed)
                {
                    _shoot = true;
                    Resources.Instance.GetSound("laser").Play();
                }

                try
                {
                    int obj = Convert.ToInt32(rayCastResult.HitObject.Tag);

                    if (currentState != _prevState && _prevState == ButtonState.Pressed)
                    {
                        Resources.Instance.Score += 100;
                        Resources.Instance.GetSound("explosion").Play();
                        Vector3 pos = new Vector3(modelLookup[obj]._mesh.Position.X, modelLookup[obj]._mesh.Position.Y, modelLookup[obj]._mesh.Position.Z);
                        Console.WriteLine(pos.ToString());
                        emitters.Add(new Emitter(game, pos));
                        space.Remove(modelLookup[obj]._mesh);
                        game.Components.Remove(modelLookup[obj]);
                        modelLookup.Remove(obj);
                    }
                }
                catch (Exception) { ;}

                space.Update();
                _prevState = currentState;
            }
        }

        private void LoadShip()
        {
            Vector3[] vertices = new Vector3[0];
            int[] indices = new int[0];
            Model shipModel = Resources.Instance.GetModel("ship");
            int mass = 5;
            Vector3 scale = new Vector3(3, 3, 3);

            ModelDataExtractor.GetVerticesAndIndicesFromModel(Resources.Instance.GetModel("asteroid1"), out vertices, out indices);
            MobileMesh mobMesh = new MobileMesh(vertices, indices, new AffineTransform(scale, BEPUutilities.Quaternion.Identity, BEPUutilities.Vector3.Zero), BEPUphysics.CollisionShapes.MobileMeshSolidity.Solid, mass);
            mobMesh.Tag = "ship";
            mobMesh.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            space.Add(mobMesh);
            mobMesh.AngularDamping = 0;
            mobMesh.LinearDamping = 0;
            mobMesh.LinearVelocity = BEPUutilities.Vector3.Zero;
            mobMesh.AngularVelocity = BEPUutilities.Vector3.Zero;
            MobileMeshModel model = new MobileMeshModel(game, mobMesh, shipModel, scale.X, this, true);
            Ship = model;
            game.Components.Add(model);
        }

        private void LoadBoundary()
        {
            GameBoundary = new Boundary(game, Resources.Instance.GetModel("boundary"), this);
            game.Components.Add(GameBoundary);
        }

        private void CreateAsteroids()
        {
            Vector3[] vertices = new Vector3[0];
            int[] indices = new int[0];
            Model asteroidPicked = Resources.Instance.GetModel("asteroid1");
            int mass = 0;
            Vector3 scale = new Vector3(0, 0, 0);

            List<Vector3> asteroidLocations = spawner.GetAsteroidLocations();
            for (int i = 0; i < asteroidLocations.Count; i++)
            {
                int rand = GameInformation.Instance.Rand.Next(0, 5);
                switch (rand)
                {
                    case 0:
                        ModelDataExtractor.GetVerticesAndIndicesFromModel(Resources.Instance.GetModel("asteroid1"), out vertices, out indices);
                        asteroidPicked = Resources.Instance.GetModel("asteroid1");
                        scale = new Vector3(3, 3, 3);
                        mass = 5;
                        break;
                    case 1:
                        ModelDataExtractor.GetVerticesAndIndicesFromModel(Resources.Instance.GetModel("asteroid2"), out vertices, out indices);
                        asteroidPicked = Resources.Instance.GetModel("asteroid2");
                        scale = new Vector3(3, 3, 3);
                        mass = 5;
                        break;
                    case 2:
                        ModelDataExtractor.GetVerticesAndIndicesFromModel(Resources.Instance.GetModel("asteroid3"), out vertices, out indices);
                        asteroidPicked = Resources.Instance.GetModel("asteroid3");
                        scale = new Vector3(5, 5, 5);
                        mass = 30;
                        break;
                    case 3:
                        ModelDataExtractor.GetVerticesAndIndicesFromModel(Resources.Instance.GetModel("asteroid4"), out vertices, out indices);
                        asteroidPicked = Resources.Instance.GetModel("asteroid4");
                        scale = new Vector3(5, 5, 5);
                        mass = 30;
                        break;
                    case 4:
                        ModelDataExtractor.GetVerticesAndIndicesFromModel(Resources.Instance.GetModel("asteroid5"), out vertices, out indices);
                        asteroidPicked = Resources.Instance.GetModel("asteroid5");
                        scale = new Vector3(6, 6, 6);
                        mass = 45;
                        break;
                    case 5:
                        ModelDataExtractor.GetVerticesAndIndicesFromModel(Resources.Instance.GetModel("asteroid6"), out vertices, out indices);
                        asteroidPicked = Resources.Instance.GetModel("asteroid6");
                        scale = new Vector3(6, 6, 6);
                        mass = 45;
                        break;
                }

                MobileMesh mobMesh = new MobileMesh(vertices, indices, new AffineTransform(scale, BEPUutilities.Quaternion.Identity, asteroidLocations[i]), BEPUphysics.CollisionShapes.MobileMeshSolidity.Solid, mass);
                mobMesh.CollisionInformation.Tag = i.ToString();
                space.Add(mobMesh);
                mobMesh.AngularDamping = 0;
                mobMesh.LinearDamping = 0;
                mobMesh.LinearVelocity = new Vector3(GameInformation.Instance.Rand.Next(0, 30) - 15, GameInformation.Instance.Rand.Next(0, 30) - 15, GameInformation.Instance.Rand.Next(0, 30) - 15);
                mobMesh.AngularVelocity = new Vector3(GameInformation.Instance.Rand.Next(1, 10) - 5, GameInformation.Instance.Rand.Next(1, 10) - 5, GameInformation.Instance.Rand.Next(1, 10) - 5);
                MobileMeshModel model = new MobileMeshModel(game, mobMesh, asteroidPicked, scale.X, this);
                modelLookup.Add(i, model);
                game.Components.Add(model);
            }
        }

        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            try
            {
                int obj = Convert.ToInt32(other.Tag);
                Resources.Instance.shipHit = true;
            }
            catch (Exception) { ;}
        }
    }

    public class Emitter
    {
        public ExplosionParticleSystem System;
        ParticleEmitter ParticleEmitter;
        Vector3 Position;
        const float MAX_LIFE = 1500;
        float Life;
        Game theGame;

        public bool Remove;

        public Emitter(Game game, Vector3 position)
        {
            theGame = game;
            System = new ExplosionParticleSystem(game, game.Content);
            game.Components.Add(System);
            ParticleEmitter = new ParticleEmitter(System, 100, MathConverter.Convert(position));
            Life = 0;
            Remove = false;
            this.Position = position;
        }

        public void Update(GameTime gameTime, Camera Camera)
        {
            Life += gameTime.ElapsedGameTime.Milliseconds;
            ParticleEmitter.ParticleSystem.SetCamera(MathConverter.Convert(Camera.ViewMatrix), MathConverter.Convert(Camera.ProjectionMatrix));
            ParticleEmitter.Update(gameTime, MathConverter.Convert(Position));
            if (Life > MAX_LIFE)
            {
                theGame.Components.Remove(System);
                Remove = true;
            }
        }
    }
}
