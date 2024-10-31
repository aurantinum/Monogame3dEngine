using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab5
{
    public class Lab5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Model torus;
        Transform cameraTransform, torusTransform;
        Camera camera;
        Effect effect;
        Texture2D texture;
        int mode = 0;
        public Lab5()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            torus = Content.Load<Model>("Torus");
            effect = Content.Load<Effect>("SimpleShading");
            texture = Content.Load<Texture2D>("Square");
            torusTransform = new Transform();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 5;
            camera = new Camera();
            camera.Transform = cameraTransform;
            foreach (ModelMesh m in torus.Meshes)
            {
                foreach (BasicEffect e in m.Effects)
                {
                    e.EnableDefaultLighting();
                    e.PreferPerPixelLighting = true;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyPressed(Keys.Tab))
                mode++;
            if (InputManager.IsKeyDown(Keys.W))
                cameraTransform.LocalPosition += cameraTransform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S))
                cameraTransform.LocalPosition += cameraTransform.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A))
                cameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D))
                cameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            effect.CurrentTechnique = effect.Techniques[mode%4]; //"0" is the first technique
            effect.Parameters["World"].SetValue(torusTransform.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
                                                                     Vector3.Right * 5);
            effect.Parameters["CameraPosition"].SetValue(cameraTransform.Position);
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0, 0));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(1, 1, 1f));
            effect.Parameters["DiffuseTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (ModelMesh mesh in torus.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            0,
                            part.PrimitiveCount);
                    }
            }

            base.Draw(gameTime);
        }
    }
}
