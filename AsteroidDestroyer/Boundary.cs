using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.BroadPhaseEntries;
using AsteroidDestroyer.Components;

namespace AsteroidDestroyer
{
    public class Boundary : DrawableGameComponent
    {
        Model _model;
        GameManager _manager;

        public Boundary(Game game, Model model, GameManager manager)
            : base(game)
        {
            this._model = model;
            this._manager = manager;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix worldMatrix = Matrix.Identity;
            switch (Resources.Instance.GameSize)
            {
                case Resources.LevelSize.SMALL:
                    worldMatrix = Matrix.Identity * Matrix.CreateScale(85);
                    break;
                case Resources.LevelSize.MEDIUM:
                    worldMatrix = Matrix.Identity * Matrix.CreateScale(100);
                    break;

                case Resources.LevelSize.LARGE:
                    worldMatrix = Matrix.Identity * Matrix.CreateScale(115);
                    break;
            }
            

            Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = MathConverter.Convert(_manager.Camera.ProjectionMatrix);
                    effect.View = MathConverter.Convert(_manager.Camera.ViewMatrix);
                    effect.World = worldMatrix;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
