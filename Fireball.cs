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
    class Fireball
    {
        //variables for the fireball
        public Vector2 position;
        public Vector2 velocity;
        public double speed;
        public double angle;
        public double bulletSpread;

        static Random random = new Random(500);

        //Random integer method
        public int randomInt(int low, int high)
        {
            int randomInteger = random.Next(low, high + 1);
            return randomInteger;
        }

        //Constructor
        public Fireball(Vector2 position, Vector2 velocity, double speed, double angle, double bulletSpread)
        {
            this.position = position;
            this.velocity = velocity;
            this.speed = speed;
            this.angle = angle;
            this.bulletSpread = bulletSpread;

            //Randomize spread
            this.velocity.X = this.velocity.X + (((float)random.NextDouble() - (float)0.5) / (float)bulletSpread);
            this.velocity.Y = this.velocity.Y + (((float)random.NextDouble() - (float)0.5) / (float)bulletSpread);
        }

        public void Update() 
        {
            //update the position of the fireball
            this.position.X = this.position.X + ((float)this.speed * this.velocity.X);
            this.position.Y = this.position.Y + ((float)this.speed * this.velocity.Y);
        }
    }
}
