using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Ship : GameObject
    {
        public Ship(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
            : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("p1_wedge"),
                Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // *** Add collider
            MeshCollider meshCollider = new MeshCollider();
            //sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            meshCollider.Model = renderer.ObjectModel;
            meshCollider.Transform = Transform;
            //sphereCollider.Transform = Transform;
            Add<Collider>(meshCollider);

            //*** Additional Property (for Asteroid, isActive = true)
            IsActive = false;
        }
        public new void Update()
        {
            if (IsActive)
            {
               if(InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))Transform.Position += Vector3.Forward * Time.ElapsedGameTime * GameConstants.ShipSpeed;
               if(InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))Transform.Position += Vector3.Left * Time.ElapsedGameTime * GameConstants.ShipSpeed;
               if(InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))Transform.Position += Vector3.Backward * Time.ElapsedGameTime * GameConstants.ShipSpeed;
               if(InputManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))Transform.Position += Vector3.Right * Time.ElapsedGameTime * GameConstants.ShipSpeed;
            }
            base.Update();
        }
    }
}
