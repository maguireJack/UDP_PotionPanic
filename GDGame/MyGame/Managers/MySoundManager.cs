using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GDLibrary.Containers;
using System.Collections.Generic;

namespace GDGame.MyGame.Managers
{
    
    class MySoundManager : SoundManager
    {
        private ContentDictionary<SoundEffect> soundDictionary;
        private Dictionary<string, SoundEffectInstance> soundInstances;

        public MySoundManager(Game game, StatusType statusType)
            : base(game, statusType)
        {
            soundInstances = new Dictionary<string, SoundEffectInstance>();
            soundDictionary = new ContentDictionary<SoundEffect>("music", Game.Content);

            soundDictionary.Load("Assets/Music/main_menu");
            soundDictionary.Load("Assets/Music/game_beat");
            soundDictionary.Load("Assets/Music/ping");
            soundDictionary.Load("Assets/Music/pestol");
            soundDictionary.Load("Assets/Music/walking");
            soundDictionary.Load("Assets/Music/grabbing");

            SoundEffectInstance sound = soundDictionary["main_menu"].CreateInstance();
            sound.IsLooped = true;
            sound.Volume = .1f;
            soundInstances.Add("main_menu", sound);
            sound.Play();

            sound = soundDictionary["game_beat"].CreateInstance();
            sound.IsLooped = true;
            sound.Volume = .08f;
            soundInstances.Add("game_beat", sound);

            sound = soundDictionary["ping"].CreateInstance();
            sound.Volume = .05f;
            sound.IsLooped = false;
            soundInstances.Add("ping", sound);

            sound = soundDictionary["pestol"].CreateInstance();
            sound.IsLooped = true;
            sound.Volume = .05f;
            soundInstances.Add("pestol", sound);

            sound = soundDictionary["grabbing"].CreateInstance();
            sound.IsLooped = false;
            sound.Volume = 0.2f;
            soundInstances.Add("grabbing", sound);

            sound = soundDictionary["walking"].CreateInstance();
            sound.IsLooped = true;
            sound.Volume = 0.4f;
            soundInstances.Add("walking", sound);


            EventDispatcher.Subscribe(EventCategoryType.Sound, HandleEvent);
        }

        protected override void HandleEvent(EventData eventData)
        {

            if (eventData.EventActionType == EventActionType.OnPlay)
            {
                soundInstances[eventData.Parameters[0] as string].Play();
            }
            else if (eventData.EventActionType == EventActionType.OnPause)
            {
                soundInstances[eventData.Parameters[0] as string].Pause();
            }
            else if (eventData.EventActionType == EventActionType.OnRestart)
            {
                string id = eventData.Parameters[0] as string;
                Reset(id);
                soundInstances[id].Play();
            }
            else if(eventData.EventActionType == EventActionType.OnPitchSet)
            {
                string id = eventData.Parameters[0] as string;
                float pitch = (float)eventData.Parameters[1];
                soundInstances[id].Pitch = pitch;
            }
        }

        private void Reset(string id)
        {
            soundInstances[id].Stop();

            SoundEffectInstance sound = soundDictionary[id].CreateInstance();
            sound.Volume = soundInstances[id].Volume;
            sound.IsLooped = soundInstances[id].IsLooped;
            soundInstances[id] = sound;
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
