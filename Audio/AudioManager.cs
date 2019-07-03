using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using OpenTK.Audio.OpenAL;
using OpenTK.Audio;

using TackEngineLib.Main;
using TackEngineLib.Engine;

namespace TackEngineLib.Audio
{
    public class AudioManager
    {
        public static AudioManager ActiveInstance;
        public static int WorkerThreadTargetRefreshRate = 60;

        private List<AudioClip> mAudioClips = new List<AudioClip>();
        private AudioContext mAudioContext;

        public AudioManager()
        {
            ActiveInstance = this;
            WorkerThreadTargetRefreshRate = 60;
        }

        public void OnStart()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            mAudioContext = new AudioContext();

            TackConsole.EngineLog(EngineLogType.ModuleStart, "", timer.ElapsedMilliseconds);
            timer.Stop();
        }

        public void OnUpdate()
        {

        }

        public void OnClose()
        {
            foreach (AudioClip clip in mAudioClips)
            {
                int id = clip.AudioId;
                AL.DeleteBuffer(clip.AudioId);
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Deleted AudioClip from AudioManager. AudioId={0}", id));
            }

            mAudioClips.Clear();

            if (mAudioClips.Count == 0)
            {
                TackConsole.EngineLog(EngineLogType.Message, "Successfully removed all AudioClips from AudioManager");
            }

            TackConsole.EngineLog(EngineLogType.Message, "Closing this instance of AudioManager");

            mAudioContext.Dispose();
            mAudioContext = null;
        }

        /// <summary>
        /// Adds and AudioClip to the AudioManager
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_debugMsgs"></param>
        internal static void AddAudioClip(AudioClip _clip, bool _debugMsgs = true)
        {
            if (ActiveInstance.mAudioClips.Contains(_clip))
            {
                if (_debugMsgs)
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("AudioManager cannot add AudioClip beause it is already in the list. AudioId=" + _clip.AudioId.ToString()));
                return;
            }

            ActiveInstance.mAudioClips.Add(_clip);
            if (_debugMsgs)
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Added AudioClip to AudioManager. AudioId={0}", _clip.AudioId));
        }

        /// <summary>
        /// Removes a clip from the AudioManager
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_debugMsgs"></param>
        internal static void RemoveAudioClip(AudioClip _clip, bool _debugMsgs = true)
        {
            if (!ActiveInstance.mAudioClips.Contains(_clip))
            {
                if (_debugMsgs)
                    TackConsole.EngineLog(EngineLogType.Error, string.Format("Trying to remove AudioClip with id '{0}' but it doesn't exist in AudioManager", _clip.AudioId));
                return;
            }

            AL.DeleteBuffer(_clip.AudioId);
            ActiveInstance.mAudioClips.Remove(_clip);
            if (_debugMsgs)
                TackConsole.EngineLog(EngineLogType.Message, string.Format("Removed AudioClip with id '{0}' from AudioManager", _clip.AudioId));
        }

        /// <summary>
        /// Gets the sound format from the specified channels and bits. Will return ALFormat.Mono8 as default
        /// </summary>
        /// <param name="channels">The number of channels</param>
        /// <param name="bits">The number of bits</param>
        /// <returns></returns>
        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1:
                    return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2:
                    return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default:
                    TackConsole.EngineLog(EngineLogType.Error, "The specified sound format is not supported.");
                    return ALFormat.Mono8;
            }
        }
    }
}
