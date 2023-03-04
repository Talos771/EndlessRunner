using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EndlessRunner.Entities;

namespace EndlessRunner.StaticClasses
{
    public static class CollisionDetection
    {
        /// <summary>
        /// Uses pixel by pixel collision detection to check if the entites have collided
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public static bool CheckCollisions(Texture2D spriteSheet, Astronaut player, Enemy enemy)
        {
            bool haveCollided = false;

            // Gets the colours of the different sprites
            Color[,] playerColours = GetColours(spriteSheet, player.SpriteSheetPosition);
            Color[,] enemyColours = GetColours(spriteSheet, enemy.SpriteSheetPosition);

            Rectangle intersection = GetIntersectionBox(player.CollisionBox, enemy.CollisionBox);


            int spriteSheetPlayerStartPosX = (int)Math.Round(intersection.X - player.Position.X);
            int spriteSheetPlayerStartPosY = (int)Math.Round(intersection.Y - player.Position.Y);

            int spriteSheetEnemyStartPosX = (int)Math.Round(intersection.X - enemy.Position.X);
            int spriteSheetEnemyStartPosY = (int)Math.Round(intersection.Y - enemy.Position.Y);

            // Checks every pixel in the intersection between collision boxes
            for (int i = 0; i < intersection.Width; i++)
            {
                for (int j = 0; j < intersection.Height; j++)
                {
                    if (
                        playerColours[spriteSheetPlayerStartPosX + i, spriteSheetPlayerStartPosY + j] != new Color(0, 0, 0, 0) &&
                        enemyColours[spriteSheetEnemyStartPosX + i, spriteSheetEnemyStartPosY + j] != new Color(0, 0, 0, 0)
                       )
                    {
                        haveCollided = true;
                    }
                }
            }

            return haveCollided;
        }

        /// <summary>
        /// Gets the colours of a sprite from the spritesheet usimg the given rectangles
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static Color[,] GetColours(Texture2D spriteSheet, Rectangle entity)
        {
            Color[] colours = new Color[entity.Width * entity.Height];

            spriteSheet.GetData(0, entity, colours, 0, entity.Width * entity.Height);

            Color[,] entityColours = new Color[entity.Width, entity.Height];



            int pass = 0;

            for (int i = 0; i < entity.Height; i++)
            {
                for (int j = 0; j < entity.Width; j++)
                {
                    entityColours[j, i] = colours[pass];
                    pass++;
                }
            }

            return entityColours;
        }

        /// <summary>
        /// Returns the overlapping rectangle
        /// </summary>
        /// <param name="rectA"></param>
        /// <param name="rectB"></param>
        /// <returns></returns>
        private static Rectangle GetIntersectionBox(Rectangle rectA, Rectangle rectB)
        {
            int topLeftX = Math.Max(rectA.X, rectB.X);
            int topLeftY = Math.Max(rectA.Y, rectB.Y);
            int bottomRightX = Math.Min(rectA.X + rectA.Width, rectB.X + rectB.Width);
            int bottomRightY = Math.Min(rectA.Y + rectA.Height, rectB.Y + rectB.Height);

            Rectangle intersection = new Rectangle(topLeftX, topLeftY, bottomRightX - topLeftX, bottomRightY - topLeftY);

            return intersection;
        }
    }
}