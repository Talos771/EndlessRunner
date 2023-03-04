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
        private const int STAR_SPAWN_MIN_DISTANCE = 275;

        private Texture2D _spriteSheetTexture;

        private EntityManager _entityManager;
        private MenuManager _menuManager;
        private Astronaut _player;

        public int DrawOrder => 0;

        public SkyManager(Texture2D texture, MenuManager menuManager, EntityManager entityManager, Astronaut player)
        {
            _spriteSheetTexture = texture;
            _entityManager = entityManager;
            _menuManager = menuManager;
            _player = player;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

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

            if (!_entityManager.GetEntitiesOfType<SkyPlanet>().Any())
            {
                // TODO add logic that has only 1 planet visible at a time
            }

            // Adds all the new entities to the entity manager
            foreach (SkyObject s in objects)
            {
                _entityManager.AddEntity(s);
            }

            // Removes every object that has moved off the screen
            foreach (SkyObject o in _entityManager.GetEntitiesOfType<SkyObject>())
            {
                if (o.Position.X < -40)
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
        }
    }
}
