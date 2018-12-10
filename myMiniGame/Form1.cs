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
		int maxEnemies = 4;
		int[] enemyX, enemyY, targetX, targetY;
		int[] enemyTrapped;
		int enemyTimer = 0;

		int maxTicks = 20;
		Bitmap myBitmap;
		Random r = new Random();
		int maxSize = 40;
		bool[,] block;
		long tickCount = 0;
		bool[,] rule = new bool[2, 9];
		bool[,] buffer;
		//123
		//4 5
		//678

		int playerX = 1;
		int playerY = 1;



		public Form1()
		{
			InitializeComponent();
			block = new bool[maxSize, maxSize];
			buffer = new bool[maxSize, maxSize];
			initBlocks(true);
			myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			enemyX = new int[maxEnemies];
			enemyY = new int[maxEnemies];
			targetX = new int[maxEnemies];
			targetY = new int[maxEnemies];
			enemyTrapped = new int[maxEnemies];
			initEnemies();
		}

		void initEnemies()
		{
			//reset enemy position
			enemyX[0] = 10; enemyY[0] = 10;
			enemyX[1] = 20; enemyY[1] = 10;
			enemyX[2] = 20; enemyY[2] = 20;
			enemyX[3] = 10; enemyY[3] = 20;
		}

		void initBlocks(bool resetRules)
		{
			if (resetRules) setRules();
			tickCount = 0;
			for (int i = 0; i < maxSize; i++)
			{
				for (int j = 0; j < maxSize; j++)
				{
					block[i, j] = (r.Next(2) == 0);
				}
			}

			for (int i = 0; i < maxSize; i++)
			{
				block[0, i] = false;
				block[maxSize - 1, i] = false;
				block[i, 0] = false;
				block[i, maxSize - 1] = false;
			}
		}

		void mainLoop()
		{
			//AI Stuff here:
			for (int i = 0; i < maxEnemies; i++)// for each enemy:
			{
				targetX[i] = playerX;// find player's position and create a target location to go after
				targetY[i] = playerY;

				// for each direction to move, get a distance that will be from target if we move this way
				//which direction is closer:
				double distUp = (Math.Pow((targetX[i] - enemyX[i]), 2) + Math.Pow((targetY[i] - (enemyY[i] - 1)), 2));
				double distDown = (Math.Pow((targetX[i] - enemyX[i]), 2) + Math.Pow((targetY[i] - (enemyY[i] + 1)), 2));
				double distLeft = (Math.Pow((targetX[i] - (enemyX[i] - 1)), 2) + Math.Pow((targetY[i] - enemyY[i]), 2));
				double distRight = (Math.Pow((targetX[i] - (enemyX[i] + 1)), 2) + Math.Pow((targetY[i] - enemyY[i]), 2));
				//determine which of the four directions gets us closest to our target
				if (block[enemyX[i], enemyY[i]-1]) distUp = 9999999999D;
				if (block[enemyX[i], enemyY[i]+1]) distDown = 9999999999D;
				if (block[enemyX[i]-1, enemyY[i]]) distLeft = 9999999999D;
				if (block[enemyX[i]+1, enemyY[i]]) distRight = 9999999999D;

				int moveX = 0;
				int moveY = 0;

				double closestDist = 9999999999D;

				/*
				if ((distUp < closestDist) && (distUp < distDown) && (distUp < distLeft) && (distUp < distRight)) { moveY = -1; }
				else if ((distDown < closestDist) && (distDown < distUp) && (distDown < distLeft) && (distDown < distRight)) { moveY = 1; }
				else if ((distLeft < closestDist) && (distLeft < distUp) && (distLeft < distDown) && (distLeft < distRight)) { moveX = -1; }
				else if ((distRight < closestDist) && (distRight < distUp) && (distRight < distLeft) && (distRight < distDown)) { moveX = 1; }
				else {// no valid move}
				*/

				if (distUp < closestDist)
				{ // (U) D L R
					closestDist = distUp;
					if (distDown < closestDist)
					{ // (D) L R
						closestDist = distDown;
						if (distLeft < closestDist)
						{ // (L) R
							closestDist = distLeft;
							if (distRight < closestDist)
							{ // Right is the answer
								moveX = 1;
							}
							else
							{// Left is the answer
								moveX = -1;
							}
						}
						else
						{ // (D) R
							if (distRight < closestDist)
							{ // Right is the answer
								moveX = 1;
							}
							else
							{// Down is the answer
								moveY = 1;
							}
						}
					}
					else
					{ // (U) [D] L R
						if (distLeft < closestDist)
						{ // (L) R
							closestDist = distLeft;
							if (distRight < closestDist)
							{ // Right is the answer
								moveX = 1;
							}
							else
							{// Left is the answer
								moveX = -1;
							}
						}
						else
						{//(U) R
							if (distRight < closestDist)
							{ // Right is the answer
								moveX = 1;
							}
							else
							{// Up is the answer
								moveY = -1;
							}
						}
					}
				}
				else
				{ //[U] (DLR)
					if (distDown < closestDist)
					{ //[U] (D) LR
						closestDist = distDown;
						if (distLeft < closestDist)
						{ //(L) R
							closestDist = distLeft;
							if (distRight < closestDist)
							{// Right is the answer
								moveX = 1;
							}
							else
							{ //Left is the answer
								moveX = -1;
							}
						}
						else
						{ //(D) R
							if (distRight < closestDist)
							{// Right is the answer
								moveX = 1;
							}
							else
							{//Down is the answer
								moveY = 1;
							}
						}
					}
					else
					{ //[UD] LR
						if (distLeft < closestDist)
						{ //(L) R
							closestDist = distLeft;
							if (distRight < closestDist)
							{// Right is the answer
								moveX = 1;
							}
							else
							{// Left is the answer
								moveX = -1;
							}
						}
						else
						{//[UDL] R
							if (distRight < closestDist)
							{// Right is the answer
								moveX = 1;
							}
							else
							{// none were closest, no valid move to be made
								enemyTrapped[i]++;
							}
						}
					}
				}
				
				//put the enemy in the new found better position
				enemyX[i] += moveX;
				enemyY[i] += moveY;

				//if enemy is trapped handle this situation

				//check for collisions with player

				
			}
		}

		private void setRules()
		{
			for (int i = 0; i < rule.GetLength(1); i++)
			{
				//rule[0, i] = r.Next(2) == 0;
				//rule[1, i] = r.Next(2) == 0;
				rule[0, i] = false; //uncomment for Conway's rules
				rule[1, i] = false; //uncomment for Conway's rules
			}
			//rule[0, 2] = true;//uncomment for Conway's rules
			//rule[0, 3] = true;//uncomment for Conway's rules
			//rule[1, 3] = true;//uncomment for Conway's rules

			label1.Text = makeRuleString();
			maxTicks = Convert.ToInt16(textMaxTicks.Text);

			string ruleString = "2368/057";
			string[] subRule = ruleString.Split('/');

			for (int k = 0; k < 2; k++)
			{

				for (int i = 0; i < subRule[k].Length; i++)
				{
					int j = Convert.ToInt16(subRule[k].Substring(i, 1));
					rule[k, j] = true;
				}
			}
			//07/345
			//012358/056
		}



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

			g.FillEllipse(Brushes.Yellow, enemyX[0] * w, enemyY[0] * w, w, w);
			g.FillEllipse(Brushes.Blue, enemyX[1] * w, enemyY[1] * w, w, w);
			g.FillEllipse(Brushes.Orange, enemyX[2] * w, enemyY[2] * w, w, w);
			g.FillEllipse(Brushes.Red, enemyX[3] * w, enemyY[3] * w, w, w);

			//}
			g.Dispose();

			pictureBox1.Image = myBitmap;
			//this.Text = r.Next(10).ToString();

			System.Threading.Thread.Sleep(33);
			enemyTimer++;
			if (enemyTimer > 29)
			{
				enemyTimer = 0;
				mainLoop();
			}
			
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
				Console.WriteLine("--------------- RULESET OF INTEREST ------------------");
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

	}
}