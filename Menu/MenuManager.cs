using EndlessRunner.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EndlessRunner.Graphics;
using SharpDX.XInput;
using System.Web;
using System.Runtime.CompilerServices;
using MonoGame.Extended;
using System.Xml.Linq;
using MonoGame.Extended.BitmapFonts;
using EndlessRunner.System;
using CppNet;

namespace EndlessRunner.Menu
{
    public class MenuManager : IGameEntity
    {
        private DeadSpaceGame _game;
        private EntityManager _entityManager;

        private Texture2D _spriteSheetTexture;
        private SpriteFont _font;

        private List<Button> MenuButtons = new List<Button>();

        private TextInput _textInput;

        public EventHandler MainMenu;
        public EventHandler PlayAgain;
        public EventHandler SaveTheGame;
        public EventHandler SaveNewCharacter;
        public EventHandler ResetSaveData;

        // Main menu buttons:
        // Play button:
        private const string PLAY_NAME = "Play";
        private const MenuState PLAY_STATE = MenuState.Main;
        private const MenuState PLAY_DESTINATION = MenuState.Transition;
        private Vector2 PLAY_SPRITE_POSITION = new Vector2(0, 324);
        private const int PLAY_WIDTH = 647;
        private Vector2 PLAY_BUTTON_POSITION = new Vector2(MAINMENU_BUTTON_INSET - PLAY_WIDTH, 16);
        // Options button:
        private const string OPTIONS_BUTTON_NAME = "Options";
        private const MenuState OPTIONS_BUTTON_STATE = MenuState.Main;
        private const MenuState OPTIONS_BUTTON_DESTINATION = MenuState.Options;
        private Vector2 OPTIONS_BUTTON_SPRITE_POSITION = new Vector2(0, 381);
        private const int OPTIONS_BUTTON_WIDTH = 580;
        private Vector2 OPTIONS_BUTTON_POSITION = new Vector2(MAINMENU_BUTTON_INSET - OPTIONS_BUTTON_WIDTH, 26 + 49);
        // Scoreboard button:
        private const string SCOREBOARD_BUTTON_NAME = "ScoreBoard";
        private const MenuState SCOREBOARD_BUTTON_STATE = MenuState.Main;
        private const MenuState SCOREBOARD_BUTTON_DESTINATION = MenuState.ScoreBoard;
        private Vector2 SCOREBOARD_BUTTON_SPRITE_POSITION = new Vector2(0, 438);
        private const int SCOREBOARD_BUTTON_WIDTH = 512;
        private Vector2 SCOREBOARD_BUTTON_POSITION = new Vector2(MAINMENU_BUTTON_INSET - SCOREBOARD_BUTTON_WIDTH, 36 + 49 * 2);
        //Exit button:
        private const string EXIT_BUTTON_NAME = "Exit";
        private const MenuState EXIT_BUTTON_STATE = MenuState.Main;
        private const MenuState EXIT_BUTTON_DESTINATION = MenuState.None;
        private Vector2 EXIT_SPRITE_POSITION = new Vector2(0, 496);
        private const int EXIT_BUTTON_WIDTH = 445;
        private Vector2 EXIT_BUTTON_POSITION = new Vector2(MAINMENU_BUTTON_INSET - EXIT_BUTTON_WIDTH, 46 + 49 * 3);

        private const int MAIN_MENU_BUTTON_HEIGHT = 49;
        private const int MAINMENU_BUTTON_INSET = 1585;

        // Game over screen buttons:
        // PLay again button
        private const string PLAY_AGAIN_NAME = "Play again";
        private const MenuState PLAY_AGAIN_STATE = MenuState.GameOver;
        private const MenuState PLAY_AGAIN_DESTINATION = MenuState.Playing;
        private Vector2 PLAY_AGAIN_SPRITE_POSITION = new Vector2(0, 329);
        private Vector2 PLAY_AGAIN_POSITION = new Vector2(485, 388);
        // Main menu button
        private const string MAIN_MENU_NAME = "Main menu";
        private const MenuState MAIN_MENU_STATE = MenuState.GameOver;
        private const MenuState MAIN_MENU_DESTINATION = MenuState.Main;
        private Vector2 MAIN_MENU_SPRITE_POSITION = new Vector2(0, 469);
        private Vector2 MAIN_MENU_POSITION = new Vector2(848, 388);

        private const int GAMEOVER_WIDTH = 268;
        private const int GAMEOVER_HEIGHT = 130;

