using Dear_ImGui_Sample;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;
using Vector3 = System.Numerics.Vector3;

namespace PolygonalLightShading
{
    public class Program : GameWindow
    {
        private ImGuiController imGuiController;
        private Shader sphereShader;
        private Camera camera;
        private SpheresCloud cloud;
        private Vector3 LightPosition = new(0, 150, 0);

        public static void Main(string[] args)
        {
            using Program program = new Program(GameWindowSettings.Default, NativeWindowSettings.Default);
            program.Title = "Spheres Demo";
            program.Size = new Vector2i(1280, 768);
            program.Run();
        }

        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            sphereShader = new Shader(
                ("sphere.vert", ShaderType.VertexShader), 
                ("sphere.geom", ShaderType.GeometryShader),
                ("sphere.frag", ShaderType.FragmentShader));
            camera = new PerspectiveCamera();
            camera.UpdateVectors();
            camera.Speed = 100;
            imGuiController = new ImGuiController(Size.X, Size.Y);
            cloud = new SpheresCloud();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            sphereShader.Dispose();
            imGuiController.Dispose();
            cloud.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            camera.Aspect = (float) Size.X / Size.Y;
            GL.Viewport(0, 0, Size.X, Size.Y);
            imGuiController.WindowResized(Size.X, Size.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            imGuiController.Update(this, (float) args.Time);

            if(ImGui.GetIO().WantCaptureMouse) return;

            KeyboardState keyboard = KeyboardState.GetSnapshot();
            MouseState mouse = MouseState.GetSnapshot();
            
            camera.HandleInput(keyboard, mouse, (float)args.Time);

            if (keyboard.IsKeyDown(Keys.Escape)) Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            sphereShader.Use();
            sphereShader.LoadMatrix4("view", camera.GetViewMatrix());
            sphereShader.LoadMatrix4("perspective", camera.GetProjectionMatrix());
            sphereShader.LoadFloat4("cameraSpaceLightPos", new Vector4(LightPosition.X, LightPosition.Y, LightPosition.Z, 1) * camera.GetViewMatrix());

            cloud.Render();

            RenderGui(args);

            Context.SwapBuffers();
        }

        private int frames;
        private double frameTime;
        private double fps;
        private void RenderGui(FrameEventArgs args)
        {
            frames++;
            frameTime += args.Time;

            if (frames == 10)
            {
                fps = 10 / frameTime;
                frameTime = 0;
                frames = 0;
            }
            ImGui.Begin("Options");

            ImGui.DragFloat3("Light Position", ref LightPosition);
            ImGui.LabelText("FPS", $"{fps:0.#}");
            
            ImGui.End();
            
            imGuiController.Render();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            
            imGuiController.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            imGuiController.MouseScroll(e.Offset);
        }
    }
}