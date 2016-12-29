using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


//C# Program made by Leo Woiceshyn 
namespace WindowsFormsApplication1
{
    public partial class myGame : Form
    {
        public myGame()
        {
            InitializeComponent();
        }

        // Defining state machine integer for serial port data as well as concurrent queues to store the X, Y, and Z acceleration data bytes.
        public int systemState = 0;
        public ConcurrentQueue<int> XAccelQueue = new ConcurrentQueue<int>();
        public ConcurrentQueue<int> YAccelQueue = new ConcurrentQueue<int>();
        public ConcurrentQueue<int> ZAccelQueue = new ConcurrentQueue<int>();

        //Defining bitmaps and graphics for the game background, textures, and sprites
        Bitmap creature;
        Bitmap background;
        Bitmap texture;
        Bitmap jewel;
        Graphics g;
        Graphics scG;
        

        //Defining the rectangles that will make up Pete, the terrain, jewels, and maze walls
        Rectangle area;
        Rectangle[] rects;
        Rectangle[] terrainrects;
        Rectangle[] walls;

        //Defining the timers for the in-game time, the collisions, and moving Pete
        Timer t;
        Timer collision;
        Timer gtimer;
        

        //Defining the four directional booleans for moving Pete
        bool right;
        bool left;
        bool up;
        bool down;

        //Defining the four directional booleans related to collisions with the walls
        bool collide_r = false;
        bool collide_t = false;
        bool collide_b = false;
        bool collide_l = false;

        //Defining the current amount of jewels that Pete has collected
        int collectedNumber = 0;
        
        //Form Load Event Handler
        private void Form1_Load(object sender, EventArgs e)
        {
            //Recognizing anything connected to the serial ports. See definition of this function below.
            ConnectedComPortUpdate();

            //Defining the creature(Pete) bitmap as an external image and making him transparent in the game
            creature = new Bitmap(@"C:\Users\glenn\Documents\Visual Studio 2010\Projects\Mech 368 Creative Component - Leo Woiceshyn\Pictures\piratepete.png");
            creature.MakeTransparent(Color.White);

            //Defining the jewel bitmap as an external image and making them transparent in the game
            jewel = new Bitmap(@"C:\Users\glenn\Documents\Visual Studio 2010\Projects\Mech 368 Creative Component - Leo Woiceshyn\Pictures\jewel.png");
            jewel.MakeTransparent(Color.White);

            //Defining the background texture for the maze as an external image 
            texture = new Bitmap(@"C:\Users\glenn\Documents\Visual Studio 2010\Projects\Mech 368 Creative Component - Leo Woiceshyn\Pictures\texture2.png");

            //Defining the background as a new bitmap that is the height and width of the form
            background = new Bitmap(this.Width, this.Height);

            //Defines Pete's dimensions and starting location as in the centre of the background
            area = new Rectangle(this.Width/2, this.Height/2, 25, 25);

            //Defining the amount of rectangles for the walls, terrain and jewels
            rects = new Rectangle[20];
            terrainrects = new Rectangle[1000];
            walls = new Rectangle[120];

            //Initializes the rectangles onto the background; see definitions of these functions below.
            MakeRect();
            MakeTerrain();
            MakeWalls();

            //Creates the system graphics for this form
            g = this.CreateGraphics();
            //Calls the drawing surface from the background bitmap
            scG = Graphics.FromImage(background);

            //Defines the properties of the three code-defined timers
            t = new Timer();
            collision = new Timer();
            gtimer = new Timer();

            t.Interval = 10;
            t.Tick += new EventHandler(t_Tick);
            t.Start();

            gtimer.Interval = 1000;
            gtimer.Tick += new EventHandler(gtimer_Tick);
          

            collision.Interval = 10;
            collision.Tick += new EventHandler(collision_Tick);
            collision.Start();

            //Starts the game with an initial explanation of Pete's circumstances and how to begin playing.
            MessageBox.Show("Welcome to Pirate Pete's Jewel Hunt! Pete has been stranded by his crew in a strange maze and his bounty of jewels has been scattered throughout the maze. Help Pete collect the jewels before the timer runs out and the maze explodes! Find your tinystick from the list of COM ports and connect it to begin.","Pirate Pete's Jewel Hunt");

            

        }


