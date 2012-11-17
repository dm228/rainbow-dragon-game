﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using RainbowDragon.HelperClasses;
using RainbowDragon.Core.Sprite;

namespace RainbowDragon.Core.Player
{
    class Dragon
    {
        public List<DragonPart> dragon;                     //The Dragon's body parts
        public DragonHead head;

        List<FollowingSprite> rainbow;                      //The sections of the rainbow

        Texture2D rainbowTexture;                           //Texture for the rainbow

        int bodySize;                                       //How many body sections the dragon has (does not include head & tail)
                                                            //Perhaps we can have the bodySize scale with the difficulty level?
        int rainbowMeter, maxRainbowMeter = 100;            //How much rainbow meter the player has, and the max rainbow meter
        int charge, maxCharge = 20;                         //How much the color blast is charged, and the max charge
        bool isCharging;                                    //Are we currently charging the blast?

        KeyboardState prevKeyState = new KeyboardState();   //Keyboard input; used to make sure the buttons are released
        GamePadState prevPadState = new GamePadState();     //Controller input; same deal

        float speedBoostTimer = 0;
        float invinciTimer = 0;
        float slowTimer = 0, poisonTimer = 0, inverseTimer = 0;
        float poiHitter = 0;

        bool invincible = false;

        ContentLoader contentLoader;

        /// <summary>
        /// Initializes all of our variables.
        /// Then, creates a head, however many body parts there are, and finally, the tail.
        /// </summary>
        public Dragon(int bodySize, ContentLoader loader)
        {
            this.bodySize = bodySize;
            dragon = new List<DragonPart>();
            rainbow = new List<FollowingSprite>();
            contentLoader = loader;
        }

        //public void LoadContent(ContentManager contentManager)
        public void Initialize(Vector2 position)
        {
            dragon.Add(new DragonHead(contentLoader.AddTexture2(Constants.DRAGON_HEAD, Constants.DRAGON_HEAD_PATH), position));
            head = (DragonHead)dragon[0];
            for (int i = 0; i < bodySize; i++)
            {
                dragon.Add(new DragonPart(dragon[i], 
                    contentLoader.AddTexture2(Constants.DRAGON_BODY, Constants.DRAGON_BODY_PATH), position));
            }
            dragon.Add(new DragonPart(dragon[bodySize], 
                contentLoader.AddTexture2(Constants.DRAGON_TAIL, Constants.DRAGON_TAIL_PATH), position));
            rainbowTexture = contentLoader.AddTexture2(Constants.RAINBOW_PART, Constants.RAINBOW_PART_PATH);
        }

        /// <summary>
        /// Calls the methods to handle our input, and to manage our rainbow trail
        /// Also calls each individual update for our dragon & rainbow trail
        /// </summary>
        public void Update(GameTime gameTime)
        {
            HandleInput();
            HandleRainbow();
            
            foreach (DragonPart part in dragon)
            {
                part.Update(gameTime);
            }

            foreach (FollowingSprite sec in rainbow)
            {
                sec.Update(gameTime);
            }
        }

        /// <summary>
        /// Calls the Draw functions for each piece of the dragon and our rainbow trail
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (DragonPart part in dragon)
            {
                part.Draw(spriteBatch);
            }

            foreach (FollowingSprite sec in rainbow)
            {
                sec.Draw(spriteBatch);
            }
        }


        private void HandleInput()
        {
            GamePadState aGamePad = GamePad.GetState(PlayerIndex.One);
            KeyboardState aKeyboard = Keyboard.GetState();

            //If we are currently charging...
            if (isCharging)
            {
                //If we are still holding down the fire button
                if (aKeyboard.IsKeyDown(Keys.Space) || aGamePad.Triggers.Right >= 0.25f)
                {
                    if (IsFullyCharged())
                    {
                        //We are fully charged

                    }
                    else if (rainbowMeter == 0)
                    {
                        //We are out of rainbow meter, and therefore we cannot continue charge the blast

                    }
                    else
                    {
                        //Charge up the Blast -- Charging uses up our rainbowMeter
                        charge++;
                        rainbowMeter--;
                    }
                }
                else
                {
                    //Fire Color Blast Thing -- The blast radius will be dependent upon the amount of charge that has been accrued


                    //We are no longer charging, so reset charge to 0
                    isCharging = false;
                    charge = 0;
                }
            }
            else if ((aKeyboard.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space)) ||
                (aGamePad.Triggers.Right >= 0.25f && prevPadState.Triggers.Right < 0.25f))
            {
                if (rainbowMeter < 10)
                {
                    //The player does not have enough meter to fire/charge a blast
                    //Do something to let the player know that they cannot fire a blast yet

                }
                else
                {
                    charge = 10;
                    rainbowMeter -= 10;
                    isCharging = true;
                }
            }

