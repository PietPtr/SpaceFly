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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //picture variables
        Texture2D spaceshipPic;
        Texture2D starsPic;
        Texture2D firePic;
        Texture2D rockPic;

        //Spaceship vectors and other variables
        Vector2 position = new Vector2(400, 240);
        Vector2 velocity;

        double angle = Math.PI / 2;
        double speed = 0.0;
        float length;

        //Constants
        int MAXSPEED = 8;
        double ACCELERATION = 0.05;
        double BRAKES = 0.1;
        double TURNSPEED = 0.1;

        int SCREENWIDTH = 1888;
        int SCREENHEIGHT = 992;

        //World matrix
        Rock[,] worldGrid = new Rock[118, 62];

        //Random number generator
        static Random rnd = new Random(987);

        //List with fireballs
        List<Fireball> fireList = new List<Fireball>();

        public Main()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = SCREENHEIGHT;
            graphics.PreferredBackBufferWidth = SCREENWIDTH;
        }

        protected override void Initialize()
        {
            //Generate world
            for (int x = 0; x < 118; x++)
            {
                for (int y = 0; y < 62; y++)
                {
                    if (x > 30 + (rnd.Next(3)))
                    {
                        worldGrid[x, y] = new Rock(new Vector2(x * 16, y * 16), rnd.Next(5, 10), false);
                    }
                    else
                    {
                        worldGrid[x, y] = new Rock(new Vector2(x * 16, y * 16), 0, true);
                    }
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spaceshipPic = Content.Load<Texture2D>("Player_ship");
            starsPic = Content.Load<Texture2D>("stars");
            firePic = Content.Load<Texture2D>("fireball");
            rockPic = Content.Load<Texture2D>("rock");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //rotate the sprite when the player presses A/D
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                angle = angle - TURNSPEED;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                angle = angle + TURNSPEED;
            }
            
            //Accelerate when W is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (!(speed >= MAXSPEED))
                {
                    speed = speed + ACCELERATION;
                }
                else if (speed > MAXSPEED)
                {
                    speed = MAXSPEED;
                }
            }
            //brake when W is not pressed
            else if (Keyboard.GetState().IsKeyUp(Keys.W))
            {
                if (! (speed <= 0))
                {
                    speed = speed - BRAKES;
                }
                else if (speed < 0)
                {
                    speed = 0;
                }
            }

            //Shoot when space is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                for (int i = 0; i <= 1; i++)
                {
                    fireList.Add(new Fireball(position, velocity, speed + 10, angle));
                }
            }

            //Debugging
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                Console.WriteLine(fireList.Count);
            }

            //update velocity
            velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            //update the position by adding the velocity to the position vectors
            position.X = position.X + ((float)speed * velocity.X);
            position.Y = position.Y + ((float)speed * velocity.Y);

            //Keep the player on the screen
            if (position.X >= SCREENWIDTH)
            {
                position.X = SCREENWIDTH;
            }
            else if (position.Y >= SCREENHEIGHT)
            {
                position.Y = SCREENHEIGHT;
            }
            else if (position.X < 0)
            {
                position.X = 0;
            }
            else if (position.Y < 0)
            {
                position.Y = 0;
            }
            
            //Collisions between rocks and the player
            if ((int)position.X / 16 < 118 && (int)position.Y / 16 < 62)
            {
                if (worldGrid[(int)position.X / 16, (int)position.Y / 16].destroyed == false)
                {
                    position.X = position.X - ((float)speed * velocity.X);
                    position.Y = position.Y - ((float)speed * velocity.Y);

                    speed = 0;
                }
            }

            //update the fireballs, and check if they are on screen
            List<int> removalList = new List<int>();

            for (int i = fireList.Count - 1; i >= 0; i--)
            {
                fireList[i].Update();

                //delete fire outside screen
                if (fireList[i].position.X >= SCREENWIDTH || fireList[i].position.X <= 0 || fireList[i].position.Y >= SCREENHEIGHT || fireList[i].position.Y <= 0)
                {
                    fireList.RemoveAt(i);
                }

                //reduce health of rocks when hit
                if (i < fireList.Count - 1 && i > 0)
                {
                    worldGrid[(int)(fireList[i].position.X / (float)16), (int)(fireList[i].position.Y / (float)16)].health--;
                    //remove fire if it has hit an undestroyed rock
                    if (worldGrid[(int)(fireList[i].position.X / (float)16), (int)(fireList[i].position.Y / (float)16)].destroyed == false)
                    {
                        fireList.RemoveAt(i);
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Create the sourcerectangle for drawing the ship
            Rectangle sourceRectangleSpaceship = new Rectangle(0, 0, spaceshipPic.Width, spaceshipPic.Height);
            Rectangle sourceRectangleFireball = new Rectangle(0, 0, firePic.Width, firePic.Height);

            //Rotates arounds the middle
            Vector2 origin = new Vector2(spaceshipPic.Width / 2, spaceshipPic.Height / 2);
            Vector2 originFireball = new Vector2(firePic.Width / 2, firePic.Height / 2);

            //drawing
            spriteBatch.Begin();

            spriteBatch.Draw(starsPic, new Vector2(0, 0), Color.Wheat);
            
            foreach (Fireball fireball in fireList) 
            {
                spriteBatch.Draw(firePic, fireball.position, sourceRectangleFireball, Color.White, (float)fireball.angle + (float)Math.PI / 2, originFireball, 1.0f, SpriteEffects.None, 1);
            }

            spriteBatch.Draw(spaceshipPic, position, sourceRectangleSpaceship, Color.White, (float)angle + (float)Math.PI / 2, origin, 1.0f, SpriteEffects.None, 1);

            
            for (int x = 0; x < 118; x++)
            {
                for (int y = 0; y < 62; y++)
                {
                    worldGrid[x, y].Update();

                    if (worldGrid[x, y].destroyed == false)
                    {
                        spriteBatch.Draw(rockPic, worldGrid[x, y].position, Color.Wheat);
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
