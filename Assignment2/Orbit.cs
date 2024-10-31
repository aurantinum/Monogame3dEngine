using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;


namespace Assignment2
{
    public class Orbit : Component, CPI311.GameEngine.IUpdateable
    {
        public GameObject toOrbitAround
        {
            get { return orbitTarget; } set
            {
                orbitTarget = value;
                orbitDistance = (orbitTarget.Transform.LocalPosition - Transform.LocalPosition).Length();
            }
        }
        GameObject orbitTarget;
        public float orbitSpeed = 3;
        float orbitDistance;
        float degrees = 0f;
        public Orbit()
        {
            //orbitTarget = GameObject;
            //orbitDistance = (orbitTarget.Transform.LocalPosition - Transform.LocalPosition).Length();
        }

        public void Update()
        {
            Debug.WriteLine(orbitTarget.Transform.LocalPosition);
            degrees += Time.ElapsedGameTime * orbitSpeed;
            Transform.LocalPosition = new Vector3((orbitDistance * (float)Math.Cos(degrees)),
                0,
                (orbitDistance * (float)Math.Sin(degrees)));
            Transform.LocalPosition = toOrbitAround.Transform.LocalPosition + (Vector3.Normalize(Transform.LocalPosition) * orbitDistance);
        }
    }
}
