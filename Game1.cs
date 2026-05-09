using System;
using PingPongMonogame.Lib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PingPongMonogame;

public class Game1 : Game
{
    internal Game1 _gameInstance;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Rectangle _screenBounds;

    private Texture2D _ball;
    private Vector2 _ballPosition;
    private Circle _ballBounds;
    private Vector2 _ballMove;
    private float BALL_SPEED = 10f;
    private float BALL_SCALE = 3f;

    private Texture2D _bet;
    private Vector2 _betPosition;
    private Rectangle _betBounds;
    private float BET_SPEED = 500f;
    private float BET_SCALE = 4f;

    public Game1()
    {
        if (_gameInstance != null)
        {
            throw new InvalidOperationException($"Only a single Game can be created");
        }

        _gameInstance = this;

        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,
            IsFullScreen = false
        };

        Content.RootDirectory = "Content";

        Window.Title = "PingPong Game";

        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _betPosition = new Vector2(10, (GraphicsDevice.PresentationParameters.BackBufferHeight - _bet.Height * BET_SCALE) * 0.5f);

        _ballPosition = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * .5f, GraphicsDevice.PresentationParameters.BackBufferHeight * .5f);

        _screenBounds = new Rectangle(
            0,
            0,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight
        );

        RandomBallMove();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _ball = Content.Load<Texture2D>("texture/ball");
        _bet = Content.Load<Texture2D>("texture/bet");
    }

    protected override void Update(GameTime gameTime)
    {
        GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
        KeyboardState keyboardState = Keyboard.GetState();

        if (gamePad.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();


        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.W))
        {
            _betPosition.Y -= BET_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.S))
        {
            _betPosition.Y += BET_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _betPosition.Y = MathHelper.Clamp(_betPosition.Y, 0, GraphicsDevice.PresentationParameters.BackBufferHeight - _bet.Height * 4.0f);

        CollisionCheck();

        base.Update(gameTime);
    }

    private void CollisionCheck()
    {
        Vector2 normal = Vector2.Zero;
        Vector2 newBallPosition = _ballPosition + _ballMove;

        _ballBounds = new(
            (int)(newBallPosition.X + (_ball.Width * BALL_SCALE * .5f)),
            (int)(newBallPosition.Y + (_ball.Height * BALL_SCALE * .5f)),
            (int)(_ball.Width * BALL_SCALE * .5f)
        );

        _betBounds = new (
            (int)_betPosition.X,
            (int)_betPosition.Y,
            _bet.Width * (int)BET_SCALE,
            _bet.Height * (int)BET_SCALE
        );

        if (_ballBounds.Left < _screenBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBallPosition.X = _screenBounds.Left;
        }
        else if (_ballBounds.Right > _screenBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBallPosition.X = _screenBounds.Right - _ball.Width * BALL_SCALE;
        }

        if (_ballBounds.Top < _screenBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBallPosition.Y = _screenBounds.Top;
        }
        else if (_ballBounds.Bottom > _screenBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBallPosition.Y = _screenBounds.Bottom - _ball.Height * BALL_SCALE;
        }

        if (normal != Vector2.Zero)
        {
            normal.Normalize();
            _ballMove = Vector2.Reflect(_ballMove, normal);
        }

        _ballPosition = newBallPosition;

    }

    private void RandomBallMove()
    {
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        _ballMove = direction * BALL_SPEED;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(_ball, _ballPosition, null, Color.White, 0.0f, Vector2.Zero, BALL_SCALE, SpriteEffects.None, 0.0f);
        _spriteBatch.Draw(_bet, _betPosition, null, Color.White, 0.0f, Vector2.Zero, BET_SCALE, SpriteEffects.None, 0.0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
