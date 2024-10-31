using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab002
{
    public class Lab002 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Sprite sprite;
        public Lab002()
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
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D t = Content.Load<Texture2D>("Square");
            SpiralMover.SpiralMove(t, new Vector2(300,300));

            //sprite = new Sprite(t);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
            //if (InputManager.IsKeyDown(Keys.Left))
            //    sprite.Position += Vector2.UnitX * -100 * Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Right))
            //    sprite.Position += Vector2.UnitX * 100 * Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Up))
            //    sprite.Position += Vector2.UnitY * -100 * Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Down))
            //    sprite.Position += Vector2.UnitY * 100 * Time.ElapsedGameTime;
            //if (InputManager.IsKeyDown(Keys.Space))
            //    sprite.Rotation += 0.05f;
            SpiralMover.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            SpiralMover.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
