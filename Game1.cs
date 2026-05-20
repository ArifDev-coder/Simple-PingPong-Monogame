using System;
using PingPongMonogame.Lib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// DEBUG LIB
using System.Diagnostics;

namespace PingPongMonogame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Rectangle _screenBounds;

    private Texture2D _ball;
    private Vector2 _ballPosition;
    private Circle _ballBounds;
    private Vector2 _ballMove;
    private float BALL_SPEED = 10f;
    private float BALL_SCALE = 3f;

    // Player Paddle
    private Texture2D _playerPaddle;
    private Vector2 _playerPaddlePosition;
    private Rectangle _playerPaddleBounds;
    private float PLAYER_PADDLE_SPEED = 500f;
    private float PLAYER_PADDLE_SCALE = 4f;

    private Texture2D _enemyPaddle;
    private Vector2 _enemyPaddlePosition;
    private Rectangle _enemyPaddleBounds;
    private float ENEMY_PADDLE_SPEED = 500f;
    private float ENEMY_PADDLE_SCALE = 4f;

    public Game1()
    {
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

        _playerPaddlePosition = new Vector2(10, (GraphicsDevice.PresentationParameters.BackBufferHeight - _playerPaddle.Height * PLAYER_PADDLE_SCALE) * 0.5f);

        _enemyPaddlePosition = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth - (_enemyPaddle.Width * ENEMY_PADDLE_SCALE + 10), (GraphicsDevice.PresentationParameters.BackBufferHeight - _enemyPaddle.Height * ENEMY_PADDLE_SCALE) * 0.5f);

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

        _playerPaddle = Content.Load<Texture2D>("texture/bet");

        _enemyPaddle = Content.Load<Texture2D>("texture/bet");
    }

    protected override void Update(GameTime gameTime)
    {
        GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
        KeyboardState keyboardState = Keyboard.GetState();

        if (gamePad.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();


        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.W))
        {
            _playerPaddlePosition.Y -= PLAYER_PADDLE_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.S))
        {
            _playerPaddlePosition.Y += PLAYER_PADDLE_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        EnemyAI(gameTime);

        CollisionCheck();

        base.Update(gameTime);
    }

    private void EnemyAI(GameTime gameTime) 
    {
        
        _playerPaddlePosition.Y = MathHelper.Clamp(_playerPaddlePosition.Y, 0, GraphicsDevice.PresentationParameters.BackBufferHeight - _playerPaddle.Height * ENEMY_PADDLE_SCALE);

        if (_ballPosition.Y > _enemyPaddlePosition.Y)
        {
            _enemyPaddlePosition.Y += ENEMY_PADDLE_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        } 
        else if(_ballPosition.Y < _enemyPaddlePosition.Y)
        {
            _enemyPaddlePosition.Y -= ENEMY_PADDLE_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _enemyPaddlePosition.Y = MathHelper.Clamp(_enemyPaddlePosition.Y, 0, GraphicsDevice.PresentationParameters.BackBufferHeight - _enemyPaddle.Height * ENEMY_PADDLE_SCALE);
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

        _playerPaddleBounds = new(
            (int)_playerPaddlePosition.X,
            (int)_playerPaddlePosition.Y,
            (int)(_playerPaddle.Width * PLAYER_PADDLE_SCALE),
            (int)(_playerPaddle.Height * PLAYER_PADDLE_SCALE)
        );

        _enemyPaddleBounds = new(
            (int)_enemyPaddlePosition.X,
            (int)_enemyPaddlePosition.Y,
            (int)(_enemyPaddle.Width * ENEMY_PADDLE_SCALE),
            (int)(_playerPaddle.Height * PLAYER_PADDLE_SCALE)
        );

        if (_ballBounds.Intersects(_enemyPaddleBounds))
        {
            float overlapLeft = _ballBounds.Right - _enemyPaddleBounds.Left;
            float overlapRight = _enemyPaddleBounds.Right - _ballBounds.Left;
            float overlapTop = _ballBounds.Bottom - _enemyPaddleBounds.Top;
            float overlapBottom = _enemyPaddleBounds.Bottom - _ballBounds.Top;

            float minOverlap = Math.Min(
                Math.Min(overlapLeft, overlapRight),
                Math.Min(overlapTop, overlapBottom)
            );

            if (minOverlap == overlapLeft)
                normal.X = -Vector2.UnitX.X;
            else if (minOverlap == overlapRight)
                normal.X = Vector2.UnitX.X;
            else if (minOverlap == overlapTop)
                normal.Y = -Vector2.UnitY.Y;
            else
                normal.Y = Vector2.UnitY.Y;

            normal.Normalize();
            _ballMove = Vector2.Reflect(_ballMove, normal);

            return;
        }

        if (_ballBounds.Intersects(_playerPaddleBounds))
        {
            float overlapLeft = _ballBounds.Right - _playerPaddleBounds.Left;
            float overlapRight = _playerPaddleBounds.Right - _ballBounds.Left;
            float overlapTop = _ballBounds.Bottom - _playerPaddleBounds.Top;
            float overlapBottom = _playerPaddleBounds.Bottom - _ballBounds.Top;

            float minOverlap = Math.Min(
                Math.Min(overlapLeft, overlapRight),
                Math.Min(overlapTop, overlapBottom)
            );

            if (minOverlap == overlapLeft)
                normal.X = -Vector2.UnitX.X;
            else if (minOverlap == overlapRight)
                normal.X = Vector2.UnitX.X;
            else if (minOverlap == overlapTop)
                normal.Y = -Vector2.UnitY.Y;
            else
                normal.Y = Vector2.UnitY.Y;

            normal.Normalize();
            _ballMove = Vector2.Reflect(_ballMove, normal);

            return;
        }

        if (_ballBounds.Left < _screenBounds.Left)
        {
            newBallPosition = new Vector2(
                GraphicsDevice.PresentationParameters.BackBufferWidth * .5f,
                GraphicsDevice.PresentationParameters.BackBufferHeight * .5f
            );

            RandomBallMove();

            _ballPosition = newBallPosition;
            return;
        }
        else if (_ballBounds.Right > _screenBounds.Right)
        {
            newBallPosition = new Vector2(
                GraphicsDevice.PresentationParameters.BackBufferWidth * .5f,
                GraphicsDevice.PresentationParameters.BackBufferHeight * .5f
            );

            RandomBallMove();

            _ballPosition = newBallPosition;
            return;
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
        Vector2 direction = new(x, y);

        _ballMove = direction * BALL_SPEED;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(_ball, _ballPosition, null, Color.White, 0.0f, Vector2.Zero, BALL_SCALE, SpriteEffects.None, 0.0f);

        _spriteBatch.Draw(_playerPaddle, _playerPaddlePosition, null, Color.White, 0.0f, Vector2.Zero, PLAYER_PADDLE_SCALE, SpriteEffects.None, 0.0f);

        _spriteBatch.Draw(_enemyPaddle, _enemyPaddlePosition, null, Color.White, 0.0f, Vector2.Zero, ENEMY_PADDLE_SCALE, SpriteEffects.None, 0.0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}