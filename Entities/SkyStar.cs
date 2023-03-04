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
    public class SkyStar : SkyObject
    {
        // Coordinates and dimensions for 3 types of star
        private const int STAR1_POS_X = 0;
        private const int STAR1_POS_Y = 132;
        private const int STAR2_POS_X = 34;
        private const int STAR2_POS_Y = 135;
        private const int STAR3_POS_X = 63;
        private const int STAR3_POS_Y = 144;
        private const int STAR1_WIDTH = 32;
        private const int STAR1_HEIGHT = 33;
        private const int STAR2_WIDTH = 27;
        private const int STAR2_HEIGHT = 27;
        private const int STAR3_WIDTH = 9;
        private const int STAR3_HEIGHT = 9;

        private MenuManager _menuManager;

        private Sprite _sprite;

        public override float Speed => 50f;

        public SkyStar(Texture2D texture, MenuManager menuManager, Astronaut player, Vector2 pos) : base(texture, menuManager,player, pos)
        {
            _menuManager = menuManager;

            int x = 0, y = 0, width = 0, height = 0;

            Random r = new Random();
            int rand = r.Next(4);

            if (rand == 0)
            {
                x = STAR1_POS_X;
                y = STAR1_POS_Y;
                width = STAR1_WIDTH;
                height = STAR1_WIDTH;
            }
            else if (rand == 1)
            {
                x = STAR2_POS_X;
                y = STAR2_POS_Y;
                width = STAR2_WIDTH;
                height = STAR2_HEIGHT;
            }
            else if (rand == 2)
            {
                x = STAR3_POS_X;
                y = STAR3_POS_Y;
                width = STAR3_WIDTH;
                height = STAR3_HEIGHT;
            }

            _sprite = new Sprite(texture, x, y, width, height);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
                _sprite.Draw(spriteBatch, Position);
        }
    }
}
