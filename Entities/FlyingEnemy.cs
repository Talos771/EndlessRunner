using EndlessRunner.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EndlessRunner.Menu;

namespace EndlessRunner.Entities
{
    public class FlyingEnemy : Enemy
    {
        // Sprite coordinates
        private const int ENEMY_SPRITE_POS_X = 0;
        private const int ENEMY_SPRITE_POS_Y = 299;
        private const int ENEMY_SPRITE_WIDTH = 78;
        private const int ENEMY_SPRITE_HEIGHT = 56;

        private const float SPEED_PPS = 150f;

        private const float ANIMATION_FRAME_LENGTH = 0.2f;
        private SpriteAnimation _animation;
        private Astronaut _player;
        private MenuManager _menuManager;

        public override Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), ENEMY_SPRITE_WIDTH, ENEMY_SPRITE_HEIGHT);
                return box;
            }
        }

        public override Rectangle SpriteSheetPosition
        {
            get
            {
                Rectangle pos = new Rectangle(
                    ENEMY_SPRITE_POS_X, ENEMY_SPRITE_POS_Y, ENEMY_SPRITE_WIDTH, ENEMY_SPRITE_HEIGHT);

                return pos;
            }
        }

        public FlyingEnemy(Astronaut astro, Vector2 position, Texture2D spriteSheet, MenuManager menuManager, EntityManager entityManager) : base(astro, position, spriteSheet, menuManager, entityManager)
        {
            _player = astro;
            _menuManager = menuManager;

            Sprite frame1 = new Sprite(spriteSheet, ENEMY_SPRITE_POS_X, ENEMY_SPRITE_POS_Y, ENEMY_SPRITE_WIDTH, ENEMY_SPRITE_HEIGHT);
            Sprite frame2 = new Sprite(spriteSheet, ENEMY_SPRITE_POS_X + ENEMY_SPRITE_WIDTH, ENEMY_SPRITE_POS_Y, ENEMY_SPRITE_WIDTH, ENEMY_SPRITE_HEIGHT);

            _animation = new SpriteAnimation();
            _animation.AddFrame(frame1, 0);
            _animation.AddFrame(frame2, ANIMATION_FRAME_LENGTH);
            _animation.AddFrame(frame1, ANIMATION_FRAME_LENGTH * 2);
            _animation.ShouldLoop = true;
            _animation.Play();
        }

        /// <summary>
        /// Moves the enemy towards the player
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_player.IsAlive)
            {
                _animation.Update(gameTime);
                Position = new Vector2(Position.X - SPEED_PPS * (float)gameTime.ElapsedGameTime.TotalSeconds, Position.Y);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
                _animation.Draw(spriteBatch, Position);
        }
    }
}
