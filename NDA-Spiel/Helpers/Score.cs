using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDA_Spiel.Helpers
{
    public class Score
    {
        private string name;
        public string Name { get { return name; } set { name = value; } }

        private int points;
        [DisplayName("Punkte")]
        public int Points { get { return points; } set { points = value; } }
    }
}
