using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EndlessRunner.Graphics;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.WIC;
using SharpDX.Win32;
using System.Runtime.CompilerServices;
using EndlessRunner.Menu;
using System.Windows.Forms;

namespace EndlessRunner.Entities
{
    public class Astronaut : IGameEntity, ICollideable
    {
        // Idle sprite dimensions and coordinates
        private const int PLAYER_IDLE_SPRITE_POS_X = 0;
        private const int PLAYER_IDLE_SPRITE_POS_Y = 0;
        private const int PLAYER_IDLE_SPRITE_WIDTH = 51;
        public const int PLAYER_IDLE_SPRITE_HEIGHT = 94;
        private Sprite _idleSprite;

        // Collisions box for feet
        private const int FEET_HIT_BOX_HEIGHT = 5;

        // Running sprite dimensions and cooridinates
        private const int PLAYER_RUNNING_SPRITE_POS_X = 51;
        private const int PLAYER_RUNNING_SPRITE_POS_Y = 0;
        private const int PLAYER_RUNNING_WIDTH = 67;
        private const int PLAYER_RUNNING_HEIGHT = 82;
        private Sprite _runningSprite;
        private SpriteAnimation _runAnimation;
        private const float RUN_ANIMATION_FRAME_LENGTH = 1 / 5f;

        // Jumping sprite dimensions and coordinates
        private const int PLAYER_JUMP_SPRITE_POS_X = 118;
        private const int PLAYER_JUMP_SPRITE_POS_Y = 0;
        private const int PLAYER_JUMP_SPRITE_WIDTH = 85;
        private const int PLAYER_JUMP_SPRITE_HEIGHT = 76;
        private Sprite _jumpSprite;
        private SpriteAnimation _jumpAnimation;
        private const float JUMP_ANIMATION_FRAME_LENGTH = 1 / 10f;

        // Attacking sprite dimensions and coordinates
        private const int PLAYER_ATTACK_SPRITE_POS_X = 203;
        private const int PLAYER_ATTACK_SPRITE_POS_Y = 0;
        private const int PLAYER_ATTACK_SPRITE_WIDTH = 89;
        private const int PLAYER_ATTACK_SPRITE_HEIGHT = 83;
        private Sprite _attackSprite;
        private SpriteAnimation _attackAnimation;
        private const float ATTACK_ANIMATION_FRAME_LENGTH = 1 / 3f;
        private bool _shouldPlayAttackAnim;

        // Dead sprite dimensions
        private const int PLAYER_DEAD_SPRITE_POS_X = 297;
        private const int PLAYER_DEAD_SPRITE_POS_Y = 25;
        private const int PLAYER_DEAD_SPRITE_POS_WIDTH = 94;
        private const int PLAYER_DEAD_SPRITE_POS_HEIGHT = 47;
        private Sprite _deadSprite;

        // Jump variables
        private const float JUMP_START_VELOCITY = -375f;
        private float _verticalVelocity;
        private float START_GRAVITY = 500f;
        private const float CANCEL_JUMP_VELOCITY = -100f;

        // Speed variables (player is responsible for the speed of the game)
        public const float START_SPEED = 100f;
        public const float MAX_SPEED = 300f;
        private const float ACCELERATION_PPS_PER_SECOND = 1.5f;
        private const float DEATH_SPEED = 200f;

        // Ammunition variables
        private const int MAXIMUM_AMMUNITION = 5;
        private const int START_AMMUNITION = 3;
        private const int POWERUP_AMMUNITION_INCREASE = 3;

        // Gravity variables
        private const int MINIMUM_GRAVITY_DECREASE = 50;
        private const int MAXIMUM_GRAVITY_DECREASE = 180;
        private const float GRAVITY_CHANGE_TIMER = 1f;
        private float _gravityChangeTimeLeft;

        // Coordinates and dimensions of ammunition sprite
        private const int AMMO_SPRITE_POS_X = 391;
        private const int AMMO_SPRITE_POS_Y = 0;
        private const int AMMO_SPRITE_WIDTH = 18;
        private const int AMMO_SPRITE_HEIGHT = 83;

        // Coordinates and dimensions of shield sprite
        private const int SHIELD_SPRITE_POS_X = 357;
        private const int SHIELD_SPRITE_POS_Y = 92;
        private const int SHIELD_SPRITE_WIDTH = 36;
        private const int SHIELD_SPRITE_HEIGHT = 44;

