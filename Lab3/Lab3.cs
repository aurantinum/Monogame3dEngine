using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
namespace Lab3
{
    public class Lab3 : Game
    {
        float viewingVolWidth = 40000f, viewingVolHeight = 40000f, viewingVolHeightAdj = 0f, viewingVolWidthAdj = 0f, nearPlaneAdj = 0f, farPlaneAdj = 0f;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont _font;
        Model model;
        Matrix world, view, projection;
        //view
        Vector3 cameraPosition = new (0, 3, 5);
        Vector3 cameraTarget = Vector3.Zero;
        Vector3 cameraUp = Vector3.Up;

        // world
        Vector3 torusPos, torusRot, torusScale;

        bool debug = false;
        public Lab3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InputManager.Initialize();
            Time.Initialize();
            base.Initialize();
            torusPos = Vector3.Zero;
            torusRot = Vector3.Zero;
            torusScale = Vector3.One;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Torus");
            _font = Content.Load<SpriteFont>("Font");
            foreach(ModelMesh m in model.Meshes)
            {
                foreach (BasicEffect e in m.Effects)
                {
                    e.EnableDefaultLighting();
                    e.PreferPerPixelLighting = true;
                }
            }
        }
        bool perspective = true;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
            //move cam
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.W))
            {
                nearPlaneAdj += Time.ElapsedGameTime;
            }
            else if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.W))
            {
                viewingVolWidthAdj += Time.ElapsedGameTime;
            }
            else if(InputManager.IsKeyDown(Keys.W))
            {
                cameraPosition += Vector3.Up * Time.ElapsedGameTime * 10;
            }
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.S))
            {
                nearPlaneAdj -= Time.ElapsedGameTime;
            }
            else if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.S))
            {
                viewingVolWidthAdj -= Time.ElapsedGameTime;
            }
            else if(InputManager.IsKeyDown(Keys.S))
            {
                cameraPosition += Vector3.Down * Time.ElapsedGameTime * 10;
            }
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.A))
            {
                farPlaneAdj += Time.ElapsedGameTime;
            }
            else if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.A))
            {
                viewingVolHeightAdj += Time.ElapsedGameTime;
            }
            else if(InputManager.IsKeyDown(Keys.A))
            {
                cameraPosition += Vector3.Left * Time.ElapsedGameTime * 10;
            }
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.D))
            {
                farPlaneAdj -= Time.ElapsedGameTime;
            }
            else if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.D))
            {
                viewingVolHeightAdj -= Time.ElapsedGameTime;
            }
            else if(InputManager.IsKeyDown(Keys.D))
            {
                cameraPosition += Vector3.Right * Time.ElapsedGameTime * 10;
            }
            //move obj
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Up))
            {
                torusScale += Vector3.One * Time.ElapsedGameTime;
            }
            else if (InputManager.IsKeyDown(Keys.Up))
            {
                torusPos += Vector3.Up * Time.ElapsedGameTime * 10;
            }
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Down))
            {
                torusScale -= Vector3.One * Time.ElapsedGameTime;
            }
            else if (InputManager.IsKeyDown(Keys.Down))
            {
                torusPos += Vector3.Down * Time.ElapsedGameTime * 10;
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                torusPos += Vector3.Left * Time.ElapsedGameTime * 10;
            }
            if (InputManager.IsKeyDown(Keys.Right))
            {
                torusPos += Vector3.Right * Time.ElapsedGameTime * 10;
            }
            if (InputManager.IsKeyDown(Keys.Insert))
            {
                torusRot += Vector3.Up * Time.ElapsedGameTime;
            }if (InputManager.IsKeyDown(Keys.Delete))
            {
                torusRot -= Vector3.Up * Time.ElapsedGameTime;
            }if (InputManager.IsKeyDown(Keys.Home))
            {
                torusRot += Vector3.Forward * Time.ElapsedGameTime;
            }if (InputManager.IsKeyDown(Keys.End))
            {
                torusRot -= Vector3.Forward * Time.ElapsedGameTime;
            }if (InputManager.IsKeyDown(Keys.PageUp))
            {
                torusRot += Vector3.Right * Time.ElapsedGameTime;
            }if (InputManager.IsKeyDown(Keys.PageDown))
            {
                torusRot -= Vector3.Right * Time.ElapsedGameTime;
            }



            perspective = InputManager.IsKeyDown(Keys.Tab);

            // TODO: Add your update logic here
            world = Matrix.CreateScale(torusScale)*
                Matrix.CreateFromYawPitchRoll(torusRot.X, torusRot.Y, torusRot.Z) *
                Matrix.CreateTranslation(torusPos);
            view = Matrix.CreateLookAt(
               cameraPosition,
               cameraPosition + Vector3.Forward,
               cameraUp);
            if (perspective)
            {
                projection = Matrix.CreatePerspectiveOffCenter(
                    0 + viewingVolWidthAdj,
                    viewingVolWidth + viewingVolWidthAdj,
                    0 + viewingVolHeightAdj,
                    viewingVolHeight + viewingVolHeightAdj,
                    //GraphicsDevice.Viewport.AspectRatio, // screen aspect
                    0.1f + nearPlaneAdj, farPlaneAdj + 1000f);
            }
            else
            {
                projection = Matrix.CreateOrthographicOffCenter(
                    0 + viewingVolWidthAdj,
                    viewingVolWidth + viewingVolWidthAdj,
                    0 + viewingVolHeightAdj,
                    viewingVolHeight + viewingVolHeightAdj,
                    //GraphicsDevice.Viewport.AspectRatio, // screen aspect
                    0.1f + nearPlaneAdj, farPlaneAdj + 1000f);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // 3D model Draw
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            model.Draw(world, view, projection);

          
            _spriteBatch.Begin();
            if (debug)
            {
                _spriteBatch.DrawString(_font, "CameraPos" + cameraPosition.X.ToString("0.00")
                                    + ", " + cameraPosition.Y.ToString("0.00") + ", " + cameraPosition.Z.ToString("0.00")
                                    , Vector2.UnitY * 20, Color.White);
                _spriteBatch.DrawString(_font, "Completed arrow key move", Vector2.UnitY * 40, Color.White);
                _spriteBatch.DrawString(_font, "Completed insert/delete home/end pgup/pgdown yaw pitch and roll controls", Vector2.UnitY * 60, Color.White);
                _spriteBatch.DrawString(_font, "Completed shift+up/down for model size", Vector2.UnitY * 80, Color.White);
                _spriteBatch.DrawString(_font, "Completed space to toggle order for world matrix", Vector2.UnitY * 100, Color.White);
                _spriteBatch.DrawString(_font, "Completed WASD for camera movement", Vector2.UnitY * 120, Color.White);
                _spriteBatch.DrawString(_font, "Completed ctrl WASD for viewing Rectangle", Vector2.UnitY * 140, Color.White);
                _spriteBatch.DrawString(_font, "Completed shift WASD for viewing planes", Vector2.UnitY * 160, Color.White);
                _spriteBatch.DrawString(_font, "Completed tab to toggle orthographic and perspective mode", Vector2.UnitY * 180, Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
