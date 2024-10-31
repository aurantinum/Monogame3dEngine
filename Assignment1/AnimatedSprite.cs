using System;
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Assignment1
{
    public class AnimatedSprite : Sprite
    {
        public int Frames {  get; set; }
        public int Rows { get; set; }
        public float Frame {  get; set; }
        public int Row {  get; set; }
        public float Speed { get; set; }
        public AnimatedSprite(Texture2D texture, int frames = 1, int rows = 1) : base(texture)
        {
            Frames = frames;
            Rows = rows;
            Row = 0;
            Frame = 0;
            Speed = 1;
            Origin = new Vector2(Texture.Width / Frames, Texture.Height / Rows);
        }
        public void NextRow(int rowsToSkip = 0)
        {
            Row += 1 + (rowsToSkip % Rows);
            while(Row > Rows) Row = 0 + (Row - Rows - 1);
        }
        public void Update(GameTime gameTime)
        {
            Frame += Speed * Time.ElapsedGameTime;
            if (Frame > Frames) Frame = 0;
            Source = new Rectangle((int)Frame * (Texture.Width / Frames), Row * (Texture.Height/Rows), Texture.Width/Frames, Texture.Height/Rows);
        }
    }
}