        //ConnectedComPortUpdate method definition.
        private void ConnectedComPortUpdate()
        {
            try
            {
                //Clears COM List
                cmbPortName.Items.Clear();

                //Accesses System Port Information and Adds it to the ComboBox
                cmbPortName.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames().ToArray());

                //Selects the last and "first" device
                cmbPortName.SelectedIndex = 0;
            }
            
            //Recognized if there are no COM ports connected to the computer instead of crashing the program.
            catch (ArgumentNullException)
            {
                MessageBox.Show("Please ensure there is a COM port connected!");
            }
         }


        //Method for the t timer which moves the rectangles up based on the value of the directional booleans.
        void t_Tick(object sender, EventArgs e)
        {
            if (up == true)
            {
                MoveRectangle(Direction.Up);
            }
            if (left == true)
            {
                MoveRectangle(Direction.Left);
            }
            if (down == true)
            {
                MoveRectangle(Direction.Down);
            }
            if (right == true)
            {
                MoveRectangle(Direction.Right);
            }


            g.DrawImage(Draw(), new Point(0, 0));

            //Checks if the rectangle of Pete intersects with the jewel rectangles and if he does, 
            //it removes that rectangle and increased the collected number by 1.
            for (int i = 0; i < rects.Length; i++)
            {
                if (area.IntersectsWith(rects[i]))
                {
                    collectedNumber++;
                    rects[i] = new Rectangle(-100, -100, 1, 1);
                }
            }
        }

        //Defines the game time to be 100 seconds.
        public int gameTime = 100;
        //Method definition for gtimer, which tracks the time left until the player loses the game.
        void gtimer_Tick(object sender, EventArgs e)
        {
            //Reduces the game time a second every tick.
            gameTime--;

            //Says that if the player has collected 20 jewels, he wins the game and a message box is shown.
            if (collectedNumber >= 20)
            {
                gtimer.Stop();
                var resultwin = MessageBox.Show(
                    "Congratulations matey, you have collected all the jewels and completed Pirate Pete's Bounty!                                 Play again?",
                    "You win!",
                    MessageBoxButtons.YesNo);
                //If he chooses no to playing again, the form closes. If he chooses yes, the form restarts.
                if (resultwin == DialogResult.No)
                {
                    Application.Exit();
                }
                else
                {
                    Application.Restart();

                }
            }

            //Says that if the game time reaches 0, the player loses the game and a message box is shown.
            if (gameTime == 0) 
            {
                gtimer.Stop();
                var resultlose = MessageBox.Show(
                    "Arrrrr! Your time be up, and Pete has been blown to smithereens. Nice going, scallywag....                                                           Play again?", 
                    "You Lose!",
                    MessageBoxButtons.YesNo);

                //If he chooses no to playing again, the form closes. If he chooses yes, the form restarts.
                if (resultlose == DialogResult.No)
                {
                    this.Close();
                }
                else
                {
                    Application.Restart();

                }

                    
            }
        }

        //Method definition for the collision tick
        void collision_Tick(object sender, EventArgs e)
        {
            collide_r = false;
            collide_t = false;
            collide_b = false;
            collide_l = false;

            //If Pete collides with one of the four walls, checks which collision it is 
            //and prevents movement in the approprate direction depending on his movement 
            //direction and the wall direction relative to him
            for (int i = 0; i < walls.Length; i++)
            {
                if (walls[i] != null)
                {
                    if (walls[i].IntersectsWith(area))
                    {
                        if (walls[i].Left < area.Right && area.X < walls[i].Left)
                        {
                            collide_r = true;
                        }

                        if (walls[i].Right > area.Left && area.X + area.Width > walls[i].Right)
                        {
                            collide_l = true;
                        }

                        if (walls[i].Bottom > area.Y && area.Y + area.Height > walls[i].Bottom)
                        {
                            collide_t = true;
                        }

                        if (walls[i].Top < area.Y + area.Height && area.Y < walls[i].Top)
                        {
                            collide_b = true;
                        }
                    }
                }
            }
        }
        //Defines an enumerator for the direction of Pete
        public enum Direction {Up, Down, Right, Left};

