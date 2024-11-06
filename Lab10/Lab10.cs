using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab10
{
    public class Lab10 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        TerrainRenderer terrain;
        Camera camera;
        Effect effect;
        public Lab10()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);
            Time.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position = Vector3.Zero;
            terrain = new TerrainRenderer(Content.Load<Texture2D>("Heightmap"),
                Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("Normalmap");
            terrain.Transform = new Transform();
            terrain.Transform.Scale *= new Vector3(1, 5, 1);
            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.3f, 0.3f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0f, 0f, 0.1f));
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyDown(Keys.W)) // move forward
                camera.Transform.LocalPosition += Vector3.Transform(Vector3.Forward * Time.ElapsedGameTime * 5, camera.Transform.Rotation);
            if (InputManager.IsKeyDown(Keys.S)) // move forward
                camera.Transform.LocalPosition += Vector3.Transform(Vector3.Backward * Time.ElapsedGameTime * 5, camera.Transform.Rotation);
            if (InputManager.IsKeyDown(Keys.A)) // move forward
                camera.Transform.LocalPosition += Vector3.Transform(Vector3.Left * Time.ElapsedGameTime * 5, camera.Transform.Rotation);
            if (InputManager.IsKeyDown(Keys.D)) // move forward
                camera.Transform.LocalPosition += Vector3.Transform(Vector3.Right * Time.ElapsedGameTime * 5, camera.Transform.Rotation);
            if (InputManager.IsKeyDown(Keys.Left)) // move forward
                camera.Transform.Rotate( Vector3.Up , Time.ElapsedGameTime * 5);
            if (InputManager.IsKeyDown(Keys.Right)) // move forward
                camera.Transform.Rotate( Vector3.Up,  -Time.ElapsedGameTime * 5);

            camera.Transform.LocalPosition = new Vector3(
                camera.Transform.LocalPosition.X,
                terrain.GetAltitude(camera.Transform.LocalPosition),
                camera.Transform.LocalPosition.Z) + Vector3.Up;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
