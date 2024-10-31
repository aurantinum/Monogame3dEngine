using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Assignment1
{
    public class Assignment1 : Game
    {
        private enum ExplorerRows
        {
            WALK_UP,
            WALK_DOWN,
            WALK_LEFT,
            WALK_RIGHT,
            RUN_UP
        }
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Random random;
        public AnimatedSprite player;
        public Sprite objective;
        public ProgressBar timerBar, distanceBar;
        float maxTime = 60f;
        float scoreDist = 10f;

        public Assignment1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D animText = Content.Load<Texture2D>("explorer");
            Texture2D square = Content.Load<Texture2D>("Square");
            player = new AnimatedSprite(animText, 8, 5);
            timerBar = new ProgressBar(square, Color.Green);
            timerBar.Position = new Vector2(35, 20);
            timerBar.Scale = new Vector2(2, 1);
            distanceBar = new ProgressBar(square, Color.Red);
            distanceBar.Position = new Vector2(150, 20);
            distanceBar.Scale = new Vector2(2, 1);
            objective = new Sprite(square);
            objective.Position = new Vector2(400, 400);
            timerBar.Value = 0;
            timerBar.MaxValue = maxTime;
            player.Speed = 10f;
            player.Position = new Vector2(300, 300);
            random = new Random();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
            player.Update(gameTime);
            timerBar.Update();
            distanceBar.Update();
            DoInput();
            DoGamePlay();
            base.Update(gameTime);
        }
        private void DoGamePlay()
        {
            timerBar.Value += Time.ElapsedGameTime;
            float distance = Vector2.Distance(player.Position , objective.Position);
            if (distance < 20) 
            {
                distanceBar.Value += distance;
                objective.Position = new Vector2(random.Next(0, GraphicsDevice.Viewport.Width), random.Next(0, GraphicsDevice.Viewport.Height));
            }

        }
        private void DoInput()
        {
            Vector2 change = Vector2.Zero;
            if (InputManager.IsKeyDown(Keys.Left))
            {
                change += Vector2.UnitX * -100 * Time.ElapsedGameTime;
                player.Row = (int)ExplorerRows.WALK_LEFT;
            }
            if (InputManager.IsKeyDown(Keys.Right))
            {
                change += Vector2.UnitX * 100 * Time.ElapsedGameTime;
                player.Row = (int)ExplorerRows.WALK_RIGHT;
            }
            if (InputManager.IsKeyDown(Keys.Up))
            {
                change += Vector2.UnitY * -100 * Time.ElapsedGameTime;
                player.Row = (int)ExplorerRows.WALK_UP;
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                change += Vector2.UnitY * 100 * Time.ElapsedGameTime;
                player.Row = (int)ExplorerRows.WALK_DOWN;
            }
            player.Position += change;
            distanceBar.Value += change.Length()/100;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            objective.Draw(_spriteBatch);
            player.Draw(_spriteBatch);

            timerBar.Draw(_spriteBatch);
            distanceBar.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
