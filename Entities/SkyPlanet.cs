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
        private const int PLANET1_SPRITE_POS_X = 0;
        private const int PLANET1_SPRITE_POS_Y = 166;
        private const int PLANET1_SPRITE_WIDTH = 99;
        private const int PLANET1_SPRITE_HEIGHT = 100;

        private const int PLANET2_SPRITE_POS_X = 99;
        private const int PLANET2_SPRITE_POS_Y = 166;
        private const int PLANET2_SPRITE_WIDTH = 100;
        private const int PLANET2_SPRITE_HEIGHT = 100;

        private const int PLANET3_SPRITE_POS_X = 199;
        private const int PLANET3_SPRITE_POS_Y = 198;
        private const int PLANET3_SPRITE_WIDTH = 100;
        private const int PLANET3_SPRITE_HEIGHT = 68;

        private const int PLANET4_SPRITE_POS_X = 299;
        private const int PLANET4_SPRITE_POS_Y = 232;
        private const int PLANET4_SPRITE_WIDTH = 35;
        private const int PLANET4_SPRITE_HEIGHT = 34;

        private MenuManager _menuManager;
        private Sprite _sprite;

        public override float Speed => 20f;

        public SkyPlanet(Texture2D texture, MenuManager menuManager, Astronaut player, Vector2 pos) : base(texture, menuManager, player, pos)
        {
            _menuManager = menuManager;

            Random r = new Random();

            int rand = r.Next(4);

            int x = 0, y = 0, width = 0, height = 0;

            if (rand == 0)
            {
                x = PLANET1_SPRITE_POS_X;
                y = PLANET1_SPRITE_POS_Y;
                width = PLANET1_SPRITE_WIDTH;
                height = PLANET1_SPRITE_HEIGHT;
            }
            else if (rand == 1)
            {
                x = PLANET2_SPRITE_POS_X;
                y = PLANET2_SPRITE_POS_Y;
                width = PLANET2_SPRITE_WIDTH;
                height = PLANET2_SPRITE_HEIGHT;
            }
            else if (rand == 2)
            {
                x = PLANET3_SPRITE_POS_X;
                y = PLANET3_SPRITE_POS_Y;
                width = PLANET3_SPRITE_WIDTH;
                height = PLANET3_SPRITE_HEIGHT;
            }
            else if (rand == 3)
            {
                x = PLANET4_SPRITE_POS_X;
                y = PLANET4_SPRITE_POS_Y;
                width = PLANET4_SPRITE_WIDTH;
                height = PLANET4_SPRITE_HEIGHT;
            }

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
