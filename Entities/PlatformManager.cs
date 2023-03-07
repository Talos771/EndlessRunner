using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EndlessRunner.Graphics;
using EndlessRunner.Menu;
using EndlessRunner.StaticClasses;

namespace EndlessRunner.Entities
{
    public class PlatformManager : IGameEntity
    {
        //Platform dimensions and coordinates
        private const int SPRITE_WIDTH = 150;
        private const int SPRITE_HEIGHT = 25;
        private const int SPRITE_POS_X = 1;
        private const int SPRITE_POS_Y = 106;
         
        // Grid location to be able to spawn platforms
        private const int GRID_TOP_LEFT_X = DeadSpaceGame.WINDOW_WIDTH;
        private const int GRID_TOP_LEFT_Y = 200;
        private const int GRID_BOTTOM_RIGHT_X = DeadSpaceGame.WINDOW_WIDTH + 800;
        private const int GRID_BOTTOM_RIGHT_Y = 750;
        private const int PLATFORM_MIN_DISTANCE = 169;
        private const int PLATFORM_MAX_DISTANCE = 300;
        private const float PLATFORM_DISTANCE_INCREMENT = 0.25f;

        private readonly List<Platform> _platforms;
        private Sprite _platformSprite;

        // We need the player to determine the speed and entity manager to add platforms to the list of entities
        // The menu manager is needed to check when the platforms should be drawn
        private readonly EntityManager _entityManager;
        private Astronaut _player;
        private MenuManager _menuManager;

        private List<Vector2> Positions { get; set; }
        public Platform PlayersPlatform { get; set; }
        public int DrawOrder { get; set; }
        public float PlatformDistance { get; set; }

        public PlatformManager(Texture2D spriteSheet, EntityManager entityManager, Astronaut player, MenuManager menuManager)
        {
            _platforms = new List<Platform>();
            _entityManager = entityManager;
            _menuManager = menuManager;

            _platformSprite = new Sprite(spriteSheet, SPRITE_POS_X, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            _player = player;

            DrawOrder = 10;
        }

        public void Update(GameTime gameTime)
        {
            // Calculates the maximum X value of all the tiles
            // If it is less than the screen width then new tiles are spawned
            if (_platforms.Any())
            {
                float maxPosX = _platforms.Max(g => g.PosX);

                if (maxPosX < DeadSpaceGame.WINDOW_WIDTH - PlatformDistance)
                    GenerateTiles(0);
            }

            // This list is needed as you can't remove items from a list during a foreach loop
            List<Platform> platformsToRemove = new List<Platform>();

            // Decreases the x value of every platform so that they move to the left of the screen
            foreach (Platform p in _platforms)
            {
                p.PosX -= _player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (p.PosX < -SPRITE_WIDTH)
                {
                    _entityManager.RemoveEntity(p);
                    platformsToRemove.Add(p);
                }
                p.Position = new Vector2(p.PosX, p.PosY);
            }

            // Removes every platform that has moved off the screen
            foreach (Platform p in platformsToRemove)
            {
                _platforms.Remove(p);
            }

            // Checks each platform to see if the player has landed on it
            // If they do land on it then they set the platform that the player is using to that platform
            foreach (Platform p in _platforms)
            {
                if (p.CollisionBox.Intersects(_player.FeetHitBox))
                {
                    _player.Land(p);
                    PlayersPlatform = p;
                }
            }

            // If the player has run off the platform then the player will need to fall
            if (PlayersPlatform != null)
                if (_player.Position.X > PlayersPlatform.Position.X + SPRITE_WIDTH)
                    _player.Drop();

            // Increases the distance between platforms up to a point (increases difficulty of the game)
            if (PlatformDistance > PLATFORM_MAX_DISTANCE)
                PlatformDistance = PLATFORM_MAX_DISTANCE;
            else
                PlatformDistance += PLATFORM_DISTANCE_INCREMENT * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        /// <summary>
        /// Initialises the ground manager by creating starting platforms
        /// </summary>
        public void Initialise()
        {
            _platforms.Clear();
            foreach (Platform p in _entityManager.GetEntitiesOfType<Platform>())
            {
                _entityManager.RemoveEntity(p);
            }

            PlatformDistance = PLATFORM_MIN_DISTANCE;

            Platform startPlatform = new Platform(new Vector2(0, 400), _platformSprite, _menuManager);
            _entityManager.AddEntity(startPlatform);
            _platforms.Add(startPlatform);

            PlayersPlatform = startPlatform;

            GenerateTiles(200);
        }

        /// <summary>
        /// Uses the Uniform Poisson Disc Sampler static class to generate a list of points that the platforms should be spawned at
        /// </summary>
        private void GenerateTiles(int topLeftX)
        {
            if (topLeftX == 0)
            {
                Positions = UniformPoissonDiscSampler.SampleRectangle(new Vector2(GRID_TOP_LEFT_X, GRID_TOP_LEFT_Y), new Vector2(GRID_BOTTOM_RIGHT_X, GRID_BOTTOM_RIGHT_Y), PlatformDistance);
            }
            else
            {
                Positions = UniformPoissonDiscSampler.SampleRectangle(new Vector2(topLeftX, GRID_TOP_LEFT_Y), new Vector2(topLeftX + DeadSpaceGame.WINDOW_WIDTH, GRID_BOTTOM_RIGHT_Y), PlatformDistance);
            }

            foreach (Vector2 pos in Positions)
            {
                Platform p = new Platform(pos, _platformSprite, _menuManager);
                _entityManager.AddEntity(p);
                _platforms.Add(p);
            }
        }
    }
}
