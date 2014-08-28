using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoUI
{
    internal class SimpleServiceProvider : IServiceProvider
    {
        GraphicsDeviceService _graphicsDevice;

        public SimpleServiceProvider(GraphicsDevice graphics)
        {
            _graphicsDevice = new GraphicsDeviceService(graphics);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IGraphicsDeviceService))
                return _graphicsDevice;
            return null;
        }
    }

    internal class GraphicsDeviceService : IGraphicsDeviceService
    {
        GraphicsDevice _device;

        public GraphicsDeviceService(GraphicsDevice device)
        {
            _device = device;
            _device.DeviceReset += DeviceReset;
            _device.DeviceResetting += DeviceResetting;
            _device.Disposing += DeviceDisposing;
        }


        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;

        public event EventHandler<EventArgs> DeviceReset;

        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDevice GraphicsDevice
        {
            get { return _device; }
        }
    }
}
