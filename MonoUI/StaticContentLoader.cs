using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace MonoUI
{
    public static class StaticContentLoader
    {
        static Dictionary<string, object> _content;
        static bool _initialized = false;

        internal static void TryInitialize(GraphicsDevice graphics)
        {
            if (!_initialized)
                Initialize(graphics);
        }

        static void Initialize(GraphicsDevice graphics)
        {
            _content = new Dictionary<string, object>();

            SimpleServiceProvider service = new SimpleServiceProvider(graphics);
            ResourceContentManager manager = new ResourceContentManager(service, Resources.ResourceManager);
            ResourceSet resourceSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            _content.Add("CheckBox_Unchecked", manager.Load<Texture2D>("CheckBox_Unchecked"));
            _content.Add("CheckBox_Checked", manager.Load<Texture2D>("CheckBox_Checked"));
            _content.Add("Font_Arial_8", manager.Load<SpriteFont>("Font_Arial_8"));
            _content.Add("Font_Arial_10", manager.Load<SpriteFont>("Font_Arial_10"));
            _content.Add("Font_Arial_12", manager.Load<SpriteFont>("Font_Arial_12"));

            //foreach (DictionaryEntry entry in resourceSet)
            //{
            //    string resourceKey = (string)entry.Key;

            //    if (resourceKey.StartsWith("Font_"))
            //    {
            //        _content.Add(resourceKey.Replace("Font_", ""), manager.Load<SpriteFont>(resourceKey));
            //    }
            //    else if (resourceKey.StartsWith("Image_"))
            //    {
            //        _content.Add(resourceKey.Replace("Image_", ""), Texture2D.FromStream(graphics, Resources.ResourceManager.(resourceKey)));
            //    }
            //}

            _initialized = true;
        }

        public static T GetItem<T>(string path)
        {
            if (_content.ContainsKey(path))
            {
                object item = _content[path];
                return (T)item;
            }
            else
                return default(T);
        }
    }
}
