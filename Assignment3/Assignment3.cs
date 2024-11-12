using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CPI311.GameEngine;
using System;
using System.Threading;
namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        List<GameObject> gameObjects;
        private const int COLLISION_THREADS = 5;
        GameObject cameraGo, box;
        Model sphere;
        Light light;
        Random random;
        Texture2D texture;
        Camera camera;
        SpriteFont font;
        float frameTime;
        int frames = 0;
        int collisionCount = 0, lastSecondCollisions = 0;
        bool haveThreadRunning = true;
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
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

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
            frames += 1;
            
            frameTime = Time.TotalGameTime.Milliseconds;
            frameTime = (frameTime - (((int)(frameTime / 1000)) * 1000))/1000;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update();
            }

            //for (int i = 0; i < COLLISION_THREADS; i++)
            //{
            ThreadPool.QueueUserWorkItem(new WaitCallback(Collide));//, i);
            //}
            if (InputManager.IsKeyDown(Keys.Up))
            {
                GameObject newObject = CreateSphereAtPosition(new Vector3(random.Next(-9, 9), random.Next(-9, 9), random.Next(-9, 9)), 
                    new Vector3(1, 1, 1), Color.WhiteSmoke);
                Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                direction.Normalize();
                newObject.Rigidbody.Impulse = direction * ((float)random.NextDouble() * 5 + 5);
                gameObjects.Add(newObject);
            }
            if (InputManager.IsKeyPressed(Keys.Down))
            {
                if (gameObjects.Count > 2)
                {
                    gameObjects.RemoveAt(gameObjects.Count - 1);
                }
            }
            if (InputManager.IsKeyPressed(Keys.Left))
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    
                    if(gameObjects[i].Rigidbody != null)
                    {
                        gameObjects[i].Rigidbody.Impulse 
                            -= Vector3.Normalize(gameObjects[i].Rigidbody.Velocity) * 100 
                            * gameObjects[i].Rigidbody.Mass * Time.ElapsedGameTime;
                    }
                }
            }
            if (InputManager.IsKeyPressed(Keys.Right))
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    
                    if(gameObjects[i].Rigidbody != null)
                    {
                        gameObjects[i].Rigidbody.Impulse 
                            += Vector3.Normalize(gameObjects[i].Rigidbody.Velocity) * 100 * 
                            gameObjects[i].Rigidbody.Mass * Time.ElapsedGameTime;
                    }
                }
            }
            
           
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
            _spriteBatch.DrawString(font, "Balls: " + (gameObjects.Count - 2 )+ "\n"
                +"Collisions on average: " + lastSecondCollisions
                , new Vector2(100, 100), Color.Black);


            int fps = 0;
            if (frameTime > 0)
            {
                fps = (int)((float) frames / frameTime);
            }
            else frames = 0;

            _spriteBatch.DrawString(font, "\nFPS: " + fps
                    , new Vector2(100, 140), Color.Black);
            
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Collide(Object o)
        {
            //int threadID = (int)o;
            //int start = threadID * gameObjects.Count/COLLISION_THREADS;
            //int end = (threadID + 1) * gameObjects.Count/COLLISION_THREADS;
            //List<GameObject> _gameObjects = gameObjects;
            int colHere = 0;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                if(gameObject == null)
                {
                    break;
                }
                if (gameObject.Rigidbody == null)
                {
                    continue;
                }
                Vector3 normal;
                
                if (gameObject.Get<SphereCollider>() != null)
                {
                    if (box.Get<BoxCollider>().Collides(gameObject.Get<SphereCollider>(), out normal))
                    {                        
                        colHere++;
                        gameObject.Rigidbody.Impulse += Vector3.Dot(normal, gameObject.Rigidbody.Velocity) * -2 * normal;
                    }

                    if (i < gameObjects.Count - 1)
                    {
                        for (int j = i + 1; j < gameObjects.Count; j++)
                        {
                            GameObject other = gameObjects[j];
                            
                            if (other == null) { break; }
                            if (other.Get<SphereCollider>() != null)
                            {
                                if (gameObject.Get<SphereCollider>().Collides(other.Get<SphereCollider>(), out normal))
                                {
                                    colHere++;
                                }
                                if (gameObject.Get<SphereCollider>().SweptCollides(other.Get<SphereCollider>(), other.Rigidbody.PreviousTransform.Position, gameObject.Rigidbody.PreviousTransform.Position, out normal)
                                    && normal != Vector3.Zero) { }
                             
                                Vector3 velocityNormal = Vector3.Dot(normal, 
                                      (gameObject.Rigidbody.Velocity * gameObject.Rigidbody.Mass) 
                                    - (other.Rigidbody.Velocity * other.Rigidbody.Mass))
                                    * -1 * normal;
                                gameObject.Rigidbody.Impulse += (velocityNormal / other.Rigidbody.Mass);
                                other.Rigidbody.Impulse += (-velocityNormal / gameObject.Rigidbody.Mass);
                                
                            }
                        }
                    }
                }
            }
            collisionCount += colHere;
        }
        private void CollisionReset(Object obj)
        {
            
            while (haveThreadRunning)
            {
                lastSecondCollisions = collisionCount;
                collisionCount = 0;
                System.Threading.Thread.Sleep(1000);
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
