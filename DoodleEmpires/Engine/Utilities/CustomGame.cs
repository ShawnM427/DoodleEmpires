using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using OpenTK;

namespace DoodleEmpires.Engine.Utilities
{
    public abstract class CustomGame : OpenTKGameWindow, IDisposable
    {
        bool _exiting = false;
        Stopwatch _watch = new Stopwatch();

        GraphicsDevice _graphics;
        GraphicsDeviceManager _graphicsManager;
        ContentManager _content;
        GameServiceContainer _services;

        public bool Exiting
        {
            get { return _exiting; }
            set { _exiting = value; }
        }
        protected ContentManager Content
        {
            get { return _content; }
            private set { _content = value; }
        }

        #region IGraphicsDeviceService Members
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphics; }
        }
        #endregion

        public CustomGame()
        {
            _services = new GameServiceContainer();
            _services.AddService(typeof(IGraphicsDeviceService), new GraphicsDeviceService(base.Handle, 640, 360));
            _graphics = ((GraphicsDeviceService)_services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            _content = new ContentManager(_services);
        }

        public void Run()
        {
            _watch.Start();

            Initialize();

            while (!Exiting)
            {
                GameTime gt = new GameTime(TimeSpan.Zero, _watch.Elapsed);
            }

            OnExit();

            _watch.Stop();
        }

        protected abstract void Update(GameTime gameTime);

        protected abstract void Draw(GameTime gameTime);

        public void Dispose()
        {
            _graphics.Dispose();
            _graphicsManager.Dispose();
            _content.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return this;
        }

        protected virtual void LoadContent() { }

        protected virtual void UnloadContent()
        {
            _content.Unload();
        }
        
        protected virtual void Initialize()
        {

        }

        protected virtual void OnExit()
        {

        }
    }

    /// <summary>
    /// Helper class responsible for creating and managing the GraphicsDevice.
    /// All GraphicsDeviceControl instances share the same GraphicsDeviceService,
    /// so even though there can be many controls, there will only ever be a single
    /// underlying GraphicsDevice. This implements the standard IGraphicsDeviceService
    /// interface, which provides notification events for when the device is reset
    /// or disposed.
    /// </summary>
    public class GraphicsDeviceService : IGraphicsDeviceService
    {
        IntPtr _windowHandle;

        #region Fields


        // Singleton device service instance.
        static GraphicsDeviceService singletonInstance;


        // Keep track of how many controls are sharing the singletonInstance.
        static int referenceCount;


        #endregion


        /// <summary>
        /// Constructor is private, because this is a singleton class:
        /// client controls should use the public AddRef method instead.
        /// </summary>
        public GraphicsDeviceService(IntPtr windowHandle, int width, int height)
        {
            parameters = new PresentationParameters();

            parameters.BackBufferWidth = Math.Max(width, 1);
            parameters.BackBufferHeight = Math.Max(height, 1);
            parameters.BackBufferFormat = SurfaceFormat.Color;

            parameters.DepthStencilFormat = DepthFormat.Depth16;

            graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, 
                                                GraphicsProfile.HiDef,
                                                parameters);
            _windowHandle = windowHandle;
        }
                
        /// <summary>
        /// Resets the graphics device to whichever is bigger out of the specified
        /// resolution or its current size. This behavior means the device will
        /// demand-grow to the largest of all its GraphicsDeviceControl clients.
        /// </summary>
        public void ResetDevice(int width, int height)
        {
            if (DeviceResetting != null)
                DeviceResetting(this, EventArgs.Empty);

            parameters.BackBufferWidth = Math.Max(parameters.BackBufferWidth, width);
            parameters.BackBufferHeight = Math.Max(parameters.BackBufferHeight, height);

            graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
                                                GraphicsProfile.HiDef,
                                                parameters);

            if (DeviceReset != null)
                DeviceReset(this, EventArgs.Empty);
        }


        /// <summary>
        /// Gets the current graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        GraphicsDevice graphicsDevice;


        // Store the current device settings.
        PresentationParameters parameters;


        // IGraphicsDeviceService events.
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
    }
}
