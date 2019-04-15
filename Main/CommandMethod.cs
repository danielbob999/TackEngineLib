using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    internal class CommandMethod : Attribute
    {
        private List<string> m_ArgList;

        public CommandMethod(string a_defaultArg, params string[] a_argList)
        {
            m_ArgList = new List<string>();
            m_ArgList.Add(a_defaultArg);
            m_ArgList.AddRange(a_argList);
        }

        public string[] GetArgList()
        {
            return m_ArgList.ToArray();
        }
    }
}
