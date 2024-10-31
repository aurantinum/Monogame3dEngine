using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
namespace Lab4
{
    public class Lab4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Model torus, sphere;
        Camera camera;
        Transform torusTransform, sphereTransform, cameraTransform;

        public Lab4()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            // TODO: use this.Content to load your game content here
            torus = Content.Load<Model>("Torus");
            sphere = Content.Load<Model>("Sphere");
            torusTransform = new Transform();
            cameraTransform = new Transform();
            sphereTransform = new Transform();
            sphereTransform.LocalPosition = Vector3.Right * 2;
            torusTransform.Parent = sphereTransform;
            cameraTransform.LocalPosition = Vector3.Backward * 5;
            torusTransform.LocalPosition = Vector3.Left * 2;

            camera = new Camera();
            camera.Transform = cameraTransform;
            foreach (ModelMesh m in torus.Meshes)
            {
                foreach (BasicEffect e in m.Effects)
                {
                    e.EnableDefaultLighting();
                    e.PreferPerPixelLighting = true;
                }
            }foreach (ModelMesh m in sphere.Meshes)
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
            if (InputManager.IsKeyDown(Keys.W))
                cameraTransform.LocalPosition += cameraTransform.Forward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S))
                cameraTransform.LocalPosition += cameraTransform.Backward * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A))
                cameraTransform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D))
                cameraTransform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
            if(InputManager.IsKeyDown(Keys.E))
                sphereTransform.Rotate(sphereTransform.Forward, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.F))
                sphereTransform.Rotate(sphereTransform.Forward, -Time.ElapsedGameTime);
            if(InputManager.IsKeyDown(Keys.P))
                torusTransform.Rotate(torusTransform.Forward, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Q))
                torusTransform.Rotate(torusTransform.Forward, -Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Up))
                sphereTransform.LocalPosition += Vector3.Forward * 5 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Down))
                sphereTransform.LocalPosition += Vector3.Backward * 5 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left))
                sphereTransform.LocalPosition += Vector3.Left * 5 * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Right))
                sphereTransform.LocalPosition += Vector3.Right * 5 * Time.ElapsedGameTime;

            camera.Transform = cameraTransform;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            torus.Draw(torusTransform.World, camera.View, camera.Projection);
            sphere.Draw(sphereTransform.World, camera.View, camera.Projection);

            _spriteBatch.Begin();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
