using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PingPongMonogame;

public class Game1 : Game
{
    internal static Game1 slime_instance;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _ball;
    private Texture2D _bet;

    public Game1()
    {
        if (slime_instance != null)
        {
            throw new InvalidOperationException($"Only a single Game can be created");
        }

        slime_instance = this;

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
        // TODO: Add your initialization logic here


        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _ball = Content.Load<Texture2D>("texture/ball");
        _bet = Content.Load<Texture2D>("texture/bet");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(_ball, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0.0f);
        _spriteBatch.Draw(_bet, new Vector2(0, _bet.Height + _ball.Height + 10), null, Color.White, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0.0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
