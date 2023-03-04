using EndlessRunner.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using EndlessRunner.System;
using System.Web;
using EndlessRunner.Menu;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using EndlessRunner.Starfield;
using SharpDX.Direct3D9;
using System.Collections.Generic;

namespace EndlessRunner
{
    public class DeadSpaceGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Window variables
        private const string GAME_TITLE = "Deadspace";
        public const int WINDOW_WIDTH = 1600;
        public const int WINDOW_HEIGHT = 800;

        // Asset file names
        private const string ASSET_NAME_SPRITESHEET_CHARACTERS = "Assets";
        private const string ASSET_NAME_SPRITESHEET_MAIN_MENU = "MainMenu";
        private const string ASSET_NAME_SPRITESHEET_GAME_OVER = "GameOver";
        private const string ASSET_NAME_SPRITESHEET_BUTTONS = "OtherButtons";
        private const string FONT_NAME_FILE = "Font";

        private const string SAVE_FILE_NAME = "EndlessRunnerSave.dat";

        // Default controls for the player
        private const int DEFAULT_PLAYER1_JUMP = (int)'W';
        private const int DEFAULT_PLAYER1_DROP = (int)'S';
        private const int DEFAULT_PLAYER1_ATTACK = (int)'R';

        // Player start coordinates
        private const int PLAYER_START_POS_X = 30;
        private const int PLAYER_START_POS_Y = 306;


        // SpriteSheets (needed many cuz im cool like that ;)
        private Texture2D _spriteSheetTexture;
        private Texture2D _mainMenuTexture;
        private Texture2D _gameOverTexture;
        private Texture2D _buttonsTexture;

        private SpriteFont _font;

        private const int SCORE_BOARD_POS_X = 25;
        private const int SCORE_BOARD_POS_Y = 10;

        private StarField _starField;

        private Astronaut _player;
        private InputController _playerInputController;

        private EntityManager _entityManager;

        private PlatformManager _platformManager;

        private EnemyManager _enemyManager;

        private SkyManager _skyManager;

        private MenuManager _menuManager;

        private ScoreBoard _scoreBoard;

        private PowerupManager _powerupManager;

        public GameState State { get; set; }

