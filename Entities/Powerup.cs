using Accessibility;
using EndlessRunner.Graphics;
using EndlessRunner.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessRunner.Entities
{
    public enum PowerupType
    {
        Ammunition,
        Gravity,
        AlienRepellent,
        Shield
    }

    public class Powerup : IGameEntity, ICollideable
    {
        private const int SPRITE_HEIGHT = 50;
        private const int SPRITE_WIDTH = 50;
        private const int SPRITE_POS_X = 157;
        private const int SPRITE_POS_Y = 91;

        private Sprite _sprite;

        private MenuManager _menuManager;

        public Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), SPRITE_WIDTH, SPRITE_HEIGHT);
                return box;
            }
        }

        public PowerupType Type { get; set; }
        public Vector2 Position { get; set; }
        public bool IsCollected { get; set; }
        public int DrawOrder { get; set; }

        public Powerup(Texture2D spriteSheet, Vector2 position, int type, MenuManager menuManager)
        {
            Position = position;
            Type = (PowerupType)type;

            _sprite = new Sprite(spriteSheet, SPRITE_POS_X + SPRITE_WIDTH * type, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);

            _menuManager = menuManager;

            DrawOrder = 14;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
                _sprite.Draw(spriteBatch, Position);
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}