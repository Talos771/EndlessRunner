using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EndlessRunner.Menu
{
    public class TextInput
    {
        private int _maxInputString;
        private KeyboardState _previousKeyBoardState;
        MenuManager _menuManager;

        public string Text { get; set; }


        public TextInput(MenuManager menuManager) 
        {
            _menuManager = menuManager;
        }

        public void Update(GameTime gameTime)
        {
            // Reads the key that should be entered
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                // If the enter key is pressed then the name has been entered and the game saves
                _menuManager.OnSave();
            }
            else if (keyboardState.IsKeyDown(Keys.Back) && !_previousKeyBoardState.IsKeyDown(Keys.Back))
            {
                // If the backspace key has been pressed it removes a letter from the string
                if (Text.Length >= 1)
                    Text = Text.Substring(0, Text.Length - 1);
            }
            else if (keyboardState != _previousKeyBoardState)
            {
                // The current pressed key is added to the

                for (int i = (int)'A'; i <= (int)'Z'; i++)
                {
                    if (keyboardState.IsKeyDown((Keys)i))
                    {
                        char keyPressed = (char)i;

                        if (Text.Length < _maxInputString)
                            Text += keyPressed.ToString();
                    }
                }

            }

            _previousKeyBoardState = keyboardState;
        }

        /// <summary>
        /// Allows the user to enter text of a specified length
        /// </summary>
        /// <param name="maxInputString"></param>
        public void NewText(int maxInputString)
        {
            _maxInputString = maxInputString;
            Text = "";
        }
    }
}