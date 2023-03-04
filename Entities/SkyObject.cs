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
    public abstract class SkyObject : IGameEntity
    {
        private Astronaut _player;

        public int DrawOrder => 5;
        public Vector2 Position { get; set; }
        public abstract float Speed { get; }

        public SkyObject(Texture2D texture, MenuManager menuManager, Astronaut player, Vector2 pos)
        {
            _player = player;

            Position = pos;
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);


        public virtual void Update(GameTime gameTime)
        {
            if (_player.IsAlive)
                Position = new Vector2(Position.X - Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, Position.Y);
        }
    }
}
