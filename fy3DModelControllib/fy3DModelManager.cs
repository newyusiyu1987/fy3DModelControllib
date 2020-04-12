using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fy3DModelControllib
{
    public partial class fy3DModelManager : UserControl
    {
        fy3DModelCanvas m_3DModelCanvas = null;

        int m_XPos = 0;
        int m_YPos = 0;
        int m_ZPos = 0;

        List<fyTransparentRange> TransparentRanges = null;

        public fy3DModelManager()
        {
            InitializeComponent();

            this.panel1.Enabled = false;
        }      

        /// <summary>
        /// 连接fy3DModelCanvas
        /// </summary>
        /// <param name="_3DModelCanvas"></param>
        public void Connect(fy3DModelCanvas _3DModelCanvas)
        {
            if (_3DModelCanvas == null) return;
            else
            {
                m_3DModelCanvas = _3DModelCanvas;
                m_3DModelCanvas.UpdatefyGridEvent += m_3DModelCanvas_UpdatefyGridEvent;
            }
        }

        /// <summary>
        /// 断开fy3DModelCanvas
        /// </summary>
        public void DisConnect()
        {
            this.Enabled = false;
            if (m_3DModelCanvas == null) return;
            else
            {
                m_3DModelCanvas.UpdatefyGridEvent -= m_3DModelCanvas_UpdatefyGridEvent;
            }

            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 100;
            numericUpDown1.Value = 0;

            numericUpDown2.Minimum = 0;
            numericUpDown2.Maximum = 100;
            numericUpDown2.Value = 0;

            numericUpDown3.Minimum = 0;
            numericUpDown3.Maximum = 100;
            numericUpDown3.Value = 0;

            trackBar2.Minimum = 0;
            trackBar2.Maximum = 100;
            trackBar2.Value = 0;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 100;
            trackBar1.Value = 0;

            trackBar3.Minimum = 0;
            trackBar3.Maximum = 100;
            trackBar3.Value = 0;

            label_ICount.Text = "ICount=0";
            label_JCount.Text = "JCount=0";
            label_KCount.Text = "KCount=0";

            label_MinValue.Text = "Min=0";
            label_MaxValue.Text = "Max=0";

            textBox1.Text = string.Empty;
        }

        //接收fy3DModelCanvas更新数据的消息
        //初始化所有参数
        void m_3DModelCanvas_UpdatefyGridEvent()
        {
            //激活控制面板
            this.Enabled = true;


            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = m_3DModelCanvas.KCount - 1;

            numericUpDown2.Minimum = 0;
            numericUpDown2.Maximum = m_3DModelCanvas.JCount - 1;

            numericUpDown3.Minimum = 0;
            numericUpDown3.Maximum = m_3DModelCanvas.ICount - 1;

            trackBar2.Minimum = 0;
            trackBar2.Maximum = m_3DModelCanvas.KCount - 1;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = m_3DModelCanvas.JCount - 1;

            trackBar3.Minimum = 0;
            trackBar3.Maximum = m_3DModelCanvas.ICount - 1;

            label_ICount.Text = "ICount=" + m_3DModelCanvas.ICount.ToString();
            label_JCount.Text = "JCount=" + m_3DModelCanvas.JCount.ToString();
            label_KCount.Text = "KCount=" + m_3DModelCanvas.KCount.ToString();

            label_MinValue.Text = "Min=" + m_3DModelCanvas.MinValue.ToString();
            label_MaxValue.Text = "Max=" + m_3DModelCanvas.MaxValue.ToString();

            textBox1.Text = m_3DModelCanvas.MinValue + "," + m_3DModelCanvas.MaxValue;

            //初始化镂空参数(采用默认的镂空参数)
            TransparentRanges = new List<fyTransparentRange>();
            fyTransparentRange TransparentRange = new fyTransparentRange();
            TransparentRange.Min = m_3DModelCanvas.MinValue;
            TransparentRange.Max = m_3DModelCanvas.MaxValue;
            TransparentRanges.Add(TransparentRange);

            //默认启动普通模式
            radioButton1.Checked = true;
            //启动默认设置（普通视图模式）
            m_3DModelCanvas.LookAt_NormalMode(fy3DModelViewMode.None);
        }                                     

        //XOY切片
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            m_ZPos = (int)numericUpDown1.Value;
            trackBar2.Value = (int)numericUpDown1.Value;
            m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.None);
        }
        //XOZ切片
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            m_YPos = (int)numericUpDown2.Value;
            trackBar1.Value = (int)numericUpDown2.Value;
            m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.None);
        }
        //YOZ切片
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            m_XPos = (int)numericUpDown3.Value;
            trackBar3.Value = (int)numericUpDown3.Value;
            m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.None);
        }

        //XOY切片
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            m_ZPos = trackBar2.Value;
            numericUpDown1.Value = trackBar2.Value;
            m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.None);
        }
        //XOZ切片
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            m_YPos = trackBar1.Value;
            numericUpDown2.Value = trackBar1.Value;
            m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.None);
        }
        //YOZ切片
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            m_XPos = trackBar3.Value;
            numericUpDown3.Value = trackBar3.Value;
            m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.None);
        }

        //启动普通模式
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.panel1.Enabled = false;
            this.panel2.Enabled = false;
            if (!radioButton1.Checked) return;
            m_3DModelCanvas.LookAt_NormalMode(fy3DModelViewMode.None);
        }

        //启动切片模式
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.panel1.Enabled = true;
            this.panel2.Enabled = false;
            if (!radioButton2.Checked) return;
            m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.None);
        }

        //启动镂空模式
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.panel1.Enabled = false;
            this.panel2.Enabled = true;
            if (!radioButton3.Checked) return;
            m_3DModelCanvas.LookAt_HollowOutMode(TransparentRanges, fy3DModelViewMode.None);
        }

        //Top View
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                m_3DModelCanvas.LookAt_NormalMode(fy3DModelViewMode.TopViewMode);
            }
            if (radioButton2.Checked)
            {
                m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.TopViewMode);
            }
            if (radioButton3.Checked)
            {
                m_3DModelCanvas.LookAt_HollowOutMode(TransparentRanges, fy3DModelViewMode.TopViewMode);
            }
        }

        //Face View
        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                m_3DModelCanvas.LookAt_NormalMode(fy3DModelViewMode.FaceViewMode);
            }
            if (radioButton2.Checked)
            {
                m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.FaceViewMode);
            }
            if (radioButton3.Checked)
            {
                m_3DModelCanvas.LookAt_HollowOutMode(TransparentRanges, fy3DModelViewMode.FaceViewMode);
            }
        }

        //Side View
        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                m_3DModelCanvas.LookAt_NormalMode(fy3DModelViewMode.SideViewMode);
            }
            if (radioButton2.Checked)
            {
                m_3DModelCanvas.LookAt_SliceMode(m_XPos, m_YPos, m_ZPos, fy3DModelViewMode.SideViewMode);
            }
            if (radioButton3.Checked)
            {
                m_3DModelCanvas.LookAt_HollowOutMode(TransparentRanges, fy3DModelViewMode.SideViewMode);
            }
        }

        //镂空显示
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                TransparentRanges = new List<fyTransparentRange>();
                string str = textBox1.Text.Trim();
                string[] str1 = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < str1.Length; i++)
                {
                    string[] str2 = str1[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    double min = double.Parse(str2[0]);
                    double max = double.Parse(str2[1]);
                    fyTransparentRange TransparentRange = new fyTransparentRange();
                    TransparentRange.Min = min;
                    TransparentRange.Max = max;
                    //如果Min和Max的取值发生冲突，并且该镂空范围无效
                    if (max < min)
                    {
                        MessageBox.Show("来自fy3DModelManager的警告\nMin&Max不符合!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }
                    TransparentRanges.Add(TransparentRange);
                }
            }
            catch
            {
                MessageBox.Show("来自fy3DModelManager的警告\n请输入正确的镂空范围!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //初始化镂空参数(采用默认的镂空参数)
                TransparentRanges = new List<fyTransparentRange>();
                fyTransparentRange TransparentRange = new fyTransparentRange();
                TransparentRange.Min = m_3DModelCanvas.MinValue;
                TransparentRange.Max = m_3DModelCanvas.MaxValue;
                TransparentRanges.Add(TransparentRange);
            }
            m_3DModelCanvas.LookAt_HollowOutMode(TransparentRanges, fy3DModelViewMode.None);
        }
    }
}
