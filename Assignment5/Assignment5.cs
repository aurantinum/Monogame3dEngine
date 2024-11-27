using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;

namespace Assignment5
{
    public class Assignment5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TerrainRenderer terrain;
        Camera camera;
        Player player;
        Effect effect;
        Light Light;
        public Assignment5()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);
            Time.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Light = new Light();
            Light.Transform = new Transform();
            Light.Transform.Position = Vector3.One * 5;
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position = Vector3.Up * 70;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            terrain = new TerrainRenderer(Content.Load<Texture2D>("mazeH"),
                Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("mazeN");
            terrain.Transform = new Transform();
            terrain.Transform.Scale *= new Vector3(1, 5, 1);
            player = new Player(terrain, Content, camera, _graphics.GraphicsDevice, Light);
            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.3f, 0.3f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0f, 0f, 0.1f));
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();
            player.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.Position);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
                player.Draw();

            }
            base.Draw(gameTime);
        }
    }
    public class Player : GameObject
    {
        public TerrainRenderer Terrain { get; set; }

        public Player(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Transform = new Transform();
            Transform.Position = new(0, 0, 0);
            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            //Add<SphereCollider>();
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1;
            collider.Transform = Transform;
            Add<SphereCollider>(collider);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

        }

        public new void Update()
        {
            // Control the player
            if (InputManager.IsKeyDown(Keys.W)) // move forward
            {
                this.Transform.LocalPosition += Vector3.Transform(Vector3.Forward * Time.ElapsedGameTime * 5, this.Transform.Rotation);
            }
            if (InputManager.IsKeyDown(Keys.S)) // move backwars
            {
                this.Transform.LocalPosition += Vector3.Transform(Vector3.Backward * Time.ElapsedGameTime * 5, this.Transform.Rotation);
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                this.Transform.LocalPosition += Vector3.Transform(Vector3.Left * Time.ElapsedGameTime * 5, this.Transform.Rotation);
            }
            if (InputManager.IsKeyDown(Keys.D))
            {
                this.Transform.LocalPosition += Vector3.Transform(Vector3.Right * Time.ElapsedGameTime * 5, this.Transform.Rotation);
            }

            // change the Y position corresponding to the terrain (maze)
            this.Transform.LocalPosition = new Vector3(
               this.Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               this.Transform.LocalPosition.Z) + Vector3.Up;

            base.Update();
        }
    }
    public class Agent : GameObject
    {
        public TerrainRenderer Terrain { get; set; }
        public AStarSearch search;
        List<Vector3> path;

        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size

        public Agent(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            path = null;

            Terrain = terrain;

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            //Add<SphereCollider>();
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1;
            collider.Transform = Transform;
            Add<SphereCollider>(collider);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);

            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.size.X / 2,
                         0,
                         gridH * j + gridH / 2 - Terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 1.0)
                        search.Nodes[j, i].Passable = false;
                }
        }

        public new void Update()
        {

            if (path != null && path.Count > 0)
            {
                // Move to the destination along the path

                if () // if it reaches to a point, go to the next in path
                {

                    if () // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
            else
            {
                // Search again to make a new path.

            }

            this.Transform.LocalPosition = new Vector3(
               this.Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               this.Transform.LocalPosition.Z) + Vector3.Up;
            base.Update();
        }
        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 0,
                         gridH * gridPos.Y + gridH / 2 - Terrain.size.Y / 2);
        }

        private void RandomPathFinding()
        {
            Random random = new Random();
            while (!(search.Start = search.Nodes[random.Next(search.Rows),
            random.Next(search.Cols)]).Passable) ;
            search.End = search.Nodes[search.Rows / 2, search.Cols / 2];
            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
        }
    }
}
