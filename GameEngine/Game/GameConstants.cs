using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public class GameConstants
    {
        //camera settings
        public const float CameraHeight = 8000.0f;

        //gameplay quantities
        public const int NumBullets = 30;
        public const int NumAsteroids = 10;
        public const int ShotPenalty = 10;
        public const int DeathPenalty = 100;
        public const int KillBonus = 100;

        //playfield demensions
        public const float PlayfieldSizeX = 1920 * 5.5f;
        public const float PlayfieldSizeY = 1080 * 7.5f;

        //speed settings
        public const float ShipSpeed = 1000.0f;
        public const float AsteroidMinSpeed = 2000.0f;
        public const float AsteroidMaxSpeed = 5000.0f;
        public const float BulletSpeed = 10000f;
        public const float BulletSpeedAdjustment = 10000f;


        //ship spawn position
        public const float PlayerSpawnX = 1920f;
        public const float PlayerSpawnY = 1080f;


    }
}
