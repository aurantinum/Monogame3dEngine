using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public interface IUpdateable { void Update(); }
    public interface IRenderable { void Draw(); }
    public interface IDrawable { void Draw(SpriteBatch spriteBatch); }

}
