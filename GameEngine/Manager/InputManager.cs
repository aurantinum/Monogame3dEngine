using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public static class InputManager
    {
        static KeyboardState PreviousKeyboardState { get; set; }
        static KeyboardState CurrentKeyboardState { get; set; }
        static MouseState PreviousMouseState { get; set; }
        static MouseState CurrentMouseState { get; set; }
        public static Keys North { set { north = value; } }
        static Keys north = Keys.W;
        public static Keys South { set { south = value; } }
        static Keys south = Keys.S;
        public static Keys East { set { east = value; } }
        static Keys east = Keys.D;
        public static Keys West { set { west = value; } }
        static Keys west = Keys.A;
        public static bool KeepMouseCentered = false;
        public static int Width = 800;
        public static int Height = 600;

        public static void Initialize()
        {
            PreviousKeyboardState = CurrentKeyboardState =
            Keyboard.GetState();
            PreviousMouseState = CurrentMouseState =
            Mouse.GetState();
        }

        public static void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            if (KeepMouseCentered)
            {
                CurrentMouseState = Mouse.GetState();
                Mouse.SetPosition(Width / 2, Height / 2);
                PreviousMouseState = Mouse.GetState();
            }
            else
            {
                PreviousMouseState = CurrentMouseState;

                CurrentMouseState = Mouse.GetState();
            }
            

        }
        public static bool IsLeftMouseDown()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsLeftMouseUp()
        {
            return CurrentMouseState.LeftButton == ButtonState.Released;
        }

        public static bool IsLeftMousePressed()
        {
            return IsLeftMouseDown() && PreviousMouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }
        public static bool IsKeyPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) &&
            PreviousKeyboardState.IsKeyUp(key);
        }
        //public static bool IsMousePressed(int button)
        //{
        //    return CurrentMouseState.(button) &&
        //   PreviousKeyboardState.IsKeyUp(key);
        //}
        public static bool IsKeyUp(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key);
        }
        public static bool IsKeyReleased(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key) &&
            PreviousKeyboardState.IsKeyDown(key);
        }
        public static float LeftMouse { get { return ((float)CurrentMouseState.LeftButton); } }
        public static float VerticalAxis { get { return (IsKeyDown(north)?1:0) - (IsKeyDown(south)?1:0); } }
        public static float HorizontalAxis { get { return (IsKeyDown(east)?1:0) - (IsKeyDown(west)?1:0); } }
        public static Vector2 MouseDelta { get {return (CurrentMouseState.Position.ToVector2() - PreviousMouseState.Position.ToVector2()); } }
        public static Vector2 MousePosition { get { return CurrentMouseState.Position.ToVector2(); }}
        public static float MouseX { get { return CurrentMouseState.X; }}
        public static float MouseY { get { return CurrentMouseState.Y; }}
        public static float MouseDeltaX { get { return (CurrentMouseState.X - PreviousMouseState.X); }}
        public static float MouseDeltaY { get { return (CurrentMouseState.Y - PreviousMouseState.Y); }}
    }
}