        //Method definition for the movement of all rectangles that aren't Pete(jewels, walls, texture)
        //relative to him; Reason is for Pete to stay centered on the screen, and allows for a bigger map
        //for him to move on.
        public void MoveRectangle(Direction d)
        {
            //Defines the speed of all the graphical components
            int speed = 6;
            //Moves the jewels
            for (int i = 0; i < rects.Length; i++)
            {
                if (d == Direction.Down  && !collide_b) { rects[i].Y -= speed; }
                if (d == Direction.Up && !collide_t) { rects[i].Y += speed; }
                if (d == Direction.Right && !collide_r) { rects[i].X -= speed; }
                if (d == Direction.Left && !collide_l) { rects[i].X += speed; }
            }
            //Moves the background texture
            for (int i = 0; i < terrainrects.Length; i++)
            {
                if (d == Direction.Down && !collide_b) { terrainrects[i].Y -= speed; }
                if (d == Direction.Up && !collide_t) { terrainrects[i].Y += speed; }
                if (d == Direction.Right && !collide_r) { terrainrects[i].X -= speed; }
                if (d == Direction.Left && !collide_l) { terrainrects[i].X += speed; }
            }
            
            //Moves the walls
            for (int i = 0; i < walls.Length; i++)
            {
                if (d == Direction.Down && !collide_b) { walls[i].Y -= speed; }
                if (d == Direction.Up && !collide_t) { walls[i].Y += speed; }
                if (d == Direction.Right && !collide_r) { walls[i].X -= speed; }
                if (d == Direction.Left && !collide_l) { walls[i].X += speed; }
            }
        }

        //Creates the background terrain on a grid of rectangles that are defined across the entire form.
        public void MakeTerrain()
        {
            int rectPosition = 0;

            for (int x = 0; x < this.Width; x += 40)
            {
                for (int y = 0; y < this.Height; y += 40)
                {
                    terrainrects[rectPosition++] = new Rectangle(x, y, 40, 40);
                }
            }
        }

