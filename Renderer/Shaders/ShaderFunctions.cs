/* Copyright (c) 2019 Daniel Phillip Robinson */
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
        public static int CompileAndLinkShaders(string _vertSource, string _fragSource)
        {
            try
            {
                // Compile vertex shader
                int vertShader = GL.CreateShader(ShaderType.VertexShader);

                // Set the shader source
                GL.ShaderSource(vertShader, _vertSource);

                // Compile shader
                GL.CompileShader(vertShader);

                GL.GetShaderInfoLog(vertShader, out string vertLogStr);

                if (vertLogStr != "")
                    TackConsole.EngineLog(EngineLogType.Error, vertLogStr);
                else
                    TackConsole.EngineLog(EngineLogType.Message, "Successfully complied vertex shader");
                


                // Create fragment shader
                int fragShader = GL.CreateShader(ShaderType.FragmentShader);

                // Set the shader source
                GL.ShaderSource(fragShader, _fragSource);

                // Compile the shader
                GL.CompileShader(fragShader);

                GL.GetShaderInfoLog(fragShader, out string fragLogStr);

                if (fragLogStr != "")
                    TackConsole.EngineLog(EngineLogType.Error, fragLogStr);
                else
                    TackConsole.EngineLog(EngineLogType.Message, "Successfully compiled fragment shader");



                // Create shader program
                int shaderProgram = GL.CreateProgram();

                // Link shaders to the shader program
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
