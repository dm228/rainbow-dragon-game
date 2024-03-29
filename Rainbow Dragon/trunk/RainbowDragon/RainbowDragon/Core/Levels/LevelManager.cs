﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using RainbowDragon.Core.Pickups;
using RainbowDragon.Core.Enemies;
using RainbowDragon.HelperClasses;
using RainbowDragon.Core.Sprites;
using RainbowDragon.Core.Player;
using Microsoft.Xna.Framework.Graphics;
using RainbowDragon.Core.Levels.Tutorials;
using RainbowDragon.Core.Screens;
namespace RainbowDragon.Core.Levels
{
    class LevelManager
    {
        int currentLevelNumber;
        int currentNumberSuns;
        int currentNumberArrows;
        int maxTime;
        List<Level> levels;
        public Level currentLevel;
        ContentLoader contentLoader;
        MusicPlayer musicPlayer;
        int screenWidth;
        int screenHeight;
        Game1 game;
        float xRatio;
        float yRatio;
        InGameScreen screen;
        
        public LevelManager(ContentLoader loader, Game1 game, MusicPlayer mPlayer, InGameScreen screen)
        {

            contentLoader = loader;
            this.game = game;
            musicPlayer = mPlayer;
            this.screen = screen;

        }


        public void Initialize(int scrWidth, int scrHeight)
        {

            screenHeight = scrHeight;
            screenWidth = scrWidth;
            xRatio = screenWidth / 16;
            yRatio = scrHeight / 9;
            currentLevelNumber = 1; //starts from 1 
            System.IO.Stream stream = TitleContainer.OpenStream("Content\\Core\\levels.xml");
            XDocument doc = XDocument.Load(stream);
            levels = new List<Level>();

            Random rand = new Random();

            foreach (XElement level in doc.Descendants("level"))
            {
                Level newLevel = new Level(contentLoader, game.GraphicsDevice, this, musicPlayer);
                newLevel.levelNumber = Convert.ToInt32(level.Element("number").Value);
                newLevel.time = Convert.ToInt32(level.Element("time").Value);
                foreach (XElement sun in level.Descendants("sun"))
                {
                    Sun newSun = new Sun();
                    if (sun.Element("type").Value == Constants.NORMAL_SUN)
                    {
                        newSun = new Sun();
                        newSun.Texture = contentLoader.AddTexture2(Constants.NORMAL_SUN, Constants.NORMAL_SUN_PATH);

                    }
                    else if (sun.Element("type").Value == Constants.ACCELERATING_SUN)
                    {
                        newSun = new AcceleratingSun();
                        newSun.Texture = contentLoader.AddTexture2(Constants.ACCELERATING_SUN, Constants.ACCELERATING_SUN_PATH);

                    }

                    else if (sun.Element("type").Value == Constants.INVINCIBLE_SUN)
                    {

                        newSun = new InvincibleSun();
                        newSun.Texture = contentLoader.AddTexture2(Constants.INVINCIBLE_SUN, Constants.INVINCIBLE_SUN_PATH);
                    }

                    //OVERRIDE TEXTURE
                    if (sun.Element("path") != null && sun.Element("name") != null)
                    {
                        newSun.Texture = contentLoader.AddTexture2(sun.Element("name").Value, sun.Element("path").Value);

                    }

                    newSun.Position = new Vector2(Convert.ToSingle(sun.Element("xposition").Value) * xRatio, Convert.ToSingle(sun.Element("yposition").Value) * yRatio);
                    newSun.Size = Convert.ToInt32(sun.Element("size").Value);
                    newSun.Source = new Rectangle((int)newSun.position.X, (int)newSun.position.Y, newSun.Size * 100, newSun.Size * 100);
                    newSun.InitializeTextures(contentLoader.AddTexture2("pupil", "Pickups\\pupil"), contentLoader.AddTexture2("eyelid", "Pickups\\eyelid"),
                        contentLoader.AddTexture2("eyelid_closed", "Pickups\\eyelid_closed"),contentLoader.AddTexture2("eye_base", "Pickups\\eye_base"),
                        contentLoader.AddTexture2("zzz", "Pickups\\zzz"));
                    newSun.rand = rand;
                    newLevel.suns.Add(newSun);

                }

                foreach (XElement arrow in level.Descendants("arrow"))
                {
                    Arrow newArrow = new Arrow();
                    if (arrow.Element("type").Value == Constants.NORMAL_ARROW)
                    {
                        newArrow = new NormalArrow();
                        newArrow.Texture = contentLoader.AddTexture2(Constants.NORMAL_ARROW, Constants.NORMAL_ARROW_PATH);

                    }
                    else if (arrow.Element("type").Value == Constants.INVERSE_ARROW)
                    {
                        newArrow = new InverseArrow();
                        newArrow.Texture = contentLoader.AddTexture2(Constants.INVERSE_ARROW, Constants.INVERSE_ARROW_PATH);

                    }
                    else if (arrow.Element("type").Value == Constants.POISON_ARROW)
                    {
                        newArrow = new PoisonArrow();
                        newArrow.Texture = contentLoader.AddTexture2(Constants.POISON_ARROW, Constants.POISON_ARROW_PATH);

                    }
                    else if (arrow.Element("type").Value == Constants.SLOW_ARROW)
                    {
                        newArrow = new SlowArrow();
                        newArrow.Texture = contentLoader.AddTexture2(Constants.SLOW_ARROW, Constants.SLOW_ARROW_PATH);

                    }

                    //OVERRIDE TEXTURE
                    if (arrow.Element("path") != null && arrow.Element("name") != null)
                    {
                        newArrow.Texture = contentLoader.AddTexture2(arrow.Element("name").Value, arrow.Element("path").Value);

                    }

                    newArrow.Direction = Convert.ToSingle(arrow.Element("direction").Value);
                    newArrow.speed = Convert.ToSingle(arrow.Element("speed").Value);
                    newArrow.Position = new Vector2(Convert.ToSingle(arrow.Element("xposition").Value) * xRatio, Convert.ToSingle(arrow.Element("yposition").Value) * yRatio);
                    newArrow.initialPosition = newArrow.position;
                    newArrow.Source = new Rectangle((int)newArrow.position.X, (int)newArrow.position.Y, newArrow.Texture.Width, newArrow.Texture.Height);
                    newLevel.arrows.Add(newArrow);

                }


                foreach (XElement tutorial in level.Descendants("tutorial"))
                {
                    if (tutorial.Element("type").Value == Constants.KEY_PRESS_TUTORIAL)
                    {
                        KeyPressTutorial tut = new KeyPressTutorial(newLevel, contentLoader.AddFont("chineseFontSmall"), new Vector2(screenWidth , screenHeight / 2), contentLoader.AddTexture2("tutorial_bg", "Levels\\tutorial_bg"));
                        tut.Text = tutorial.Element("text").Value;
                        foreach (XElement key in tutorial.Descendants("key"))
                        {
                            if(key.Value == "left")
                            {
                                tut.keys.Add(Microsoft.Xna.Framework.Input.Keys.Left);
                            }
                            else if (key.Value == "right")
                            {
                                tut.keys.Add(Microsoft.Xna.Framework.Input.Keys.Right);
                            }
                            else if (key.Value == "up")
                            {
                                tut.keys.Add(Microsoft.Xna.Framework.Input.Keys.Up);
                            }
                            else if (key.Value == "down")
                            {
                                tut.keys.Add(Microsoft.Xna.Framework.Input.Keys.Down);
                            }
                            else if (key.Value == "space")
                            {
                                tut.keys.Add(Microsoft.Xna.Framework.Input.Keys.Space);
                            }
                            else if (key.Value == "escape")
                            {
                                tut.keys.Add(Microsoft.Xna.Framework.Input.Keys.Escape);
                            }
                            else if (key.Value == "enter")
                            {
                                tut.keys.Add(Microsoft.Xna.Framework.Input.Keys.Enter);
                            }

                            
                        }
                        tut.type = Constants.KEY_PRESS_TUTORIAL;
                        newLevel.tutorials.Add(tut);
                        

                    }
                    else if (tutorial.Element("type").Value == Constants.TIMER_TUTORIAL)
                    {
                        TimerTutorial tut = new TimerTutorial(newLevel, contentLoader.AddFont("chineseFontSmall"), new Vector2(screenWidth, screenHeight / 2), contentLoader.AddTexture2("tutorial_bg", "Levels\\tutorial_bg"));
                        tut.Text = tutorial.Element("text").Value;
                        tut.maxTime = Convert.ToInt32( tutorial.Element("time").Value);
                        tut.type = Constants.TIMER_TUTORIAL;
                        newLevel.tutorials.Add(tut);

                    }
                    else if (tutorial.Element("type").Value == Constants.COLLISION_TUTORIAL)
                    {

                        CollisionTutorial tut = new CollisionTutorial(newLevel, contentLoader.AddFont("chineseFontSmall"), new Vector2(screenWidth, screenHeight / 2), contentLoader.AddTexture2("tutorial_bg", "Levels\\tutorial_bg"));
                        tut.Text = tutorial.Element("text").Value;
                        tut.target = tutorial.Element("target").Value;
                        tut.type = Constants.COLLISION_TUTORIAL;
                        newLevel.tutorials.Add(tut);
                    }





                }
                string back = level.Element("background").Value;
                newLevel.ColoredBackgroud = contentLoader.AddTexture2(back, Constants.BACKGROUND_BASE_PATH + back);
                newLevel.BWBackgroud = contentLoader.AddTexture2("bw_" + back, Constants.BACKGROUND_BASE_PATH + "bw_" + back);
                newLevel.BackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                if (level.Element("playerX") != null && level.Element("playerY") != null)
                {
                    newLevel.playerPosition = new Vector2(Convert.ToSingle(level.Element("playerX").Value)*xRatio, Convert.ToSingle(level.Element("playerY").Value)*yRatio);
                }
                else
                {
                    newLevel.playerPosition = new Vector2(screenWidth / 2, screenHeight / 2);
                }

                levels.Add(newLevel);
                
            }


            currentLevel = GetCurrentLevel();
            currentLevel.Initialize();
            game.StartTransition(currentLevelNumber);
            screen.RestartPlayer(currentLevel.playerPosition);
            #region Old Version XML parser
            /*
            levels = (from level in doc.Descendants("level")
                      select new Level()
                      {
                          levelNumber = Convert.ToInt32(level.Element("number").Value),
                          time = Convert.ToInt32(level.Element("time").Value),
                          suns = (from sun in level.Descendants("sun")
                                  select new Sun()
                                  {
                                      position = new Vector2(Convert.ToSingle(sun.Element("xposition").Value), Convert.ToSingle(sun.Element("yposition").Value)),
                                      size = Convert.ToInt32(sun.Element("size").Value),
                                      type = sun.Element("type").Value

                                  }).ToList(),
                          arrows = (from arrow in level.Descendants("arrow")
                                    select new Arrow()
                                    {
                                        direction = Convert.ToInt32(arrow.Element("direction").Value),
                                        position = new Vector2(Convert.ToSingle(arrow.Element("xposition").Value), Convert.ToSingle(arrow.Element("yposition").Value)),
                                        type = arrow.Element("type").Value



                                    }).ToList()


                      }).ToList();


            if (levels[0].levelNumber == currentLevelNumber)
            {
                currentLevel = levels[0];

            }
            else
            {
                foreach (Level level in levels)
                {
                    if (level.levelNumber == currentLevelNumber)
                    {
                        currentLevel = level;
                        break;
                    }

                }

            }

            List<Arrow> newArrows = new List<Arrow>();

            foreach (Arrow arrow in currentLevel.arrows)
            {
                if (arrow.type == Constants.NORMAL_ARROW)
                {
                    newArrows.Add(new NormalArrow());

                }
                else
                {
                    //TODO: add remaining arrows
                }

            }

            */
            #endregion

        }


