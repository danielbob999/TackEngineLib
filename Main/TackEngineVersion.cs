using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    public class TackEngineVersion
    {
        private int m_Major;
        private int m_Minor;
        private int m_Patch;
        private string m_Desc;

        // Properties

        /// <summary>
        /// The major number of this TackEngineVersion. E.g [1].2.3 FullRelease
        /// </summary>
        public int Major
        {
            get { return m_Major; }
        }

        /// <summary>
        /// The minor number of this TackEngineVersion E.g 1.[2].3 FullRelease
        /// </summary>
        public int Minor
        {
            get { return m_Minor; }
        }

        /// <summary>
        /// The patch number of this TackEngineVersion. E.g 1.2.[3] FullRelease
        /// </summary>
        public int Patch
        {
            get { return m_Patch; }
        }

        /// <summary>
        /// The string description of this TackEngineVersion. E.g 1.2.3 [FullRelease]
        /// </summary>
        public string Desc
        {
            get { return m_Desc; }
        }

        internal TackEngineVersion(int _major, int _minor, int _patch, string _desc)
        {
            m_Major = _major;
            m_Minor = _minor;
            m_Patch = _patch;
            m_Desc = _desc;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", m_Major, m_Minor, m_Patch);
        }
    }
}
