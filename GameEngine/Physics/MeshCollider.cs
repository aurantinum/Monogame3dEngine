using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class MeshCollider : Collider
    {
        public Model Model { get; set; }
        public MeshCollider()
        {
            
        }
        public MeshCollider(Model model)
        {
            Model = model;
        }
        public override bool Collides(Collider other, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                normal = Vector3.Zero;
                SphereCollider collider = other as SphereCollider;
                
                foreach (var mesh in Model.Meshes)
                {
                    if (ContainmentType.Intersects == mesh.BoundingSphere.Contains(new BoundingSphere(collider.Transform.Position, collider.Radius)))
                    {
                        normal = Vector3.Normalize(Transform.Position - collider.Transform.Position);
                        return true;
                    }
                    
                }
            }
            else if(other is BoxCollider)
            {
                normal = Vector3.Zero;
                BoxCollider collider = other as BoxCollider;
                foreach (var mesh in Model.Meshes)
                {
                    if (ContainmentType.Intersects == mesh.BoundingSphere.Contains
                        (new BoundingBox(collider.Transform.Position - (Vector3.One * collider.Size), 
                        collider.Transform.Position + (Vector3.One * collider.Size))))
                    {
                        normal = Vector3.Normalize(Transform.Position - collider.Transform.Position);
                        return true;
                    }
                }
            }
            else if(other is MeshCollider)
            {
                normal = Vector3.Zero;
                MeshCollider collider = other as MeshCollider;
                
            }
            return base.Collides(other, out normal);
        }
        public override float? Intersects(Ray ray)
        {
            return null;
        }
    }
}
