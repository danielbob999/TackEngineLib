/* Copyright (c) 2019 Daniel Phillip Robinson */
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
        private int mAudioId;

        // Audio clip data
        private int mChannelNum;
        private int mBitsPerSample;
        private int mSampleNum;
        private byte[] mAudioData;

        /// <summary>
        /// The ID generated from AL.GenBuffers
        /// </summary>
        public int AudioId
        {
            get { return mAudioId; }
        }

        /// <summary>
        /// The number if channels this AudioClip has
        /// </summary>
        public int Channels
        {
            get { return mChannelNum; }
        }

        /// <summary>
        /// The number if bits per sample this AudioClip has
        /// </summary>
        public int BitsPerSample
        {
            get { return mBitsPerSample; }
        }

        /// <summary>
        /// The number of samples this AudioClip has
        /// </summary>
        public int SampleNum
        {
            get { return mSampleNum; }
        }

        /// <summary>
        /// The data of this AudioClip stored in a byte[] format
        /// </summary>
        public byte[] AudioData
        {
            get { return mAudioData; }
        }

        internal AudioClip(int _chNum, int _bitsPerSam, int _samNum, byte[] _data)
        {
            mChannelNum = _chNum;
            mBitsPerSample = _bitsPerSam;
            mSampleNum = _samNum;
            mAudioData = _data;
        }

        /// <summary>
        /// Loads the audio data into memory
        /// </summary>
        public void Create()
        {
            mAudioId = AL.GenBuffer();
            AL.BufferData(mAudioId, AudioManager.GetSoundFormat(mChannelNum, mBitsPerSample), mAudioData, mAudioData.Length, mSampleNum);

            TackConsole.EngineLog(EngineLogType.Message, string.Format("Loaded and new AudioClip into memory. AudioId={0}", mAudioId));

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
                int numchannels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                tmp_channels= numchannels;
                tmp_bps = bits_per_sample;
                tmp_sampleNum = sample_rate;

                tmp_data = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            return new AudioClip(tmp_channels, tmp_bps, tmp_sampleNum, tmp_data);
        }
    }
}
