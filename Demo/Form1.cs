using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using fy3DModelControllib;
using soy.Algorithms.Geometry;

namespace Demo
{
    public partial class Form1 : Form
    {
        ExGrid m_Grid = null;

        public Form1()
        {
            InitializeComponent();

            fy3DModelManager1.Connect(fy3DModelCanvas1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_Grid = GridHelper.ReadGridFromGSLIB_FastForm();

            //m_Grid = new fyGrid(1, 1, 1, 200, 200, 1, 0, 0, 0);
            //Random r = new Random();
            //for (int i = 0; i < m_Grid.CellCount; i++)
            //{
            //    m_Grid.SetCell(i, r.Next(0, 10));
            //}
            fy3DModelCanvas1.UpdatefyGrid(m_Grid);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GridHelper.WriteGridToGSLIB_FastForm(m_Grid);
        }
    }
}
