using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CPI311.GameEngine
{
    public class FirstPersonController : GameObject
    {
        public float MouseSensitivity = 2f;
        public float MovementSpeed = 5f;
        public FirstPersonController() 
        {
            Add<Camera>();
            Add<Rigidbody>();
            Add<SphereCollider>();
            Camera.FieldOfView = MathHelper.ToRadians(90);
            Camera.NearPlane = .1f;
        }
        public new void Update()
        {

            Vector3 howToMove = new Vector3(InputManager.HorizontalAxis, 0, -InputManager.VerticalAxis) * Time.ElapsedGameTime * MovementSpeed;
            howToMove += new Vector3(0, 0, -InputManager.LeftMouse) * Time.ElapsedGameTime * MovementSpeed;

            //Handle view input for first person
            if (InputManager.IsKeyDown(Keys.Up))
            {
                Transform.Rotate(Vector3.Right, -MathHelper.ToRadians(Time.ElapsedGameTime * 10));
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                Transform.Rotate(Vector3.Right, -MathHelper.ToRadians(-Time.ElapsedGameTime * 10));
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                Camera.Transform.Rotate(Vector3.Up, -MathHelper.ToRadians(Time.ElapsedGameTime * 10));
            }
            if (InputManager.IsKeyDown(Keys.Right))
            {
                Camera.Transform.Rotate(Vector3.Up, -MathHelper.ToRadians(-Time.ElapsedGameTime * 10));
            }
            Transform.Rotate(Vector3.Up, -MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaX * MouseSensitivity));
            Camera.Transform.Rotate(Vector3.Right, -MathHelper.ToRadians(Time.ElapsedGameTime * InputManager.MouseDeltaY * MouseSensitivity));
            Rigidbody.Impulse += howToMove;

            base.Update();
        }
    }
}
