using Microsoft.Xna.Framework;
using System;

namespace CPI311.GameEngine
{
    public class SphereCollider : Collider
    {
        public float Radius { get; set; }
        public override bool Collides(Collider other, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;
                if ((Transform.Position - collider.Transform.Position).LengthSquared()
                < System.Math.Pow(Radius + collider.Radius, 2))
                {
                    normal = Vector3.Normalize
                    (Transform.Position - collider.Transform.Position);
                    return true;
                }
            }
            else if (other is BoxCollider) return other.Collides(this, out normal);

            return base.Collides(other, out normal);
        }
        public bool SweptCollides(Collider other, Vector3 otherLastPosition,
            Vector3 lastPosition, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;
                // Calculate the vectors for two spheres
                Vector3 vp = Transform.Position - lastPosition; // Velocity of this sphere
                Vector3 vq = collider.Transform.Position - otherLastPosition; // Velocity of other sphere

                // Calculate the A and B (relative velocity and distance)
                Vector3 A = vq - vp; // Relative velocity vector
                Vector3 B = Transform.Position - collider.Transform.Position; // Initial distance vector between the centers

                // Calculate the coefficients a, b, and c
                float a = Vector3.Dot(A, A); // a = |A|^2
                float b = 2 * Vector3.Dot(B, A); // b = 2 * (B • A)
                float c = Vector3.Dot(B, B) - (Radius + collider.Radius) * (Radius + collider.Radius); // c = |B|^2 - (r1 + r2)^2

                float disc = (b * b) - (4 * a * c); // Discriminant (b^2 - 4ac)

                if (disc >= 0)
                {
                    // Solve for t (we're interested in the first root)
                    float t = (-b - MathF.Sqrt(disc)) / (2 * a);
                    float t2 = (-b + MathF.Sqrt(disc)) / (2 * a);
                    t = t > t2 ? t : t2;
                    if (t >= 0 && t <= 1)
                    {
                        Vector3 p = lastPosition + t * vp;
                        Vector3 q = otherLastPosition + t * vq;
                        Vector3 intersect = Vector3.Lerp(
                        p, q, this.Radius / (this.Radius + collider.Radius));
                        normal = Vector3.Normalize(intersect);
                        return true;
                    }
                }

            }
            else if (other is BoxCollider) return other.Collides(this, out normal);
            return base.Collides(other, out normal);
        }
        public override float? Intersects(Ray ray)
        {
            Matrix worldInv = Matrix.Invert(Transform.World);
            ray.Position = Vector3.Transform(ray.Position, worldInv);
            ray.Direction = Vector3.TransformNormal(ray.Direction, worldInv);
            float length = ray.Direction.Length();
            ray.Direction /= length; // same as normalization
            float? p =  new BoundingSphere(Vector3.Zero, Radius).Intersects(ray);
            if (p != null)
                return (float)p * length;
            return null;
        }

    }
}