        // Return button
        private const string RETURN_NAME = "Return";
        private const MenuState RETURN_STATE = MenuState.ScoreBoard;
        private const MenuState RETURN_STATE_2 = MenuState.Options;
        private const MenuState RETURN_DESTINATION = MenuState.Main;
        private Vector2 RETURN_SPRITE_POSITION = new Vector2(0, 0);
        private Vector2 RETURN_POSITION = new Vector2(DeadSpaceGame.WINDOW_WIDTH - RETURN_WIDTH - 10, DeadSpaceGame.WINDOW_HEIGHT - RETURN_HEIGHT - 10);
        private const int RETURN_WIDTH = 267;
        private const int RETURN_HEIGHT = 108;

        // Options menu buttons:
        // Change key buttons
        private const string JUMP_CHANGE_NAME = "Change Jump";
        private Vector2 JUMP_CHANGE_POSITION = new Vector2(570, 150);
        private const string DROP_CHANGE_NAME = "Change Drop";
        private Vector2 DROP_CHANGE_POSITION = new Vector2(570, 287);
        private const string ATTACK_CHANGE_NAME = "Change Attack";
        private Vector2 ATTACK_CHANGE_POSITION = new Vector2(910, 218);
        private const MenuState CHANGE_STATE = MenuState.Options;
        private const MenuState CHANGE_DESTINATION = MenuState.TextEntry;
        private Vector2 CHANGE_SPRITE_POSITION = new Vector2(0, 392);
        private const int CHANGE_WIDTH = 120;
        private const int CHANGE_HEIGHT = 117;

        // Reset save data button
        private const string RESET_NAME = "Reset";
        private const MenuState RESET_STATE = MenuState.Options;
        private const MenuState RESET_DESTINATION = MenuState.Options;
        private Vector2 RESET_SPRITE_POSITION = new Vector2(0, 511);
        private Vector2 RESET_POSITION = new Vector2(639, 650);
        private const int RESET_WIDTH = 322;
        private const int RESET_HEIGHT = 177;



        // Main menu background
        private Sprite _mainMenuBackground;
        private const int BACKGROUND_MAIN_POS_X = 0;
        private const int BACKGROUND_MAIN_POS_Y = 0;
        private const int BACKGROUND_MAIN_WIDTH = 700;
        private const int BACKGROUND_MAIN_HEIGHT = 320;
        private Vector2 _mainMenuBackgroundPosition = new Vector2(DeadSpaceGame.WINDOW_WIDTH - 700, 0);
        private const float TRANSITION_ANIMATION_SPEED = 275f;

        // Game over background
        private Sprite _gameOverBackground;
        private const int BACKGROUND_GAMEOVER_POS_X = 0;
        private const int BACKGROUND_GAMEOVER_POS_Y = 0;
        private const int BACKGROUND_GAMEOVER_WIDTH = 700;
        private const int BACKGROUND_GAMEOVER_HEIGHT = 310;
        private Vector2 _gameOverBackgroundPosition = new Vector2(450, 245);

        // Scoreboard background
        private Sprite _scoreboardBackground;
        private const int BACKGROUND_SB_POS_X = 0;
        private const int BACKGROUND_SB_POS_Y = 109;
        private const int BACKGROUND_SB_WIDTH = 700;
        private const int BACKGROUND_SB_HEIGHT = 75;
        private Vector2 _scoreBoardBackgroundPosition = new Vector2(450, 20);

        // Options menu background
        private Sprite _optionsBackground;
        private const int BACKGROUND_OPTIONS_POS_X = 0;
        private const int BACKGROUND_OPTIONS_POS_Y = 186;
        private const int BACKGROUND_OPTIONS_WIDTH = 490;
        private const int BACKGROUND_OPTIONS_HEIGHT = 80;
        private Vector2 _optionsBackgroundPosition = new Vector2(555, 20);

        // Text input background
        private Sprite _textBox;
        private const int TEXT_BOX_POS_X = 0;
        private const int TEXT_BOX_POS_Y = 272;
        private const int TEXT_BOX_WIDTH = 595;
        private const int TEXT_BOX_HEIGHT = 117;
        private Vector2 _textBoxPosition = new Vector2(503, 340);


        public string JumpKey { get; set; }
        public string DropKey { get; set; }
        public string AttackKey { get; set; }

        public MenuState State { get; set; }
        public string NameToSave { get; set; }
        public string ButtonNameToChange { get; set; }
        public char CharacterToChange { get; set; }
        public int DrawOrder { get; } = 0;

