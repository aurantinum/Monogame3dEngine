using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public class Rigidbody : Component, IUpdateable
    {
        public bool UseGravity { get; set; }
        public Transform PreviousTransform { get; set; }
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Impulse { get; set; }
        public float KineticFriction { get; set; }
        public bool UseDrag { get; set; }
        public Rigidbody()
        {
            KineticFriction = 0;
            UseDrag = false;
            UseGravity = false;
            PreviousTransform = new Transform();
        }
        public void Update()
        {
            PreviousTransform.Position = Transform.Position;
            PreviousTransform.Rotation = Transform.Rotation;
            PreviousTransform.Scale = Transform.Scale;
            //if (UseGravity)
            //{
            //    Acceleration += new Vector3(0, Physics.Gravity, 0);
            //}
            Velocity += (Acceleration * Time.ElapsedGameTime) + (Impulse / Mass);
            Transform.LocalPosition += (Velocity * Time.ElapsedGameTime);
            if (UseDrag)
            {
                if (Acceleration.Length() > 0)
                {
                    Vector3 friction = KineticFriction * new Vector3(0, Physics.Gravity, 0) * Mass * Time.ElapsedGameTime;
                    float accMag = Acceleration.Length();
                    Acceleration.Normalize();
                    Acceleration = (Acceleration) * ((accMag - friction.Length()) / Mass);
                }
                else
                {
                    Acceleration = Vector3.Zero;
                    if (Velocity.Length() > 0)
                    {
                        Vector3 friction = KineticFriction * new Vector3(0, Physics.Gravity, 0) * Mass * Time.ElapsedGameTime;
                        float velMag = Velocity.Length();
                        Velocity.Normalize();
                        Velocity = (Velocity) * (velMag - friction.Length());
                    }
                    else Velocity = Vector3.Zero; 
                }
            }
            Impulse = Vector3.Zero;
        }
    }
}
