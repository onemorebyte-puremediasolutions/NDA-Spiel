using DevExpress.Utils.DirectXPaint;
using NDA_Spiel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NDA_Spiel.Helpers.Settings;

namespace NDA_Spiel
{
    public partial class StartForm : Form
    {
        private Setting selectedSettings = Setting.Easy;
        private List<Score> scores = new List<Score>();

        public StartForm()
        {
            InitializeComponent();
            ReadDate();
        }

        private void ReadDate()
        {
            dataGridView1.DataSource = null;

            string allText = File.ReadAllText("Scores.txt");

            if (string.IsNullOrEmpty(allText))
            {
                return;
            }

            scores = new List<Score>();

            string[] entrys = allText.Split('|');
            foreach (string entry in entrys)
            {
                if (entry == "")
                {
                    break;
                }

                Score score = new Score();
                score.Name = entry.Split(',')[0];
                score.Points = Convert.ToInt32(entry.Split(',')[1]);
                scores.Add(score);
            }

            scores = scores.OrderByDescending(x => x.Points).ToList();
            
            dataGridView1.DataSource = scores;

            SetRowStyle();
        }

        private void SetRowStyle()
        {

        }

        private void rdb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = (RadioButton)sender;

            switch (button.Name)
            {
                case "rdbEasy":
                    selectedSettings = Setting.Easy;
                    break;
                case "rdbMiddle":
                    selectedSettings = Setting.Middle;
                    break;
                case "rdbHard":
                    selectedSettings = Setting.Hard;
                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Bitte geben sie einen Namen an","No name Given",MessageBoxButtons.OK,MessageBoxIcon.Warning);

                return;
            }

            if (scores.Any(x => x.Name == txtName.Text))
            {
                if (MessageBox.Show("Dieser Name hat schon einen eintrag, wollen sie diesen überschreiben", "Name already used", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    OpenLvl();
                }
                else
                {
                    return;
                }
            }
            else
            {
                CreateAndAddNewScore();

                OpenLvl();
            }
        }

        private void OpenLvl()
        {
            Lab1 lvl1 = new Lab1(selectedSettings, scores, txtName.Text);
            this.Visible = false;
            lvl1.ShowDialog();
            this.Visible = true;
            ReadDate();

            if (lvl1.DialogResult == DialogResult.Retry)
            {
                lvl1.Close();
                lvl1.Dispose();
                btnStart.PerformClick();
                return;
            }
            lvl1.Close();
            lvl1.Dispose();
        }
        

        private void CreateAndAddNewScore()
        {
            Score score = new Score();
            score.Name = txtName.Text;
            score.Points = 0;
            scores.Add(score);
        }

        private void btnRules_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("rules.txt"));
        }
    }
}
