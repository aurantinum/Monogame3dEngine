using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Material 
    {
        public Texture2D Texture;
        public float Shininess;
        public Effect effect;
        public int Passes { get { return effect.CurrentTechnique.Passes.Count; } }
        public int CurrentTechnique { get; set; }
        public Matrix World;
        public Camera Camera;
        public Vector3 Diffuse {  get; set; } 
        public Vector3 Ambient { get; set; }
        public Vector3 Specular { get; set; }
        public Light Light { get; set; }
        public Material(Matrix world, Camera camera, Light light, ContentManager content, string filename,
            int currentTechnique, float shininess, Texture2D texture)
        {
            Light = light;
            effect = content.Load<Effect>(filename);
            World = world; Camera = camera;
            Texture = texture;
            Shininess = shininess;
            CurrentTechnique = currentTechnique;
            Diffuse = Color.Gray.ToVector3();
            Specular = Color.Gray.ToVector3();
            Ambient = Color.Gray.ToVector3();
        }
        public virtual void Apply(int currentPass)
        {
            effect.CurrentTechnique = effect.Techniques[CurrentTechnique];
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.Position);
            effect.Parameters["CameraPosition"].SetValue(Camera.Transform.Position);
            effect.Parameters["Shininess"].SetValue(Shininess);
            effect.Parameters["AmbientColor"].SetValue(Ambient);
            effect.Parameters["DiffuseColor"].SetValue(Diffuse);
            effect.Parameters["SpecularColor"].SetValue(Specular);
            effect.Parameters["DiffuseTexture"].SetValue(Texture);
            effect.CurrentTechnique.Passes[currentPass].Apply();
        }
    }
}
