using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using CPI311.GameEngine;

namespace Lab002
{
    public static class SpiralMover
    {
        private static float radiusMl = 1f;
        private static float phaseMl = 1f;
        public static Sprite sprite { get; set; }
        static Vector2 Position { get; set; }
        static float Phase { get; set; }
        static float Radius { get; set; }
        static float Frequency { get; set; }
        static float Amplitude { get; set; }
        static float Speed { get; set; }
        public static void SpiralMove(Texture2D texture, Vector2 position, float radius = 150, float speed = 0.01f,
            float frequency = 20, float amplitude = 10, float phase = 0)
        {
            sprite = new Sprite(texture);
            Position = position;
            Radius = radius;
            Frequency = frequency;  
            Amplitude = amplitude;
            Speed = speed;

            sprite.Position = position;
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
        public static void Update(GameTime gameTime)
        {
            float lInput = 0f, rInput = 0f, uInput = 0f, dInput = 0f;
            Phase += Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.Left))
            {
                lInput = -1f * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.Right))
            {
                rInput = Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.Up))
            {
                uInput = -1f * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                dInput =  Time.ElapsedGameTime;
            }
            phaseMl += uInput + dInput;
            radiusMl += lInput + rInput;
            
            sprite.Position = Position + new Vector2(
                (float)(((Radius * radiusMl)+ Math.Cos(Phase * phaseMl)) * Math.Cos(Phase * phaseMl)),
                (float)(((Radius * radiusMl) + Math.Cos(Phase * phaseMl)) * Math.Sin(Phase * phaseMl)));
        }
    }
}
