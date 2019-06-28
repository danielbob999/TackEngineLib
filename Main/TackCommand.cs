using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Engine;

namespace TackEngineLib.Main
{
    internal class TackCommand
    {
        private string m_CommandCallString;
        private EngineDelegates.CommandDelegate m_CommandDelegate;
        private List<string> m_CommandArgList = new List<string>();

        /// <summary>
        /// The string used to call the command
        /// </summary>
        public string CommandCallString
        {
            get { return m_CommandCallString; }
        }

        /// <summary>
        /// The delegate called when this command is run
        /// </summary>
        public EngineDelegates.CommandDelegate CommandDelegate
        {
            get { return m_CommandDelegate; }
        }

        /// <summary>
        /// The list of arg combinations used with this command. If arg option is "", no argument is required
        /// </summary>
        public List<string> CommandArgList
        {
            get { return m_CommandArgList; }
        }

        public TackCommand(string a_callName, EngineDelegates.CommandDelegate a_delegate, List<string> a_argList)
        {
            m_CommandCallString = a_callName;
            m_CommandDelegate = a_delegate;
            m_CommandArgList = a_argList;
        }
    }
}
