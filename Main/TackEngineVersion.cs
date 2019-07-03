/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public class TackEngineVersion
    {
        private int mMajor;
        private int mMinor;
        private int mPatch;
        private string mDesc;
        private int mBuildNumber;

        // Properties

        /// <summary>
        /// The major number of this TackEngineVersion. E.g [1].2.3 FullRelease
        /// </summary>
        public int Major
        {
            get { return mMajor; }
        }

        /// <summary>
        /// The minor number of this TackEngineVersion E.g 1.[2].3 FullRelease
        /// </summary>
        public int Minor
        {
            get { return mMinor; }
        }

        /// <summary>
        /// The patch number of this TackEngineVersion. E.g 1.2.[3] FullRelease
        /// </summary>
        public int Patch
        {
            get { return mPatch; }
        }

        /// <summary>
        /// The string description of this TackEngineVersion. E.g 1.2.3 [FullRelease]
        /// </summary>
        public string Desc
        {
            get { return mDesc; }
        }

        public int BuildNumber
        {
            get { return mBuildNumber; }
        }

        internal TackEngineVersion(int _major, int _minor, int _patch, string _desc, int _buildn)
        {
            mMajor = _major;
            mMinor = _minor;
            mPatch = _patch;
            mDesc = _desc;
            mBuildNumber = _buildn;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2} (#{3})", mMajor, mMinor, mPatch, mBuildNumber);
        }

    }
}
