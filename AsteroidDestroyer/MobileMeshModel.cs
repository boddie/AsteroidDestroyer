using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using AsteroidDestroyer.Components;
using Vector3 = BEPUutilities.Vector3;
using System;

namespace AsteroidDestroyer
{
    public class MobileMeshModel : DrawableGameComponent
    {
        public MobileMesh _mesh;

        Model _model;
        float _scale;
        GameManager _manager;
        bool _controlled;

        public MobileMeshModel(Game game, MobileMesh mesh, Model model, float scale, GameManager manager)
            : base(game)
        {
            this._mesh = mesh;
            this._model = model;
            this._scale = scale;
            this._manager = manager;
            this._controlled = false;
        }

        public MobileMeshModel(Game game, MobileMesh mesh, Model model, float scale, GameManager manager, bool controlled)
            : base(game)
        {
            this._mesh = mesh;
            this._model = model;
            this._scale = scale;
            this._manager = manager;
            this._controlled = controlled;
            if (controlled)
                Resources.Instance.Ship = _mesh;
        }

        public override void Update(GameTime gameTime)
        {
            if (Resources.Instance.GameOver)
            {
                _mesh.LinearVelocity = Vector3.Zero;
                _mesh.AngularVelocity = Vector3.Zero;
            }

            if (!Resources.Instance.inSphere(new Microsoft.Xna.Framework.Vector3(_mesh.Position.X, _mesh.Position.Y, _mesh.Position.Z)))
            {
                _mesh.LinearVelocity = _mesh.LinearVelocity * -1;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix worldMatrix = MathConverter.Convert(_mesh.WorldTransform);

            Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = MathConverter.Convert(_manager.Camera.ProjectionMatrix);
                    effect.View = MathConverter.Convert(_manager.Camera.ViewMatrix);
                    effect.World = Matrix.CreateScale(_scale) * worldMatrix;
                }

                if(!_controlled)
                    mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
