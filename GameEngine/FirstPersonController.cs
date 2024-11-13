using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class FirstPersonController : GameObject
    {
        public float MouseSensitivity = 2f;
        public float MovementSpeed = 15f;
        public float MaxSpeed = 5f;
        public FirstPersonController() : base()
        {
            Transform.Position = new(0,0,0);
            Add<Camera>();
            Add<Rigidbody>();
            Rigidbody.Mass = 1;
            Rigidbody.UseDrag = true;
            Rigidbody.KineticFriction = 0.2f;
            Add<SphereCollider>();
            Camera.FieldOfView = MathHelper.ToRadians(90);
            Camera.NearPlane = .1f;
        }
        public new void Update()
        {

            Vector3 howToMove = new Vector3(InputManager.HorizontalAxis, 0, -InputManager.VerticalAxis) * Time.ElapsedGameTime * MovementSpeed;
            if(InputManager.MouseDelta.X != 0)
                Camera.Transform.Rotate(Vector3.Up, -MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaX * MouseSensitivity));
            if(InputManager.MouseDeltaY != 0)
                Camera.Transform.Rotate(Vector3.Right, MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaY * MouseSensitivity));
            howToMove = Vector3.Transform(howToMove, Transform.Rotation);
            howToMove.Y = 0;
            Rigidbody.Acceleration += howToMove;
            base.Update();
        }
    }
}