            prevKeyState = aKeyboard;
            prevPadState = aGamePad;
        }

        /// <summary>
        /// Manages our rainbow trail
        /// </summary>
        public void HandleRainbow()
        {
           int sections = rainbowMeter / 10;

            //If this occurs, then our rainbow trail is up-to-date
            if (sections == rainbow.Count) return;

            //If our rainbow is too long, make it shorter
            while (sections < rainbow.Count)
                rainbow.RemoveAt(rainbow.Count - 1);

            //If our rainbow is too short, then create new pieces for it
            while (sections > rainbow.Count)
            {
                FollowingSprite father;

                //The first part of our rainbow follows the dragon's tail
                if (rainbow.Count == 0)
                    father = dragon[dragon.Count - 1];
                else
                    father = rainbow[rainbow.Count - 1];

                rainbow.Add(new FollowingSprite(father, rainbowTexture, father.position, 0.2f, father.speed, father.rotation));
            }
        }

        public void AddToRainbowMeter(int amt)
        {
            if (rainbowMeter == 0 && amt < 0)
            {
                //We died!
                return;
            }

            rainbowMeter += amt;

            if (rainbowMeter > maxRainbowMeter)
                rainbowMeter = maxRainbowMeter;
            if (rainbowMeter < 0)
            {
                rainbowMeter = 0;
            }
        }

        public bool IsMeterFull()
        {
            return rainbowMeter == maxRainbowMeter;
        }

        public bool IsFullyCharged()
        {
            return charge == maxCharge;
        }

        //BUFFS
        public void SpeedBoost()
        {
            if (speedBoostTimer == 0)
                head.speed *= 2;
            speedBoostTimer = 7.5f;
        }

        public void Invinciblility()
        {
            if (invinciTimer == 0)
                invincible = true;
            invinciTimer = 3;
        }

        //DEBUFFS
        public void Slow()
        {
            if (slowTimer == 0)
                head.speed /= 2;
            slowTimer = 7.5f;
        }

        public void Inverse()
        {
            if (inverseTimer == 0)
                head.inversed = true;
            inverseTimer = 5;
        }

        public void Poison()
        {
            poisonTimer = 10;
            poiHitter = 1;
        }

        public void UpdateTimers(GameTime gameTime)
        {
            //Speed Boost Powerup
            if (speedBoostTimer > 0)
                speedBoostTimer -= gameTime.ElapsedGameTime.Seconds;
            if (speedBoostTimer < 0)
            {
                speedBoostTimer = 0;
                head.speed /= 2;
            }

            //Invincibility Powerup
            if (invinciTimer > 0)
                invinciTimer -= gameTime.ElapsedGameTime.Seconds;
            if (invinciTimer < 0)
            {
                invinciTimer = 0;
                invincible = false;
            }

            //Slow Debuff
            if (slowTimer > 0)
                slowTimer -= gameTime.ElapsedGameTime.Seconds;
            if (slowTimer < 0)
            {
                slowTimer = 0;
                head.speed *= 2;
            }

            //Inverse Debuff
            if (inverseTimer > 0)
                inverseTimer -= gameTime.ElapsedGameTime.Seconds;
            if (inverseTimer < 0)
            {
                inverseTimer = 0;
                head.inversed = false;
            }

            //Poison Debuff
            if (poisonTimer > 0)
                poisonTimer -= gameTime.ElapsedGameTime.Seconds;
            if (poisonTimer < 0)
            {
                poisonTimer = 0;
                poiHitter = 0;
            }

            if (poiHitter > 0)
                poiHitter -= gameTime.ElapsedGameTime.Seconds;
            if (poiHitter < 0)
            {
                AddToRainbowMeter(-2);
                if (poisonTimer > 0)
                    poiHitter = 1;
                else
                    poiHitter = 0;
            }
        }
    }
}
