using EndlessRunner.Entities;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessRunner.System
{
    public class InputController
    {
        private bool _isBlocked;

        private Astronaut _player;
        private KeyboardState _previousKeyboardState;

        private int _jumpKey;
        private int _dropKey;
        private int _attackKey;

        public InputController(Astronaut player, int jump, int drop, int attack)
        {
            _player = player;

            _jumpKey = jump;
            _dropKey = drop;
            _attackKey = attack;
        }

        public void ProcessControlls(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (!_isBlocked)
            {
                bool isJumpKeyPressed = keyboardState.IsKeyDown((Keys)_jumpKey);
                bool wasJumpKeyPressed = _previousKeyboardState.IsKeyDown((Keys)_jumpKey);

                bool isDropKeyPressed = keyboardState.IsKeyDown((Keys)_dropKey);

                bool isAttackKeyPressed = keyboardState.IsKeyDown((Keys)_attackKey);
                bool wasAttackKeyPressed = _previousKeyboardState.IsKeyDown((Keys)_attackKey);

                if (!wasAttackKeyPressed && isAttackKeyPressed)
                {
                    _player.Attack(gameTime);
                }
                else if (!wasJumpKeyPressed && isJumpKeyPressed)
                {
                    if (_player.State != PlayerState.Jumping)
                        _player.BeginJump();
                }
                else if (_player.State == PlayerState.Jumping && !isJumpKeyPressed)
                {
                    _player.CancelJump();
                }
                else if (keyboardState.IsKeyDown((Keys)_dropKey))
                {
                    _player.LandBlocked = true;
                    _player.Drop();

                    // TODO Doesn't go through platform if pressed - it has to be held
                    // maybe need to change it
                }
                else if (_player.State == PlayerState.Falling && !isDropKeyPressed)
                {
                    _player.LandBlocked = false;
                }

            }

            _previousKeyboardState = keyboardState;

            _isBlocked = false;
        }

        /// <summary>
        /// Changes the jump key
        /// </summary>
        /// <param name="newKey"></param>
        public void ChangeJumpKey(int newKey)
        {
            _jumpKey = newKey;
        }

        /// <summary>
        /// Changes the drop key
        /// </summary>
        /// <param name="newKey"></param>
        public void ChangeDropKey(int newKey)
        {
            _dropKey = newKey;
        }

        /// <summary>
        /// Changes the attack key
        /// </summary>
        /// <param name="newKey"></param>
        public void ChangeAttackKey(int newKey)
        {
            _attackKey = newKey;
        }

        /// <summary>
        /// Blocks the input so no 
        /// </summary>
        public void BlockInputTemporarily()
        {
            _isBlocked = true;
        }
    }
}
