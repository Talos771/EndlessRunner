using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using Accessibility;
using EndlessRunner.Menu;

namespace EndlessRunner.Starfield
{
    public class Star
    {
        private MenuManager _menuManager;

        private const int MAX_STAR_RADIUS = 10;
        private const int DEAFAULT_SPEED = -30;
        private const float TRANSITION_SPEED = 3f;

        private float slowDownFactor;

        private float x;
        private float y;
        private float z;
        private int screenWidth;
        private int screenHeight;
        private int screenDepth;

        private float sx;
        private float sy;
        private float radius;

        Color colour = Color.White;

        Random r = new Random();

        public Star(int width, int height, int depth, MenuManager menuManager)
        {
            _menuManager = menuManager;

            screenWidth = width;
            screenHeight = height;
            screenDepth = depth;

            slowDownFactor = 1;

            x = r.Next(-width / 2, width / 2);
            y = r.Next(-height / 2, height / 2);
            z = r.Next(0, depth);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(sx, sy, radius, 5, colour, radius);
        }

        public void Update(GameTime gameTime)
        {
            // Moves the star closer
            if (_menuManager.State == MenuState.Main)
            {
                z += DEAFAULT_SPEED;
                slowDownFactor = 1;
            }
            else
            {
                z += DEAFAULT_SPEED / slowDownFactor;
                slowDownFactor += TRANSITION_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }


            sx = Map(x / z, -1, 1, 0, screenWidth);
            sy = Map(y / z, -1, 1, 0, screenHeight);

            // Increases the size of the star when they get closer to the screen
            radius = Map(z, 0, screenDepth, MAX_STAR_RADIUS, 0);

            // Resets position if a star exits the screen
            if (z < 1)
            {
                x = r.Next(-screenWidth / 2, screenWidth / 2);
                y = r.Next(-screenHeight / 2, screenHeight / 2);
                z = screenDepth; // Means they are always far away
            }
        }

        public float Map(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}