#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace SpaceFly
{
    class Rock
    {
        public Vector2 position;
        public int health;
        public bool destroyed;

        public Rock(Vector2 position, int health, bool destroyed)
        {
            this.position = position;
            this.health = health;
            this.destroyed = destroyed;
        }

        public void Update() 
        {
            if (this.health <= 0)
            {
                this.destroyed = true;
            }
        }
    }
}
