﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;


namespace CPI311.GameEngine
{
    public class Checkbox : GUIElement
    {
        public Texture2D Box {  get; set; }
        public bool Checked { get; set; }
        public override void Update()
        {
            if (InputManager.IsMouseReleased(0) &&
                    Bounds.Contains(InputManager.MousePosition))
            {
                Checked = !Checked;
                OnAction();
            }
        }
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            int width = Math.Min(Bounds.Width, Bounds.Height);
            spriteBatch.Draw(Box, new Rectangle(Bounds.X, Bounds.Y, width, width),
            Checked ? Color.Red : Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + width,
            Bounds.Y), Color.Black);
        }
    }
}
