using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Audio.OpenAL;

using TackEngineLib.Audio;
using TackEngineLib.Main;
using TackEngineLib.Engine;

namespace TackEngineLib.Objects.Components
{
    public class AudioComponent : TackComponent
    {
        private int mAudioSourceId;
        private AudioClip mAudioClip;
        private bool mLoopAudio;

        /// <summary>
        /// Gets the ID of the audio source attached to this component
        /// </summary>
        public int AudioSourceId
        {
            get { return mAudioSourceId; }
        }

        /// <summary>
        /// Gets/sets the AudioClip if this component
        /// </summary>
        public AudioClip AudioClip
        {
            get { return mAudioClip; }
            set { mAudioClip = value; }
        }


        /// <summary>
        /// Gets/Sets whether the AudioClip should loop
        /// </summary>
        public bool LoopAudio
        {
            get { return mLoopAudio; }
            set { mLoopAudio = value; }
        }

        public AudioComponent()
        {
            mAudioSourceId = AL.GenSource();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnRender()
        {
            base.OnRender();
        }

        /// <summary>
        /// Plays the active AudioClip
        /// </summary>
        public void Play()
        {
            AL.Source(mAudioSourceId, ALSourcei.Buffer, mAudioClip.AudioId);
            AL.Source(mAudioSourceId, ALSourceb.Looping, false);

            AL.SourcePlay(mAudioSourceId);
        }
    }
}