        //Creates the walls and outside border of the maze
        public void MakeWalls()
        {
            //The four outside walls
            walls[0] = new Rectangle(-10,-10,this.Width + 30, 10); //Top Wall
            walls[1] = new Rectangle(-10, this.Height + 30, this.Width +40, 10); //Bottom Wall
            walls[2] = new Rectangle(-10, -10, 10, this.Height + 40); //Left wall
            walls[3] = new Rectangle(this.Width + 20, -10, 10, this.Height + 40); //Right Wall

            //The Maze 

            //Vertical Walls from Right to Left
            walls[4] = new Rectangle(this.Width - 100, -10, 10, 100);
            walls[5] = new Rectangle(this.Width - 40, 40, 10, 50);
            walls[6] = new Rectangle(this.Width - 220, -10, 10, 100);
            walls[7] = new Rectangle(this.Width - 100, 80, 60, 10);
            walls[8] = new Rectangle(this.Width - 210, 40, 40, 10);
            walls[9] = new Rectangle(this.Width - 20, 130, 40, 10);
            walls[10] = new Rectangle(this.Width - 20, 230, 40, 10);
            walls[11] = new Rectangle(this.Width - 20, 330, 40, 10);
            walls[12] = new Rectangle(this.Width - 20, 380, 40, 10);
            walls[13] = new Rectangle(this.Width - 20, 460, 40, 10);
            walls[99] = new Rectangle(this.Width - 20, 620, 40, 10);
            walls[14] = new Rectangle(this.Width - 100, 180, 10, 40);
            walls[15] = new Rectangle(this.Width - 100, 280, 10, 140);
            walls[16] = new Rectangle(this.Width - 100, 520, 10, 110);
            walls[68] = new Rectangle(this.Width - 30, 620, 10, 50);
            walls[17] = new Rectangle(this.Width - 30, 520, 10, 50);
            walls[18] = new Rectangle(this.Width - 100, 520, 10, 110);
            walls[19] = new Rectangle(this.Width - 220, 140, 10, 190);
            walls[20] = new Rectangle(this.Width - 220, 420, 10, 150);
            walls[21] = new Rectangle(this.Width - 160, 470, 10, 50);
            walls[22] = new Rectangle(this.Width - 160, 580, 10, 50);
            walls[23] = new Rectangle(this.Width - 160, 320, 10, 50);
            // walls[98] = new Rectangle(this.Width - 160, 220, 10, 50);
            walls[24] = new Rectangle(this.Width - 280, 50, 10, 265);
            walls[25] = new Rectangle(this.Width - 280, 380, 10, 100);
            walls[26] = new Rectangle(this.Width - 280, 570, 10, 100);
            walls[101] = new Rectangle(this.Width - 200, 620, 10, 100);
            walls[27] = new Rectangle(this.Width - 340, -10, 10, 160);
            walls[28] = new Rectangle(this.Width - 340, 520, 10, 50);
            walls[29] = new Rectangle(this.Width - 340, 670, 10, 50);
            walls[30] = new Rectangle(this.Width - 400, 40, 10, 160);
            walls[31] = new Rectangle(this.Width - 400, 470, 10, 100);
            walls[32] = new Rectangle(this.Width - 460, 150, 10, 100);
            walls[33] = new Rectangle(this.Width - 460, 470, 10, 50);
            walls[34] = new Rectangle(this.Width - 460, 570, 10, 50);
            walls[35] = new Rectangle(this.Width - 460, 670, 10, 50);
            walls[36] = new Rectangle(this.Width - 520, 620, 10, 50);
            walls[37] = new Rectangle(this.Width - 520, 520, 10, 50);
            walls[38] = new Rectangle(this.Width - 520, 150, 10, 50);
            walls[39] = new Rectangle(this.Width - 580, 100, 10, 420);
            walls[40] = new Rectangle(this.Width - 580, 620, 10, 50);
            walls[41] = new Rectangle(this.Width - 640, 250, 10, 140);
            walls[42] = new Rectangle(this.Width - 640, 520, 10, 50);
            walls[43] = new Rectangle(this.Width - 700, -10, 10, 150);
            walls[44] = new Rectangle(this.Width - 700, 200, 10, 50);
            walls[45] = new Rectangle(this.Width - 700, 340, 10, 89);
            walls[46] = new Rectangle(this.Width - 700, 480, 10, 40);
            walls[47] = new Rectangle(this.Width - 700, 570, 10, 50);
            walls[102] = new Rectangle(this.Width - 650, 670, 10, 50);
            walls[48] = new Rectangle(this.Width - 760, 470, 10, 90);
            walls[49] = new Rectangle(this.Width - 760, 620, 10, 50);
            walls[50] = new Rectangle(this.Width - 760, 250, 10, 100);
            walls[51] = new Rectangle(this.Width - 760, 40, 10, 50);
            walls[52] = new Rectangle(this.Width - 820, -10, 10, 50);
            walls[53] = new Rectangle(this.Width - 820, 200, 10, 100);
            walls[54] = new Rectangle(this.Width - 820, 350, 10, 50);
            walls[63] = new Rectangle(this.Width - 30, 130, 10, 60);
            walls[69] = new Rectangle(this.Width - 160, 230, 10, 40);



            // Vertical Walls from Top to Bottom
            walls[55] = new Rectangle(this.Width - 150, 80, 100, 10);
            walls[56] = new Rectangle(this.Width - 640, 40, 240, 10);
            walls[57] = new Rectangle(this.Width - 640, 90, 240, 10);
            walls[58] = new Rectangle(this.Width - 900, 90, 150, 10);
            walls[58] = new Rectangle(this.Width - 900, 90, 150, 10);
            walls[59] = new Rectangle(this.Width - 220, 130, 150, 10);
            walls[60] = new Rectangle(this.Width - 820, 140, 240, 10);
            walls[61] = new Rectangle(this.Width - 520, 140, 70, 10);
            walls[62] = new Rectangle(this.Width - 400, 190, 50, 10);
            walls[64] = new Rectangle(this.Width - 220, 180, 190, 10);
            walls[65] = new Rectangle(this.Width - 820, 190, 180, 10);
            walls[66] = new Rectangle(this.Width - 580, 240, 300, 10);
            walls[67] = new Rectangle(this.Width - 160, 270, 120, 10);
            walls[70] = new Rectangle(this.Width - 690, 290, 50, 10);
            walls[71] = new Rectangle(this.Width - 810, 290, 50, 10);
            walls[72] = new Rectangle(this.Width - 210, 320, 50, 10);
            walls[73] = new Rectangle(this.Width - 750, 340, 50, 10);
            walls[74] = new Rectangle(this.Width - 280, 370, 60, 10);
            walls[75] = new Rectangle(this.Width - 640, 390, 60, 10);
            walls[76] = new Rectangle(this.Width - 820, 390, 60, 10);
            walls[77] = new Rectangle(this.Width - 900, 430, 260, 10);
            walls[78] = new Rectangle(this.Width - 900, 340, 90, 10);
            walls[79] = new Rectangle(this.Width - 220, 410, 160, 10);
            walls[80] = new Rectangle(this.Width - 160, 460, 150, 10);
            walls[81] = new Rectangle(this.Width - 700, 470, 420, 10);
            walls[82] = new Rectangle(this.Width - 100, 510, 80, 10);
            walls[83] = new Rectangle(this.Width - 340, 510, 60, 10);
            walls[84] = new Rectangle(this.Width - 690, 510, 60, 10);
            walls[85] = new Rectangle(this.Width - 900, 510, 50, 10);
            walls[86] = new Rectangle(this.Width - 900, 560, 210, 10);
            walls[87] = new Rectangle(this.Width - 630, 560, 180, 10);
            walls[88] = new Rectangle(this.Width - 390, 560, 50, 10);
            walls[89] = new Rectangle(this.Width - 280, 560, 60, 10);
            walls[90] = new Rectangle(this.Width - 200, 620, 100, 10);
            walls[91] = new Rectangle(this.Width - 510, 620, 230, 10);
            walls[92] = new Rectangle(this.Width - 700, 620, 120, 10);
            walls[93] = new Rectangle(this.Width - 810, 620, 50, 10);
            walls[94] = new Rectangle(this.Width - 110, 670, 90, 10);
            walls[95] = new Rectangle(this.Width - 370, 670, 30, 10);
            walls[96] = new Rectangle(this.Width - 760, 670, 120, 10);
            walls[97] = new Rectangle(this.Width - 900, 670, 50, 10);


        }

