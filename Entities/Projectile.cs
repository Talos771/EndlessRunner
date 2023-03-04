using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EndlessRunner.Graphics;
using Microsoft.Xna.Framework;
using EndlessRunner.Menu;

namespace EndlessRunner.Entities
{
    public class Projectile : Enemy
    {
        // Sprite dimensions and coordinates
        private const int PROJECTILE_SPRITE_POS_X = 326;
        private const int PROJECTILE_SPRITE_POS_Y = 0;
        private const int PROJECTILE_SPRITE_WIDTH = 24;
        private const int PROJECTILE_SPRITE_HEIGHT = 9;

        private const float SPEED_PPS = 400f;

        private Sprite _sprite;

        private Astronaut _player;
        private MenuManager _menuManager;

        public override Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), PROJECTILE_SPRITE_WIDTH, PROJECTILE_SPRITE_HEIGHT);
                return box;
            }
        }

        public override Rectangle SpriteSheetPosition
        {
            get
            {
                Rectangle pos = new Rectangle(
                    PROJECTILE_SPRITE_POS_X, PROJECTILE_SPRITE_POS_Y, PROJECTILE_SPRITE_WIDTH, PROJECTILE_SPRITE_HEIGHT);

                return pos;
            }
        }

        public Projectile(Astronaut astro, Vector2 position, Texture2D spriteSheet, MenuManager menuManager, EntityManager entityManager) : base(astro, position, spriteSheet, menuManager, entityManager)
        {
            _sprite = new Sprite(spriteSheet, PROJECTILE_SPRITE_POS_X, PROJECTILE_SPRITE_POS_Y, PROJECTILE_SPRITE_WIDTH, PROJECTILE_SPRITE_HEIGHT);
            _player = astro;
            _menuManager = menuManager;
            DrawOrder = 15;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_player.IsAlive)
                Position = new Vector2(Position.X - SPEED_PPS * (float)gameTime.ElapsedGameTime.TotalSeconds, Position.Y);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
                _sprite.Draw(spriteBatch, Position);
        }
    }
}