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
    public class Platform : IGameEntity, ICollideable
    {
        private const int SPRITE_WIDTH = 150;
        private const int SPRITE_HEIGHT = 25;

        MenuManager _menuManager;
        public Vector2 Position { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public int DrawOrder { get; set; }
        public Sprite Sprite { get; }
        public bool HasObject { get; set; }

        public Rectangle CollisionBox
        {
            get
            {
                Rectangle collisionBox = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y), SPRITE_WIDTH, 10);
                return collisionBox;
            }
        }

        public Platform(Vector2 position, Sprite sprite, MenuManager menuManager)
        {
            _menuManager = menuManager;

            PosX = position.X;
            PosY = position.Y;
            Sprite = sprite;

            Position = new Vector2(PosX, PosY);

            HasObject = false;

            DrawOrder = 10;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
                Sprite.Draw(spriteBatch, Position);
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
