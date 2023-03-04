using EndlessRunner.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessRunner.Entities
{
    public class PowerupManager : IGameEntity
    {
        private const int POWERUP_DESPAWN_DISTANCE = -100;

        Texture2D _spriteSheetTexture;

        private Astronaut _astronaut;
        private EntityManager _entityManager;
        private ScoreBoard _scoreBoard;
        private EnemyManager _enemyManager;
        private MenuManager _menuManager;

        private const int SPAWN_MIN_DISTANCE = 100;
        private const int SPAWN_MAX_DISTANCE = 200;
        private double _previousSpawnScore;
        private double _targetSpawnScore;

        private const int AMMO_RNG = 30;
        private const int GRAVITY_RNG = 25;
        private const int REPELLENT_RNG = 30;
        private const int SHIELD_RNG = 30;

        private List<Powerup> _powerups = new List<Powerup>();

        public int DrawOrder => 0;
        public bool IsEnabled { get; set; }

        public PowerupManager(Texture2D spriteSheet, Astronaut astronaut, EntityManager entityManager, ScoreBoard scoreBoard, EnemyManager enemyManager, MenuManager menuManager)
        {
            _spriteSheetTexture = spriteSheet;

            _astronaut = astronaut;
            _entityManager = entityManager;
            _scoreBoard = scoreBoard;
            _enemyManager = enemyManager;
            _menuManager = menuManager;

            _targetSpawnScore = 75;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            List<Platform> platforms = new List<Platform>();
            Random r = new Random();

            // Gets the list of all the platforms that are not on the screen
            // These can be used to spawn powerups
            foreach (Platform p in _entityManager.GetEntitiesOfType<Platform>())
            {
                if (p.Position.X > DeadSpaceGame.WINDOW_WIDTH)
                    platforms.Add(p);
            }

            // Removes every entity that has moved off the screen from the list of entites
            foreach (Powerup p in _entityManager.GetEntitiesOfType<Powerup>())
            {
                if (p.Position.X < POWERUP_DESPAWN_DISTANCE)
                    _entityManager.RemoveEntity(p);
            }

            // A new powerup is spawned based on the score
            foreach (Platform p in platforms)
            {
                if (!p.HasObject)
                {
                    if (_scoreBoard.Score - _previousSpawnScore >= _targetSpawnScore)
                    {
                        _targetSpawnScore = r.NextDouble() * (SPAWN_MAX_DISTANCE - SPAWN_MIN_DISTANCE) + SPAWN_MIN_DISTANCE;

                        _previousSpawnScore = _scoreBoard.Score;

                        SpawnPowerup(p);
                        p.HasObject = true;
                    }
                }
            }

            // If the player collides with a powerup then the result is dealt with
            foreach (Powerup p in _powerups)
            {
                if (p.CollisionBox.Intersects(_astronaut.CollisionBox))
                    ProcessPowerup(p);
            }

            // Removes powerups that haven been collected
            List<Powerup> powerupsToRemove = new List<Powerup>();
            foreach (Powerup p in _powerups)
            {
                if (p.IsCollected)
                    powerupsToRemove.Add(p);
            }
            foreach (Powerup p in powerupsToRemove)
            {
                _powerups.Remove(p);
                _entityManager.RemoveEntity(p);
            }

            // Moves every powerup to the left according to the player's speed
            foreach (Powerup p in _powerups)
            {
                float posX = p.Position.X - _astronaut.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                p.Position = new Vector2(posX, p.Position.Y);
            }
        }

        /// <summary>
        /// Spawns a powerup on the designated platform
        /// </summary>
        /// <param name="platform"></param>
        private void SpawnPowerup(Platform platform)
        {
            Powerup powerup;
            Vector2 position;
            int powerupType;

            Random r = new Random();

            int powerupRNG = r.Next(AMMO_RNG + GRAVITY_RNG + REPELLENT_RNG + SHIELD_RNG + 1);


            if (powerupRNG < AMMO_RNG)
                powerupType = 0;
            else if (powerupRNG < AMMO_RNG + GRAVITY_RNG)
                powerupType = 1;
            else if (powerupRNG < AMMO_RNG + GRAVITY_RNG + REPELLENT_RNG)
                powerupType = 2;
            else
                powerupType = 3;


            position = new Vector2(platform.Position.X + 50, platform.Position.Y - 55);

            powerup = new Powerup(_spriteSheetTexture, position, powerupType, _menuManager);

            platform.HasObject = true;
            
            _powerups.Add(powerup);
            _entityManager.AddEntity(powerup);
        }

        /// <summary>
        /// Carries out the powerup
        /// </summary>
        /// <param name="powerup"></param>
        private void ProcessPowerup(Powerup powerup)
        {
            if (powerup.Type == PowerupType.Ammunition)
                _astronaut.GetAmmunition();
            else if (powerup.Type == PowerupType.Gravity)
                _astronaut.ChangeGravity();
            else if (powerup.Type == PowerupType.AlienRepellent)
                _enemyManager.RepelAliens();
            else if (powerup.Type == PowerupType.Shield)
                _astronaut.GetShield();

            powerup.IsCollected = true;
        }

        public void Reset()
        {
            foreach (Powerup p in _entityManager.GetEntitiesOfType<Powerup>())
            {
                _entityManager.RemoveEntity(p);
                _powerups.Remove(p);
            }

            _targetSpawnScore = 75;
            _previousSpawnScore = -1;
        }
    }
}