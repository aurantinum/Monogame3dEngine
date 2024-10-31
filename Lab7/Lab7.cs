using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using CPI311.GameEngine;
using System.Threading;

namespace Lab7
{
    public class Lab7 : Game
    {
        bool haveThreadRunning = true;
        int lastSecondCollisions = 0;
        int numSpheres = 5;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Random random;
        Light light;
        List<Transform> transforms;
        List<Renderer> renderers;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        BoxCollider boxCollider;
        int numberCollisions = 0;
        Model model;
        Camera camera;
        SpriteFont font;
        public Lab7()
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
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));
            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            renderers = new List<Renderer> ();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Sphere");
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 20;
            font = Content.Load<SpriteFont>("Font");
            Transform lightTransform = new Transform();
            light = new Light();
            light.Transform = lightTransform;
            light.Transform.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
            //Input
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                AddSphere();
            }


            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();
            
            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                        Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                        numberCollisions++;
                    Vector3 velocityNormal = Vector3.Dot(normal,
                        rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                        * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                    rigidbodies[i].Impulse += velocityNormal / 2;
                    rigidbodies[j].Impulse += -velocityNormal / 2;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            foreach (Renderer renderer in renderers) renderer.Draw();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Collisions: " + numberCollisions, new Vector2(100, 100), Color.Black);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
        private void AddSphere()
        {
            Transform transform = new Transform();
            transform.LocalPosition += (float)random.NextDouble() * Vector3.One * 5; //avoid overlapping each sphere
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = 1;
            Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
            renderers.Add(renderer);
        }
    }
}
