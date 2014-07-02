using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Entities;
using System;

/// <summary>
/// Represents an implementation of a 2D camera
/// </summary>
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
    /// Begins rendering with this camera
    /// </summary>
    void BeginDraw();

    /// <summary>
    /// Ends the rendering pass and presents this camera's view
    /// </summary>
    void EndDraw();
    
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

    /// <summary>
    /// Gets or sets the post-processing effect for this camera
    /// </summary>
    Effect PostEffect { get; set; }
}

/// <summary>
/// Represents a 2-Dimensional camera
/// </summary>
public class Camera2D : ICamera2D
{
    protected RenderTarget2D _renderTarget;
    protected Effect _postEffect;
    protected SpriteBatch _spriteBatch;

    private Vector2 _position;
    /// <summary>
    /// The height of the graphics viewport
    /// </summary>
    protected float _viewportHeight;
    /// <summary>
    /// The width of the graphics viewport
    /// </summary>
    protected float _viewportWidth;
    /// <summary>
    /// The bounds that this camera has in the world
    /// </summary>
    protected Rectangle _viewBounds = new Rectangle(0,0,0,0);

    /// <summary>
    /// The graphics device that this camera is bound to
    /// </summary>
    protected GraphicsDevice _graphics;

    public Effect PostEffect
    {
        get { return _postEffect; }
        set { _postEffect = value; }
    }
    
    /// <summary>
    /// Creates a new 2D camera
    /// </summary>
    /// <param name="graphics">The graphics device to bind to</param>
    public Camera2D(GraphicsDevice graphics)
    {
        _graphics = graphics;
        _spriteBatch = new SpriteBatch(graphics);
        
        Initialize();
    }

    #region Properties

    /// <summary>
    /// Gets or sets this camera's position
    /// </summary>
    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }
    /// <summary>
    /// Gets or sets this camera's rotation
    /// </summary>
    public float Rotation { get; set; }
    /// <summary>
    /// Gets or sets this camera's orgin
    /// </summary>
    public Vector2 Origin { get; set; }
    /// <summary>
    /// Gets or sets this camera's scale
    /// </summary>
    public Vector2 Scale { get; set; }
    /// <summary>
    /// Gets the screen centre for this camera
    /// </summary>
    public Vector2 ScreenCenter { get; protected set; }
    /// <summary>
    /// Gets or sets this camera's transformation matrix
    /// </summary>
    public Matrix Transform { get; set; }
    /// <summary>
    /// Gets or sets this camera's projection matrix
    /// </summary>
    public Matrix Projection { get; set; }
    /// <summary>
    /// Gets or sets this camera's focus object
    /// </summary>
    public IFocusable Focus { get; set; }
    /// <summary>
    /// Gets or sets this camera's movement speed
    /// </summary>
    public float MoveSpeed { get; set; }
    /// <summary>
    /// Gets the view bounds for this camera
    /// </summary>
    public Rectangle ViewBounds { get { return _viewBounds; } }
    /// <summary>
    /// Gets the snap bounds for this camera
    /// </summary>
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

    public void BeginDraw()
    {
        if (_renderTarget == null)
            _renderTarget = new RenderTarget2D(_graphics, _graphics.Viewport.Width, _graphics.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24);

        _graphics.SetRenderTarget(_renderTarget);
        _graphics.Clear(Color.White);
    }

    public void EndDraw()
    {
        _graphics.SetRenderTarget(null);
        _graphics.Clear(Color.Black);

        if (_postEffect != null)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                RasterizerState.CullCounterClockwise, _postEffect);
        }
        else
        {
            _spriteBatch.Begin();
        }

        _spriteBatch.Draw(_renderTarget, _graphics.Viewport.Bounds, Color.White);

        _spriteBatch.End();
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
    /// <param name="screenPoint">The screen co-ordinates to convert</param>
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