using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;
namespace Lab9
{
    public class Lab9 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Model cube;
        Camera camera;
        Model sphere;
        AStarSearch search;
        List<Vector3> path;
        Random random;

        int size;
        public Lab9()
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
            Time.Initialize();
            path = new List<Vector3>();
            size = 100;
            camera = new Camera();
            random = new Random();
            // ****************************************************************
            search = new AStarSearch(size, size); // size of grid 

            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(size), random.Next(size)].Passable = false;
            search.Start = search.Nodes[0,0];
            search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1];
            search.End.Passable = true;

            search.Search(); // A search is made here.

            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }
            // *****************************************************************
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = (Vector3.One * 10) * 5;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            cube = Content.Load<Model>("cube");
            sphere = Content.Load<Model>("sphere");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                int randRow = random.Next(size);
                int randCol = random.Next(size);
                search.Start = search.Nodes[randRow, randCol]; // assign a random start node (passable)
                randRow = random.Next(size);
                randCol = random.Next(size);
                search.End = search.Nodes[randRow, randCol]; // assign a random end node (passable)
                search.Start.Passable = true;
                search.End.Passable = true;
                search.Search();
                path.Clear();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }

            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) *
                       Matrix.CreateTranslation(node.Position), camera.View, camera.Projection);

            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.1f, 0.1f, 0.1f) *
                     Matrix.CreateTranslation(position), camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
