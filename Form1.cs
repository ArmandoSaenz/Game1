using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game1
{
    public partial class Form1 : Form
    {
        // Constantes para el tamaño del jugador y enemigo, velocidad del enemigo y el intervalo del temporizador

        const int PLAYER_SIZE = 20;
        const int ENEMY_SIZE = 10;
        const int ENEMY_SPEED = 9;
        const int TIMER_INTERVAL = 20;

        // Rectángulos para el jugador y enemigo, punto para la velocidad del enemigo, indicador de fin de juego y puntaje del jugador
        Rectangle player;
        List<Enemy> enemies = new List<Enemy>();
        List<Enemy> enemies2Delete = new List<Enemy>();
        Point enemyHorizontalVelocity; 
        Point enemyVerticalVelocity;
        bool isGameOver = false;
        int score = 0, loops=0;

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Dibuja al jugador
            e.Graphics.FillRectangle(Brushes.Blue, player);

            // Dibuja al enemigo
            foreach(Enemy enemy in enemies)
                e.Graphics.FillRectangle(Brushes.Red, enemy.Body);

            // Dibuja el puntaje
            Font font = new Font("Arial", 20);
            e.Graphics.DrawString(("Puntos: " + score.ToString()), font, Brushes.DarkGreen, new Point(10, 10));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    player.X -= 5;
                    break;
                case Keys.D:
                    player.X += 5;
                    break;
                case Keys.W:
                    player.Y -= 5;
                    break;
                case Keys.S:
                    player.Y += 5;
                    break;
                case Keys.D1:
                    // Reiniciar las variables del juego
                    timer1.Enabled = false;
                    player = new Rectangle((pictureBox1.Width - PLAYER_SIZE) / 2, (pictureBox1.Height - PLAYER_SIZE) / 2, PLAYER_SIZE, PLAYER_SIZE);
                    enemies.Clear();
                    isGameOver = false;
                    score = 0;
                    loops = 0;
                    timer1.Enabled = true;
                    pictureBox1.Invalidate();
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int enemiesCount = enemies.Count;
            player.X = player.X < 0 ? 0 : (player.X + PLAYER_SIZE > pictureBox1.Width ? pictureBox1.Width - PLAYER_SIZE : player.X);
            player.Y = player.Y < 0 ? 0 : (player.Y + PLAYER_SIZE > pictureBox1.Height ? pictureBox1.Height - PLAYER_SIZE : player.Y);
            enemies2Delete.Clear();
            foreach (Enemy enemy in enemies)
            {
                if(enemy is VerticalEnemy)
                {
                    (enemy as VerticalEnemy).Move(ENEMY_SPEED + score);
                    if (enemy.Y > pictureBox1.Height || enemy.Y < 0)
                    {
                        ++score;
                        enemies2Delete.Add(enemy);
                    }
                }
                else if(enemy is HorizontalEnemy)
                {
                    (enemy as HorizontalEnemy).Move(ENEMY_SPEED + score);
                    if (enemy.X > pictureBox1.Width || enemy.X < 0)
                    {
                        ++score;
                        enemies2Delete.Add(enemy);
                    }
                }
                if (player.IntersectsWith(enemy.Body))
                {
                    isGameOver = true;
                    break;
                }
            }
            if (isGameOver)
            {
                timer1.Enabled = false;
                MessageBox.Show("Game over! Tu puntaje es " + score);
            }
            enemies.RemoveAll(enemytmp => enemies2Delete.Exists(arg => arg == enemytmp));
            ++loops;
            loops %= 200;
            if (loops % (score < 50 ? 100-score:50) == 0)
                enemies.Add(new VerticalEnemy(ENEMY_SIZE, pictureBox1.Height, pictureBox1.Width));
            else if (loops % (score < 30 ? 50 - score : 20) == 0)
                enemies.Add(new HorizontalEnemy(ENEMY_SIZE, pictureBox1.Width, pictureBox1.Height));
            pictureBox1.Invalidate();
        }

        public Form1()
        {
            InitializeComponent();
            player = new Rectangle((pictureBox1.Width - PLAYER_SIZE)/2, (pictureBox1.Height - PLAYER_SIZE)/2, PLAYER_SIZE, PLAYER_SIZE);
            enemyVerticalVelocity = new Point(0, ENEMY_SPEED);
            enemyHorizontalVelocity = new Point(ENEMY_SPEED, 0);
            timer1.Interval = TIMER_INTERVAL;
            timer1.Enabled = true;
        }
    }
    internal class Enemy
    {
        protected Rectangle body;
        public Rectangle Body { get => body; protected set { body = value; } }
        public bool Direction = false;
        public int X { get => body.X; }
        public int Y { get => body.Y; }
    }
    internal class HorizontalEnemy : Enemy
    {
        public HorizontalEnemy (int enemySize, int xPosition ,int maxPosition)
        {
            Direction = new Random().Next(10) > 5;
            Body = new Rectangle(Direction ? 0 : xPosition, new Random().Next(maxPosition), enemySize, enemySize);
        }

        public new void Move(int movement)
        {
            body.X += Direction ? movement : -movement;
        }
    }
    internal class VerticalEnemy : Enemy
    {
        public VerticalEnemy(int enemySize, int yPosition, int maxPosition)
        {
            Direction = new Random().Next(10) > 5;
            Body = new Rectangle(new Random().Next(maxPosition),Direction ? 0 : yPosition, enemySize, enemySize);
        }

        public new void Move(int movement)
        {
            body.Y += Direction ? movement : -movement;
        }

    }
}
