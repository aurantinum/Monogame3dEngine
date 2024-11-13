using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Asteroids :GameObject
    {
        Ship Ship;
        public Asteroids(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light, Ship ship)
            : base()
        {
            Ship = ship;
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("asteroid4"),
                Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            //*** Additional Property (for Asteroid, isActive = true)
            IsActive = false;
        }
        public new void Update()
        {
            if (!IsActive) return;

            if (Transform.Position.X > GameConstants.PlayfieldSizeX)
            {
                Transform.Position -= Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
                Rigidbody.Velocity = Rigidbody.Velocity.Length() * Vector3.Normalize(Transform.Position - Ship.Transform.Position) * -1;
            }
            if (Transform.Position.X < -GameConstants.PlayfieldSizeX)
            {
                Transform.Position += Vector3.UnitX * 2 * GameConstants.PlayfieldSizeX;
                Rigidbody.Velocity = Rigidbody.Velocity.Length() * Vector3.Normalize(Transform.Position - Ship.Transform.Position) * -1;

            }
            if (Transform.Position.Z > GameConstants.PlayfieldSizeY)
            {
                Transform.Position -= Vector3.UnitZ * 2 * GameConstants.PlayfieldSizeY;
                Rigidbody.Velocity = Rigidbody.Velocity.Length() * Vector3.Normalize(Transform.Position - Ship.Transform.Position) * -1;

            }
            if (Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                Transform.Position += Vector3.UnitZ * 2 * GameConstants.PlayfieldSizeY;
                Rigidbody.Velocity = Rigidbody.Velocity.Length() * Vector3.Normalize(Transform.Position - Ship.Transform.Position) * -1;

            }

            base.Update();
        }

    }
}
