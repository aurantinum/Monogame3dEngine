using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {
        public Color FillColor {  get; set; }
        private float _value;
        public float Value {
            get 
            {
                return _value; 
            }
            set 
            { 
                _value = Math.Clamp(value, 0f, MaxValue);
            } 
        }
        public float MaxValue { get; set; }
        public float Speed { get; set; }
        public int VerticalPadding, HorizontalPadding; 
        public ProgressBar(Texture2D background, Color fillColor) : base(background)
        {
            MaxValue = 100f;
            Value = 0f;
            FillColor = fillColor;
            Speed = 0f;
            VerticalPadding = 0;
            HorizontalPadding = 0;
        }
        public void Update()
        {
            Value += Speed * Time.ElapsedGameTime;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch); // let the sprite do its work
            Rectangle FillRect = new Rectangle(
                HorizontalPadding, 
                VerticalPadding,
                (int)((Value/MaxValue) * (float) (Texture.Width - HorizontalPadding)),
                Texture.Height - VerticalPadding);
            spriteBatch.Draw(Texture, Position, FillRect, FillColor, Rotation, Origin, Scale, Effects, Layer);
        }

    }
}