        private const int AMMUNITION_DISPLAY_X = 10;
        private const int AMMUNITION_DISPLAY_Y = DeadSpaceGame.WINDOW_HEIGHT - AMMO_SPRITE_HEIGHT - 10;
        private const int SHIELD_DISPLAY_X = 160;
        private const int SHIELD_DISPLAY_Y = DeadSpaceGame.WINDOW_HEIGHT - SHIELD_SPRITE_HEIGHT - 10;

        private float _startPosY;

        public EventHandler Died;

        private MenuManager _menuManager;

        private EntityManager _entityManager;

        private Texture2D _spriteSheetTexture;

        private Sprite _ammoSprite;
        private Sprite _shieldSprite;

        public Vector2 Position { get; set; }
        public bool IsAlive { get; private set; }
        public float Speed { get; private set; }
        public float AccelerationByGravity { get; set; }
        public bool LandBlocked { get; set; }
        public PlayerState State { get; set; }
        public int Ammunition { get; set; }
        public bool HasShield { get; set; }
        public bool IsEnabled { get; set; }
        public int DrawOrder { get; set; }
        public Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle(
                    (int)Math.Round(Position.X),
                    (int)Math.Round(Position.Y),
                    PLAYER_IDLE_SPRITE_WIDTH,
                    PLAYER_IDLE_SPRITE_HEIGHT
                );

