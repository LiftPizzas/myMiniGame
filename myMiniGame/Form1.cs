using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//fixme: player movement, smooth movement for everything.
//fixme: alternate behavior: random wandering until they find player's "scent trail" and then start following it (quickly) toward the player
//fixme: flashing status for "alerted" enemies.
//fixme: choose target in radius of player instead of projecting ahead/behind?
//fixme: improve graphics, use sprites for player/enemies and something for maze walls

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
		int playerMoveX, playerMoveY;
		int playerTimer = 0;

		bool isCalculating = true;
		int[][] enemyTrailX; //enemy number, position in trail
		int[][] enemyTrailY;
		int maxTrailLength = 40;
		int[] trailLength;
		int[] enemyTrailPosition;
		Brush[] enemyColor = new Brush[] { Brushes.LightGoldenrodYellow, Brushes.LightBlue, Brushes.LightGray, Brushes.LightPink };

		public Form1()
		{
			InitializeComponent();

			//initialize search stuff

			trailLength = new int[maxEnemies];
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
		}

		void initEnemies()
		{
			for (int i = 0; i < lastX.Length; i++) { lastX[i] = playerX; lastY[i] = playerY; }
			
			
			for (int i = 0; i < maxEnemies; i++)
			{
				enemyStrategy[i] = i; //r.Next(3);
									  // 0 direct, 1 headoff, 2 behind, 3 wander
				bool isValid = false;
				while (!isValid)
				{
					//reset enemy position
					enemyX[i] = r.Next(maxSize - 16) + 8;
					enemyY[i] = r.Next(maxSize - 16) + 8;
					//check to see if it's a valid position, if not, adjust by one until it is.
					if (!block[enemyX[i], enemyY[i]]) isValid = true;
				}
			}
		}

		void initBlocks(bool resetRules)
		{
			isCalculating = true;
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
			if (isCalculating) return;
			//AI Stuff here:
			for (int i = 0; i < maxEnemies; i++)// for each enemy:
			{

				enemyLine[i]--;
				if (enemyTrailPosition[i] >= trailLength[i]-1 || enemyLine[i] <= 0)//decide when the enemy needs to make a new trail
				{
					enemyTrailPosition[i]=0;
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
					else if (enemyStrategy[i] == 2)//follow player's trail
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
					//ensure target is within the bounds of the playfield (values 1-38, because 0 and 39 are the borders)
					if (targetX[i] < 1) targetX[i] = 1; else if (targetX[i] >= maxSize - 2) targetX[i] = maxSize - 2;
					if (targetY[i] < 1) targetY[i] = 1; else if (targetY[i] >= maxSize - 2) targetY[i] = maxSize - 2;

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
					trailLength[i] = 1;
					bool successFlag = false;

					while (!doneFlag)
					{

						// for each leaf in the list
						//check each of the 4 neighbors

						int tx = leafListX[currentLeaf];
						int ty = leafListY[currentLeaf];
						trailLength[i] = searchCells[tx, ty] + 1; //neighbors will be this one's value plus 1
						// values must be from 1-38 to be "valid" here otherwise we skip them in our search
						if ((tx >= 1) && (tx <= maxSize - 2) && (ty >= 1) && (ty <= maxSize - 2))
						{
							tx--;//to the left
							if (tx >= 1)
							{
								if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
								{// give it a position number and add it to the leaf list
									if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
									searchCells[tx, ty] = trailLength[i];
									leafListX[numLeaves] = tx;
									leafListY[numLeaves] = ty;
									numLeaves++;
								}
							}

							//right
							tx += 2;
							if (tx <= maxSize - 2)
							{
								if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
								{// give it a position number and add it to the leaf list
									if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
									searchCells[tx, ty] = trailLength[i];
									leafListX[numLeaves] = tx;
									leafListY[numLeaves] = ty;
									numLeaves++;
								}
							}

							//above
							tx -= 1;
							ty -= 1;
							if (ty >= 1)
							{
								if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
								{// give it a position number and add it to the leaf list
									if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
									searchCells[tx, ty] = trailLength[i];
									leafListX[numLeaves] = tx;
									leafListY[numLeaves] = ty;
									numLeaves++;
								}
							}

							//below
							ty += 2;
							if (ty <= maxSize - 2)
							{
								if (!block[tx, ty] && (searchCells[tx, ty] == 0))//if the neighbor is empty and not already filled in
								{// give it a position number and add it to the leaf list
									if ((tx == enemyX[i]) && (ty == enemyY[i])) { doneFlag = true; successFlag = true; }
									searchCells[tx, ty] = trailLength[i];
									leafListX[numLeaves] = tx;
									leafListY[numLeaves] = ty;
									numLeaves++;
								}
							}
							//trailLength[i]++; //increment the trail counter
						}
						//iterate leaf list, update our pointer
						currentLeaf++;//check if any leaves left
						if ((currentLeaf > leafListX.Length) || (currentLeaf >= numLeaves)) doneFlag = true;
						
					}

					if (successFlag)
					{//assemble the trail from the last position to the target
					 //starting at enemyY we're going to find the lowest neighbor
						int trailBest = 9999999;
						trailLength[i] = 0;
						int tx = enemyX[i];
						int ty = enemyY[i];

						while (trailBest > 1)  
						{
							int dx = 0;
							int dy = 0; //winning directions

							//find the lowest neighbor:
							int cx = tx -1;
							int cy = ty; //check to the left
							if ((cx > 0) && (searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0))
							{
								dx = -1;
								dy = 0;
								trailBest = searchCells[cx, cy];
							}
							cx += 2; //to the right
							if ((cx < maxSize) && (searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0))
							{
								dx = 1;
								dy = 0;
								trailBest = searchCells[cx, cy];
							}
							cx --;
							cy--; //check above
							if ((cy > 0) && (searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0))
							{
								dx = 0;
								dy = -1;
								trailBest = searchCells[cx, cy];
							}
							//check below
							cy += 2;
							if ((cy < maxSize) && (searchCells[cx, cy] < trailBest) && (searchCells[cx, cy] != 0))
							{
								dx = 0;
								dy = 1;
								trailBest = searchCells[cx, cy];
							}

							//now we've found the lowest, add that to the trail and update current pointer
							tx += dx;
							ty += dy;
							enemyTrailX[i][trailLength[i]] = tx;
							enemyTrailY[i][trailLength[i]] = ty;
							trailLength[i]++;
							if (dx == 0 && dy == 0 || trailLength[i] >= maxTrailLength-1)
								break; 
							//enemytrail x and y now contains the XY coordinates of the trail cells of the desired path
							enemyTrailPosition[i] = 0;
						}
						//showSearch(searchCells, i);
					}
					else
					{
						//just move randomly
						trailLength[i] = 0;
						enemyTrailPosition[i] = 0;
					}

				}


				if (enemyTrailPosition[i] < trailLength[i])
				{
					bool oldFlag = block[enemyX[i],enemyY[i]];
					//figure out where the enemy's next move wants to be along the current path
					enemyX[i] = enemyTrailX[i][enemyTrailPosition[i]];
					enemyY[i] = enemyTrailY[i][enemyTrailPosition[i]];
					enemyTrailPosition[i]++;
					if( !oldFlag && block[enemyX[i], enemyY[i]])
					{
						int l = 0;
					}

					//check for collisions with player
					if ((enemyX[i] == playerX) && (enemyY[i] == playerY))
					{
						System.Media.SoundPlayer audio = new System.Media.SoundPlayer(Properties.Resources.ding);
						audio.Play();
					}
				}
			}
		}

		private void showSearch(int[,] searchCells, int e)
		{
			Console.WriteLine("\r------------------------------------------------------------");
			string s = "";
			for (int y = 0; y < maxSize; y++)
			{
				for (int x = 0; x < maxSize; x++)
				{
					if (block[x, y]) s += "X,";
					else s += searchCells[x, y] + ",";
				}
				Console.WriteLine(s);
				s = "";
			}

			s = "";
			for (int y = 0; y < maxSize; y++)
			{
				for (int x = 0; x < maxSize; x++)
				{
					bool isTrail = false;
					for (int i = 0; i < trailLength[e]; i++) if (enemyTrailX[e][i] == x && enemyTrailY[e][i] == y)
						{
							isTrail = true;
							
						}

					if (isTrail)
					{
						s += "P,";
					}
					else
					{
						if (block[x, y]) s += "X,";
						else s += searchCells[x, y] + ",";
					}
				}
				Console.WriteLine(s);
				s = "";
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
			else
			{
				if (tickCount == maxTicks) initEnemies();
				else isCalculating = false;
			}

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
			
			for (int i = 0; i < maxEnemies; i++)
			{
				for (int j = 0; j < trailLength[i]; j++)
				{
					g.FillEllipse(enemyColor[i], enemyTrailX[i][j] * w, enemyTrailY[i][j] * w, w, w);
				}
			}
			g.FillEllipse(Brushes.LightGoldenrodYellow, dtX[0] * w, dtY[0] * w, w, w);
			g.FillEllipse(Brushes.LightBlue, dtX[1] * w, dtY[1] * w, w, w);
			g.FillEllipse(Brushes.LightGray, dtX[2] * w, dtY[2] * w, w, w);
			g.FillEllipse(Brushes.LightPink, dtX[3] * w, dtY[3] * w, w, w);



			g.FillEllipse(Brushes.Yellow, enemyX[0] * w, enemyY[0] * w, w, w);
			g.FillEllipse(Brushes.Blue, enemyX[1] * w, enemyY[1] * w, w, w);
			g.FillEllipse(Brushes.Orange, enemyX[2] * w, enemyY[2] * w, w, w);
			g.FillEllipse(Brushes.Red, enemyX[3] * w, enemyY[3] * w, w, w);

			g.FillEllipse(Brushes.Green, playerX * w, playerY * w, w, w);// draw the player
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

			playerTimer++;
			if (playerTimer >= 4)
			{
				playerTimer = 0;
				playerMoved();
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
			//move player and check for collisions or out of bounds
			playerX += playerMoveX;
			if ((playerX >= maxSize) || (playerX <= 1) || (block[playerX, playerY]))
			{// if the player goes invalid place, undo last movement and set move speed to zero
				playerX -= playerMoveX;
				playerMoveX = 0;
			}
			playerY += playerMoveY;
			if ((playerY >= maxSize) || (playerY <= 1) || (block[playerX, playerY]))
			{
				playerY -= playerMoveY;
				playerMoveY = 0;
			}

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
				//check for collisions with background maze
				if ((playerY + 1 < maxSize) && (!block[playerX, playerY + 1]))
				{
					playerMoveX = 0;
					playerMoveY = 1;
				}
			}
			if (e.KeyCode == Keys.Up)
			{
				if ((playerY - 1 < maxSize) && (!block[playerX, playerY - 1]))
				{
					playerMoveX = 0;
					playerMoveY = -1;
				}
			}
			if (e.KeyCode == Keys.Right)
			{
				if ((playerX + 1 < maxSize) && (!block[playerX + 1, playerY]))
				{
					playerMoveX = 1;
					playerMoveY = 0;
				}

			}
			if (e.KeyCode == Keys.Left)
			{
				if ((playerX - 1 < maxSize) && (!block[playerX - 1, playerY]))
				{
					playerMoveX = -1;
					playerMoveY = 0;
				}
			}
		}


		private void form_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Down)  playerMoveY = 0;
			if (e.KeyCode == Keys.Up)    playerMoveY = 0;
			if (e.KeyCode == Keys.Left)  playerMoveX = 0;
			if (e.KeyCode == Keys.Right) playerMoveX = 0;
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