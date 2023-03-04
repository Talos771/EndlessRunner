using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EndlessRunner.Graphics
{
    public class SpriteAnimation
    {
        private List<SpriteAnimationFrame> _frames = new List<SpriteAnimationFrame>();

        public SpriteAnimationFrame this[int index]
        {
            get
            {
                return GetFrame(index);
            }
        }

        public SpriteAnimationFrame CurrentFrame
        {
            get
            {
                return _frames.Where(f => f.TimeStamp <= PlaybackProgress).OrderBy(f => f.TimeStamp).LastOrDefault();
            }
        }

        public float Duration
        {
            get
            {
                if (!_frames.Any())
                    return 0;

                return _frames.Max(f => f.TimeStamp);
            }
        }

        public bool IsPlaying { get; private set; }
        public float PlaybackProgress { get; private set; }
        public bool ShouldLoop { get; set; } = true;

        /// <summary>
        /// Adds a frame to the animation
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="timeStamp"></param>
        public void AddFrame(Sprite sprite, float timeStamp)
        {
            SpriteAnimationFrame frame = new SpriteAnimationFrame(sprite, timeStamp);

            _frames.Add(frame);
        }

        public void Update(GameTime gameTime)
        {
            if (IsPlaying)
            {
                PlaybackProgress += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (PlaybackProgress > Duration)
                {
                    // If it is a looping animation it resets the playback progress to 0 so it continues
                    if (ShouldLoop)
                        PlaybackProgress -= Duration;
                    else
                        Stop();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            SpriteAnimationFrame frame = CurrentFrame;
            if (frame != null)
                frame.Sprite.Draw(spriteBatch, position);
        }

        /// <summary>
        /// Plays the animation
        /// </summary>
        public void Play()
        {
            IsPlaying = true;
        }

        /// <summary>
        /// Stops playing the animation
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
            PlaybackProgress = 0;
        }

        /// <summary>
        /// Returns the frame in the animation at the given location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SpriteAnimationFrame GetFrame(int index)
        {
            if (index < 0 || index >= _frames.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "A frame with index " + index + " does not exist in this animation.");

            return _frames[index];
        }

        /// <summary>
        /// Clears the animation
        /// </summary>
        public void Clear()
        {
            Stop();
            _frames.Clear();
        }
    }
}
