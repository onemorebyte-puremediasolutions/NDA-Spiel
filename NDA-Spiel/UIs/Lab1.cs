using NDA_Spiel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NDA_Spiel.Helpers.Settings;

namespace NDA_Spiel
{
    public partial class Lab1 : Form
    {
        Random random = new Random();

        private Keys pressedKey;
        private bool keyDown;
        private const int speed = 2;
        private Setting difficulty;
        private double timeLeft = 70;
        private List<Score> scores;
        private string playerName;
        private double playTime = 0.0;
        private Point startPoint;

        public Lab1(Setting difficulty, List<Score> scores, string playerName)
        {
            InitializeComponent();
            this.difficulty = difficulty;
            this.scores = scores;
            this.playerName = playerName;

            startPoint = picPlayer.Location;

            SetDifficulty();
            playerTimer.Start();
        }

        private void SetDifficulty()
        {
            switch (difficulty)
            {
                case Setting.Hard:
                    SetHardSettings();
                    break;
                case Setting.Middle:
                    SetTimeLeftTimer();
                    break;
                default:
                    break;
            }
        }

        private void SetHardSettings()
        {
            SetColorTimer();
            timeLeft = 50;
            SetTimeLeftTimer();
        }
        private void SetColorTimer() 
        { 
            ColorTimer.Start();
        }

        private void SetTimeLeftTimer()
        {
            lblTime.Visible = true;
            lblTimeLeft.Visible = true;
            timeLeftTimer.Start();
        }

        private Color GetRandomColor()
        {
            int R = random.Next(0, 255);
            int G = random.Next(0, 255);
            int B = random.Next(0, 255);


            return Color.FromArgb(R, G, B);
        }
        private void CheckForCollision()
        {
            Rectangle playerBounds = picPlayer.Bounds;

            foreach (var controlObject in Controls)
            {

                if (controlObject is Label)
                {
                    Label wall = (Label) controlObject;

                    Rectangle wallBounds = wall.Bounds;

                    if (playerBounds.IntersectsWith(wallBounds))
                    {
                        picPlayer.Location = startPoint;
                    }
                }
                else if (controlObject is PictureBox)
                {
                    PictureBox picBox = (PictureBox) controlObject;

                    if (picBox.Name == "picGoal")
                    {
                        Rectangle picBounds = picBox.Bounds;

                        if (playerBounds.IntersectsWith(picBounds))
                        {
                            StopTimers();
                            
                            keyDown = false;

                            SetScore();

                            WriteDataToTxt();

                            MessageBox.Show("Finished");
                            this.Close();
                            break;
                        }
                    }
                }
                
            }
        }

        private void StopTimers()
        {
            playerTimer.Stop();
            ColorTimer.Stop();
            timeLeftTimer.Stop();
        }

        private void SetScore()
        {
            int points = (90 - Convert.ToInt32(playTime)) * (int)difficulty;

            double new_value = (points - 0) / (double)(270 - 0) * 100;

            points = Convert.ToInt32(Math.Floor(new_value));

            if (points <= 0)
            {
                points = 0;
            }

            scores.FirstOrDefault(x => x.Name == playerName).Points = points;
        }

        private void WriteDataToTxt()
        {
            string stringToWrite = "";

            foreach (Score entry in scores)
            {
                stringToWrite += $"{entry.Name},{entry.Points}|";
            }
            File.WriteAllText("Scores.txt", stringToWrite);
            
        }

        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            foreach (var lbl in Controls)
            {
                if (lbl is Label)
                {
                    Label wall = lbl as Label;

                    wall.BackColor = GetRandomColor();
                }
            }
        }

        private void playerTimer_Tick(object sender, EventArgs e)
        {
            if (keyDown)
            {
                if (pressedKey == Keys.Up)
                {
                    picPlayer.Location = new Point(picPlayer.Location.X, picPlayer.Location.Y - speed);
                }
                else if (pressedKey == Keys.Down)
                {
                    picPlayer.Location = new Point(picPlayer.Location.X, picPlayer.Location.Y + speed);
                }
                else if (pressedKey == Keys.Left)
                {
                    picPlayer.Location = new Point(picPlayer.Location.X - speed, picPlayer.Location.Y);
                }
                else if (pressedKey == Keys.Right)
                {
                    picPlayer.Location = new Point(picPlayer.Location.X + speed, picPlayer.Location.Y);
                }
            }
            CheckForCollision();
            playTime += 0.01;
        }

        private void Lab1_KeyDown(object sender, KeyEventArgs e)
        {
            pressedKey = e.KeyCode;
            keyDown = true;
        }

        private void Lab1_KeyUp(object sender, KeyEventArgs e)
        {
            keyDown = false;
        }

        private void timeLeftTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft <= 0.0)
            {
                StopTimers();

                keyDown = false;

                this.DialogResult = MessageBox.Show("Game Over, die Zeit ist abgelaufen", "Game Over", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
                
                
                this.Close();
            }

            timeLeft = timeLeft - 1;

            string displayTime = timeLeft.ToString();

            lblTime.Text = $"{displayTime} Seconds";
        }
    }
}
