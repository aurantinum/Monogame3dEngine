using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using CPI311.GameEngine;

namespace Lab8
{
    public class Lab8 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SoundEffect soundEffect;
        Camera camera, topDownCamera;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;
        Model model;
        Effect effect;
        Texture2D texture;
        public Lab8()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InputManager.Initialize();
            Time.Initialize();
            ScreenManager.Initialize(_graphics);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            soundEffect = Content.Load<SoundEffect>("Gun");

            // *** Lab 8 Item ***********************
            ScreenManager.Setup(true, 1920, 1080);
            //***************************************
            effect = Content.Load<Effect>("SimpleShading");
            texture = Content.Load<Texture2D>("Square");
            model = Content.Load<Model>("Sphere");
            colliders = new List<Collider>();
            transforms = new List<Transform>();
            cameras = new List<Camera>();   


            Transform transform = new Transform();
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1.2f;
            collider.Transform = transform;
            colliders.Add(collider);
            transforms.Add(transform);




            //
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);
            camera.Size = new Vector2(0.5f, 1f);
            camera.AspectRatio = camera.Viewport.AspectRatio;

            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            topDownCamera.Position = new Vector2(0.8f, 0f);
            topDownCamera.Size = new Vector2(0.2f, 0.4f);
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            //lab 8 
            Ray ray = camera.ScreenPointToWorldRay(InputManager.MousePosition);

            foreach (Collider collider in colliders)
            {
                if (collider.Intersects(ray) != null)
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                }
                else
                {
                    effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                }

            }

            base.Update(gameTime);
        }
        private void PlaySound()
        {
            SoundEffectInstance instance = soundEffect.CreateInstance();
            AudioListener listener = new AudioListener();
            listener.Position = camera.Transform.Position;
            listener.Forward = camera.Transform.Forward;
            AudioEmitter emitter = new AudioEmitter();
            emitter.Position = listener.Position;
            instance.Apply3D(listener, emitter);
            instance.Play();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Camera camera in cameras)
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState();
                GraphicsDevice.Viewport = camera.Viewport;
                Matrix view = camera.View;
                Matrix projection = camera.Projection;
                effect.CurrentTechnique = effect.Techniques[1];
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
                effect.Parameters["Shininess"].SetValue(20f);
                effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
                effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));
                effect.Parameters["DiffuseTexture"].SetValue(texture);
                foreach (Transform transform in transforms)
                {
                    effect.Parameters["World"].SetValue(transform.World);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        foreach (ModelMesh mesh in model.Meshes)
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                                GraphicsDevice.Indices = part.IndexBuffer;
                                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                    part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                            }
                    }
                }

                base.Draw(gameTime);

            }
        }
    }
}