        //Generates the 20 jewels at random locations on the map.
        public Rectangle[] MakeRect()
        {
            Random r = new Random();

            for (int i = 0; i < rects.Length; i++)
            {
                rects[i] = new Rectangle(r.Next(0, this.Width), r.Next(0, this.Height), 20, 20);
            }

            return rects;
        }

        //Draws the background, map, maze, jewels, creature, as well as the timer and jewel count onto the screen.
        public Bitmap Draw()
        {
            scG.Clear(Color.FromArgb(255, Color.AliceBlue)); 

            for (int i = 0; i < terrainrects.Length; i++)
            {
                scG.DrawImage(texture, terrainrects[i]);
            }
            scG.FillRectangles(Brushes.Black, walls); 
            for (int j = 0; j < rects.Length; j++)
            {
                scG.DrawImage(jewel, rects[j]);
            }
            scG.DrawImage(creature, area);
            scG.DrawString(collectedNumber.ToString(),new Font("Arial", 15), Brushes.Magenta, new PointF(850, 10));
            scG.DrawString(gameTime.ToString(), new Font("Arial", 15), Brushes.Magenta, new PointF(700, 10));
            scG.DrawString(":", new Font("Arial", 16), Brushes.Magenta, new PointF(840, 10));
            scG.DrawString("Time Remaining:", new Font("Arial", 16), Brushes.Magenta, new PointF(490, 7));
            scG.DrawImage(jewel, new PointF(820,10));
            return background;
        }

