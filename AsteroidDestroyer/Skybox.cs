using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConversionHelper;
using AsteroidDestroyer.Components;

namespace AsteroidDestroyer
{
    public class Skybox : DrawableGameComponent
    {
        private Model _model;
        private Texture2D _texture;
        private GameManager _manager;

        public Skybox(Game game, Model model, Texture2D texture, GameManager manager)
            : base(game)
        {
            _model = model;
            _texture = texture;
            _manager = manager;
        }

        public override void Draw(GameTime gameTime)
        {
            Vector3 cameraPosition = MathConverter.Convert(_manager.Camera.Position);

            Matrix[] transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (_texture != null)
                    {
                        effect.TextureEnabled = true;
                        effect.Texture = _texture;
                    }
                    effect.EnableDefaultLighting();
                    effect.Projection = MathConverter.Convert(_manager.Camera.ProjectionMatrix);
                    effect.View = MathConverter.Convert(_manager.Camera.ViewMatrix);
                    effect.World = Matrix.CreateScale(3000) * Matrix.CreateTranslation(cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
