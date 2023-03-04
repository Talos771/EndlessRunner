using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EndlessRunner.Graphics;
using Microsoft.Xna.Framework.Graphics;
using EndlessRunner.Menu;

namespace EndlessRunner.Entities
{
    public class MeleeEnemy : Enemy
    {
        // Sprite coordinates
        private const int ENEMY_SPRITE_POS_X = 0;
        private const int ENEMY_SPRITE_POS_Y = 445;
        private const int ENEMY_SPRITE_WIDTH = 38;
        private const int ENEMY_SPRITE_HEIGHT = 66;

        private Sprite _sprite;

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

        public MeleeEnemy(Astronaut astro, Vector2 position, Texture2D spriteSheet, MenuManager menuManager, EntityManager entityManager) : base(astro, position, spriteSheet, menuManager, entityManager)
        {
            _menuManager = menuManager;

            _sprite = new Sprite(spriteSheet, ENEMY_SPRITE_POS_X, ENEMY_SPRITE_POS_Y, ENEMY_SPRITE_WIDTH, ENEMY_SPRITE_HEIGHT);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
                _sprite.Draw(spriteBatch, Position);
        }
    }
}
