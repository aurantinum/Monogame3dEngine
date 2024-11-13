using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System;
using CPI311.GameEngine;
using System.Diagnostics;

namespace Assignment4
{
    public class Assignment4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Random random;
        Camera camera;
        Light light;
        //Audio components
        SoundEffect gunSound;
        SoundEffectInstance soundInstance;
        //Visual components
        Ship ship;
        Asteroids[] asteroidList = new Asteroids[GameConstants.NumAsteroids];
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        //Score & background
        int score;
        Texture2D stars;
        SpriteFont lucidaConsole;
        // Particles
        ParticleManager particleManager;
        Texture2D particleTex;
        Effect particleEffect;

        SoundEffect asteroidExplosion, shipExplosion, engineSound;
        private SoundEffectInstance engineSoundInstance;
        int bulletCount, asteroidCount;

        bool sound;
        // Define these as class fields
        private readonly Vector2 scorePosition = new Vector2(20, 20);
        private readonly Color textColor = Color.White;
        private readonly StringBuilder stringBuilder = new StringBuilder();


        public Assignment4()
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
            ScreenManager.Setup((int)GameConstants.PlayfieldSizeX, (int)GameConstants.PlayfieldSizeY);
            camera = new Camera();
            camera.Transform.LocalPosition = Vector3.Up * GameConstants.CameraHeight;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            camera.NearPlane = GameConstants.CameraHeight - 1000f;
            camera.FarPlane = GameConstants.CameraHeight + 1000f;
            random = new Random();
            ship = new Ship(Content, camera, GraphicsDevice, light);
            for (int i = 0; i < GameConstants.NumBullets; i++)
                bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
            ResetAsteroids(); // look at the below private method
            
            bulletCount = bulletList.Length;
            asteroidCount = asteroidList.Length;
            stars = Content.Load<Texture2D>("B1_stars");
            gunSound = Content.Load<SoundEffect>("tx0_fire1");
            asteroidExplosion = Content.Load<SoundEffect>("explosion2");
            shipExplosion = Content.Load<SoundEffect>("explosion2");
            engineSound = Content.Load<SoundEffect>("engine_2");
            engineSoundInstance = engineSound.CreateInstance();
            engineSoundInstance.IsLooped = true;
            lucidaConsole = Content.Load<SpriteFont>("Font");
            // *** Particle
            particleManager = new ParticleManager(GraphicsDevice, 100);
          
            particleEffect = Content.Load<Effect>("ParticleShader-complete");
            particleTex = Content.Load<Texture2D>("fire");
            sound = true;
            ship.IsActive = true;
        }
        private void ResetAsteroids()
        {
            float xStart;
            float yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                else
                    xStart = (float)GameConstants.PlayfieldSizeX;
                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                asteroidList[i] = new Asteroids(Content, camera, GraphicsDevice, light, ship);
                asteroidList[i].Transform.Position = new Vector3(xStart, 0.0f, yStart);
                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].Rigidbody.Velocity = new Vector3(
                   -(float)Math.Sin(angle), 0, (float)Math.Cos(angle)) *
            (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() *
            GameConstants.AsteroidMaxSpeed);
                asteroidList[i].IsActive = true;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);
            ship.Update();
            particleManager.Update();

            ManageEngineSound();
            for (int i = 0; i < bulletList.Length; i++)
                bulletList[i].Update();
            for (int i = 0; i < asteroidList.Length; i++) 
                asteroidList[i].Update();

            if(score < 0) score = 0;

            if (InputManager.IsLeftMousePressed())
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    if (!bulletList[i].IsActive)
                    {
                        bulletList[i].Rigidbody.Velocity =
                             (ship.Transform.Forward) * GameConstants.BulletSpeedAdjustment;
                        bulletList[i].Transform.LocalPosition = ship.Transform.Position +
                                                    (200 * bulletList[i].Transform.Forward);
                        bulletList[i].IsActive = true;
                        score -= GameConstants.ShotPenalty;
                        // sound
                        soundInstance = gunSound.CreateInstance();
                        soundInstance.Play();
                        break; //exit the loop 	
                    }
                }
            }
        Vector3 normal;
            for (int i = 0; i < asteroidList.Length; i++)
            {
                if (asteroidList[i] == null) continue;
                if (asteroidList[i].IsActive)
                {
                    for (int j = 0; j < bulletList.Length; j++)
                    {
                        if (bulletList[j] == null) continue;

                        /*
                        && !(bulletList[j].Transform.Position.X > GameConstants.PlayfieldSizeX ||
                        bulletList[j].Transform.Position.X < -GameConstants.PlayfieldSizeX ||
                        bulletList[j].Transform.Position.Z > GameConstants.PlayfieldSizeY ||
                        bulletList[j].Transform.Position.Z < -GameConstants.PlayfieldSizeY))
                         */
                        if (bulletList[j].IsActive )
                        {
                            if (asteroidList[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                
                                Particle particle = particleManager.getNext();
                                particle.Position = asteroidList[i].Transform.Position;
                                particle.Velocity = new Vector3(
                                    random.Next(-5, 5), 2, random.Next(-50, 50));
                                particle.Acceleration = new Vector3(0, 3, 0);
                                particle.MaxAge = random.Next(10, 12);
                                particle.Init();
                                asteroidList[i].IsActive = false;

                            }
                        }
                    
                    }
                }
            }
            base.Update(gameTime);
        }
        private void ManageEngineSound()
        {
            bool isMoving = InputManager.HorizontalAxis + InputManager.VerticalAxis > 0;
            if(isMoving && engineSoundInstance.State != SoundState.Playing)
                engineSoundInstance.Play();
            else if (!isMoving && engineSoundInstance.State == SoundState.Playing) 
                engineSoundInstance.Stop();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            stringBuilder.Clear();
            AppendGameText(stringBuilder);
            _spriteBatch.Begin();
            _spriteBatch.Draw(stars, new Rectangle(0,0,ScreenManager.Width, ScreenManager.Height), Color.White);
            _spriteBatch.DrawString(lucidaConsole, stringBuilder, scorePosition, textColor);
            _spriteBatch.End();
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ship.Draw();
            for(int i = 0; i < GameConstants.NumBullets; i++)
            {
                if(bulletList[i].IsActive)
                    bulletList[i].Draw();
            }
            for(int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if(asteroidList[i].IsActive)
                    asteroidList[i].Draw();
            }
            particleManager.Draw(GraphicsDevice);


            base.Draw(gameTime);
        }
        private void AppendGameText(StringBuilder stringBuilder) {stringBuilder.Append(score); }
    }
}
