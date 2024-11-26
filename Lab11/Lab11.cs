using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Lab11
{
    public class Scene
    {
        public delegate void CallMethod();
        public CallMethod Update;
        public CallMethod Draw;
        public Scene(CallMethod update, CallMethod draw)
        { Update = update; Draw = draw; }
    }
    public class Lab11 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public List<GUIElement> guiElements;
        public List<GUIElement> otherGuiElements;
        public List<GameObject> gameObjects;
        Texture2D texture;
        Dictionary<String, Scene> scenes;
        Scene currentScene;
        Button exitButton;
        Checkbox checkBox;
        SpriteFont font;
        Color background = Color.White;
        public Lab11()
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
            ScreenManager.Initialize(_graphics);
            scenes = new Dictionary<string, Scene>();
            guiElements = new List<GUIElement>();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");
            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];
            exitButton = new Button();
            exitButton.Texture = texture;
            exitButton.Text = "Exit";
            exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            checkBox = new Checkbox();
            checkBox.Text = "Menu";
            checkBox.Checked = true;
            checkBox.Texture = texture;
            checkBox.Box = texture;
            checkBox.Bounds = new Rectangle(50, 80, 50, 20);
            guiElements.Add(exitButton);
            //guiElements.Add(checkBox);

            checkBox.Action += CheckBox;
            exitButton.Action += ExitGame;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            currentScene.Update();
            checkBox.Update();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);
            currentScene.Draw();
            base.Draw(gameTime);
        }
        void ExitGame(GUIElement element)
        {
            if (!ScreenManager.IsFullScreen)
            {
                ScreenManager.Setup(true, 1920, 1080);

                exitButton.Bounds = new Rectangle(50, 50, 300, 20);
                checkBox.Bounds = new Rectangle(50, 80, 50, 20);


            }
            else
            {
                ScreenManager.Setup(false, 1280, 720);
                exitButton.Bounds = new Rectangle(50, 50, 300, 20);
                checkBox.Bounds = new Rectangle(50, 80, 50, 20);


            }
            //currentScene = scenes["Play"];
        }
        void CheckBox(GUIElement element)
        {
            if (!checkBox.Checked)
            {
                currentScene = scenes["Play"];
            }
            else
            {
                currentScene = scenes["Menu"];
            }
        }
        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw()
        {
            _spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(_spriteBatch, font);
            checkBox.Draw(_spriteBatch, font);
            _spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
        }
        void PlayDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Play Mode! Press \"Esc\" to go back", Vector2.Zero, Color.Black);
            checkBox.Draw(_spriteBatch, font);
            _spriteBatch.End();
        }
    }
}
