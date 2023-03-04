using EndlessRunner.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessRunner.Starfield
{
    public class StarField
    {
        List<Star> stars = new List<Star>();
        MenuManager _menuManager;

        private int width;
        private int height;
        private int numStars;

        public StarField(int width, int height, int numStars, MenuManager menuManager)
        {
            _menuManager = menuManager;

            this.width = width;
            this.height = height;
            this.numStars = numStars;

            for (int i = 0; i < numStars; i++)
            {
                Star s = new Star(width, height, width, _menuManager);
                stars.Add(s);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Star s in stars)
                s.Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Star s in stars)
                s.Update(gameTime);
        }
    }
}