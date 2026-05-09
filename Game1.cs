using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PingPongMonogame;

public class Game1 : Game
{
    internal Game1 _gameInstance;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _ball;
    private Texture2D _bet;
    private Vector2 _betPosition;


    private float _betSpeed = 500f;
    private float _betScale;

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
        _betPosition = new Vector2(10, GraphicsDevice.PresentationParameters.BackBufferHeight - _bet.Height * _betScale * 0.5f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _ball = Content.Load<Texture2D>("texture/ball");
        _bet = Content.Load<Texture2D>("texture/bet");
        _betScale = 4f;
    }

    protected override void Update(GameTime gameTime)
    {
        GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
        KeyboardState keyboardState = Keyboard.GetState();

        if (gamePad.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();


        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.W))
        {
            _betPosition.Y -= _betSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.S))
        {
            _betPosition.Y += _betSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _betPosition.Y = MathHelper.Clamp(_betPosition.Y, 0, GraphicsDevice.PresentationParameters.BackBufferHeight - _bet.Height * 4.0f);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // _spriteBatch.Draw(_ball, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0.0f);
        _spriteBatch.Draw(_bet, _betPosition, null, Color.White, 0.0f, Vector2.Zero, _betScale, SpriteEffects.None, 0.0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
