using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace myMiniGame
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			block = new bool[maxSize, maxSize];
			buffer = new bool[maxSize, maxSize];
			initBlocks(true);
			myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			timer1.Enabled = true;
		}

		void initBlocks(bool resetRules)
		{
			if (resetRules)	setRules();
			tickCount = 0;
			for (int i = 0; i <maxSize; i++)
			{
				for (int j = 0; j < maxSize; j++)
				{
					block[i, j] = (r.Next(2) == 0);
				}
			}

			for (int i = 0; i < maxSize; i++)
			{
				block[0, i] = false;
				block[maxSize-1, i] = false;
				block[i, 0] = false;
				block[i, maxSize-1] = false;
			}
		}

		void mainLoop()
		{
			timer1.Enabled = false;
			pictureBox1.Refresh();
		}

		private void setRules()
		{
			for (int i = 0; i < rule.GetLength(1); i++) 
			{
				rule[0, i] = r.Next(2) == 0;
				rule[1, i] = r.Next(2) == 0;
				//rule[0, i] = false; //uncomment for Conway's rules
				//rule[1, i] = false; //uncomment for Conway's rules
			}
			//rule[0, 2] = true;//uncomment for Conway's rules
			//rule[0, 3] = true;//uncomment for Conway's rules
			//rule[1, 3] = true;//uncomment for Conway's rules

			label1.Text = makeRuleString();
			maxTicks = Convert.ToInt16(textMaxTicks.Text);

			//07/345
			//012358/056
		}

		int maxTicks = 20;
		Bitmap myBitmap;
		Random r = new Random();
		int maxSize = 40;
		bool[,] block;
		long tickCount = 0;
		bool[,] rule = new bool[2,9];
		bool[,] buffer;
		//123
		//4 5
		//678

		int playerX = 1;
		int playerY = 1;

		private void cellTick()
		{
			for (int i = 1; i < maxSize - 1; i++)
			{
				for (int j = 1; j < maxSize - 1; j++)
				{
					int tSum = 0;
					if (block[i - 1, j - 1]) tSum++;
					if (block[i, j - 1]) tSum++;
					if (block[i + 1, j - 1]) tSum++;
					if (block[i - 1, j]) tSum++;
					if (block[i + 1, j]) tSum++;
					if (block[i - 1, j + 1]) tSum++;
					if (block[i, j + 1]) tSum++;
					if (block[i + 1, j + 1]) tSum++;
					//now we have the number of neighbors that are "On"
					if (block[i, j]) buffer[i, j] = rule[0, tSum];
					else buffer[i, j] = rule[1, tSum];
				}

			}
			// now everything is in the buffer, copy it back
			for (int i = 1; i < maxSize - 1; i++)
			{
				for (int j = 1; j < maxSize - 1; j++)
				{
					block[i, j] = buffer[i, j];
				}
			}
		}

		private void picRepaint(object sender, PaintEventArgs e)
		{
			tickCount++;
			Text = tickCount.ToString();
			if (tickCount < maxTicks) cellTick();
			int w = 16;
			//using (Graphics g = Graphics.)
			//{
			Graphics g = Graphics.FromImage(myBitmap);
				for (int i = 0; i < maxSize; i++)
				{
					for (int j = 0; j < maxSize; j++)
					{
						if (block[i, j]) g.FillRectangle(Brushes.Black, i * w, j * w, w, w);
						else g.FillRectangle(Brushes.White, i * w, j * w, w, w);
					}
				}

			g.FillEllipse(Brushes.Green, playerX * w, playerY * w, w, w);

			//}
			g.Dispose();

			pictureBox1.Image = myBitmap;
			//this.Text = r.Next(10).ToString();

			System.Threading.Thread.Sleep(33);

			timer1.Start();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			mainLoop();
		}


		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private string makeRuleString()
		{
			string s = "";
			for (int i = 0; i < 9; i++) if (rule[0, i]) s += i.ToString();
			if (s.Length == 0) s += "-";
			string t = "";
			for (int i = 0; i < 9; i++) if (rule[1, i]) t += i.ToString();
			if (t.Length == 0) t += "-";
			s += "/" + t;
			return s;
		}

		private void form_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space) initBlocks(true);
			if (e.KeyCode == Keys.Enter)
			{//save this setting, display in the console
				Console.WriteLine ("--------------- RULESET OF INTEREST ------------------");
				Console.WriteLine(makeRuleString());
			}
			if (e.KeyCode == Keys.Back) initBlocks(false);

			//player movement
			if (e.KeyCode == Keys.Down)
			{
				playerY++;
				if ((playerY >= maxSize) || (block[playerX, playerY])) playerY--;
				//check for collisions with background maze
			}
			if (e.KeyCode == Keys.Up)
			{
				playerY--;
				if ((playerY <= 0) || (block[playerX, playerY])) playerY++;
				//check for collisions with background maze

			}
			if (e.KeyCode == Keys.Right)
			{
				//check for collisions with background maze
				playerX++;
				if ((playerX >= maxSize) || (block[playerX, playerY])) playerX--;
			}
			if (e.KeyCode == Keys.Left)
			{
				//check for collisions with background maze
				playerX--;
				if ((playerX <= 0) || (block[playerX, playerY])) playerX++;
			}
		}



		private void bUp_Click(object sender, EventArgs e)
		{
			maxTicks++;
			textMaxTicks.Text = maxTicks.ToString();
		}

		private void bDown_Click(object sender, EventArgs e)
		{
			maxTicks++;
			maxTicks = Math.Max(maxTicks, 1);
			textMaxTicks.Text = maxTicks.ToString();
		}

		private void Form1_MouseClick(object sender, MouseEventArgs e)
		{
			Focus();
		}
	}
}
