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
			for (int i = 0; i < 9; i++)
			{
				rule[0, i] = r.Next(2) == 0;
				rule[1, i] = r.Next(2) == 0;
				//rule[0, i] = false; //uncomment for Conway's rules
				//rule[1, i] = false; //uncomment for Conway's rules
			}
			//rule[0, 2] = true;//uncomment for Conway's rules
			//rule[0, 3] = true;//uncomment for Conway's rules
			//rule[1, 3] = true;//uncomment for Conway's rules

		}


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
			if (tickCount < 20)	cellTick();
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

			//}
			g.Dispose();

			pictureBox1.Image = myBitmap;
			//this.Text = r.Next(10).ToString();

			/*
			block = new bool[maxSize, maxSize];
			for (int i = 0; i < maxSize; i++)
			{
				for (int j = 0; j < maxSize; j++)
				{
					block[i, j] = (r.Next(2) == 0);
				}
			}
			*/
			
			for (long i = 0; i < 10000000; i++)
			{
				int j = 0;
			}

			timer1.Start();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			mainLoop();
		}


		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void form_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space) initBlocks(true);
			if (e.KeyCode == Keys.Enter)
			{//save this setting, display in the console
				Console.WriteLine ("--------------- RULESET OF INTEREST ------------------");
				string s = "";
				for (int i = 0; i < 9; i++) s += rule[0, i].ToString() + ", ";
				Console.WriteLine("Rule ALIVE = " + s);
				s = "";
				for (int i = 0; i < 9; i++) s += rule[0, i].ToString() + ", ";
				Console.WriteLine("Rule DEAD = " + s);
			}
			if (e.KeyCode == Keys.Back) initBlocks(false);
		}
	}
}
