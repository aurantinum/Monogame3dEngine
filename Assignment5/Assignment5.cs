using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Assignment5
{
    public class Assignment5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public bool ShouldRun = true;
        TerrainRenderer terrain;
        Camera camera;
        Player player;
        Effect effect;
        Light Light;
        List<Agent> agents;
        Bomb bomb;
        public static int Score = 0;
        SpriteFont spriteFont;

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
            agents = new List<Agent>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Font");
            Light = new Light();
            Score = 0;
            Light.Transform = new Transform();
            Light.Transform.Position = Vector3.One * 5 + Vector3.Backward * 45;
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
            for (int i = 0; i < 3; i++)
            {
                Agent agent = new Agent(terrain, Content, camera, _graphics.GraphicsDevice, Light);
                agents.Add(agent);
            }
            bomb = new Bomb(terrain, Content, camera, _graphics.GraphicsDevice, Light, player);
                agents.Add(bomb);
            //agent.Transform.Position = new(10, 0, 10);
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
            {
                Exit();
            }

            Time.Update(gameTime);
            InputManager.Update();
            if (bomb.HitPlayer)
            {
                Restart();
            }
            player.Update();
            foreach (var a in agents)
            {
                a.Update();
                Vector3 normal;
                if(a.Get<SphereCollider>().Collides(player.Get<SphereCollider>(), out normal))
                {
                    a.OnCollisionWithPlayer();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(spriteFont, "Score: " + Score, new Vector2(50, 50), Color.Black);
            _spriteBatch.End();
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
                foreach(var a in agents)
                {
                    a.Draw();
                }

            }
            base.Draw(gameTime);
        }
        protected void Restart()
        {
            agents.Clear();
            LoadContent();
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
            Texture2D texture = Content.Load<Texture2D>("Texture_01_A");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            renderer.Material.Diffuse = Color.BlueViolet.ToVector3();
            renderer.Material.Specular = Color.BlueViolet.ToVector3();
            renderer.Material.Ambient = Color.BlueViolet.ToVector3();
            Add<Renderer>(renderer);

        }

        public new void Update()
        {
            
            // Control the player
            if (InputManager.IsKeyDown(Keys.W)) // move forward
            {
                if (Terrain.GetAltitude(Vector3.Transform(Vector3.Forward * Time.ElapsedGameTime * 5, this.Transform.Rotation) + this.Transform.Position) < 0.1f)
                    this.Transform.LocalPosition += Vector3.Transform(Vector3.Forward * Time.ElapsedGameTime * 5, this.Transform.Rotation);
            }
            if (InputManager.IsKeyDown(Keys.S)) // move backwars
            {
                if (Terrain.GetAltitude(Vector3.Transform(Vector3.Backward * Time.ElapsedGameTime * 5, this.Transform.Rotation) + this.Transform.Position) < 0.1f)

                    this.Transform.LocalPosition += Vector3.Transform(Vector3.Backward * Time.ElapsedGameTime * 5, this.Transform.Rotation);
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                if (Terrain.GetAltitude(Vector3.Transform(Vector3.Left * Time.ElapsedGameTime * 5, this.Transform.Rotation) + this.Transform.Position) < 0.1f)

                    this.Transform.LocalPosition += Vector3.Transform(Vector3.Left * Time.ElapsedGameTime * 5, this.Transform.Rotation);
            }
            if (InputManager.IsKeyDown(Keys.D))
            {
                if (Terrain.GetAltitude(Vector3.Transform(Vector3.Right * Time.ElapsedGameTime * 5, this.Transform.Rotation) + this.Transform.Position) < 0.1f)

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
        public List<Vector3> path;

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
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1;
            collider.Transform = Transform;
            Add<SphereCollider>(collider);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);
            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.size.X / 2,
                         0,
                         gridH * j + gridH / 2 - Terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 1f)
                        search.Nodes[j, i].Passable = false;
                }
        }

        public override void Update()
        {

            if (path != null && path.Count > 0)
            {
                // Move to the destination along the path

                Vector3 currP = Transform.Position;
                Vector3 destP = GetGridPosition(path[0]);
                currP.Y = 0;
                destP.Y = 0;
                Vector3 direction = Vector3.Distance(destP, currP) == 0 ? Vector3.Zero : Vector3.Normalize(destP - currP);
                this.Rigidbody.Velocity = new Vector3(direction.X, 0, direction.Z) * speed;
                if (Vector3.Distance(destP, currP) < 1f) // if it reaches to a point, go to the next in path
                {
                    path.Remove(path[0]);
                    if (path.Count == 0) // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
            else
            {
                // Search again to make a new path.
                RandomPathFinding();
                this.Transform.LocalPosition = GetGridPosition(path[0]);
            }

            this.Transform.LocalPosition = new Vector3(
               this.Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               this.Transform.LocalPosition.Z) + Vector3.Up;
            base.Update();
        }
        public Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 
                0,
                gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }
        public Vector3 WorldToGrid(Vector3 target)
        {
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;
            return new ((int)((target.X + Terrain.size.X / 2 - gridW / 2) / gridW), 0,
            (int)((target.Z + Terrain.size.Y / 2 - gridH / 2) / gridH));
        }
        public virtual void OnCollisionWithPlayer()
        {
            Assignment5.Score += 1;
            RandomPathFinding();
            this.Transform.LocalPosition = GetGridPosition(path[0]);
        }

        public virtual void RandomPathFinding()
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
    public class Bomb : Agent
    {
        Player Player;
        public bool HitPlayer;
        float Timer = 5f;

        public Bomb(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, Player player) : base(terrain, Content, camera, graphicsDevice, light)
        {
            Player = player;
            Get<Renderer>().Material.Diffuse = Color.Red.ToVector3();
            Get<Renderer>().Material.Specular = Color.Red.ToVector3();
            Get<Renderer>().Material.Ambient = Color.Red.ToVector3();
            InitPathFind();
            this.Transform.LocalPosition = GetGridPosition(path[0]);
        }
        public override void Update()
        {
            Timer -= Time.ElapsedGameTime;
            base.Update();
            if (Timer <= 0)
            {
                RandomPathFinding();
                Timer = 5f;
            }
        }
        public override void OnCollisionWithPlayer()
        {
            HitPlayer = true;
        }
        public void InitPathFind()
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
        public override void RandomPathFinding()
        {


            if (path == null)
                search.Start = search.End;
            else
            {
                AStarNode node = search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z, (int)WorldToGrid(Transform.LocalPosition).X];
                if (node.Passable) 
                {
                    search.Start = node;
                }
                else
                {
                    if(search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z + 1, (int)WorldToGrid(Transform.LocalPosition).X].Passable)
                    {
                        search.Start = search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z + 1, (int)WorldToGrid(Transform.LocalPosition).X];
                    }
                    else if(search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z - 1, (int)WorldToGrid(Transform.LocalPosition).X].Passable)
                    {
                        search.Start = search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z - 1, (int)WorldToGrid(Transform.LocalPosition).X];
                    }
                    else if (search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z, (int)WorldToGrid(Transform.LocalPosition).X + 1].Passable)
                    {
                        search.Start = search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z, (int)WorldToGrid(Transform.LocalPosition).X + 1];
                    }
                    else if (search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z , (int)WorldToGrid(Transform.LocalPosition).X - 1].Passable)
                    {
                        search.Start = search.Nodes[(int)WorldToGrid(Transform.LocalPosition).Z, (int)WorldToGrid(Transform.LocalPosition).X - 1];
                    }
                }
            }
            
            Vector3 target = WorldToGrid(Player.Transform.LocalPosition);           
            if (search.Nodes[(int)target.Z, (int)target.X].Passable)
                search.End = search.Nodes[(int)target.Z, (int)target.X];
            else
            {
                if (search.Nodes[(int)target.Z + 1, (int)target.X].Passable)
                {
                    search.Start = search.Nodes[(int)target.Z + 1, (int)target.X];
                }
                else if (search.Nodes[(int)target.Z - 1, (int)target.X].Passable)
                {
                    search.Start = search.Nodes[(int)target.Z - 1, (int)target.X];
                }
                else if (search.Nodes[(int)target.Z, (int)target.X + 1].Passable)
                {
                    search.Start = search.Nodes[(int)target.Z, (int)target.X + 1];
                }
                else if (search.Nodes[(int)target.Z, (int)target.X - 1].Passable)
                {
                    search.Start = search.Nodes[(int)target.Z, (int)target.X - 1];
                }
            }
            search.Search();
            AStarNode current = search.End;
            path = new List<Vector3>();
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
            

        }
    }
    
}
