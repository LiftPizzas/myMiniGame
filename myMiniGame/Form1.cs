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
		int[] enemyX, enemyY, targetX, targetY, moveX, moveY;
		int[] enemyTrapped;
		int enemyTimer = 0;
		int[] enemyLine;
		int[] enemyStrategy;

		int[] dtX, dtY; //debug Target Display positions

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
		int[] lastX, lastY;
		int lastPointer = 0;
		
		int[][] enemyTrailX; //enemy number, position in trail
		int[][] enemyTrailY;
		int maxTrailLength = 40;
		int[] enemyTrailPosition;

		public Form1()
		{
			InitializeComponent();

			//initialize search stuff
			
			enemyTrailX = new int[maxEnemies][];
			enemyTrailY = new int[maxEnemies][];
			for (int i = 0; i < maxEnemies; i++)
			{
				enemyTrailX[i] = new int[maxTrailLength];
				enemyTrailY[i] = new int[maxTrailLength];
			}
			enemyTrailPosition = new int[maxEnemies];

			block = new bool[maxSize, maxSize];
			buffer = new bool[maxSize, maxSize];
			initBlocks(true);
			myBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			enemyX = new int[maxEnemies];
			enemyY = new int[maxEnemies];
			moveX = new int[maxEnemies];
			moveY = new int[maxEnemies];
			targetX = new int[maxEnemies];
			targetY = new int[maxEnemies];
			dtX = new int[maxEnemies];
			dtY = new int[maxEnemies];
			enemyTrapped = new int[maxEnemies];
			enemyStrategy = new int[maxEnemies];
			enemyLine = new int[maxEnemies];
			lastX = new int[10];
			lastY = new int[10];
			initEnemies();
		}

		void initEnemies()
		{
			for (int i = 0; i < lastX.Length; i++) { lastX[i] = playerX; lastY[i] = playerY; }
			//reset enemy position
			enemyX[0] = 10; enemyY[0] = 10;
			enemyX[1] = 20; enemyY[1] = 10;
			enemyX[2] = 20; enemyY[2] = 20;
			enemyX[3] = 10; enemyY[3] = 20;
			for (int i = 0; i < maxEnemies; i++)
			{
				enemyStrategy[i] = i; //r.Next(3);
				// 0 direct, 1 headoff, 2 behind, 3 wander
			}
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
				
				enemyLine[i]--; //fixme: check to see if end of trail is hit
				if (enemyTrailPosition[i] >= enemyTrailX[i].Length || enemyLine[i] <= 0)//decide when the enemy needs to make a new trail
				{
					moveX[i] = 0;
					moveY[i] = 0;
					enemyLine[i] = r.Next(10);
					if (enemyStrategy[i] == 0)
					{
						targetX[i] = playerX;// find player's position and create a target location to go after
						targetY[i] = playerY;
					}
					else if (enemyStrategy[i] == 1) // try to get ahead of player
					{ //use last movement, project into the future by 4 moves
						int tPointer = lastPointer - 4;
						if (tPointer < 0) tPointer += lastX.Length;
						targetX[i] = playerX + (playerX - lastX[tPointer]);
						targetY[i] = playerY + (playerY - lastY[tPointer]);
					}
					else if (enemyStrategy[i] == 2)
					{
						int tPointer = lastPointer - 4;
						if (tPointer < 0) tPointer += lastX.Length;
						targetX[i] = lastX[tPointer];
						targetY[i] = lastY[tPointer];
					}
					else
					{ //just wander toward a random target
						targetX[i] = r.Next(maxSize - 2) + 1;
						targetY[i] = r.Next(maxSize - 2) + 1;
					}
					dtX[i] = targetX[i];
					dtY[i] = targetY[i];

					int[,] searchCells = new int[maxSize, maxSize];
					int[] leafListX = new int[1600];
					int[] leafListY = new int[1600];
					int numLeaves = 1;
					int currentLeaf = 0;
					leafListX[0] = targetX[i];
					leafListY[0] = targetY[i];
					bool doneFlag = false;
					int trailLength = 1;
					bool successFlag = false;

					while (!doneFlag)
					{

						// for each leaf in the list
						//check each of the 4 neighbors

						int tx = leafListX[currentLeaf] - 1; //to the left
						int ty = leafListY[currentLeaf];
						if (tx < 1) tx = 1; else if (tx >= maxSize-3) tx = maxSize-3;
						if (ty < 1) ty = 1; else if (ty >= maxSize-3) ty = maxSize-3;
						if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
						{// give it a position number and add it to the leaf list
							if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
							searchCells[tx, ty] = trailLength;
							leafListX[numLeaves] = tx;
							leafListY[numLeaves] = ty;
							numLeaves++;
						}

						//right
						tx += 2;
						if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
						{// give it a position number and add it to the leaf list
							if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
							searchCells[tx, ty] = trailLength;
							leafListX[numLeaves] = tx;
							leafListY[numLeaves] = ty;
							numLeaves++;
						}

						//above
						tx -= 1;
						ty -= 1;
						if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
						{// give it a position number and add it to the leaf list
							if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
							searchCells[tx, ty] = trailLength;
							leafListX[numLeaves] = tx;
							leafListY[numLeaves] = ty;
							numLeaves++;
						}

						//below
						ty += 2;
						if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
						{// give it a position number and add it to the leaf list
							if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
							searchCells[tx, ty] = trailLength;
							leafListX[numLeaves] = tx;
							leafListY[numLeaves] = ty;
							numLeaves++;
						}
						trailLength++; //increment the trail counter

						//iterate leaf list, update our pointer
						currentLeaf++;//check if any leaves left
						if ((currentLeaf > leafListX.Length) || (currentLeaf >= numLeaves)) doneFlag = true;
					}
					 
					if (successFlag)
					{//assemble the trail from the last position to the target
					 //starting at enemyY we're going to find the lowest neighbor
						int trailBest = 9999999;
						trailLength = 0;
						int tx = enemyX[i];
						int ty = enemyY[i];

						while (trailBest > 0)
						{
							int dx = 0;
							int dy = 0; //winning directions
							//find the lowest neighbor:
							int cx = tx -1;
							int cy = ty; //check to the left
							if ((searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0)) dx = -1; dy = 0;
							cx += 2; //to the right
							if ((searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0)) dx = 1; dy = 0;
							cx --;
							cy--; //check above
							if ((searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0)) dx = 0; dy = -1;
							//check below
							cy += 2;
							if ((searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0)) dx = 0; dy = 1;
							//now we've found the lowest, add that to the trail and update current pointer
							trailLength++;
							tx += dx;
							ty += dy;
							enemyTrailX[i][trailLength] = tx;
							enemyTrailY[i][trailLength] = ty;
							if (dx == 0 && dy == 0 || trailLength >= maxTrailLength-1) break;
							//enemytrail x and y now contains the XY coordinates of the trail cells of the desired path
							enemyTrailPosition[i] = 0;
						}

					}
					else
					{
						//just move randomly
					}

				}

				//figure out where the enemy's next move wants to be along the current path
				enemyX[i] += enemyTrailX[i][enemyTrailPosition[i]];
				enemyY[i] += enemyTrailY[i][enemyTrailPosition[i]];

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

			g.FillEllipse(Brushes.LightGoldenrodYellow, dtX[0] * w, dtY[0] * w, w, w);
			g.FillEllipse(Brushes.LightBlue, dtX[1] * w, dtY[1] * w, w, w);
			g.FillEllipse(Brushes.LightGray, dtX[2] * w, dtY[2] * w, w, w);
			g.FillEllipse(Brushes.LightPink, dtX[3] * w, dtY[3] * w, w, w);


			//}
			g.Dispose();

			pictureBox1.Image = myBitmap;
			//this.Text = r.Next(10).ToString();

			System.Threading.Thread.Sleep(33);
			enemyTimer++;
			if (enemyTimer > 6)
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

		private void playerMoved()
		{
			lastPointer++;
			if (lastPointer >= lastX.Length) lastPointer = 0;
			lastX[lastPointer] = playerX;
			lastY[lastPointer] = playerY;
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
				playerMoved();
			}
			if (e.KeyCode == Keys.Up)
			{
				playerY--;
				if ((playerY <= 0) || (block[playerX, playerY])) playerY++;
				//check for collisions with background maze
				playerMoved();
			}
			if (e.KeyCode == Keys.Right)
			{
				//check for collisions with background maze
				playerX++;
				if ((playerX >= maxSize) || (block[playerX, playerY])) playerX--;
				playerMoved();
			}
			if (e.KeyCode == Keys.Left)
			{
				//check for collisions with background maze
				playerX--;
				if ((playerX <= 0) || (block[playerX, playerY])) playerX++;
				playerMoved();
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