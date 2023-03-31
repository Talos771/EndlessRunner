using EndlessRunner.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EndlessRunner.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using MonoGame.Extended;

namespace EndlessRunner.Menu
{
    public class Button : IGameEntity
    {
        MenuManager _menuManager;

        SpriteFont _font;

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        // Conditions under which the button will be displayed
        public MenuState DisplayState { get; set; }
        // When pressed the menu will be changed to this state
        public MenuState Destination { get; set; }
        public bool IsPressed { get; set; }
        public Sprite Sprite { get; }
        public int DrawOrder { get; }
        private Rectangle ButtonBounds { get; set; }

        public Button(Texture2D spriteSheet, MenuManager menuManager, string name, MenuState displayState, MenuState destination, Vector2 spritePosition, int width, int height, Vector2 position, SpriteFont font)
        {
            _menuManager = menuManager;

            _font = font;

            Name = name;
            DisplayState = displayState;
            Destination = destination;
            Position = position;

            Sprite = new Sprite(spriteSheet, (int)spritePosition.X, (int)spritePosition.Y, width, height);
            
            DrawOrder = 100;
            IsPressed = false;

            ButtonBounds = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (ButtonBounds.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed && _menuManager.State != MenuState.Transition)
            {
                IsPressed = true;
            }

            ButtonBounds = new Rectangle((int)Position.X, (int)Position.Y, ButtonBounds.Width, ButtonBounds.Height);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
            
            if (Name == "Change Jump")
                spriteBatch.DrawString(_font, _menuManager.JumpKey, new Vector2(570 + 27, 150 + 35), Color.White, 0, new Vector2(0, 0), 3, 0, 0);
            else if (Name == "Change Drop")
                spriteBatch.DrawString(_font, _menuManager.DropKey, new Vector2(570 + 27, 287 + 35), Color.White, 0, new Vector2(0, 0), 3, 0, 0);
            else if (Name == "Change Attack")
                spriteBatch.DrawString(_font, _menuManager.AttackKey, new Vector2(910 + 27, 218 + 35), Color.White, 0, new Vector2(0, 0), 3, 0, 0);
        }
    }
}