                return box;
            }
        }
        public Rectangle FeetHitBox
        {
            get
            {
                Rectangle box = new Rectangle(
                    (int)Math.Round(Position.X),
                    (int)Math.Round(Position.Y + PLAYER_IDLE_SPRITE_HEIGHT - FEET_HIT_BOX_HEIGHT),
                    PLAYER_IDLE_SPRITE_WIDTH,
                    FEET_HIT_BOX_HEIGHT
                );

                return box;
            }
        }

        public Rectangle SpriteSheetPosition
        {
            get
            {
                Rectangle pos = new Rectangle(
                    PLAYER_IDLE_SPRITE_POS_X, PLAYER_IDLE_SPRITE_POS_Y, PLAYER_IDLE_SPRITE_WIDTH, PLAYER_IDLE_SPRITE_HEIGHT);

                return pos;
            }
        }

        public Astronaut(Texture2D spriteSheet, EntityManager entityManager, Vector2 position, MenuManager menuManager)
        {
            Position = position;
            _startPosY = position.Y;

            _entityManager = entityManager;
            _spriteSheetTexture = spriteSheet;

            _menuManager = menuManager;

            IsAlive = true;
            State = PlayerState.Idle;
            IsEnabled = false;

            _ammoSprite = new Sprite(spriteSheet, AMMO_SPRITE_POS_X, AMMO_SPRITE_POS_Y, AMMO_SPRITE_WIDTH, AMMO_SPRITE_HEIGHT);
            _shieldSprite = new Sprite(spriteSheet, SHIELD_SPRITE_POS_X, SHIELD_SPRITE_POS_Y, SHIELD_SPRITE_WIDTH, SHIELD_SPRITE_HEIGHT);

            _idleSprite = new Sprite(_spriteSheetTexture, PLAYER_IDLE_SPRITE_POS_X, PLAYER_IDLE_SPRITE_POS_Y, PLAYER_IDLE_SPRITE_WIDTH, PLAYER_IDLE_SPRITE_HEIGHT);
            _runningSprite = new Sprite(_spriteSheetTexture, PLAYER_RUNNING_SPRITE_POS_X, PLAYER_RUNNING_SPRITE_POS_Y, PLAYER_RUNNING_WIDTH, PLAYER_RUNNING_HEIGHT);
            _jumpSprite = new Sprite(_spriteSheetTexture, PLAYER_JUMP_SPRITE_POS_X, PLAYER_JUMP_SPRITE_POS_Y, PLAYER_JUMP_SPRITE_WIDTH, PLAYER_JUMP_SPRITE_HEIGHT);
            _attackSprite = new Sprite(_spriteSheetTexture, PLAYER_ATTACK_SPRITE_POS_X, PLAYER_ATTACK_SPRITE_POS_Y, PLAYER_ATTACK_SPRITE_WIDTH, PLAYER_ATTACK_SPRITE_HEIGHT);
            _deadSprite = new Sprite(_spriteSheetTexture, PLAYER_DEAD_SPRITE_POS_X, PLAYER_DEAD_SPRITE_POS_Y, PLAYER_DEAD_SPRITE_POS_WIDTH, PLAYER_DEAD_SPRITE_POS_HEIGHT);

            _runAnimation = new SpriteAnimation();
            _runAnimation.AddFrame(_idleSprite, 0);
            _runAnimation.AddFrame(_runningSprite, RUN_ANIMATION_FRAME_LENGTH);
            _runAnimation.AddFrame(_runAnimation[0].Sprite, RUN_ANIMATION_FRAME_LENGTH * 2);
            _runAnimation.Play();

            _jumpAnimation = new SpriteAnimation();
            _jumpAnimation.AddFrame(_runningSprite, 0);
            _jumpAnimation.AddFrame(_jumpSprite, JUMP_ANIMATION_FRAME_LENGTH);
            _jumpAnimation.AddFrame(_jumpAnimation[0].Sprite, JUMP_ANIMATION_FRAME_LENGTH * 2);
            _jumpAnimation.Play();

            _attackAnimation = new SpriteAnimation();
            CreateAttackAnimation();
            _attackAnimation.Play();
        }

        /// <summary>
        /// Initialises the player when the game first starts or is replayed
        /// </summary>
        public void Initialise()
        {
            Speed = START_SPEED;
            AccelerationByGravity = START_GRAVITY;
            _verticalVelocity = 0;
            State = PlayerState.Running;

            Ammunition = START_AMMUNITION;
            HasShield = false;
            _gravityChangeTimeLeft = 0;

            IsAlive = true;
            IsEnabled = true;

            foreach (PlayerProjectile p in _entityManager.GetEntitiesOfType<PlayerProjectile>())
            {
                _entityManager.RemoveEntity(p);
            }

            Position = new Vector2(Position.X, _startPosY);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Only draws if the game is being played
            if (_menuManager.State == MenuState.Playing)
            {                
                if (_shouldPlayAttackAnim)
                {
                    _attackAnimation.Draw(spriteBatch, Position);
                    _attackAnimation.Update(gameTime);

                    if (_attackAnimation.PlaybackProgress == 0)
                        _shouldPlayAttackAnim = false;
                }
                else if (State == PlayerState.Idle)
                {
                    _idleSprite.Draw(spriteBatch, Position);
                }
                else if (State == PlayerState.Jumping || State == PlayerState.Falling)
                {
                    _jumpAnimation.Draw(spriteBatch, Position);
                }
                else if (State == PlayerState.Running)
                {
                    _runAnimation.Draw(spriteBatch, Position);
                }
                else
                {
                    _deadSprite.Draw(spriteBatch, Position);
                }


                // Displays the ammunition the player currently has
                if (Ammunition > 0)
                {
                    for (int i = 0; i < Ammunition; i++)
                    {
                        Vector2 pos = new Vector2(AMMUNITION_DISPLAY_X + i * (AMMO_SPRITE_WIDTH + 10), AMMUNITION_DISPLAY_Y);
                        _ammoSprite.Draw(spriteBatch, pos);
                    }
                }

                // Displays whether the player has a shield or not
                if (HasShield)
                {
                    Vector2 pos = new Vector2(SHIELD_DISPLAY_X, SHIELD_DISPLAY_Y);
                    _shieldSprite.Draw(spriteBatch, pos);
                }
            }
        }


        public void Update(GameTime gameTime)
        {
            // Controls the physics of the character using SUVAT
            if (State == PlayerState.Jumping || State == PlayerState.Falling)
            {
                float v = _verticalVelocity;
                float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float a = AccelerationByGravity;

                // S = vt - 0.5at^2
                Vector2 s = new Vector2(0, v * t - (float)(0.5 * a * Math.Pow(t, 2)));
                Position = new Vector2(Position.X, Position.Y) + s;
                // v = u + at
                _verticalVelocity = v + a * t;

                if (_verticalVelocity >= 0)
                    State = PlayerState.Falling;

            }
            else if (State == PlayerState.Running)
            {
                _runAnimation.Update(gameTime);
            }   
            else if (State == PlayerState.Dead)
            {
                if (Position.Y < DeadSpaceGame.WINDOW_HEIGHT)
                {
                    Position = new Vector2(Position.X, Position.Y + DEATH_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    OnDied();
                    State = PlayerState.Idle;
                }

            }

            // Removes each projectile that has moved off the screen
            List<PlayerProjectile> projectilesToRemove = new List<PlayerProjectile>();  

            foreach (PlayerProjectile p in _entityManager.GetEntitiesOfType<PlayerProjectile>())
            {
                if (p.Position.X > DeadSpaceGame.WINDOW_WIDTH)
                    projectilesToRemove.Add(p);
            }
            foreach (PlayerProjectile p in projectilesToRemove)
            {
                _entityManager.RemoveEntity(p);
            }

            // Increases the speed at which the platforms move (gives the illusion the player is running faster)
            if (State != PlayerState.Idle && State != PlayerState.Dead)
                Speed += ACCELERATION_PPS_PER_SECOND * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Speed > MAX_SPEED)
                Speed = MAX_SPEED;

            // The player dies if they fall out of the world
            if (Position.Y > DeadSpaceGame.WINDOW_HEIGHT)
                Die();

            if (_gravityChangeTimeLeft > 0)
                _gravityChangeTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds * GRAVITY_CHANGE_TIMER;
            else
                AccelerationByGravity = START_GRAVITY;
        }

        /// <summary>
        /// Creates an attack animation whenever the attack button is pressed
        /// </summary>
        public void CreateAttackAnimation()
        {
            _attackAnimation.Clear();

            _attackAnimation.ShouldLoop = false;

            _attackAnimation.AddFrame(_attackSprite, 0);
            _attackAnimation.AddFrame(_attackAnimation[0].Sprite, ATTACK_ANIMATION_FRAME_LENGTH );
        }

        /// <summary>
        /// Begins the characters jump by changing its state and increasing velocity upwards
        /// </summary>
        /// <returns></returns>
        public bool BeginJump()
        {
            if (State == PlayerState.Jumping || State == PlayerState.Falling || State == PlayerState.Dead || IsEnabled == false)
                return false;

            State = PlayerState.Jumping;

            _verticalVelocity = JUMP_START_VELOCITY;

            return true;
        }

        /// <summary>
        /// Makes the player start falling
        /// </summary>
        /// <returns></returns>
        public bool Drop()
        {
            if (State != PlayerState.Running)
                return false;

            State = PlayerState.Falling;
            return true;
        }

        /// <summary>
        /// If the jump key is released before the full jump has been completed then the maximum height is not reached
        /// </summary>
        /// <returns></returns>
        public bool CancelJump()
        {
            if (State != PlayerState.Jumping)
                return false;

            _verticalVelocity = _verticalVelocity < CANCEL_JUMP_VELOCITY ? CANCEL_JUMP_VELOCITY : 0;
            return true;
        }

        /// <summary>
        /// Lands the player on the platform when its feet come into contact with it
        /// </summary>
        /// <param name="targetPlatform"></param>
        public void Land(Platform targetPlatform)
        {
            if (State != PlayerState.Falling || LandBlocked == true)
                return;

            _verticalVelocity = 0;
            Position = new Vector2(Position.X, targetPlatform.Position.Y - PLAYER_IDLE_SPRITE_HEIGHT);
            State = PlayerState.Running;
        }

        /// <summary>
        /// Allows the player to attack, creating a projectile
        /// </summary>
        public void Attack(GameTime gameTime)
        {
            if (Ammunition <= 0)
                return;
            
            CreateAttackAnimation();
            _attackAnimation.Play();
            _shouldPlayAttackAnim = true;

            Ammunition -= 1;

            CreateBullet();
        }

        /// <summary>
        /// Creates a projectile
        /// </summary>
        private void CreateBullet()
        {
            Vector2 bulletPos = new Vector2(Position.X + 81, Position.Y + 34);
            PlayerProjectile proj = new PlayerProjectile(_spriteSheetTexture, this, bulletPos, _menuManager);

            _entityManager.AddEntity(proj);
        }

        /// <summary>
        /// Increases ammunition when the ammunition powerup is picked up
        /// </summary>
        public void GetAmmunition()
        {
            Ammunition += POWERUP_AMMUNITION_INCREASE;

            if (Ammunition > MAXIMUM_AMMUNITION)
                Ammunition = MAXIMUM_AMMUNITION;
        }

        public void GetShield()
        {
            HasShield = true;
        }

        /// <summary>
        /// Changes the gravity when a gravity powerup is picked up
        /// </summary>
        public void ChangeGravity()
        {
            Random r = new Random();

            double newGrav;
            newGrav = r.NextDouble() * (MAXIMUM_GRAVITY_DECREASE - MINIMUM_GRAVITY_DECREASE) + MINIMUM_GRAVITY_DECREASE;

            AccelerationByGravity -= (float)newGrav;

            _gravityChangeTimeLeft = 10;
        }

        protected virtual void OnDied()
        {
            EventHandler handler = Died;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Kills the player which ends the game
        /// </summary>
        /// <returns></returns>
        public bool Die()
        {
            if (!IsAlive)
                return false;

            State = PlayerState.Dead;
            Speed = 0;
            IsAlive = false;

            return true;
        }
    }
}
