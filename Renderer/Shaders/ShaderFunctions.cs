using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Engine;
using OpenTK.Graphics.OpenGL;

namespace TackEngineLib.Renderer.Shaders
{
    public static class ShaderFunctions
    {
        public static string m_ColourVertexShader = @"#version 150 core
            in vec2 position;
            in vec3 color;
            out vec3 Color;
            void main()
            {
                Color = color;
                gl_Position = vec4(position, 0.0, 1.0);
            }
        ";

        public static string m_ColourFragmentShader = @"#version 150 core
            in vec3 Color;
            out vec4 outColor;
            void main()
            {
                outColor = vec4(Color, 1.0);
            }
        ";

        public static string m_ImageVertexShader = @"#version 150 core
            in vec2 position;
            in vec3 color;
            in vec2 texcoord;
            out vec3 Color;
            out vec2 Texcoord;
            void main()
            {
                Color = color;
                Texcoord = texcoord;
                gl_Position = vec4(position, 0.0, 1.0);
            }
        ";

        public static string m_ImageFragmentShader = @"#version 150 core
            in vec3 Color;
            in vec2 Texcoord;

            out vec4 outColor;

            uniform sampler2D tex;

            void main()
            {
                outColor = texture(tex, Texcoord) * vec4(Color, 1.0);
            }
        ";

        public static int CompileAndLinkShaders()
        {
            try
            {
                // Compile vertex shader
                int vertShader = GL.CreateShader(ShaderType.VertexShader);

                // GET SOURCE FROM VERTSHADER FILE

                GL.CompileShader(vertShader);

                GL.GetShaderInfoLog(vertShader, out string vertLogStr);

                if (vertLogStr != "")
                    TackConsole.EngineLog(EngineLogType.Error, vertLogStr);
                else
                    TackConsole.EngineLog(EngineLogType.Message, "Successfully complied vertex shader");

                // Compile fragment shader
                int fragShader = GL.CreateShader(ShaderType.FragmentShader);

                // GET SOURCE FROM FRAGSHADER FILE

                GL.CompileShader(fragShader);

                GL.GetShaderInfoLog(fragShader, out string fragLogStr);

                if (fragLogStr != "")
                    TackConsole.EngineLog(EngineLogType.Error, fragLogStr);
                else
                    TackConsole.EngineLog(EngineLogType.Message, "Successfully compiled fragment shader");

                // Link vertex and fragment shaders
                int shaderProgram = GL.CreateProgram();
                GL.AttachShader(shaderProgram, vertShader);
                GL.AttachShader(shaderProgram, fragShader);
                GL.LinkProgram(shaderProgram);

                GL.GetProgramInfoLog(shaderProgram, out string progLogStr);

                if (progLogStr != "")
                    TackConsole.EngineLog(EngineLogType.Error, progLogStr);
                else
                    TackConsole.EngineLog(EngineLogType.Message, "Successfully created shader program");

                GL.DeleteShader(vertShader);
                GL.DeleteShader(fragShader);

                return shaderProgram;
            }
            catch (Exception e)
            {
                TackConsole.EngineLog(EngineLogType.Error, e.ToString());
                return -1;
            }
        }
    }
}
