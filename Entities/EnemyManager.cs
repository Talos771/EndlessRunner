using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EndlessRunner.Menu;
using static Assimp.Metadata;

namespace EndlessRunner.Entities
{
    public class EnemyManager : IGameEntity
    {
        // Dimensions of enemies
        private const int RANGED_SPRITE_WIDTH = 66;
        private const int RANGED_SPRITE_HEIGHT = 88;
        private const int MELEE_SPRITE_WIDTH = 38;
        private const int MELEE_SPRITE_HEIGHT = 66;

        private const int ENEMY_DESPAWN_DISTANCE = -100;

        // Variables that allow ranged enemies to fire multiple times
        private double _rangedEnemyRefireRng;
        private const float RANGED_ENEMY_FIRE_RNG_INCREASE = 0.000025f;
        private const double MAXIMUM_FIRE_RNG = 0.1;

        // Variables that increase the chances of an enemy spawning the longer the game goes on
        private const int SPAWN_MIN_DISTANCE = 10;
        private const int START_SPAWN_MAX_DISTANCE = 100;
        private const float SPAWN_DISTANCE_DECREASE = 0.3f;
        private const int MAX_DISTANCE_DECREASE = 12;
        private double _previousSpawnScore;
        private double _targetSpawnScore;
        private double _spawnMaxDistance;

        // Minimum and maximum spawn y-coordinates for flying enemies
        private const int MIN_FLYING_ENEMY_POS_Y = 100;
        private const int MAX_FLYING_ENEMY_POS_Y = 700;

        private readonly EntityManager _entityManager;
        private readonly Astronaut _player;

        private Texture2D _spriteSheet;

        private MenuManager _menuManager;
        private ScoreBoard _scoreBoard;

        public int DrawOrder => 0;

        public bool IsEnabled { get; set; }

        public EnemyManager(EntityManager entityManager, Astronaut astronaut, Texture2D spriteSheet, MenuManager menuManager, ScoreBoard scoreBoard)
        {
            _entityManager = entityManager;
            _player = astronaut;
            _menuManager = menuManager;
            _scoreBoard = scoreBoard;

            _spriteSheet = spriteSheet;

            _spawnMaxDistance = START_SPAWN_MAX_DISTANCE;
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
            // These can be used to spawn enemies
            foreach (Platform p in _entityManager.GetEntitiesOfType<Platform>())
            {
                if (p.Position.X > DeadSpaceGame.WINDOW_WIDTH)
                    platforms.Add(p);
            }

            // If there is nothing on a platform there is a chance that an enemy will spawn on that platform
            // The chance is based on the score - the higher the score, the more likely an enemy will spawn
            foreach (Platform p in platforms)
            {
                if (!p.HasObject)
                {
                    if (_previousSpawnScore <= 0 || (_scoreBoard.Score - _previousSpawnScore >= _targetSpawnScore))
                    {
                        _targetSpawnScore = r.NextDouble() * (_spawnMaxDistance - SPAWN_MIN_DISTANCE) + SPAWN_MIN_DISTANCE;

                        _previousSpawnScore = _scoreBoard.Score;

                        SpawnEnemy(p);
                        p.HasObject = true;
                    }
                }
            }

            // Removes every entity that has moved off the screen from the list of entites
            foreach (Enemy e in _entityManager.GetEntitiesOfType<Enemy>())
            {
                if (e.Position.X < ENEMY_DESPAWN_DISTANCE)
                    _entityManager.RemoveEntity(e);
            }

            // Logic that allows ranged enemies to fire projectiles
            foreach (RangedEnemy e in _entityManager.GetEntitiesOfType<RangedEnemy>())
            {
                if (e.Position.X < DeadSpaceGame.WINDOW_WIDTH)
                {
                    if (!e.HasFired)
                    {
                        if (r.Next(1001) < 10)
                        {
                            e.HasFired = true;
                            SpawnProjectile(e);
                        }
                    }
                    else
                    {
                        if (r.NextDouble() < _rangedEnemyRefireRng)
                        {
                            e.HasFired = false;
                        }
                    }
                }
            }

            // When a player projectile hits an enemy the enemy is killed
            foreach (PlayerProjectile p in _entityManager.GetEntitiesOfType<PlayerProjectile>())
            {
                foreach (Enemy e in _entityManager.GetEntitiesOfType<Enemy>())
                {
                    if (e.CollisionBox.Intersects(p.CollisionBox))
                        if (!e.Equals(typeof(Projectile)))
                        {
                            _entityManager.RemoveEntity(e);
                            _entityManager.RemoveEntity(p);
                        }
                }
            }

            // Decreases the spawn distance between enemies the longer the game goes on
            if (_spawnMaxDistance <= MAX_DISTANCE_DECREASE)
                _spawnMaxDistance = MAX_DISTANCE_DECREASE;
            else
                _spawnMaxDistance -= gameTime.ElapsedGameTime.TotalSeconds * (SPAWN_DISTANCE_DECREASE + 1 / _player.Speed);

            // Increases the chance that a ranged enemy will fire multiple times
            if (_rangedEnemyRefireRng >= MAXIMUM_FIRE_RNG)
                _rangedEnemyRefireRng = MAXIMUM_FIRE_RNG;
            else
                _rangedEnemyRefireRng += gameTime.ElapsedGameTime.TotalSeconds * RANGED_ENEMY_FIRE_RNG_INCREASE;
        }

