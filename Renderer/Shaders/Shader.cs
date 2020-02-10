using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using TackEngineLib.Main;

namespace TackEngineLib.Renderer.Shaders {
    public class Shader {
        public int Id { get; private set; }
        public string Name { get; }
        public TackShaderType Type { get; private set; }
        internal bool CompiledAndLinked { get; private set; }
        public bool SupportsBatchRendering { get; private set; }
        public Dictionary<string, ActiveUniformType> Uniforms { get; private set; }

        public Shader(string shaderName, TackShaderType type, string vertexSoure, string fragmentSource) {
            Name = shaderName;
            Type = type;

            TackConsole.EngineLog(Engine.EngineLogType.Message, "Starting compilation and linking of shader with name: " + Name);

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

            Uniforms = GetShaderUniformVars();

            TackConsole.EngineLog(Engine.EngineLogType.Message, "Successfully created shader program with Id: {0}. Name: \"{1}\". SupportsBatchRendering: {2}", Id, Name, SupportsBatchRendering);

            GL.DeleteShader(vertShaderId);
            GL.DeleteShader(fragShaderId);

            Id = shaderProgram;
            CompiledAndLinked = true;
        }

        private int CompileSubShader(string source, ShaderType type) {
            if (type != ShaderType.VertexShader && type != ShaderType.FragmentShader) {
                TackConsole.EngineLog(Engine.EngineLogType.Error, "Cannot compile sub-shader of unknown type.");
                return -1;
            }

            // Compile shader
            int subShaderId = GL.CreateShader(type);

            // Set the shader source
            GL.ShaderSource(subShaderId, source);

            // Compile shader
            GL.CompileShader(subShaderId);

            GL.GetShaderInfoLog(subShaderId, out string logStr);

            if (logStr != "") {
                TackConsole.EngineLog(Engine.EngineLogType.Error, logStr);
                return -1;
            }

            TackConsole.EngineLog(Engine.EngineLogType.Message, "Successfully complied sub-shader. Type: {0}, Lines: {1}.", type.ToString(), source.Count(x => x == '\n'));
            return subShaderId;
        }

        public void Destroy() {
            GL.DeleteProgram(Id);
        }

        private Dictionary<string, ActiveUniformType> GetShaderUniformVars() {
            Dictionary<string, ActiveUniformType> vars = new Dictionary<string, ActiveUniformType>();

            // Get count of uniforms
            int uniformCount = 0;

            GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out uniformCount);

            SupportsBatchRendering = false;

            // for loop that iterates through 0-count, getting the uniform name/type at pos i
            for (int i = 0; i < uniformCount; i++) {
                string uniformName = GL.GetActiveUniform(Id, i, out int size, out ActiveUniformType type);

                if (type == ActiveUniformType.Sampler2D || uniformName == "b_texture") {
                    SupportsBatchRendering = true;
                }

                vars.Add(uniformName, type);
            }

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
