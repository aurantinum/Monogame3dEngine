using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CPI311.GameEngine;
using System;
using System.Diagnostics;
using System.Threading;
namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        List<GameObject> gameObjects;
        GameObject cameraGo, box;
        Model sphere;
        Light light;
        Random random;
        Texture2D texture;
        Camera camera;
        SpriteFont font;
        public Assignment3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();
            gameObjects = new List<GameObject>();
            random = new Random();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            light = new Light();
            light.Transform = new Transform();
            light.Transform.Position = Vector3.Backward * 10 + Vector3.Right * 5;
            texture = Content.Load<Texture2D>("Square");
            sphere = Content.Load<Model>("Sphere");
            font = Content.Load<SpriteFont>("Font");
            // set up camera
            cameraGo = new GameObject();
            cameraGo.Transform.Position = Vector3.Backward * 20;
            cameraGo.Add<Camera>();
            gameObjects.Add(cameraGo);
            camera = cameraGo.Camera;

            //box collider
            box = new GameObject();
            box.name = "Box";
            box.Transform.Position = new Vector3(0, 0, 0);
            box.Add<BoxCollider>();
            box.Get<BoxCollider>().Transform = new Transform();
            box.Get<BoxCollider>().Transform.Position = new Vector3(0, 0, 0);
            box.Get<BoxCollider>().Size = 10f;
            gameObjects.Add(box);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyPressed(Keys.Up))
            {
                GameObject newObject = CreateSphereAtPosition(new Vector3(random.Next(-9, 9), random.Next(-9, 9), random.Next(-9, 9)), 
                    new Vector3(1, 1, 1), Color.WhiteSmoke);
                Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                direction.Normalize();
                newObject.Rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
                gameObjects.Add(newObject);
            }
            if (InputManager.IsKeyPressed(Keys.Down))
            {
                if (gameObjects.Count > 2)
                {
                    gameObjects.RemoveAt(gameObjects.Count - 1);
                }
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                foreach (var gameObject in gameObjects)
                {
                    if(gameObject.Rigidbody != null)
                    {
                        Debug.WriteLine("AddingVel");
                        gameObject.Rigidbody.Impulse 
                            -= Vector3.Normalize(gameObject.Rigidbody.Velocity) * 10 * Time.ElapsedGameTime;
                    }
                }
            }
            if (InputManager.IsKeyDown(Keys.Right))
            {
                foreach (var gameObject in gameObjects)
                {
                    if(gameObject.Rigidbody != null)
                    {
                        Debug.WriteLine("AddingVel");

                        gameObject.Rigidbody.Impulse 
                            += Vector3.Normalize(gameObject.Rigidbody.Velocity) * 10 * Time.ElapsedGameTime;
                    }
                }
            }
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update();
                
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(Collide));
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
            _spriteBatch.DrawString(font, gameObjects.Count + "", new Vector2(100, 100), Color.Black);
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void Collide(Object o)
        {

            foreach (GameObject gameObject in gameObjects)
            {
                Vector3 normal;
                if (gameObject.Get<SphereCollider>() != null)
                {
                    if (box.Get<BoxCollider>().Collides(gameObject.Get<SphereCollider>(), out normal))
                    {
                        if (Vector3.Dot(normal, gameObject.Rigidbody.Velocity) < 0)
                            gameObject.Rigidbody.Impulse += Vector3.Dot(normal, gameObject.Rigidbody.Velocity) * -2 * normal;
                    }
                }

                if (gameObject.Get<SphereCollider>() != null)
                {
                    
                    if (gameObjects.IndexOf(gameObject) < gameObjects.Count - 1)
                    {
                        foreach (GameObject other in gameObjects.GetRange(gameObjects.IndexOf(gameObject) + 1, gameObjects.Count - 1 - gameObjects.IndexOf(gameObject)))
                        {
                            if (other.Get<SphereCollider>() != null)
                            {
                                gameObject.Get<SphereCollider>().SweptCollides(other.Get<SphereCollider>(), other.Rigidbody.PreviousTransform.Position, gameObject.Rigidbody.PreviousTransform.Position, out normal);
                                Vector3 velocityNormal = Vector3.Dot(normal, gameObject.Rigidbody.Velocity - other.Rigidbody.Velocity)
                                    * -2 * normal * gameObject.Rigidbody.Mass * other.Rigidbody.Mass;
                                gameObject.Rigidbody.Impulse += velocityNormal / 2.5f;
                                other.Rigidbody.Impulse += -velocityNormal / 2.5f;
                            }
                        }
                    }
                }
            }

        }


        private GameObject CreateSphereAtPosition(Vector3 position, Vector3 scale, Color color)
        {
            GameObject newSphere = new GameObject();
            newSphere.Transform.LocalPosition = position;
            newSphere.Transform.Scale = scale;
            newSphere.Add<Rigidbody>();
            newSphere.Rigidbody.Mass = (float)random.Next(1,10) ;
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * newSphere.Transform.LocalScale.Y;
            newSphere.Add(sphereCollider);
            newSphere.Add(new Renderer(sphere, newSphere.Transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
            newSphere.Get<Renderer>().Material.Diffuse = color.ToVector3();
            newSphere.Get<Renderer>().Material.Specular = color.ToVector3();
            newSphere.Get<Renderer>().Material.Ambient = color.ToVector3();
            return newSphere;
        }
    }
}
