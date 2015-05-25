﻿using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bonsai.Shaders
{
    public class Shader : IDisposable
    {
        int program;
        int texture;
        int timeLocation;
        TexturedQuad quad;
        string vertexSource;
        string fragmentSource;
        event Action update;
        IGameWindow shaderWindow;
        double time;

        internal Shader(string name, IGameWindow window, string vertexShader, string fragmentShader)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            if (vertexShader == null)
            {
                throw new ArgumentNullException("vertexShader");
            }

            if (fragmentShader == null)
            {
                throw new ArgumentNullException("fragmentShader");
            }

            Name = name;
            shaderWindow = window;
            vertexSource = vertexShader;
            fragmentSource = fragmentShader;
        }

        public bool Enabled { get; set; }

        public string Name { get; private set; }

        public int Program
        {
            get { return program; }
        }

        public int Texture
        {
            get { return texture; }
        }

        public void Update(Action action)
        {
            update += action;
        }

        int CreateShader()
        {
            int status;
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                var message = string.Format(
                    "Failed to compile vertex shader.\nShader name: {0}\n{1}",
                    Name,
                    GL.GetShaderInfoLog(vertexShader));
                throw new ShaderException(message);
            }

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                var message = string.Format(
                    "Failed to compile fragment shader.\nShader name: {0}\n{1}",
                    Name,
                    GL.GetShaderInfoLog(fragmentShader));
                throw new ShaderException(message);
            }

            var shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                var message = string.Format(
                    "Failed to link shader program.\nShader name: {0}\n{1}",
                    Name,
                    GL.GetProgramInfoLog(shaderProgram));
                throw new ShaderException(message);
            }

            return shaderProgram;
        }

        public void Load()
        {
            time = 0;
            texture = GL.GenTexture();
            quad = new TexturedQuad();
            program = CreateShader();
            timeLocation = GL.GetUniformLocation(program, "time");
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void RenderFrame(FrameEventArgs e)
        {
            time += e.Time;
            if (Enabled)
            {
                GL.UseProgram(program);
                GL.BindTexture(TextureTarget.Texture2D, texture);
                if (timeLocation >= 0)
                {
                    GL.Uniform1(timeLocation, (float)time);
                }

                var action = Interlocked.Exchange(ref update, null);
                if (action != null)
                {
                    action();
                }

                quad.Draw();
            }
        }

        public void Dispose()
        {
            if (quad != null)
            {
                quad.Dispose();
                GL.DeleteTextures(1, ref texture);
                quad = null;
                update = null;
            }
        }
    }
}