        //Event handler for data received from the serial port.
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            //Databyte integer
            int dataByte;


            //While the serial port is open and there are bytes to read, runs a state machine to get the acceleration values
            while (serialPort.IsOpen && serialPort.BytesToRead != 0)
            {
                try
                {
                    dataByte = serialPort.ReadByte();
                }
                catch
                {
                    dataByte = 0;
                }

                if (dataByte == 255)
                {
                    systemState++;
                }

                else if (systemState == 1)
                {
                    XAccelQueue.Enqueue(dataByte);
                    systemState++;


                }

                else if (systemState == 2)
                {
                    YAccelQueue.Enqueue(dataByte);
                    systemState++;

                }

                else if (systemState == 3)
                {
                    ZAccelQueue.Enqueue(dataByte);
                    systemState = 0;
                }
            }

        }

        //Event handler for clicking the COM port connection button.
        private void butPortState_Click(object sender, EventArgs e)
        {
            //If the serial port is not currently open, opens it, starts the event timer, game timer, and changes the
            //text of the button to Disconnect
            if (!serialPort.IsOpen)
            {

                cmbPortName.Enabled = false;
                eventTimer.Enabled = true;
                serialPort.PortName = cmbPortName.Text;
                butPortState.Text = "Disconnect";
                serialPort.Open();
                gtimer.Start();

            }
            //If the serial port is open, closes it, stops the event timer, the game timer, and changes the text
            // of the port state button to Connect
            else
            {
                if (serialPort.IsOpen)
                {

                    cmbPortName.Enabled = true;
                    eventTimer.Enabled = false;
                    butPortState.Text = "Connect";
                    serialPort.Close();
                    gtimer.Stop();
                }
            }
        }

        //Defines the three directional acceleration values as public integers so they can be accessed from other
        //events if needed.
        public int XAccel;
        public int YAccel;
        public int ZAccel;

        //Event handler for the eventTimer's tick event 
        private void eventTimer_Tick(object sender, EventArgs e)
        {
            while (XAccelQueue.Count != 0 && YAccelQueue.Count != 0 && ZAccelQueue.Count != 0)
            {

                //Tries to dequeue a value out of the three concurrent queues every timer tick.
                XAccelQueue.TryDequeue(out XAccel);               
                YAccelQueue.TryDequeue(out YAccel);
                ZAccelQueue.TryDequeue(out ZAccel);

                //Changes the directional booleans of Pete based on the orientation of the tinystick.
                //8 possible directions: up, left, down, right, up left, up right, down left, down right.
                if (XAccel < 110)
                {
                    up = true;
                }
                else
                {
                    up = false;
                }
                if (XAccel > 140)
                {
                    down = true;
                }
                else
                {
                    down = false;
                }
                if (YAccel >145)
                {
                    left = true;
                }
                else
                {
                    left = false;
                }
                if (YAccel < 100)
                {
                    right = true;
                }
                else
                {
                    right = false;
                }
                if (XAccel < 110 && YAccel > 145 && ZAccel < 145 )
                {
                    up = true;
                    left = true;
                }
                if (XAccel > 105 && YAccel < 110 && ZAccel < 120)
                {
                    up = true;
                    right = true;
                }
                if (XAccel > 135 && YAccel > 145 && ZAccel < 110)
                {
                    down = true;
                    left = true;
                }
                if (XAccel > 125 && YAccel < 110 && ZAccel < 110)
                {
                    down = true;
                    right = true;
                }


            }



            
        }

        
    }
}
