using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoUI
{
    public abstract class GUITextElement : GUIElement
    {
        /// <summary>
        /// The alignment of text within this control
        /// </summary>
        protected TextAlignment _alignment = TextAlignment.Centred;
        /// <summary>
        /// This label's text
        /// </summary>
        protected string _text;
        /// <summary>
        /// This label's font
        /// </summary>
        protected SpriteFont _font;

        /// <summary>
        /// Gets or sets the text for this label
        /// </summary>
        public virtual string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets the text alignment for this control
        /// </summary>
        public virtual TextAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets the font for this text element
        /// </summary>
        public virtual SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;
                Invalidating = true;
            }
        }
        
        /// <summary>
        /// Creates a new instance of a GUI text element
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="parent">The parent GUI container</param>
        protected GUITextElement(GraphicsDevice graphics, GUIContainer parent)
            : base(graphics, parent)
        {
            if (_font == null)
            {
                SimpleServiceProvider service = new SimpleServiceProvider(graphics);
                ResourceContentManager manager = new ResourceContentManager(service, Resources.ResourceManager);
                _font = manager.Load<SpriteFont>("BasicFont");
            }
            _text = "";
        }
    }
}
