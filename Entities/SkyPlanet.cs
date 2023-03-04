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
    public class SkyPlanet : SkyObject
    {
        private MenuManager _menuManager;
        private Sprite _sprite;

        public override float Speed => 20f;

        public SkyPlanet(Texture2D texture, MenuManager menuManager, Astronaut player, Vector2 pos) : base(texture, menuManager, player, pos)
        {
            _menuManager = menuManager;

            Random r = new Random();

            int rand = r.Next(5);

            int x = 0, y = 0, width = 0, height = 0;

            // TODO add plant sprites

            _sprite = new Sprite(texture, x, y, width, height);
        }


        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
            {
                _sprite.Draw(spriteBatch, Position);
            }
        }
    }
}
