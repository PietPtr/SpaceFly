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
        Texture2D rockPicdmg1;
        Texture2D rockPicdmg2;

        //Spaceship vectors and other variables
        Vector2 position;
        Vector2 velocity;

        double angle = Math.PI / 2;
        double speed = 0.0;
        float length;

        //Constants
        const int MAXSPEED = 8;
        const double ACCELERATION = 0.05;
        const double BRAKES = 0.1;
        const double TURNSPEED = 0.1;

        const int SCREENWIDTH = 1888;
        const int SCREENHEIGHT = 992;

        const int GENERATIONREACH = 1;

        //Fireball variables
        double bulletSpread = 20;

        //World matrix and list to hold existing rocks
        Rock[,] worldGrid = new Rock[118, 62];
        List<Vector2> rockList = new List<Vector2>();

        //Random number generator
        static Random random = new Random();

        //List with fireballs
        List<Fireball> fireList = new List<Fireball>();

        //Hold the state of the keyboard 1 frame ago
        KeyboardState oldKeyboardState;
        MouseState oldMouseState;

        //Hold the font
        SpriteFont font;

        public int randomInt(int low, int high)
        {
            int randomInteger = random.Next(low, high + 1);
            return randomInteger;
        }

        public void changeBlockCheck(int x, int y, int deltaX, int deltaY)
        {
            if (randomInt(0, 1) == 1)
            {
                worldGrid[x + deltaX, y + deltaY].destroyed = false;
                worldGrid[x + deltaX, y + deltaY].health = 10;

                rockList.Add(new Vector2(x + deltaX, y + deltaY));
            }
        }

        public void Generator()
        {
            //Select a random position to place the starting block.
            Vector2 randomPos = new Vector2(randomInt(1, 117), randomInt(1, 61));

            //make the block visible
            worldGrid[(int)randomPos.X, (int)randomPos.Y].destroyed = false;
            worldGrid[(int)randomPos.X, (int)randomPos.Y].health = 10;
            rockList.Add(randomPos);

            //limits the amount of rocks generated
            for (int i = 0; i < GENERATIONREACH; i++)
            {
                //Loop through the worldGrid matrix
                for (int listCount = rockList.Count - 1; listCount >= 0; listCount--)
                {
                    //coords in small vars for faster coding
                    int x = (int)rockList[listCount].X;
                    int y = (int)rockList[listCount].Y;

                    try
                    {
                        if (worldGrid[x, y].destroyed == false)
                        {
                            if (worldGrid[x + 1, y].destroyed == true)
                            {
                                changeBlockCheck(x, y, 1, 0);
                            }
                            else if (worldGrid[x - 1, y].destroyed == true)
                            {
                                changeBlockCheck(x, y, -1, 0);
                            }
                            else if (worldGrid[x, y + 1].destroyed == true)
                            {
                                changeBlockCheck(x, y, 0, 1);
                            }
                            else if (worldGrid[x, y - 1].destroyed == true)
                            {
                                changeBlockCheck(x, y, 0, -1);
                            }
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
                Console.Write(".");
            }
            Console.Write(" done!");
        }

        //Remove holes
        public void Smooth()
        {
            Console.Write("\nLOADING");

            //Loop through every block in the map matrix
            for (int x = 0; x < worldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < worldGrid.GetLength(1); y++)
                {
                    if (worldGrid[x, y].destroyed == true)
                    {
                        int surroundCount = 0;

                        try
                        {
                            if (worldGrid[x + 1, y].destroyed == false)
                            {
                                surroundCount++;
                            }
                            else if (worldGrid[x - 1, y].destroyed == false)
                            {
                                surroundCount++;
                            }
                            else if (worldGrid[x, y + 1].destroyed == false)
                            {
                                surroundCount++;
                            }
                            else if (worldGrid[x, y - 1].destroyed == false)
                            {
                                surroundCount++;
                            }
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            continue;
                        }

                        if (surroundCount >= 4)
                        {
                            worldGrid[x, y].destroyed = false;
                            worldGrid[x, y].health = 10;
                        }
                    }
                }
            }
        }

        public void fillWorldGrid()
        {
            //Create the rock objects
            for (int x = 0; x < worldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < worldGrid.GetLength(1); y++)
                {
                    worldGrid[x, y] = new Rock(new Vector2(x * 16, y * 16), 0, true);
                }
            }
        }

        public Main()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = SCREENHEIGHT;
            graphics.PreferredBackBufferWidth = SCREENWIDTH;

            //Make the mouse visible
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            fillWorldGrid();

            //Set a random position outside asteroids
            Vector2 randomPos = new Vector2(randomInt(0, 117), randomInt(0, 62));
            while (true)
            {
                if (worldGrid[(int)randomPos.X, (int)randomPos.Y].destroyed == true)
                {
                    position = new Vector2(randomPos.X * 16, randomPos.Y * 16);
                    break;
                }
                else
                {
                    randomPos = new Vector2(randomInt(0, 117), randomInt(0, 62));
                    continue;
                }
            }

            //Generate world
            int amountOfAsteroids = randomInt(15, 20);
            for (int i = 0; i <= amountOfAsteroids; i++)
            {
                Generator();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load the pictures
            spaceshipPic = Content.Load<Texture2D>("Player_ship");
            starsPic = Content.Load<Texture2D>("stars");
            firePic = Content.Load<Texture2D>("fireball");
            rockPic = Content.Load<Texture2D>("rock");
            rockPicdmg1 = Content.Load<Texture2D>("rockdmg1");
            rockPicdmg2 = Content.Load<Texture2D>("rockdmg2");

            //Load the font
            font = Content.Load<SpriteFont>("font");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            //Rotate the sprite relative to the mouse position
            angle = Math.Asin((Mouse.GetState().Position.Y - position.Y) / (Math.Sqrt(Math.Pow(Mouse.GetState().Position.X - position.X, 2) + Math.Pow(Mouse.GetState().Position.Y - position.Y, 2))));
            
           
            if (Mouse.GetState().Position.X < position.X)
            {
                angle = Math.PI - angle;
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

            //Reset and Reload the map when F5 is pressed
            if (Keyboard.GetState().IsKeyUp(Keys.F5) && oldKeyboardState.IsKeyDown(Keys.F5))
            {
                worldGrid = new Rock[118, 62];
                rockList = new List<Vector2>();

                fillWorldGrid();

                int amountOfAsteroids = randomInt(5, 15);
                for (int i = 0; i <= amountOfAsteroids; i++)
                {
                    Generator();
                }
            }

            //Shoot when space is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                for (int i = 0; i <= 1; i++)
                {
                    fireList.Add(new Fireball(position, velocity, speed + 10, angle, bulletSpread));
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
            
            //Bullet spread
            if (Mouse.GetState().ScrollWheelValue - oldMouseState.ScrollWheelValue < 0)
            {
                bulletSpread = Math.Pow(bulletSpread, 1.1);
            }
            else if (Mouse.GetState().ScrollWheelValue - oldMouseState.ScrollWheelValue > 0)
            {
                bulletSpread = Math.Pow(bulletSpread, 1.0 / 1.1);
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

            oldKeyboardState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();

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

            
            for (int x = 0; x < worldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < worldGrid.GetLength(1); y++)
                {
                    worldGrid[x, y].Update();

                    if (worldGrid[x, y].destroyed == false)
                    {
                        if (worldGrid[x, y].health > 8)
                        {
                            spriteBatch.Draw(rockPic, worldGrid[x, y].position, Color.White);
                        }
                        else if (worldGrid[x, y].health > 3 && worldGrid[x, y].health <= 7)
                        {
                            spriteBatch.Draw(rockPicdmg1, worldGrid[x, y].position, Color.White);
                        }
                        else if (worldGrid[x, y].health <= 3)
                        {
                            spriteBatch.Draw(rockPicdmg2, worldGrid[x, y].position, Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(rockPic, worldGrid[x, y].position, Color.White);
                        }
                    }
                }
            }

            //Debug text
            try
            {
                spriteBatch.DrawString(font, bulletSpread.ToString(), new Vector2(10, 12 * 0), Color.White);
            }
            catch (System.IndexOutOfRangeException)
            {

            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
