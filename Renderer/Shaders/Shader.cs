using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using TackEngineLib.Main;

namespace TackEngineLib.Renderer.Shaders {
    public class Shader {
        public int Id { get; internal set; }
        internal bool CompiledAndLinked { get; set; }

        public Shader(string vertexSoure, string fragmentSource) {
            // Create shader program
            int shaderProgram = GL.CreateProgram();

            // Generate subshader ids
            int vertShaderId = CompileSubShader(vertexSoure, ShaderType.VertexShader);
            int fragShaderId = CompileSubShader(fragmentSource, ShaderType.FragmentShader);

            if (vertShaderId == -1 || fragShaderId == -1) {
                CompiledAndLinked = false;
                Id = -1;
                return;
            }

            // Link shaders to the shader program
            GL.AttachShader(shaderProgram, vertShaderId);
            GL.AttachShader(shaderProgram, fragShaderId);
            GL.LinkProgram(shaderProgram);

            GL.GetProgramInfoLog(shaderProgram, out string progLogStr);

            if (progLogStr != "") {
                TackConsole.EngineLog(Engine.EngineLogType.Error, progLogStr);
                Id = -1;
                CompiledAndLinked = false;
                return;
            }
                
            TackConsole.EngineLog(Engine.EngineLogType.Message, "Successfully created shader program");

            GL.DeleteShader(vertShaderId);
            GL.DeleteShader(fragShaderId);

            Id = shaderProgram;
            CompiledAndLinked = true;
        }

        private int CompileSubShader(string source, ShaderType type) {
            if (type != ShaderType.VertexShader && type != ShaderType.FragmentShader) {
                TackConsole.EngineLog(Engine.EngineLogType.Error, "Cannot compile sub shader of unknown type.");
                return -1;
            }

            // Compile shader
            int subShaderId = GL.CreateShader(ShaderType.VertexShader);

            // Set the shader source
            GL.ShaderSource(subShaderId, source);

            // Compile shader
            GL.CompileShader(subShaderId);

            GL.GetShaderInfoLog(subShaderId, out string logStr);

            if (logStr != "") {
                TackConsole.EngineLog(Engine.EngineLogType.Error, logStr);
                return -1;
            }

            TackConsole.EngineLog(Engine.EngineLogType.Message, "Successfully complied vertex shader. " + source.Count(x => x == '\n') + " lines.");
            return subShaderId;
        }

        public List<Tuple<string, ActiveUniformType>> GetShaderUniformVars() {
            List<Tuple<string, ActiveUniformType>> vars = new List<Tuple<string, ActiveUniformType>>();

            // Get count of uniforms
            int uniformCount;

            // for loop that iterates through 0-count, getting the uniform name/type at pos i

            // add tuple (name, type) to list

            return vars;
        }

        internal void SetUniformValue(string name, int value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal void SetUniformValue(string name, double value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal void SetUniformValue(string name, float value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal void SetUniformValue(string name, uint value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal void Use() {
            GL.UseProgram(Id);
        }
    }
}
