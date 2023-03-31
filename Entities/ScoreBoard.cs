using EndlessRunner.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using System.CodeDom;

namespace EndlessRunner.Entities
{
    public class ScoreBoard : IGameEntity
    {
        // Coordinates for numbers on the spritesheet
        private const int TEXTURE_COORDS_NUMBER_X = 0;
        private const int TEXTURE_COORDS_NUMBER_Y = 271;
        private const int TEXTURE_COORDS_NUMBER_WIDTH = 18;
        private const int TEXTURE_COORDS_NUMBER_HEIGHT = 24;

        private const int BACKGROUND_SCORE_POS_X = 184;
        private const int BACKGROUND_SCORE_POS_Y = 272;
        private const int BACKGROUND_SCORE_WIDTH = 130;

        // Score display digits
        private const byte NUMBER_DIGITS_TO_DRAW = 5;
        private const int MAX_SCORE = 99999;

        private const float SCORE_INCREMENT_MULTIPLIER = 0.035f;

        // Scoreboard table display settings
        private const int NUMBER_POS_X = 60;
        private const int NAME_POS_X = 400;
        private const int SCORE_POS_X = 1000;
        private const int TEX_START_POS_Y = 200;
        private const int ROW_FACTOR_Y = 55;
        private const int TEXT_SCALE_FACTOR = 2;

        Texture2D _texture;
        SpriteFont _font;

        private Astronaut _astronaut;

        private MenuManager _menuManager;

        private double _score;

        public double Score
        {
            get => _score;
            set => _score = Math.Max(0, Math.Min(MAX_SCORE, value));
        }

        public int DisplayScore => (int)Math.Floor(Score);
        public List<int> HighScore { get; set; }
        public List<string> Names { get; set; }
        public int DrawOrder => 100;
        public Vector2 Position { get; set; }

        public ScoreBoard(Texture2D texture, Astronaut astronaut, MenuManager menuManager, Vector2 position, SpriteFont spriteFont)
        {
            _astronaut = astronaut;
            _texture = texture;
            _font = spriteFont;
            Position = position;
            _menuManager = menuManager;
        }

        public void Update(GameTime gameTime)
        {
            Score += _astronaut.Speed * SCORE_INCREMENT_MULTIPLIER * gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_menuManager.State == MenuState.Playing)
            {
                DrawScore(spriteBatch, DisplayScore, Position.X + BACKGROUND_SCORE_WIDTH + 4, Position.Y);
                spriteBatch.Draw(_texture, Position, new Rectangle(BACKGROUND_SCORE_POS_X, BACKGROUND_SCORE_POS_Y, BACKGROUND_SCORE_WIDTH, TEXTURE_COORDS_NUMBER_HEIGHT), Color.White);
            }
            else if (_menuManager.State == MenuState.ScoreBoard)
            {
                DrawScoreTable(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Takes an integer input then returns an array of single digits representing that number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private int[] SplitDigits(int input)
        {
            string inputStr = input.ToString().PadLeft(NUMBER_DIGITS_TO_DRAW, '0');
            int[] result = new int[inputStr.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (int)char.GetNumericValue(inputStr[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets the correct digit sprite from the spritesheet
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        private Rectangle GetDigitBounds(int digit)
        {
            int posX = TEXTURE_COORDS_NUMBER_X + digit * TEXTURE_COORDS_NUMBER_WIDTH;
            int posY = TEXTURE_COORDS_NUMBER_Y;

            return new Rectangle(posX, posY, TEXTURE_COORDS_NUMBER_WIDTH, TEXTURE_COORDS_NUMBER_HEIGHT);
        }

        /// <summary>
        /// Draws the number to the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="score"></param>
        /// <param name="posY"></param>
        private void DrawScore(SpriteBatch spriteBatch, int score, float posX, float posY)
        {
            int[] scoreDigits = SplitDigits(score);

            // Draws each digit separately
            foreach (int digit in scoreDigits)
            {
                Rectangle textureCoords = GetDigitBounds(digit);

                Vector2 position = new Vector2(posX, posY);
                spriteBatch.Draw(_texture, position, textureCoords, Color.White);
                posX += TEXTURE_COORDS_NUMBER_WIDTH + 4;
            }
        }

        /// <summary>
        /// Adds the score to the list of the highest scores then sorts into the correct order
        /// </summary>
        /// <param name="nameToAdd"></param>
        public void TryAddNewScore(string nameToAdd)
        {
            // Makes sure an null scores are removed
            HighScore.Remove(0);
            Names.Remove("");

            int tempScore;
            string tempName;

            HighScore.Add(DisplayScore);
            Names.Add(nameToAdd);

            bool sorted = false;

            // Bubble sort to sort the list of highscores from lowest to highest
            while (!sorted)
            {
                sorted = true;

                for (int i = 0; i < HighScore.Count - 1; i++)
                {
                    if (HighScore[i + 1] > HighScore[i])
                    {
                        tempScore = HighScore[i + 1];
                        HighScore[i + 1] = HighScore[i];
                        HighScore[i] = tempScore;

                        tempName = Names[i + 1];
                        Names[i + 1] = Names[i];
                        Names[i] = tempName;

                        sorted = false;
                    }
                }
            }

            if (HighScore.Count > 10)
            {
                HighScore.RemoveAt(HighScore.Count - 1);
                Names.RemoveAt(Names.Count - 1);
            }
        }

        /// <summary>
        /// Displays the highscores and names
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void DrawScoreTable(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Color displayColor = Color.Orange;

            spriteBatch.DrawString(_font, "Rank", new Vector2(NUMBER_POS_X, TEX_START_POS_Y - 70), displayColor, 0, new Vector2(), TEXT_SCALE_FACTOR, 0, 0);
            spriteBatch.DrawString(_font, "Name", new Vector2(NAME_POS_X, TEX_START_POS_Y - 70), displayColor, 0, new Vector2(), TEXT_SCALE_FACTOR, 0, 0);
            spriteBatch.DrawString(_font, "Score", new Vector2(SCORE_POS_X, TEX_START_POS_Y - 70), displayColor, 0, new Vector2(), TEXT_SCALE_FACTOR, 0, 0);


            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                    displayColor = Color.Red;
                else
                    displayColor = Color.White;

                spriteBatch.DrawString(_font, (i + 1).ToString() + ".", new Vector2(NUMBER_POS_X, TEX_START_POS_Y + ROW_FACTOR_Y * i), displayColor, 0, new Vector2(), TEXT_SCALE_FACTOR, 0, 0);
            }
            
            for (int i = 0; i < HighScore.Count; i++)
            {
                if (i == 0)
                    displayColor = Color.Red;
                else
                    displayColor = Color.White;

                spriteBatch.DrawString(_font, Names[i].ToString(), new Vector2(NAME_POS_X, TEX_START_POS_Y + ROW_FACTOR_Y * i), displayColor, 0, new Vector2(), TEXT_SCALE_FACTOR, 0, 0);
                spriteBatch.DrawString(_font, HighScore[i].ToString(), new Vector2(SCORE_POS_X, TEX_START_POS_Y + ROW_FACTOR_Y * i), displayColor, 0, new Vector2(), TEXT_SCALE_FACTOR, 0, 0);
            }
        }
    }
}
