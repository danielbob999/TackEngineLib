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
        private int m_AudioSourceId;
        private AudioClip m_AudioClip;
        private bool m_LoopAudio;

        /// <summary>
        /// Gets the ID of the audio source attached to this component
        /// </summary>
        public int AudioSourceId
        {
            get { return m_AudioSourceId; }
        }

        /// <summary>
        /// Gets/sets the AudioClip if this component
        /// </summary>
        public AudioClip AudioClip
        {
            get { return m_AudioClip; }
            set { m_AudioClip = value; }
        }


        /// <summary>
        /// Gets/Sets whether the AudioClip should loop
        /// </summary>
        public bool LoopAudio
        {
            get { return m_LoopAudio; }
            set { m_LoopAudio = value; }
        }

        public AudioComponent()
        {
            m_AudioSourceId = AL.GenSource();
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
            AL.Source(m_AudioSourceId, ALSourcei.Buffer, m_AudioClip.AudioId);
            AL.Source(m_AudioSourceId, ALSourceb.Looping, false);

            AL.SourcePlay(m_AudioSourceId);
        }
    }
}
