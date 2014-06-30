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

        static Random rnd = new Random(500);

        //Constructor
        public Fireball(Vector2 position, Vector2 velocity, double speed, double angle)
        {
            this.position = position;
            this.velocity = velocity;
            this.speed = speed;
            this.angle = angle;

            //Randomize spread
            this.velocity.X = this.velocity.X + (((float)rnd.NextDouble() - (float)0.5) / (float)10);
            this.velocity.Y = this.velocity.Y + (((float)rnd.NextDouble() - (float)0.5) / (float)10);
        }

        public void Update() 
        {
            //update the position of the fireball
            this.position.X = this.position.X + ((float)this.speed * this.velocity.X);
            this.position.Y = this.position.Y + ((float)this.speed * this.velocity.Y);
        }
    }
}
