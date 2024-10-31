using CPI311.GameEngine;
using Microsoft.Xna.Framework;

namespace Assignment2
{
    public class RotateSphere : Component, CPI311.GameEngine.IUpdateable
    {
        public float rotationSpeed = 30f;
        float rotAmt = 0;
        public Vector3 axis;
        public RotateSphere() { }
        public void Update()
        {
            rotAmt += rotationSpeed * Time.ElapsedGameTime;
            GameObject.Transform.Rotate(axis, rotAmt);
        }
    }
}
