using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EndlessRunner.Menu;
using EndlessRunner.StaticClasses;

namespace EndlessRunner.Entities
{
    public abstract class Enemy : IGameEntity, ICollideable
    {
        private Astronaut _player;
        private Texture2D _spriteSheet;
        private EntityManager _entityManager;


        public abstract Rectangle CollisionBox { get; }

        public abstract Rectangle SpriteSheetPosition { get; }

        public int DrawOrder { get; set; }

        public Vector2 Position { get; protected set; }

        protected Enemy(Astronaut astro, Vector2 position, Texture2D spriteSheet, MenuManager menuManager, EntityManager entityManager)
        {
            Position = position;
            _player = astro;
            _spriteSheet = spriteSheet;
            _entityManager = entityManager;
        }


        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        /// <summary>
        /// Moves the enemies in line with the players speed
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            float posX = Position.X - _player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position = new Vector2(posX, Position.Y);

            CheckCollisions();
        }

        /// <summary>
        /// Checks to see if the player has collided with an enemy
        /// If they have, the player dies and the game ends
        /// </summary>
        private void CheckCollisions()
        {
            Rectangle playerCollisionBox = _player.CollisionBox;
            Rectangle enemyCollisionBox = CollisionBox;

            if (enemyCollisionBox.Intersects(playerCollisionBox))
            {
                if (CollisionDetection.CheckCollisions(_spriteSheet, _player, this))
                {
                    if (!_player.HasShield)
                        _player.Die();
                    else
                    {
                        _entityManager.RemoveEntity(this);
                        _player.HasShield = false;
                    }
                }
            }
        }
    }
}