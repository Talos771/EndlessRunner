using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using EndlessRunner.Graphics;
using EndlessRunner.Menu;

namespace EndlessRunner.Entities
{
    public class RangedEnemy : Enemy
    {
        // Sprite dimensions and coordinates
        private const int ENEMY_SPRITE_POS_X = 0;
        private const int ENEMY_SPRITE_POS_Y = 356;
        private const int ENEMY_SPRITE_WIDTH = 66;
        private const int ENEMY_SPRITE_HEIGHT = 88;

        private Sprite _sprite;

        private MenuManager _menuManager;

        public bool HasFired { get; set; }

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

        public RangedEnemy(Astronaut astro, Vector2 position, Texture2D spriteSheet, MenuManager menuManager, EntityManager entityManager) : base(astro, position, spriteSheet, menuManager, entityManager)
        {
            _sprite = new Sprite(spriteSheet, ENEMY_SPRITE_POS_X, ENEMY_SPRITE_POS_Y, ENEMY_SPRITE_WIDTH, ENEMY_SPRITE_HEIGHT);
            _menuManager = menuManager;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
                _sprite.Draw(spriteBatch, Position);
        }
    }
}
