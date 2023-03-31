using EndlessRunner.Menu;
using EndlessRunner.StaticClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessRunner.Entities
{
    public class SkyManager : IGameEntity
    {
        private const int STAR_SPAWN_RADIUS = 700;
        private const int STAR_SPAWN_MIN_DISTANCE = 250;

        private const int MIN_PLANET_POS_Y = 50;
        private const int MAX_PLANET_POS_Y = 450;
        private const int MAX_PLANET_DISTANCE = 675;
        private const int MIN_PLANET_DISTANCE = 250;
        private double _targetPlanetSpawnScore = 0;

        private double _previousSpawnScore;

        private Texture2D _spriteSheetTexture;

        private EntityManager _entityManager;
        private MenuManager _menuManager;
        private Astronaut _player;
        private ScoreBoard _scoreBoard;

        public int DrawOrder => 0;

        public SkyManager(Texture2D texture, MenuManager menuManager, EntityManager entityManager, Astronaut player, ScoreBoard scoreBoard)
        {
            _spriteSheetTexture = texture;
            _entityManager = entityManager;
            _menuManager = menuManager;
            _player = player;
            _scoreBoard = scoreBoard;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        //TODO make it so only generates when the game is played
        public void Update(GameTime gameTime)
        {
            List<SkyObject> objects = new List<SkyObject>();
            List<Vector2> positions;

            // Spawns a new set of stars if all stars have been removed from the game
            if (!_entityManager.GetEntitiesOfType<SkyStar>().Any())
            {
                positions = UniformPoissonDiscSampler.SampleCircle(new Vector2(DeadSpaceGame.WINDOW_WIDTH + STAR_SPAWN_RADIUS, 0), STAR_SPAWN_RADIUS, STAR_SPAWN_MIN_DISTANCE);

                foreach (Vector2 p in positions)
                {
                    SkyStar s = new SkyStar(_spriteSheetTexture, _menuManager, _player, p);
                    objects.Add(s);
                }
            }

            // Spawns new planets if the target score for spawning has been reached
            if (_scoreBoard.Score - _previousSpawnScore >= _targetPlanetSpawnScore)
            {
                Random r = new Random();
                Vector2 position = new Vector2(DeadSpaceGame.WINDOW_WIDTH, r.Next(MIN_PLANET_POS_Y, MAX_PLANET_POS_Y));

                SkyPlanet p = new SkyPlanet(_spriteSheetTexture, _menuManager, _player, position);
                _targetPlanetSpawnScore = _scoreBoard.Score + r.NextDouble() * MAX_PLANET_DISTANCE;

                _entityManager.AddEntity(p);

                _targetPlanetSpawnScore = r.NextDouble() * (MAX_PLANET_DISTANCE - MIN_PLANET_DISTANCE) + MIN_PLANET_DISTANCE;
                _previousSpawnScore = _scoreBoard.Score;
            }

            // Adds all the new entities to the entity manager
            foreach (SkyObject s in objects)
            {
                _entityManager.AddEntity(s);
            }

            // Removes every object that has moved off the screen
            foreach (SkyObject o in _entityManager.GetEntitiesOfType<SkyObject>())
            {
                if (o.Position.X < -125)
                    _entityManager.RemoveEntity(o);
            }
        }

        /// <summary>
        /// Resets the sky manager
        /// </summary>
        public void Reset()
        {
            foreach (SkyObject o in _entityManager.GetEntitiesOfType<SkyObject>())
            {
                _entityManager.RemoveEntity(o);
            }

            _targetPlanetSpawnScore = 0;
            _previousSpawnScore = 0;
        }
    }
}
