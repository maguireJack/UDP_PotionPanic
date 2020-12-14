using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Managers;
using GDLibrary.GameComponents;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Audio;
using GDLibrary.Containers;

namespace GDGame.MyGame.Managers
{
    
    class MySoundManager : SoundManager
    {
        private ContentDictionary<SoundEffect> soundDictionary;
        private SoundEffect effect;
        private SoundEffectInstance pingSoundEffect;
        private SoundEffectInstance menuSoundEffect;
        private SoundEffectInstance pestolSoundEffect;
        private SoundEffectInstance footStepsSoundEffect;
        private SoundEffectInstance grabbingSoundEffect;
        


        public MySoundManager(Game game, StatusType statusType)
            : base(game, statusType)
        {
            soundDictionary = new ContentDictionary<SoundEffect>("music", Game.Content);
            soundDictionary.Load("Assets/Music/main_menu");
            soundDictionary.Load("Assets/Music/ping");
            soundDictionary.Load("Assets/Music/pestol");
            soundDictionary.Load("Assets/Music/walking");
            soundDictionary.Load("Assets/Music/grabbing");

            effect = soundDictionary["ping"];
            pingSoundEffect = effect.CreateInstance();

            effect = soundDictionary["main_menu"];
            menuSoundEffect = effect.CreateInstance();

            effect = soundDictionary["pestol"];
            pestolSoundEffect = effect.CreateInstance();

            effect = soundDictionary["walking"];
            footStepsSoundEffect = effect.CreateInstance();

            effect = soundDictionary["grabbing"];
            grabbingSoundEffect = effect.CreateInstance();

            EventDispatcher.Subscribe(EventCategoryType.Sound, HandleEvent);
            
        }
        protected override void HandleEvent(EventData eventData)
        {
            

            switch (eventData.EventActionType)
            {
                case EventActionType.OnPlay:
                    switch (eventData.Parameters[0] as string)
                    {
                        case "ping":
                            pingSoundEffect.Volume = .05f;
                            pingSoundEffect.IsLooped = false;
                            pingSoundEffect.Play();
                            break;
                        case "main_menu":
                            menuSoundEffect.IsLooped = true;
                            menuSoundEffect.Volume = .1f;
                            menuSoundEffect.Play();
                            break;
                        case "pestol":
                            pestolSoundEffect.IsLooped = true;
                            pestolSoundEffect.Volume = .05f;
                            pestolSoundEffect.Play();
                            break;
                        case "walking":
                            footStepsSoundEffect.IsLooped = true;
                            footStepsSoundEffect.Volume = 0.4f;
                            footStepsSoundEffect.Play();
                            break;
                        case "grabbing":
                            grabbingSoundEffect.IsLooped = false;
                            grabbingSoundEffect.Volume = 0.2f;
                            grabbingSoundEffect.Play();
                            break;
                    }
                    break;

                case EventActionType.OnPause:
                    switch (eventData.Parameters[0] as string)
                    {
                        case "ping":
                            pingSoundEffect.Pause();
                            break;
                        case "main_menu":
                            menuSoundEffect.Pause();
                            break;
                        case "pestol":
                            pestolSoundEffect.Pause();
                            break;
                        case "walking":
                            footStepsSoundEffect.Pause();
                            break;
                        case "grabbing":
                            grabbingSoundEffect.Pause();
                            break;
                    }
                    break;

            }

        }

        public override void Update(GameTime gameTime)
        {
            
        }

        
    }
}