        public DeadSpaceGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _entityManager = new EntityManager();
            State = GameState.Playing;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Window.Title = GAME_TITLE;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteSheetTexture = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET_CHARACTERS);
            _mainMenuTexture = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET_MAIN_MENU);
            _gameOverTexture = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET_GAME_OVER);
            _buttonsTexture = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET_BUTTONS);

            _font = Content.Load<SpriteFont>(FONT_NAME_FILE);

            _menuManager = new MenuManager(this, _entityManager, _mainMenuTexture, _gameOverTexture, _buttonsTexture, _font);
            _menuManager.MainMenu += menu_NewGame;
            _menuManager.PlayAgain += menu_PlayAgain;
            _menuManager.SaveTheGame += menu_SaveGame;

            _starField = new StarField(WINDOW_WIDTH, WINDOW_WIDTH, 500, _menuManager);

            _player = new Astronaut(_spriteSheetTexture, _entityManager, new Vector2(PLAYER_START_POS_X, PLAYER_START_POS_Y), _menuManager);
            _player.DrawOrder = 20;
            _player.Died += player_Died;
            _playerInputController = new InputController(_player, DEFAULT_PLAYER1_JUMP, DEFAULT_PLAYER1_DROP, DEFAULT_PLAYER1_ATTACK);

            _platformManager = new PlatformManager(_spriteSheetTexture, _entityManager, _player, _menuManager);
            _scoreBoard = new ScoreBoard(_spriteSheetTexture, _player, _menuManager, new Vector2(SCORE_BOARD_POS_X, SCORE_BOARD_POS_Y), _font);
            _enemyManager = new EnemyManager(_entityManager, _player, _spriteSheetTexture, _menuManager, _scoreBoard);
            _powerupManager = new PowerupManager(_spriteSheetTexture, _player, _entityManager, _scoreBoard, _enemyManager, _menuManager);
            _skyManager = new SkyManager(_spriteSheetTexture, _menuManager, _entityManager, _player);

            _entityManager.AddEntity(_player);
            _entityManager.AddEntity(_platformManager);
            _entityManager.AddEntity(_enemyManager);
            _entityManager.AddEntity(_menuManager);
            _entityManager.AddEntity(_scoreBoard);
            _entityManager.AddEntity(_powerupManager);
            _entityManager.AddEntity(_skyManager);

            _platformManager.Initialise();

            LoadSaveData();
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            // KeyboardState keyboardState = Keyboard.GetState();

            if (State == GameState.Playing)
                _playerInputController.ProcessControlls(gameTime);

            if (_menuManager.State == MenuState.Main || _menuManager.State == MenuState.Transition)
                _starField.Update(gameTime);

            _entityManager.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _entityManager.Draw(_spriteBatch, gameTime);

            if (_menuManager.State == MenuState.Main || _menuManager.State == MenuState.Transition)
                _starField.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        public void PlayGame()
        {
            _player.Initialise();
            State = GameState.Playing;
            _enemyManager.IsEnabled = true;
            _powerupManager.IsEnabled = true;
            _scoreBoard.Score = 0;
        }

        /// <summary>
        /// An event is raised when the player dies
        /// this stops the game and shows the game over screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void player_Died(object sender, EventArgs eventArgs)
        {
            State = GameState.GameOver;
            _enemyManager.IsEnabled = false;
            _powerupManager.IsEnabled = false;

            bool shouldAdd = false;

            foreach (int num in _scoreBoard.HighScore)
            {
                if (_scoreBoard.DisplayScore > num || _scoreBoard.HighScore.Count < 10)
                    shouldAdd = true;
            }

            if (shouldAdd)
            {
                // If a score high enough to be on the scoreboard is reached then the menu manager gets a name
                _menuManager.GetNameForSave();
            }
            else
            {
                _menuManager.ShowGameOver();
            }
        }

        /// <summary>
        /// Event raised when a new game should be played
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void menu_NewGame(object sender, EventArgs eventArgs)
        {
            _platformManager.Initialise();
            _enemyManager.Reset();
            _powerupManager.Reset();
            _skyManager.Reset();
            _player.State = PlayerState.Idle;
            _player.Position = new Vector2(PLAYER_START_POS_X, PLAYER_START_POS_Y);
        }

        /// <summary>
        /// Event raised when the play again button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void menu_PlayAgain(object sender, EventArgs eventArgs)
        {
            menu_NewGame(sender, eventArgs);

            PlayGame();
        }

        /// <summary>
        /// Event raised when the game needs to be saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void menu_SaveGame(object sender, EventArgs eventArgs)
        {
            _scoreBoard.TryAddNewScore(_menuManager.Name);

            SaveGame();
        }

        /// <summary>
        /// Saves the hisghscore and the name to a file
        /// </summary>
        private void SaveGame()
        {
            SaveState saveState = new SaveState()
            {
                HighScore = _scoreBoard.HighScore,
                Names = _scoreBoard.Names
            };

            try
            {
                using (FileStream fs = new FileStream(SAVE_FILE_NAME, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, saveState);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("An error occured while saving the game: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Loads the highscore and names save data from the file 
        /// </summary>
        private void LoadSaveData()
        {
            try
            {
                using (FileStream fs = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    SaveState saveState = bf.Deserialize(fs) as SaveState;

                    if (saveState != null)
                    {
                        if (_scoreBoard.Score != null)
                        {
                            _scoreBoard.HighScore = saveState.HighScore;
                            _scoreBoard.Names = saveState.Names;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occured while loading the game: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Resets the highscore to 0
        /// </summary>
        private void ResetSaveState()
        {
            _scoreBoard.HighScore.Clear();
            _scoreBoard.Names.Clear();

            _scoreBoard.HighScore.Add(0);
            _scoreBoard.Names.Add("");

            SaveGame();
        }
    }
}