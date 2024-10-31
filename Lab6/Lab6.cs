using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using CPI311.GameEngine;

namespace Lab6
{
    public class Lab6 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Random random;
        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        BoxCollider boxCollider;
        int numberCollisions = 0;
        Model model;
        Camera camera;
        SpriteFont font;
        public Lab6()
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
            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.LocalPosition += i * Vector3.One * 5 ; //avoid overlapping each sphere
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;
                Vector3 direction = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;
                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);
            }
            
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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
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

            _spriteBatch.Begin();
            foreach (Transform transform in transforms)
                model.Draw(transform.World, camera.View, camera.Projection);
            _spriteBatch.DrawString(font, "Collisions: " + numberCollisions, new Vector2(100, 100), Color.Black);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
