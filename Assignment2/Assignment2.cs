using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment2
{
    public class Assignment2 : Game
    {
        Scene scene;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont font;
        Model sphereModel;
        Texture2D texture;
        Light light;
        GameObject sun, mercury, earth, moon, viewer, plane, player;
        float mouseSens = 2f, playerSpeed = 10;
        bool firstPerson = false;

        public Assignment2()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            scene = new Scene();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            sphereModel = Content.Load<Model>("cube");
            texture = Content.Load<Texture2D>("Square");
            Transform lightTransform = new Transform();
            light = new Light();
            light.Transform = lightTransform;
            light.Transform.Position = Vector3.Backward * 10 + Vector3.Right * 5;
            viewer = new GameObject();
            viewer.Add<Camera>();
            plane = CreateSphereAtPosition(new Vector3(0, -0.05f, 0), new Vector3(1000, 0.001f, 1000), Color.WhiteSmoke);
            plane.Remove<SphereCollider>();
            scene.Add(plane);
            viewer.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            viewer.Camera.Transform.Position = new Vector3(0, 100, 0);
            viewer.Camera.NearPlane /= 10;
            viewer.Camera.FieldOfView -= MathHelper.ToRadians(15);
            player = new GameObject();
            player.Transform.Position = new Vector3(10, 0, 10);
            player.Add<Rigidbody>();
            player.Rigidbody.Mass = 1;
            player.Add(new Renderer(sphereModel, player.Transform, viewer.Camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
            player.Get<Renderer>().Material.Diffuse = Color.Black.ToVector3();
            player.Get<Renderer>().Material.Specular = Color.Black.ToVector3();
            player.Get<Renderer>().Material.Ambient = Color.Black.ToVector3();
            scene.Add(player);
            scene.Add(viewer);
            font = Content.Load<SpriteFont>("Font");
            CreatePlanets();
            Scene.AddScene(scene);
        }

        

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Scene.Update(gameTime);
            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                //switch from first to third and back
                if (firstPerson)
                {
                    firstPerson = false;
                    viewer.Transform.LocalPosition = new Vector3(0, 100, 0);
                    viewer.Transform.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver2);
                    viewer.Camera.FieldOfView = MathHelper.ToRadians(90);
                    viewer.Camera.NearPlane = .1f;
                }
                else
                {
                    firstPerson = true;
                    viewer.Camera.Transform.LocalPosition = player.Transform.Position;
                    viewer.Transform.LocalRotation = player.Transform.Rotation;
                    viewer.Camera.FieldOfView = MathHelper.ToRadians(75);
                    viewer.Camera.NearPlane = 0.01f;
                }
            }
            //handle player movement
            Vector3 howToMove = new Vector3(InputManager.HorizontalAxis, 0, -InputManager.VerticalAxis) * Time.ElapsedGameTime * playerSpeed;
            howToMove += new Vector3(0, 0, -InputManager.LeftMouse) * Time.ElapsedGameTime * playerSpeed;
            if (firstPerson)
            {
                //Handle view input for first person
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
                player.Transform.Position += Vector3.Transform(howToMove, viewer.Transform.Rotation);
                viewer.Transform.Position = player.Transform.Position;
            }
            else
            {
                //view input for third person
                if (InputManager.IsKeyDown(Keys.PageUp))
                {
                    viewer.Camera.FieldOfView += MathHelper.ToRadians(Time.ElapsedGameTime * 20);
                    if(viewer.Camera.FieldOfView > MathHelper.ToRadians(200))
                    {
                        viewer.Camera.FieldOfView = MathHelper.ToRadians(200);
                    }
                }
                if (InputManager.IsKeyDown(Keys.PageDown))
                {
                    viewer.Camera.FieldOfView -= MathHelper.ToRadians(Time.ElapsedGameTime * 20);
                    if (viewer.Camera.FieldOfView < MathHelper.ToRadians(10))
                    {
                        viewer.Camera.FieldOfView = MathHelper.ToRadians(10);
                    }
                }
                player.Transform.Position += howToMove;
            }
            //Control Anim Speed
            if (InputManager.IsKeyDown(Keys.C))
            {
                foreach(GameObject go in scene.GameObjects)
                {
                    if(go.Get<Orbit>() != null)
                        go.Get<Orbit>().orbitSpeed -= Time.ElapsedGameTime;
                    if(go.Get<RotateSphere>() != null)
                        go.Get<RotateSphere>().rotationSpeed -= Time.ElapsedGameTime;
                }
            }
            if (InputManager.IsKeyDown(Keys.V))
            {
                foreach (GameObject go in scene.GameObjects)
                {
                    if (go.Get<Orbit>() != null)
                        go.Get<Orbit>().orbitSpeed += Time.ElapsedGameTime;
                    if (go.Get<RotateSphere>() != null)
                        go.Get<RotateSphere>().rotationSpeed += Time.ElapsedGameTime;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            Scene.Draw();

            _spriteBatch.DrawString(font, "Move:WASD/Left Mouse\nLook:Arrows/Mouse\nZoom:pgup/dn\nAnimSpeed:C/V\n"+player.Transform.Position.ToString(), new Vector2(100, 100), Color.Black);
            _spriteBatch.End();
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
            newSphere.Add(new Renderer(sphereModel, newSphere.Transform, viewer.Camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture));
            newSphere.Get<Renderer>().Material.Diffuse = color.ToVector3();
            newSphere.Get<Renderer>().Material.Specular = color.ToVector3();
            newSphere.Get<Renderer>().Material.Ambient = color.ToVector3();
            return newSphere;
        }
        private void CreatePlanets()
        {
            sun = CreateSphereAtPosition(new Vector3(0,30,0), Vector3.One * 5, Color.Yellow);
            sun.Add<RotateSphere>();
            scene.Add(sun);
            mercury = CreateSphereAtPosition(new Vector3(0, 30, 40), Vector3.One * 2, Color.Red);
            mercury.Add<Orbit>();
            mercury.Get<Orbit>().toOrbitAround = sun;
            scene.Add(mercury);
            earth = CreateSphereAtPosition(new Vector3(0, 30, 90), Vector3.One * 3, Color.Aqua);
            earth.Add<Orbit>();
            earth.Get<Orbit>().toOrbitAround = sun;
            earth.Add<RotateSphere>();
            scene.Add(earth);
            moon = CreateSphereAtPosition(earth.Transform.LocalPosition + new Vector3(0, 0, 10), Vector3.One, Color.Gray);
            moon.Add<RotateSphere>();
            moon.Add<Orbit>();
            moon.Get<Orbit>().toOrbitAround = earth;
            moon.Get<Orbit>().orbitSpeed = 10f;
            moon.Get<RotateSphere>().rotationSpeed = 20f;
            scene.Add(moon);
        }
    }
}
