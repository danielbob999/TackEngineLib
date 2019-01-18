using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using OpenTK.Audio.OpenAL;

using TackEngineLib.Main;
using TackEngineLib.Engine;

namespace TackEngineLib.Audio
{
    public class AudioClip
    {
        private int m_AudioId;

        // Audio clip data
        private int m_ChannelNum;
        private int m_BitsPerSample;
        private int m_SampleNum;
        private byte[] m_AudioData;

        /// <summary>
        /// The ID generated from AL.GenBuffers
        /// </summary>
        public int AudioId
        {
            get { return m_AudioId; }
        }

        /// <summary>
        /// The number if channels this AudioClip has
        /// </summary>
        public int Channels
        {
            get { return m_ChannelNum; }
        }

        /// <summary>
        /// The number if bits per sample this AudioClip has
        /// </summary>
        public int BitsPerSample
        {
            get { return m_BitsPerSample; }
        }

        /// <summary>
        /// The number of samples this AudioClip has
        /// </summary>
        public int SampleNum
        {
            get { return m_SampleNum; }
        }

        /// <summary>
        /// The data of this AudioClip stored in a byte[] format
        /// </summary>
        public byte[] AudioData
        {
            get { return m_AudioData; }
        }

        internal AudioClip(int _chNum, int _bitsPerSam, int _samNum, byte[] _data)
        {
            m_ChannelNum = _chNum;
            m_BitsPerSample = _bitsPerSam;
            m_SampleNum = _samNum;
            m_AudioData = _data;
        }

        /// <summary>
        /// Loads the audio data into memory
        /// </summary>
        public void Create()
        {
            m_AudioId = AL.GenBuffer();
            AL.BufferData(m_AudioId, AudioManager.GetSoundFormat(m_ChannelNum, m_BitsPerSample), m_AudioData, m_AudioData.Length, m_SampleNum);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Loaded and new AudioClip into memory. AudioId={0}", m_AudioId));

            AudioManager.AddAudioClip(this);
        }

        /// <summary>
        /// Removes AudioClip data from memory
        /// </summary>
        public void Destory()
        {
            AudioManager.RemoveAudioClip(this);
        }

        /// <summary>
        /// Loads a piece of audio from the main disk
        /// </summary>
        /// <param name="_path">The location of the audio</param>
        /// <returns></returns>
        public static AudioClip LoadFromFile(string _path)
        {
            if (!File.Exists(_path))
            {
                TackConsole.EngineLog(EngineLogType.Error, "Audio file at path '{0}' does not exist", _path);
                return new AudioClip(0, 0, 0, new byte[] { });
            }

            int tmp_channels, tmp_bps, tmp_sampleNum;
            byte[] tmp_data;

            using (BinaryReader reader = new BinaryReader(File.Open(_path, FileMode.Open)))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                tmp_channels= num_channels;
                tmp_bps = bits_per_sample;
                tmp_sampleNum = sample_rate;

                tmp_data = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            return new AudioClip(tmp_channels, tmp_bps, tmp_sampleNum, tmp_data);
        }
    }
}
