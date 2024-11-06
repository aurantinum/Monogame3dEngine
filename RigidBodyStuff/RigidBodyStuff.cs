using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace RigidBodyStuff
{
    public class RigidBodyStuff : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        List<GameObject> gameObjects;
        FirstPersonController player;
        Texture2D texture;
        Model sphereModel;
        GameObject plane;
        SpriteFont font;
        Light light;
        Camera camera;
        Effect effect;
        public RigidBodyStuff()
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
            InputManager.KeepMouseCentered = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gameObjects = new List<GameObject>();
            light = new Light();
            light.Transform = new Transform();
            light.Transform.Position = Vector3.Backward * 10 + Vector3.Right * 5;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            sphereModel = Content.Load<Model>("Sphere");
            texture = Content.Load<Texture2D>("Square");
            effect = Content.Load<Effect>("SimpleShading");
            font = Content.Load<SpriteFont>("Font");
            player = new FirstPersonController();
            player.Transform.Position = new(0, 1, 0);
            camera = player.Camera;
            player.Add(new Renderer(sphereModel, player.Transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
            player.Get<Renderer>().Material.Diffuse = Color.Black.ToVector3();
            player.Get<Renderer>().Material.Specular = Color.Black.ToVector3();
            player.Get<Renderer>().Material.Ambient = Color.Black.ToVector3();
            gameObjects.Add(player);
            plane = CreateSphereAtPosition(new Vector3(0, -0.05f, 0), new Vector3(1000, 0.001f, 1000), Color.WhiteSmoke);
            gameObjects.Add(plane);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            if(InputManager.IsKeyPressed(Keys.R) && player.Rigidbody == null)
            {
                Rigidbody r = new Rigidbody();
                r.Transform = player.Transform;
                r.Mass = 1;
                player.Add(r);
            }
            foreach (GameObject gameObject in gameObjects)
            {
                Debug.WriteLine(gameObject.name);
                if (gameObject is FirstPersonController)
                {
                    (gameObject as FirstPersonController).Update();
                }
                else
                {
                    gameObject.Update();
                }
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw();
            }
            _spriteBatch.Begin();
          
            _spriteBatch.DrawString(font, player.Rigidbody.Velocity.ToString() , new Vector2(100, 100), Color.Black);
            
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private GameObject CreateSphereAtPosition(Vector3 position, Vector3 scale, Color color)
        {
            GameObject newSphere = new GameObject();
            newSphere.Transform.LocalPosition = position;
            newSphere.Transform.Scale = scale;
            newSphere.Add<Rigidbody>();
            newSphere.Rigidbody.Mass = 1;
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * newSphere.Transform.LocalScale.Y;
            newSphere.Add<SphereCollider>(sphereCollider);
            newSphere.Add(new Renderer(sphereModel, newSphere.Transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
            newSphere.Get<Renderer>().Material.Diffuse = color.ToVector3();
            newSphere.Get<Renderer>().Material.Specular = color.ToVector3();
            newSphere.Get<Renderer>().Material.Ambient = color.ToVector3();
            return newSphere;
        }
    }
}
