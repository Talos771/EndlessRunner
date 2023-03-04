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
    public class PlayerProjectile : IGameEntity, ICollideable
    {
        // Bullet sprite coordinates and dimensions
        private const int SPRITE_POS_X = 309;
        private const int SPRITE_POS_Y = 0;
        private const int SPRITE_WIDTH = 16;
        private const int SPRITE_HEIGHT = 10;

        private const float SPEED_PPS = 200f;

        private Astronaut _player;

        private Sprite _bulletSprite;

        private MenuManager _menuManager;

        public Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), SPRITE_WIDTH, SPRITE_HEIGHT);
                return box;
            }
        }
        public Vector2 Position { get; set; }
        public int DrawOrder => 11;

        public PlayerProjectile(Texture2D spriteSheetTexture, Astronaut astronaut, Vector2 position, MenuManager menuManager)
        {
            _player = astronaut;
            Position = position;
            _menuManager = menuManager;

            _bulletSprite = new Sprite(spriteSheetTexture, SPRITE_POS_X, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
            {
                _bulletSprite.Draw(spriteBatch, Position);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_player.IsAlive)
                Position = new Vector2(Position.X + (_player.Speed + SPEED_PPS) * (float)gameTime.ElapsedGameTime.TotalSeconds, Position.Y);
        }
    }
}