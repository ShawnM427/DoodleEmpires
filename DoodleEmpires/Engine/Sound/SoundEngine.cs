using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Sound
{
    /// <summary>
    /// Represents the sound manager
    /// </summary>
    public class SoundEngine
    {
        Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        AudioListener _listener;
        Vector2 _listenerPosition = Vector2.Zero;
        float _listenRadius = 500.0f;

        /// <summary>
        /// Gets or sets the position of the listener
        /// </summary>
        public Vector2 ListenerPosition
        {
            get { return _listenerPosition; }
            set { _listenerPosition = value; }
        }

        /// <summary>
        /// Creates a new instance of a sound engine
        /// </summary>
        public SoundEngine()
        {
            _listener = new AudioListener();
            _listener.Position = new Vector3(0);
        }

        /// <summary>
        /// Adds a sound effect
        /// </summary>
        /// <param name="name">The name of the instance</param>
        /// <param name="effect">The sound effect to add</param>
        public void AddSound(string name, SoundEffect effect)
        {
            _sounds.Add(name, effect);
        }

        /// <summary>
        /// Plays a sound with the given name
        /// </summary>
        /// <param name="name">The name of the sound</param>
        /// <param name="position">The position to play the sound at</param>
        public void PlaySound(string name, Vector2 position) 
        {
            SoundEffectInstance instance = _sounds[name].CreateInstance();

            if (SetSoundForCamera(instance, position, 0.5f))
                instance.Play();
        }

        /// <summary>
        /// Sets the sound effect instance to apply to the camera
        /// </summary>
        /// <param name="sound">The sound effect instance to modify</param>
        /// <param name="position">The position to play the sound from</param>
        /// <param name="baseVolume">The base volume of the sound</param>
        /// <returns>True if the sound needs to be played</returns>
        private bool SetSoundForCamera(SoundEffectInstance sound, Vector2 position, float baseVolume)
        {
            Vector2 screenDistance = (position - _listenerPosition) / _listenRadius;
            float fade = MathHelper.Clamp(2f - screenDistance.Length(), 0, 1);
            sound.Volume = fade * fade * baseVolume;
            sound.Pan = MathHelper.Clamp(screenDistance.X, -1, 1);
            return fade > 0;
        }
    }
}
