using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Entities;
using System;

public interface ICamera2D
{
    /// <summary>
    /// Gets or sets the position of the camera
    /// </summary>
    /// <value>The position.</value>
    Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the move speed of the camera.
    /// The camera will tween to its destination.
    /// </summary>
    /// <value>The move speed.</value>
    float MoveSpeed { get; set; }

    /// <summary>
    /// Gets or sets the rotation of the camera.
    /// </summary>
    /// <value>The rotation.</value>
    float Rotation { get; set; }

    /// <summary>
    /// Gets the origin of the viewport (accounts for Scale)
    /// </summary>        
    /// <value>The origin.</value>
    Vector2 Origin { get; }

    /// <summary>
    /// Gets or sets the scale of the Camera
    /// </summary>
    /// <value>The scale.</value>
    Vector2 Scale { get; set; }

    /// <summary>
    /// Gets the screen center (does not account for Scale)
    /// </summary>
    /// <value>The screen center.</value>
    Vector2 ScreenCenter { get; }

    /// <summary>
    /// Gets the transform that can be applied to 
    /// the SpriteBatch Class.
    /// </summary>
    /// <see cref="SpriteBatch"/>
    /// <value>The transform.</value>
    Matrix Transform { get; }

    /// <summary>
    /// Gets the projection matrix for this camera
    /// </summary>
    Matrix Projection { get; }

    /// <summary>
    /// Gets or sets the focus of the Camera.
    /// </summary>
    /// <seealso cref="IFocusable"/>
    /// <value>The focus.</value>
    IFocusable Focus { get; set; }
    
    /// <summary>
    /// Determines whether the target is in view given the specified position.
    /// This can be used to increase performance by not drawing objects
    /// directly in the viewport
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="texture">The texture.</param>
    /// <returns>
    ///     <c>true</c> if the target is in view at the specified position; otherwise, <c>false</c>.
    /// </returns>
    bool IsInView(Vector2 position, Texture2D texture);

    /// <summary>
    /// Gets the world co-ordinates for the given screen co-ordinates
    /// </summary>
    /// <param name="screenCoords">The screen co-ordinates to convert</param>
    /// <returns>The point in the world relating to the point on the screen</returns>
    Vector2 PointToWorld(Vector2 screenCoords);

    /// <summary>
    /// Gets the world bounds that this camera displays
    /// </summary>
    Rectangle ViewBounds { get; }

    /// <summary>
    /// Gets or sets the bounds that this camera is limited to
    /// </summary>
    Rectangle ScreenBounds { get; set; }
}

public class Camera2D : ICamera2D
{
    private Vector2 _position;
    protected float _viewportHeight;
    protected float _viewportWidth;
    protected Rectangle _viewBounds = new Rectangle(0,0,0,0);

    protected GraphicsDevice _graphics;
    
    public Camera2D(GraphicsDevice graphics)
    {
        _graphics = graphics;
        
        Initialize();
    }

    #region Properties

    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public Vector2 ScreenCenter { get; protected set; }
    public Matrix Transform { get; set; }
    public Matrix Projection { get; set; }
    public IFocusable Focus { get; set; }
    public float MoveSpeed { get; set; }
    public Rectangle ViewBounds { get { return _viewBounds; } }
    public Rectangle ScreenBounds { get; set; }

    #endregion

    /// <summary>
    /// Called when the GameComponent needs to be initialized. 
    /// </summary>
    public void Initialize()
    {
        _viewportWidth = _graphics.Viewport.Width;
        _viewportHeight = _graphics.Viewport.Height;

        Scale = new Vector2(1, 1);
        ScreenCenter = new Vector2(_viewportWidth / 2, _viewportHeight / 2);
        MoveSpeed = 2f;
        
        Matrix projection = Matrix.CreateOrthographicOffCenter(0, _graphics.Viewport.Width, _graphics.Viewport.Height, 0, 0, 1);
        Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

        Projection = halfPixelOffset * projection;
    }

    /// <summary>
    /// Updates this camera
    /// </summary>
    /// <param name="gameTime">The current game time</param>
    public void Update(GameTime gameTime)
    {
        Origin = ScreenCenter / Scale;

        if (Focus != null)
        {
            // Move the Camera to the position that it needs to go
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _position.X += (Focus.Position.X - Position.X) * MoveSpeed * delta;
            _position.Y += (Focus.Position.Y - Position.Y) * MoveSpeed * delta;
        }

        if (_position.X - Origin.X < ScreenBounds.Left)
            _position.X = ScreenBounds.Left + Origin.X;
        if (_position.Y - Origin.Y < ScreenBounds.Top)
            _position.Y = ScreenBounds.Top + Origin.Y;

        if (_position.X + Origin.X > ScreenBounds.Right)
            _position.X = ScreenBounds.Right - Origin.X;
        if (_position.Y + Origin.Y > ScreenBounds.Bottom)
            _position.Y = ScreenBounds.Bottom - Origin.Y;

        _viewBounds.X = (int)(Position.X - Origin.X);
        _viewBounds.Y = (int)(Position.Y - Origin.Y);
        _viewBounds.Width = (int)(Origin.X * 2) + 1;
        _viewBounds.Height = (int)(Origin.Y * 2) + 1;

        //if (!ScreenBounds.Contains(ViewBounds))
        //{
        //    _position.X = ViewBounds.X < ScreenBounds.X ? ScreenBounds.X + Origin.X : _position.X;
        //    _position.Y = ViewBounds.X < ScreenBounds.Y ? ScreenBounds.Y + Origin.Y : _position.Y;
        //}

        // Create the Transform used by any
        // spritebatch process
        Transform = Matrix.Identity *
                    Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                    Matrix.CreateScale(new Vector3(Scale, 1.0F));
    }

    /// <summary>
    /// Determines whether the target is in view given the specified position.
    /// This can be used to increase performance by not drawing objects
    /// directly in the viewport
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="texture">The texture.</param>
    /// <returns>
    ///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsInView(Vector2 position, Texture2D texture)
    {
        // If the object is not within the horizontal bounds of the screen

        if ((position.X + texture.Width) < (Position.X - Origin.X) || (position.X) > (Position.X + Origin.X))
            return false;

        // If the object is not within the vertical bounds of the screen
        if ((position.Y + texture.Height) < (Position.Y - Origin.Y) || (position.Y) > (Position.Y + Origin.Y))
            return false;

        // In View
        return true;
    }

    /// <summary>
    /// Gets the world co-ordinates for the given screen co-ordinates
    /// </summary>
    /// <param name="screenCoords">The screen co-ordinates to convert</param>
    /// <returns>The point in the world relating to the point on the screen</returns>
    public Vector2 PointToWorld(Vector2 screenPoint)
    {
        Vector3 temp = _graphics.Viewport.Unproject(new Vector3(screenPoint, 0), Projection, Transform, Matrix.Identity);
        return new Vector2(temp.X, temp.Y);
    }

    /// <summary>
    /// Gets the world co-ordinates for the given screen co-ordinates
    /// </summary>
    /// <param name="x">The x coordinate to translate</param>
    /// <param name="y">The y coordinate to translete</param>
    /// <returns>The point in the world relating to the point on the screen</returns>
    public Vector2 PointToWorld(float x, float y)
    {
        Vector3 temp = _graphics.Viewport.Unproject(new Vector3(new Vector2(x, y), 0), Projection, Transform, Matrix.Identity);
        return new Vector2(temp.X, temp.Y);
    }
}