using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;

namespace FinalProject
{
    public class FinalProject : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Model cube, sphere;
        SpriteFont font;
        int Score;
        Texture2D square;
        Effect effect;
        TerrainRenderer terrain;
        Camera camera;
        Light light;
        List<eBullet> gos;
        List<Upgrade> upgrades;
        List<Sprite> menuSprites;
        List<GUIElement> menuElements;
        GameObject playerSphere;
        Dictionary<String, Scene> scenes;
        Scene currentScene;
        //Agent agent;
        List<Agent> agents;
        int Health = 5;
        int MaxHealth = 5;
        ProgressBar healthBar;
        int bulletCount = 0;
        int maxBullets = 20;
        int Wave = 1;
        public FinalProject()
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
            scenes = new Dictionary<string, Scene>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(MainGameUpdate, MainGameDraw));
            currentScene = scenes["Menu"];
            // TODO: use this.Content to load your game content here
            cube = Content.Load<Model>("cube");
            sphere = Content.Load<Model>("Sphere");
            font = Content.Load<SpriteFont>("Font");
            square = Content.Load<Texture2D>("Square");
            LoadMenu();
            LoadLevel();
            //gos.Add(playerSphere);
        }
        private void MainMenuUpdate()
        {
            foreach (var a in menuElements)
            {
                a.Update();
            }
        }
        private void MainMenuDraw()
        {
            GraphicsDevice.Clear(Color.AntiqueWhite);
            _spriteBatch.Begin();
            foreach (var a in menuElements)
            {
                a.Draw(_spriteBatch, font);
            }
            _spriteBatch.End();
        }
        private void LoadMenu()
        {
            menuSprites = new List<Sprite>();
            menuElements = new List<GUIElement>();
            Button b = new Button();
            b.Texture = square;
            b.Text = "Start";
            b.Bounds = new Rectangle(50, 50, 300, 20);
            b.Action += ChangeMenuToLevel;
            menuElements.Add(b);
        }
        private void ChangeMenuToLevel(GUIElement g)
        {
            LoadLevel();
            currentScene = scenes["Play"];
        }
        private void ChangeLevelToMenu()
        {
            LoadMenu();
            currentScene = scenes["Menu"];
        }
        private void LoadLevel()
        {
            MaxHealth = 5; Health = MaxHealth;
            maxBullets = 20; Wave = 1;
            bulletCount = 0;
            terrain = new TerrainRenderer(Content.Load<Texture2D>("TexturedLevelHM"),
                Vector2.One * 100, Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("TexturedLevelNM");
            terrain.Transform = new Transform();
            terrain.Transform.Scale *= new Vector3(1, 5, 1);
            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(Color.Black.ToVector3());
            effect.Parameters["DiffuseColor"].SetValue(Color.DarkGray.ToVector3());
            effect.Parameters["SpecularColor"].SetValue(Color.DarkGray.ToVector3());
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.Position = Vector3.Up * 50;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            light = new Light();
            Score = 0;
            light.Transform = new Transform();
            light.Transform.Position = Vector3.One * 5 + Vector3.Backward * 70;
            gos = new List<eBullet>();
            agents = new List<Agent>();
            upgrades = new List<Upgrade>();
            playerSphere = new GameObject();
            playerSphere.Transform.LocalPosition += Vector3.Right * 5;
            Renderer renderer = (new Renderer(sphere, playerSphere.Transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20, square));
            renderer.Material.Diffuse = Color.BlueViolet.ToVector3();
            renderer.Material.Specular = Color.BlueViolet.ToVector3();
            renderer.Material.Ambient = Color.BlueViolet.ToVector3();
            playerSphere.Add<Renderer>(renderer);
            Transform t = new Transform();
            t.LocalPosition = Vector3.Zero;
            playerSphere.Add<OrbitAndRotate>();
            playerSphere.Get<OrbitAndRotate>().toOrbitAround = t;
            healthBar = new ProgressBar(square, Color.Green);
            healthBar.Value = Health;
            healthBar.MaxValue = Health;
            healthBar.Position = new Vector2(40, 20);
            healthBar.Scale = new Vector2(2, 1);
            Agent agent = new Agent(terrain, Content, camera, GraphicsDevice, light);
            agents.Add(agent);
           
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            Time.Update(gameTime);
            InputManager.Update();
            currentScene.Update();
            base.Update(gameTime);
        }

        private void MainGameUpdate()
        {
            if(Health < 0)
            {
                ChangeLevelToMenu();
            }
            if (InputManager.IsKeyDown(Keys.Space))
            {
                foreach (var a in upgrades)
                {
                    a.ShouldMove = true;
                }
            }
            else
            {
                foreach (var a in upgrades)
                {
                    a.ShouldMove = false;
                }
            }
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                if (bulletCount < maxBullets)
                {
                    eBullet bullet = new eBullet(camera, Content, light, GraphicsDevice, Vector3.Normalize(playerSphere.Transform.Position), terrain);
                    gos.Add(bullet);
                    bulletCount++;
                }
            }
            if (InputManager.IsKeyReleased(Keys.Space))
            {

            }
            if (InputManager.IsKeyUp(Keys.Space))
            {

            }


            playerSphere.Update();
            for (int i = 0; i < upgrades.Count; i++)
            {
                var a = upgrades[i];
                a.Update();
                if (a.shouldDealDamage)
                {
                    switch (a.upgradeType)
                    {
                        case UpgradeType.HealthRestore:
                            Health += a.OnCollision();
                            break;
                        case UpgradeType.MaxHealth:
                            MaxHealth += a.OnCollision();
                            break;
                        case UpgradeType.MaxAmmo:
                            maxBullets += a.OnCollision();
                            break;
                    }
                    upgrades.Remove(a);
                    i--;
                }
                if (a.LifeTime <= 0)
                {
                    upgrades.Remove(a);
                    i--;
                }
            }
            for (int i = 0; i < gos.Count; i++)
            {
                var a = gos[i];
                a.Update();
                foreach (var b in agents)
                {
                    if (!b.isDone)
                    {
                        Vector3 normal;
                        if (a.Get<SphereCollider>().Collides(b.Get<SphereCollider>(), out normal))
                        {
                            Score += b.OnCollision();
                            if (b.ShouldSpawnUpgrade)
                            {
                                upgrades.Add(new Upgrade(terrain, Content, camera, GraphicsDevice, light));
                            }
                            b.isDone = true;
                            a.shouldDestroy = true;
                        }
                    }
                }
                if (a.shouldDestroy)
                {
                    gos.Remove(a);
                    i--;
                    bulletCount--;
                }
            }
            int notDone = 0;
            foreach (var a in agents)
            {
                a.Update();

                if (a.shouldDealDamage)
                {
                    a.isDone = true;
                    a.shouldDealDamage = false;
                    Health -= 1;
                }
                if (!a.isDone) notDone++;
            }
            if (notDone == 0)
            {
                //new wave
                foreach (var a in agents)
                {
                    a.Reset();
                }
                Wave++;
                Agent agent = new Agent(terrain, Content, camera, GraphicsDevice, light);
                agents.Add(agent);
                agent.Reset();
                for (int i = 0; i < gos.Count; i++)
                {
                    gos.RemoveAt(i);
                    i--;
                    bulletCount--;
                }
            }

            healthBar.Value = Health;
            healthBar.MaxValue = MaxHealth;
            healthBar.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            currentScene.Draw();
            base.Draw(gameTime);
        }

        private void MainGameDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
                playerSphere.Draw();
                foreach (var a in agents)
                {
                    if (!a.isDone)
                    {
                        a.Draw();
                    }
                }
                foreach (var a in gos)
                {
                    a.Draw();
                }
                foreach (var a in upgrades) a.Draw();

            }
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Score: " + Score + "\nWave:" + Wave + "\nAmmo:" + gos.Count + "/" + maxBullets, new Vector2(50, 50), Color.Black);
            healthBar.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }
    public class eBullet : GameObject
    {
        Vector3 Direction;
        TerrainRenderer Terrain;
        public eBullet(Camera camera, Microsoft.Xna.Framework.Content.ContentManager Content, Light light, GraphicsDevice GraphicsDevice, Vector3 direction, TerrainRenderer terrain) : base()
        {
            Terrain = terrain;
            Direction = direction;
            direction.Y = 0;            
            Transform.Position = new(0, 0, 0);
            Transform.Scale = new(.5f, .5f, .5f);
            Add<Rigidbody>();
            Get<Rigidbody>().Transform = Transform;
            Get<Rigidbody>().Mass = 1;
            Add<SphereCollider>();
            Get<SphereCollider>().Radius = 1f;
            Get<SphereCollider>().Transform = Transform;
            Renderer renderer = (new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, GraphicsDevice, light, 1, "SimpleShading", 20, Content.Load<Texture2D>("Square")));
            renderer.Material.Diffuse = Color.Black.ToVector3();
            renderer.Material.Specular = Color.Black.ToVector3();
            renderer.Material.Ambient = Color.Black.ToVector3();
            Add<Renderer>(renderer);

        }
        public bool shouldDestroy = false;
        public override void Update()
        {
            if (Transform.LocalPosition.X >  256|| Transform.LocalPosition.Y > 256 || Transform.LocalPosition.X < -256 || Transform.LocalPosition.Y < -256)
            {
                shouldDestroy = true;
            }
            if (Terrain.GetAltitude(Transform.LocalPosition) > 1f)
                shouldDestroy = true;
            this.Rigidbody.Velocity = Direction * 5f;
            this.Transform.LocalPosition = new Vector3(
               this.Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               this.Transform.LocalPosition.Z) + Vector3.Up;
            base.Update();
        }

    }
    public class OrbitAndRotate : Component, CPI311.GameEngine.IUpdateable
    {
        public Transform toOrbitAround
        {
            get { return orbitTarget; }
            set
            {
                orbitTarget = value;
                orbitDistance = (orbitTarget.LocalPosition - Transform.LocalPosition).Length();
            }
        }
        Transform orbitTarget;
        public float orbitSpeed = 3;
        public float orbitDistance;
        float degrees = 0f;
        public float rotationSpeed = 30f;
        float rotAmt = 0;
        public Vector3 axis;
        public OrbitAndRotate() { }
        public void Update()
        {
            rotAmt += rotationSpeed * Time.ElapsedGameTime;
            GameObject.Transform.Rotate(axis, rotAmt);
            degrees += Time.ElapsedGameTime * orbitSpeed;
            Transform.LocalPosition = new Vector3((orbitDistance * (float)Math.Cos(degrees)),
                0,
                (orbitDistance * (float)Math.Sin(degrees)));
            Transform.LocalPosition = toOrbitAround.LocalPosition + (Vector3.Normalize(Transform.LocalPosition) * orbitDistance);
        }
    }
    public class Agent : GameObject
    {
        public TerrainRenderer Terrain { get; set; }
        public AStarSearch search;
        public List<Vector3> path;
        public bool shouldDealDamage = false;
        public bool isDone = false;
        private float speed = 5f; //moving speed
        private int gridSize = 256; //grid size
        public bool ShouldSpawnUpgrade = false;

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
            Get<Renderer>().Material.Diffuse = Color.Red.ToVector3();
            Get<Renderer>().Material.Specular = Color.Red.ToVector3();
            Get<Renderer>().Material.Ambient = Color.Red.ToVector3();
            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.size.X / 2,
                         0,
                         gridH * j + gridH / 2 - Terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 0.75f)
                        search.Nodes[j, i].Passable = false;
                }
            isDone = true;
            Reset();
        }

        public override void Update()
        {
            if (!isDone)
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
                            isDone = true;
                            shouldDealDamage = true;
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
            }
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
            return new((int)((target.X + Terrain.size.X / 2 - gridW / 2) / gridW), 0,
            (int)((target.Z + Terrain.size.Y / 2 - gridH / 2) / gridH));
        }
        public virtual int OnCollision()
        {
            isDone = true;
            return 1;
        }

        public virtual void Reset()
        {
            RandomPathFinding();
            ShouldSpawnUpgrade = new Random().Next(10) > 7;
            this.Transform.LocalPosition = GetGridPosition(path[0]);
            isDone = false;
            shouldDealDamage = false;
        }
        public virtual void RandomPathFinding()
        {
            Random random = new Random();
            int x = random.Next(search.Rows);
            int y = random.Next(search.Cols);
            
            while (!(search.Start = search.Nodes[x,
            y]).Passable
            || !(search.Rows / 2 - search.Rows / 4 < x && x < search.Rows / 2 + search.Rows / 4)
            || !(search.Cols / 2 - search.Cols / 4 < y && y < search.Cols / 2 + search.Cols / 4))
            {
                x = random.Next(search.Rows);
                y = random.Next(search.Cols);
            }
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
    public enum UpgradeType
    {
        HealthRestore, MaxHealth, MaxAmmo,
    }
    public class  Upgrade : Agent
    {
        public bool ShouldMove = false;
        public float LifeTime = 5f;
        public UpgradeType upgradeType;
        public Upgrade(TerrainRenderer terrain, ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base(terrain, Content, camera, graphicsDevice, light)
        {
            Random random = new Random();
            switch (random.Next(3))
            {
                case (int)UpgradeType.HealthRestore:
                    Get<Renderer>().Material.Diffuse = Color.Green.ToVector3();
                    Get<Renderer>().Material.Specular = Color.Green.ToVector3();
                    Get<Renderer>().Material.Ambient = Color.Green.ToVector3();
                    upgradeType = UpgradeType.HealthRestore;
                    break;
                case (int)UpgradeType.MaxHealth:
                    Get<Renderer>().Material.Diffuse = Color.Orange.ToVector3();
                    Get<Renderer>().Material.Specular = Color.Orange.ToVector3();
                    Get<Renderer>().Material.Ambient = Color.Orange.ToVector3();
                    upgradeType = UpgradeType.MaxHealth;
                    break;
                case (int)UpgradeType.MaxAmmo:
                    Get<Renderer>().Material.Diffuse = Color.Yellow.ToVector3();
                    Get<Renderer>().Material.Specular = Color.Yellow.ToVector3();
                    Get<Renderer>().Material.Ambient = Color.Yellow.ToVector3();
                    upgradeType = UpgradeType.MaxAmmo;
                    break;
            }
        }
        public override void Update()
        {
            LifeTime -= Time.ElapsedGameTime;
            ShouldSpawnUpgrade = false;
            if (ShouldMove)
            {
                base.Update();
            }
        }

    }
    public class Scene
    {
        public delegate void CallMethod();
        public CallMethod Update;
        public CallMethod Draw;
        public Scene(CallMethod update, CallMethod draw)
        { Update = update; Draw = draw; }
    }
}
