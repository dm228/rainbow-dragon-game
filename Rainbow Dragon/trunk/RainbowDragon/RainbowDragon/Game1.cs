using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RainbowDragon.HelperClasses;
using RainbowDragon.Core.Screens;

namespace RainbowDragon
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int currentGameState;

        //Screens
        StartScreen startScreen;
        InGameScreen inGameScreen;
        GameoverScreen gameOverScreen;
        public void setGameState(int newState)
        {
            currentGameState = newState;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            setGameState(Constants.GAME_STATE_START);
            inGameScreen = new InGameScreen(this);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            if (currentGameState == Constants.GAME_STATE_START)
            {
                startScreen.Update(gameTime);
            }
            else if (currentGameState == Constants.GAME_STATE_INGAME)
            {
                inGameScreen.Update(gameTime);
            }
            else if (currentGameState == Constants.GAME_STATE_OVER)
            {
                gameOverScreen.Update(gameTime);
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            if (currentGameState == Constants.GAME_STATE_START)
            {
                startScreen.Draw(spriteBatch);
            }
            else if (currentGameState == Constants.GAME_STATE_INGAME)
            {
                inGameScreen.Draw(spriteBatch);
            }
            else if (currentGameState == Constants.GAME_STATE_OVER)
            {
                gameOverScreen.Draw(spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}