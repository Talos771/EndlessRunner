using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace EndlessRunner.Entities
{
    public class EntityManager
    {
        private readonly List<IGameEntity> _entities = new List<IGameEntity>();

        private readonly List<IGameEntity> _entitiesToAdd = new List<IGameEntity>();
        private readonly List<IGameEntity> _entitiesToRemove = new List<IGameEntity>();

        public IEnumerable<IGameEntity> Entities => new ReadOnlyCollection<IGameEntity>(_entities);


        public void Update(GameTime gameTime)
        {
            foreach (IGameEntity entity in _entities)
            {
                if (_entitiesToRemove.Contains(entity))
                    continue;

                entity.Update(gameTime);
            }

            foreach (IGameEntity entity in _entitiesToAdd)
            {
                _entities.Add(entity);
            }

            foreach (IGameEntity entity in _entitiesToRemove)
            {
                _entities.Remove(entity);
            }

            _entitiesToAdd.Clear();
            _entitiesToRemove.Clear();
        }

        /// <summary>
        /// Draws all the entites currently in the game
        /// They are ordered by their DrawOrder (this draws entites with a higher DrawOrder at the top)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (IGameEntity entity in _entities.OrderBy(e => e.DrawOrder))
            {
                entity.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Adds an entity to the list to the list of game entities
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(IGameEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity), "Null cannot be added as an entity");

            _entitiesToAdd.Add(entity);
        }

        /// <summary>
        /// Removes the entity from the list of game entites
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(IGameEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity), "Null is not a valid entity");

            _entitiesToRemove.Add(entity);
        }

        public void Clear()
        {
            _entitiesToRemove.AddRange(_entities);
        }

        /// <summary>
        /// Returns all instances of IGameEntity of a specific type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetEntitiesOfType<T>() where T : IGameEntity
        {
            return _entities.OfType<T>();
        }
    }
}