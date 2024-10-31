using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System.Reflection;
namespace Assignment2Camera
{
    public class Assignment2Camera : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;
        Model sphereModel, planeModel;
        Light light;
        Camera camera;
        GameObject plane, viewer;
        Texture2D texture;
        List<GameObject> scene;
        float mouseSens = 2f;
        bool firstPerson = true;
        public Assignment2Camera()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            //IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            scene = new List<GameObject>();
            base.Initialize();
        }

        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            sphereModel = Content.Load<Model>("Sphere");
            planeModel = Content.Load<Model>("Plane");
            texture = Content.Load<Texture2D>("Square");
            Transform lightTransform = new Transform();
            light = new Light();
            light.Transform = lightTransform;
            light.Transform.Position = Vector3.Backward * 10 + Vector3.Right * 5;
            viewer = new GameObject();
            viewer.Add<Camera>();
            viewer.Camera.NearPlane /= 10;
            viewer.Camera.FieldOfView -= MathHelper.ToRadians(15);
            viewer.Add<Rigidbody>();
            viewer.Rigidbody.Mass = 1;
            viewer.Add(new Renderer(sphereModel, viewer.Transform, viewer.Camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
            viewer.Get<Renderer>().Material.Diffuse = Color.Yellow.ToVector3();
            viewer.Get<Renderer>().Material.Specular = Color.Yellow.ToVector3();
            viewer.Get<Renderer>().Material.Ambient = Color.Yellow.ToVector3();
            scene.Add(viewer);
            font = Content.Load<SpriteFont>("Font");
            

            scene.Add(CreateSphere(new Vector3(10, 0, 10), Vector3.One, Color.Red));
            scene.Add(CreateSphere(new Vector3(10, 0, -10), Vector3.One * 5, Color.Orange));
            scene.Add(CreateSphere(new Vector3(10, 0, 0), Vector3.One, Color.Green));
            scene.Add(CreateSphere(new Vector3(-10, 0, 0), Vector3.One, Color.Blue));
            scene.Add(CreateSphere(new Vector3(-10, 0, -10), Vector3.One, Color.Violet));
            scene.Add(CreateSphere(new Vector3(0, 0, -10), Vector3.One, Color.LemonChiffon));
            scene.Add(CreateSphere(new Vector3(-10, 0, 10), Vector3.One, Color.Gray));
            scene.Add(CreateSphere(new Vector3(0, 0, 10), Vector3.One, Color.MediumSpringGreen));
        }

        private GameObject CreateSphere(Vector3 position, Vector3 scale, Color color)
        {
            GameObject newSphere = new GameObject();
            newSphere.Transform.LocalPosition = position;
            newSphere.Transform.Scale = scale;
            newSphere.Add<Rigidbody>();
            newSphere.Rigidbody.Mass = 1;
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * newSphere.Transform.LocalScale.Y;
            newSphere.Add<SphereCollider>(sphereCollider);
            newSphere.Add(new Renderer(sphereModel, newSphere.Transform, viewer.Camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
            newSphere.Get<Renderer>().Material.Diffuse = color.ToVector3();
            newSphere.Get<Renderer>().Material.Specular = color.ToVector3();
            newSphere.Get<Renderer>().Material.Ambient = color.ToVector3();
            return newSphere;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            foreach (GameObject go in scene)
            {
                go.Update();
            }
            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                //switch from first to third and back
                if (firstPerson)
                {
                    firstPerson = false;
                    viewer.Camera.Transform.LocalPosition = new Vector3(0, 10, -10);
                    viewer.Camera.Transform.LocalRotation = Quaternion.CreateFromAxisAngle(viewer.Camera.Transform.Right, -MathHelper.PiOver2/2);
                    viewer.Camera.FieldOfView = MathHelper.ToRadians(90);
                    viewer.Camera.NearPlane = .1f;
                }
                else
                {
                    firstPerson = true;
                    viewer.Camera.Transform.LocalPosition = Vector3.Zero;
                    viewer.Camera.Transform.LocalRotation = Quaternion.Identity;
                    viewer.Camera.FieldOfView = MathHelper.ToRadians(75);
                    viewer.Camera.NearPlane = 0.01f;
                }
            }
            if (firstPerson)
            {
                //Handle view input for first person
                if (InputManager.IsKeyDown(Keys.Up))
                {
                    viewer.Transform.Rotate(Vector3.Right, MathHelper.ToRadians(Time.ElapsedGameTime * 10));
                }
                if (InputManager.IsKeyDown(Keys.Down))
                {
                    viewer.Transform.Rotate(Vector3.Right, MathHelper.ToRadians(-Time.ElapsedGameTime * 10));
                }
                if (InputManager.IsKeyDown(Keys.Left))
                {
                    viewer.Camera.Transform.Rotate(Vector3.Up, MathHelper.ToRadians(Time.ElapsedGameTime * 10));
                }
                if (InputManager.IsKeyDown(Keys.Right))
                {
                    viewer.Camera.Transform.Rotate(Vector3.Up, MathHelper.ToRadians(-Time.ElapsedGameTime * 10));
                }
                viewer.Transform.Rotate(Vector3.Up, MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaX * mouseSens));
                viewer.Camera.Transform.Rotate(Vector3.Right, MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaY * mouseSens));
            }
            else
            {
                if (InputManager.IsKeyDown(Keys.Up))
                {
                    viewer.Transform.Rotate(Vector3.Right, -MathHelper.ToRadians(Time.ElapsedGameTime * 10));
                }
                if (InputManager.IsKeyDown(Keys.Down))
                {
                    viewer.Transform.Rotate(Vector3.Right, -MathHelper.ToRadians(-Time.ElapsedGameTime * 10));
                }
                if (InputManager.IsKeyDown(Keys.Left))
                {
                    viewer.Camera.Transform.Rotate(Vector3.Up, -MathHelper.ToRadians(Time.ElapsedGameTime * 10));
                }
                if (InputManager.IsKeyDown(Keys.Right))
                {
                    viewer.Camera.Transform.Rotate(Vector3.Up, -MathHelper.ToRadians(-Time.ElapsedGameTime * 10));
                }
                viewer.Transform.Rotate(Vector3.Up, -MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaX * mouseSens));
                viewer.Camera.Transform.Rotate(Vector3.Right, -MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaY * mouseSens));
            }
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            foreach (GameObject go in scene) go.Draw();
            _spriteBatch.Begin();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
