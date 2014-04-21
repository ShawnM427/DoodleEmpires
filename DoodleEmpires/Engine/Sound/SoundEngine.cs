using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Sound
{
    public class SoundEngine
    {
        Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        AudioListener _listener;
        Vector2 _listenerPosition = Vector2.Zero;
        float _listenRadius = 500.0f;

        public Vector2 ListenerPosition
        {
            get { return _listenerPosition; }
            set { _listenerPosition = value; }
        }

        public SoundEngine()
        {
            _listener = new AudioListener();
            _listener.Position = new Vector3(0);
        }

        public void AddSound(string name, SoundEffect effect)
        {
            _sounds.Add(name, effect);
        }

        public void PlaySound(string name, Vector2 position) 
        {
            SoundEffectInstance instance = _sounds[name].CreateInstance();
            if (SetSoundForCamera(instance, position, 0.5f))
                instance.Play();
        }

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