        /// <summary>
        /// Spawns a melee, ranged, or flying enemy on the specified platform
        /// </summary>
        /// <param name="p"></param>
        private void SpawnEnemy(Platform p)
        {
            Enemy enemy;
            Random r = new Random();

            int meleeChance = 60;
            int rangedChance = 40;
            int flyingChance = 20;

            int rng = r.Next(meleeChance + rangedChance + flyingChance + 1);

            float posX;
            float posY;

            if (rng <= meleeChance)
            {
                posX = p.Position.X + r.Next(0, 150 - MELEE_SPRITE_WIDTH + 1);
                posY = p.Position.Y - MELEE_SPRITE_HEIGHT;

                enemy = new MeleeEnemy(_player, new Vector2(posX, posY), _spriteSheet, _menuManager, _entityManager);
            }
            else if (rng <= meleeChance + rangedChance)
            {
                posX = p.Position.X + r.Next(0, 150 - RANGED_SPRITE_WIDTH + 1);
                posY = p.Position.Y - RANGED_SPRITE_HEIGHT;

                enemy = new RangedEnemy(_player, new Vector2(posX, posY), _spriteSheet, _menuManager, _entityManager);
            }
            else
            {
                posX = p.Position.X;
                posY = r.Next(MIN_FLYING_ENEMY_POS_Y, MAX_FLYING_ENEMY_POS_Y+ 1);
                enemy = new FlyingEnemy(_player, new Vector2(posX, posY), _spriteSheet, _menuManager, _entityManager);
            }

            enemy.DrawOrder = 15;
            _entityManager.AddEntity(enemy);
        }

        /// <summary>
        /// When the repel aliens powerup is collected then the spawn distance between aliens is increased
        /// </summary>
        public void RepelAliens()
        {
            _spawnMaxDistance = START_SPAWN_MAX_DISTANCE;
        }

        /// <summary>
        /// Spawns a projectile from the specified ranged enemy
        /// </summary>
        /// <param name="enemy"></param>
        private void SpawnProjectile(RangedEnemy enemy)
        {
            Vector2 pos = new Vector2(enemy.Position.X, enemy.Position.Y + 50);
            Projectile projectile = new Projectile(_player, pos, _spriteSheet, _menuManager, _entityManager);

            _entityManager.AddEntity(projectile);
        }

        /// <summary>
        /// Resets the enemy manager so all enemies are removed from the game
        /// </summary>
        public void Reset()
        {
            foreach (Enemy e in _entityManager.GetEntitiesOfType<Enemy>())
            {
                _entityManager.RemoveEntity(e);
            }

            _targetSpawnScore = 0;
            _previousSpawnScore = -1;
            _spawnMaxDistance = START_SPAWN_MAX_DISTANCE;

            _rangedEnemyRefireRng = 0;
        }
    }
}