        public MenuManager(DeadSpaceGame game, EntityManager entityManager, Texture2D mainMenuSpriteSheet, Texture2D gameoverMenuSpriteSheet, Texture2D buttonsTexture, SpriteFont font)
        {
            _game = game;
            _font = font;

            JumpKey = "W";
            DropKey = "S";
            AttackKey = "R";

            _textInput = new TextInput(this);

            _spriteSheetTexture = mainMenuSpriteSheet;
            _mainMenuBackground = new Sprite(mainMenuSpriteSheet, BACKGROUND_MAIN_POS_X, BACKGROUND_MAIN_POS_Y, BACKGROUND_MAIN_WIDTH, BACKGROUND_MAIN_HEIGHT);
            _gameOverBackground = new Sprite(gameoverMenuSpriteSheet, BACKGROUND_GAMEOVER_POS_X, BACKGROUND_GAMEOVER_POS_Y, BACKGROUND_GAMEOVER_WIDTH, BACKGROUND_GAMEOVER_HEIGHT);
            _scoreboardBackground = new Sprite(buttonsTexture, BACKGROUND_SB_POS_X, BACKGROUND_SB_POS_Y, BACKGROUND_SB_WIDTH, BACKGROUND_SB_HEIGHT);
            _optionsBackground = new Sprite(buttonsTexture, BACKGROUND_OPTIONS_POS_X, BACKGROUND_OPTIONS_POS_Y, BACKGROUND_OPTIONS_WIDTH, BACKGROUND_OPTIONS_HEIGHT);
            _textBox = new Sprite(buttonsTexture, TEXT_BOX_POS_X, TEXT_BOX_POS_Y, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT);
            _entityManager = entityManager;
            
            // Main menu buttons
            Button b = new Button(mainMenuSpriteSheet, this, PLAY_NAME, PLAY_STATE, PLAY_DESTINATION, PLAY_SPRITE_POSITION, PLAY_WIDTH, MAIN_MENU_BUTTON_HEIGHT, PLAY_BUTTON_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(mainMenuSpriteSheet, this, OPTIONS_BUTTON_NAME, OPTIONS_BUTTON_STATE, OPTIONS_BUTTON_DESTINATION, OPTIONS_BUTTON_SPRITE_POSITION, OPTIONS_BUTTON_WIDTH, MAIN_MENU_BUTTON_HEIGHT, OPTIONS_BUTTON_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(mainMenuSpriteSheet, this, SCOREBOARD_BUTTON_NAME, SCOREBOARD_BUTTON_STATE, SCOREBOARD_BUTTON_DESTINATION, SCOREBOARD_BUTTON_SPRITE_POSITION, SCOREBOARD_BUTTON_WIDTH, MAIN_MENU_BUTTON_HEIGHT, SCOREBOARD_BUTTON_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(mainMenuSpriteSheet, this, EXIT_BUTTON_NAME, EXIT_BUTTON_STATE, EXIT_BUTTON_DESTINATION, EXIT_SPRITE_POSITION, EXIT_BUTTON_WIDTH, MAIN_MENU_BUTTON_HEIGHT, EXIT_BUTTON_POSITION, _font);
            MenuButtons.Add(b);
            
            // Game over buttons
            b = new Button(gameoverMenuSpriteSheet, this, PLAY_AGAIN_NAME, PLAY_AGAIN_STATE, PLAY_AGAIN_DESTINATION, PLAY_AGAIN_SPRITE_POSITION, GAMEOVER_WIDTH, GAMEOVER_HEIGHT, PLAY_AGAIN_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(gameoverMenuSpriteSheet, this, MAIN_MENU_NAME, MAIN_MENU_STATE, MAIN_MENU_DESTINATION, MAIN_MENU_SPRITE_POSITION, GAMEOVER_WIDTH, GAMEOVER_HEIGHT, MAIN_MENU_POSITION, _font);
            MenuButtons.Add(b);

            // Scoreboard button
            b = new Button(buttonsTexture, this, RETURN_NAME, RETURN_STATE, RETURN_DESTINATION, RETURN_SPRITE_POSITION, RETURN_WIDTH, RETURN_HEIGHT, RETURN_POSITION, _font);
            MenuButtons.Add(b);

            // Options buttons
            b = new Button(buttonsTexture, this, RETURN_NAME, RETURN_STATE_2, RETURN_DESTINATION, RETURN_SPRITE_POSITION, RETURN_WIDTH, RETURN_HEIGHT, RETURN_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(buttonsTexture, this, JUMP_CHANGE_NAME, CHANGE_STATE, CHANGE_DESTINATION, CHANGE_SPRITE_POSITION, CHANGE_WIDTH, CHANGE_HEIGHT, JUMP_CHANGE_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(buttonsTexture, this, DROP_CHANGE_NAME, CHANGE_STATE, CHANGE_DESTINATION, CHANGE_SPRITE_POSITION, CHANGE_WIDTH, CHANGE_HEIGHT, DROP_CHANGE_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(buttonsTexture, this, ATTACK_CHANGE_NAME, CHANGE_STATE, CHANGE_DESTINATION, CHANGE_SPRITE_POSITION, CHANGE_WIDTH, CHANGE_HEIGHT, ATTACK_CHANGE_POSITION, _font);
            MenuButtons.Add(b);
            b = new Button(buttonsTexture, this, RESET_NAME, RESET_STATE, RESET_DESTINATION, RESET_SPRITE_POSITION, RESET_WIDTH, RESET_HEIGHT, RESET_POSITION, _font);
            MenuButtons.Add(b);


            State = MenuState.Main;

            foreach (Button bu in MenuButtons)
            {
                if (bu.DisplayState == State)
                    _entityManager.AddEntity(bu);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Button b in MenuButtons)
            {
                if (b.IsPressed == true)
                {
                    ProcessButtons(b, gameTime);

                    // Adds the new buttons that should be displayed to the entity manager and removes the ones that should no longer be displayed
                    if (State != MenuState.Transition)
                    {
                        foreach (Button bu in _entityManager.GetEntitiesOfType<Button>())
                            _entityManager.RemoveEntity(bu);
                        foreach (Button bu in MenuButtons)
                            if (bu.DisplayState == State)
                                _entityManager.AddEntity(bu);
                    }
                }
            }

            if (State == MenuState.TextEntry)
                _textInput.Update(gameTime);
        }

        /// <summary>
        /// All buttons are drawn by the entity manager, as the background isn't an entity it needs to be drawn by the menu manager
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State == MenuState.Main || State == MenuState.Transition)
            {
                _mainMenuBackground.Draw(spriteBatch, _mainMenuBackgroundPosition);
            }
            else if (State == MenuState.GameOver)
            {
                _gameOverBackground.Draw(spriteBatch, _gameOverBackgroundPosition);
            }
            else if (State == MenuState.ScoreBoard)
            {
                _scoreboardBackground.Draw(spriteBatch, _scoreBoardBackgroundPosition);
            }
            else if (State == MenuState.Options)
            {
                _optionsBackground.Draw(spriteBatch, _optionsBackgroundPosition);

                spriteBatch.DrawString(_font, "Jump :", new Vector2(JUMP_CHANGE_POSITION.X - 170, JUMP_CHANGE_POSITION.Y + 30), Color.White, 0, new Vector2(0, 0), 3, 0, 0);
                spriteBatch.DrawString(_font, "Drop :", new Vector2(DROP_CHANGE_POSITION.X - 165, DROP_CHANGE_POSITION.Y + 30), Color.White, 0, new Vector2(0, 0), 3, 0, 0);
                spriteBatch.DrawString(_font, "Attack :", new Vector2(ATTACK_CHANGE_POSITION.X - 190, ATTACK_CHANGE_POSITION.Y + 30), Color.White, 0, new Vector2(0, 0), 3, 0, 0);
            }
            else if (State == MenuState.TextEntry)
            {
                _textBox.Draw(spriteBatch, _textBoxPosition);
                spriteBatch.DrawString(_font, _textInput.Text, new Vector2(_textBoxPosition.X + 14, _textBoxPosition.Y + 30), Color.White, 0, new Vector2(0, 0), 3, 0, 0);
                spriteBatch.DrawString(_font, _textInput.DisplayText + ":", new Vector2(_textBoxPosition.X, _textBoxPosition.Y - 125), Color.White, 0, new Vector2(0, 0), 4, 0, 0);
            }
        }

        /// <summary>
        /// Deals with the result of pressing a button
        /// </summary>
        /// <param name="pressedButton"></param>
        /// <param name="gameTime"></param>
        private void ProcessButtons(Button pressedButton, GameTime gameTime)
        {
            State = pressedButton.Destination;

            // Plays a wipe transition if the play button was pressed
            if (pressedButton.Name == PLAY_NAME)
            {
                _mainMenuBackgroundPosition.X += (float)gameTime.ElapsedGameTime.TotalSeconds * TRANSITION_ANIMATION_SPEED;
                foreach (Button bu in _entityManager.GetEntitiesOfType<Button>())
                {
                    float newPosX = bu.Position.X + (float)gameTime.ElapsedGameTime.TotalSeconds * TRANSITION_ANIMATION_SPEED;
                    bu.Position = new Vector2(newPosX, bu.Position.Y);
                }

                // Plays the game when the transition has finished
                if (_mainMenuBackgroundPosition.X > DeadSpaceGame.WINDOW_WIDTH)
                {
                    pressedButton.IsPressed = false;
                    State = MenuState.Playing;
                    OnPlayPressed();
                }
            }
            else if (pressedButton.Name == MAIN_MENU_NAME)
            {
                // Moves the menu back to its original position
                _mainMenuBackgroundPosition.X -= (float)gameTime.ElapsedGameTime.TotalSeconds * TRANSITION_ANIMATION_SPEED;

                // Transition that moves each button back to its original position
                foreach (Button bu in _entityManager.GetEntitiesOfType<Button>())
                {
                    float newPosX = bu.Position.X - (float)gameTime.ElapsedGameTime.TotalSeconds * TRANSITION_ANIMATION_SPEED;
                    bu.Position = new Vector2(newPosX, bu.Position.Y);
                }

                if (_mainMenuBackgroundPosition.X <= DeadSpaceGame.WINDOW_WIDTH - 700)
                {
                    OnMainMenuPressed();
                    _mainMenuBackgroundPosition.X = DeadSpaceGame.WINDOW_WIDTH - 700;
                    pressedButton.IsPressed = false;
                }
            }
            else if (pressedButton.Name == PLAY_AGAIN_NAME)
            {
                // Restarts the game
                OnPlayPressed();
                pressedButton.IsPressed = false;
            }
            else if (pressedButton.Name == EXIT_BUTTON_NAME)
            {
                // Closes the game
                _game.Exit();
            }
            else if (pressedButton.Name == JUMP_CHANGE_NAME || pressedButton.Name == DROP_CHANGE_NAME || pressedButton.Name == ATTACK_CHANGE_NAME)
            {
                _textInput.NewText(1, "Enter key");
                pressedButton.IsPressed = false;
                ButtonNameToChange = pressedButton.Name;
            }
            else if (pressedButton.Name == RESET_NAME)
            {
                OnResetSaveData();
                pressedButton.IsPressed = false;
            }
            else
            {
                // If the button doesn't have a special purpose, it won't return back to being unpressed, therefore it needs to be manualyl changed
                pressedButton.IsPressed = false;
            }
        }


        /// <summary>
        /// As no buttons are pressed for the game over screen it has to be manually displayed
        /// </summary>
        public void ShowGameOver()
        {
            State = MenuState.GameOver;

            foreach (Button b in MenuButtons)
                if (b.DisplayState == State)
                    _entityManager.AddEntity(b);
        }

        /// <summary>
        /// As no buttons are pressed for the options menu after a new character is entered it has to be manually displayed
        /// </summary>
        public void ShowOptions()
        {
            State = MenuState.Options;

            foreach (Button b in MenuButtons)
                if (b.DisplayState == State)
                    _entityManager.AddEntity(b);
        }

        /// <summary>
        /// When a highscore has been reached the menu state is set so that text can be entered
        /// </summary>
        public void GetNameForSave()
        {
            _textInput.NewText(10, "Enter name");
            State = MenuState.TextEntry;
        }

        /// <summary>
        /// Event that is raised when the main menu button is pressed
        /// </summary>
        protected virtual void OnMainMenuPressed()
        {
            EventHandler handler = MainMenu;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Even that is raised when a new game should be played
        /// </summary>
        protected virtual void OnPlayPressed()
        {
            EventHandler handler = PlayAgain;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that is raised when the game needs to be saved
        /// </summary>
        public virtual void OnSave()
        {
            ShowGameOver();
            NameToSave = _textInput.Text;

            EventHandler handler = SaveTheGame;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that is raised when a new character is entered
        /// </summary>
        public virtual void OnCharacterEntered()
        {
            ShowOptions();

            State = MenuState.Options;
            CharacterToChange = _textInput.Text[0];

            EventHandler handler = SaveNewCharacter;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the saved data needs to be reset
        /// </summary>
        protected virtual void OnResetSaveData()
        {
            EventHandler handler = ResetSaveData;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
