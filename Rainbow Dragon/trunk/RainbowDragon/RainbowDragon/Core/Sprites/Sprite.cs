﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace RainbowDragon.Core.Sprites
{
    class Sprite
    {
        protected Texture2D texture;

        public Vector2 position;

        public Rectangle size;

        float scale = 1;
        Rectangle source;

        public Texture2D Texture { get { return texture; } set { texture = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                size = new Rectangle(0, 0, (int)(source.Width * scale), (int)(source.Height * scale));
            }
        }

        
        public Rectangle Source
        {
            get { return source; }
            set
            {
                source = value;
                size = new Rectangle(0, 0, (int)(source.Width * scale), (int)(source.Height * scale));
            }
        }

        public Sprite(Texture2D texture, Vector2 position, float scale)
        {
            this.texture = texture;
            this.position = position;
            this.scale = scale;
            Source = new Rectangle(0, 0, (int)(texture.Width * scale), (int)(texture.Height * scale));
        }

        public Sprite()
        {

        }


        //I am making this method virtual because depending on the type of sprite, you might need to do more complicated things
        //like rotation or scaling.
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        //Update method is not included because at its core a sprite shouldn't need update. Other types of sprites, like the ones that move,
        //will have their own update method.



    }
}