        public Level GetCurrentLevel()
        {
            Level tempNewLevel = new Level(contentLoader, game.GraphicsDevice, this, musicPlayer);
            if (levels[0].levelNumber == currentLevelNumber)
            {
                tempNewLevel = levels[0];

            }
            else
            {
                foreach (Level level in levels)
                {
                    if (level.levelNumber == currentLevelNumber)
                    {
                        tempNewLevel = level;
                        break;
                    }

                }

            }

            return tempNewLevel;
        }

        public void Update(GameTime gameTime)
        {

            //Here should only have code to change level(or rather call method to change level) if certain conditions are met
            //also code to check if game over and send respective message to ingameScreen
            //also call methods that check if more arrows need to be sent or more pickups
            //collision detection maybe should have its own method and be called separately from ingamescreen
            currentLevel.Update(gameTime);

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            currentLevel.Draw(spriteBatch);

        }

        public void IncreaseLevel()
        {
            if (currentLevelNumber < levels.Count)
            {
                currentLevel.Kill();
                currentLevelNumber++;
                currentLevel = GetCurrentLevel();
                currentLevel.Initialize();
                game.StartTransition(currentLevelNumber);
                screen.RestartPlayer(currentLevel.playerPosition);
                
            }
        }

        public void CheckForCollision(Dragon player)
        {
            foreach (Sun sun in currentLevel.suns)
            {
                sun.UpdatePlayerPosition(player.head.position);
            }
            
            //Check each part of the dragon
            foreach (DragonPart part in player.dragon)
            {
                //Check for collisions with suns
                foreach (Sun sun in currentLevel.suns)
                {
                    if (!sun.isInvisible)
                    {
                        if (part.Hitbox.Intersects(sun.Hitbox))
                        {
                            if (sun.GetType() == typeof(AcceleratingSun))
                            {
                                player.SpeedBoost();
                            }
                            else if (sun.GetType() == typeof(InvincibleSun))
                            {
                                player.Invinciblility();
                            }
                            else
                            {
                                player.AddToRainbowMeter(5 * sun.Size);      //The size is taken into account when adding to the meter
                                if (currentLevel.currentTutorial != null)
                                {
                                    if (currentLevel.currentTutorial.type == Constants.COLLISION_TUTORIAL)
                                    {
                                        ((CollisionTutorial)currentLevel.currentTutorial).CheckForCollision("Sun");
                                    }
                                }
                            }
                            sun.DisappearSun();
                        }
                    }
                }

                //Check for collisions with arrows
                foreach (Arrow arrow in currentLevel.arrows)
                {
                    if (!player.IsInvincible())
                    {
                        if (part.Hitbox.Intersects(arrow.Hitbox))
                        {
                            if (arrow.GetType() == typeof(SlowArrow))
                            {
                                player.Slow();
                                arrow.position = arrow.initialPosition;
                            }
                            else if (arrow.GetType() == typeof(InverseArrow))
                            {
                                player.Inverse();
                                arrow.position = arrow.initialPosition;
                            }
                            else if (arrow.GetType() == typeof(PoisonArrow))
                            {
                                player.Poison();
                                arrow.position = arrow.initialPosition;
                            }
                            else
                            {
                                player.AddToRainbowMeter(-10);
                                arrow.position = arrow.initialPosition;
                                //currentLevel.deadArrows.Add(arrow);
                            }
                        } 
                    }

                    if (arrow.Direction >= 0 && arrow.Direction <= 90)
                    {
                        if (arrow.position.X >= screenWidth + arrow.Source.Width/2 || arrow.position.Y >= screenHeight + arrow.Source.Height/2)
                        {
                            arrow.RestartPosition();
                        }
                    }
                    else if (arrow.Direction >= 90 && arrow.Direction <= 180)
                    {
                        if (arrow.position.X < 0 - arrow.Source.Width / 2 || arrow.position.Y >= screenHeight + arrow.Source.Height / 2)
                        {
                            arrow.RestartPosition();
                        }
                    }
                    else if (arrow.Direction >= 180 && arrow.Direction <= 270)
                    {
                        if (arrow.position.X < 0 - arrow.Source.Width / 2 || arrow.position.Y < 0 - arrow.Source.Height / 2)
                        {
                            arrow.RestartPosition();
                        }
                    }
                    else if (arrow.Direction >= 270 && arrow.Direction <= 360)
                    {
                        if (arrow.position.X >= screenWidth + arrow.Source.Width / 2 || arrow.position.Y < 0 - arrow.Source.Height / 2)
                        {
                            arrow.RestartPosition();
                        }
                    }



                }

                //currentLevel.RemoveArrows();
            }
        }


        public void PauseGame(Texture2D texture)
        {

            game.PauseGame(texture);
        }

        public void UnPause()
        {
            currentLevel.UnPause();
        }

        

    }
}
