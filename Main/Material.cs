using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using TackEngineLib.Renderer.Shaders;

namespace TackEngineLib.Main {
    public class Material {
        public Shader Shader { get; internal set; }
        private Dictionary<Tuple<string, ActiveUniformType>, object> m_uniformVariables;

        public Material(Shader shader) {
            m_uniformVariables = new Dictionary<Tuple<string, ActiveUniformType>, object>();

            Shader = shader;

            RefreshShaderUniformVars();
        }

        /// <summary>
        /// Gets the uniform variable names/types from the shader and populates the local dictionary
        /// </summary>
        private void RefreshShaderUniformVars() {
        }

        public void SetVariableValue<T>(string varName, T varValue) {
            foreach (Tuple<string, ActiveUniformType> t in m_uniformVariables.Keys) {
                if (t.Item1 == varName) {
                    m_uniformVariables[t] = varValue;
                }
            }
        }

        /// <summary>
        /// Returns the type of a shader uniform variable. Will return 0 if shader variable doesn't exist
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public ActiveUniformType GetVariableType(string varName) {
            foreach (Tuple<string, ActiveUniformType> t in m_uniformVariables.Keys) {
                if (t.Item1 == varName) {
                    return t.Item2;
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets a list of all the uniform variables that the shader uses
        /// </summary>
        /// <returns></returns>
        public List<string> GetVariableNames() {
            List<string> names = new List<string>();

            foreach (Tuple<string, ActiveUniformType> t in m_uniformVariables.Keys) {
                names.Add(t.Item1);
            }

            return names;
        }

        /// <summary>
        /// Tells the renderer to use the attached shader and sets all uniform variables for the shader
        /// </summary>
        internal void MakeMaterialActive() {
            foreach (Tuple<string, ActiveUniformType> t in m_uniformVariables.Keys) {
                if (t.Item2 == ActiveUniformType.UnsignedInt) {
                    Shader.SetUniformValue(t.Item1, (uint)m_uniformVariables[t]);
                } else if (t.Item2 == ActiveUniformType.Int) {
                    Shader.SetUniformValue(t.Item1, (int)m_uniformVariables[t]);
                } else if (t.Item2 == ActiveUniformType.Float) {
                    Shader.SetUniformValue(t.Item1, (float)m_uniformVariables[t]);
                } else if (t.Item2 == ActiveUniformType.Double) {
                    Shader.SetUniformValue(t.Item1, (double)m_uniformVariables[t]);
                } else {

                }
            }
        }
    }
}
