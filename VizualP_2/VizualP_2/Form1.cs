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
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Serialization;

namespace VP_2
{
    public partial class Form1 : Form
    {
        private bool _sizeChanged = false;
        private int _numberOfPoints = 10;
        private float _parameterStart = 0;
        private float _parameterEnd = 1;
        private int _initialGridViewHeight;
        private int _initialChartHeight;
        private int _initialChartWidth;
        private int _initialFormHeight;
        private int _initialFormWidth;
        private int _initialButtonsPanelOffset;
        private List<Point> points;

        public Form1()
        {
            InitializeComponent();
            chart1.Legends[0].Enabled = false;
        }

        public void DisplayPoints(List<Point> points) 
        {
            this.points = points;
            DataTable dt = new DataTable();
            dt.Columns.Add("X", typeof(float));
            dt.Columns.Add("Y", typeof(float));
            foreach (var point in points) 
            {
                dt.Rows.Add(point.X,point.Y);
            }
            dataGridView1.DataSource = dt;
            chart1.Series[0].Points.Clear();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            foreach (var point in points)//remove continues when resolve this exception
            {
                float key;
                float value;
                if (point.X == float.PositiveInfinity)
                {
                    key = float.MaxValue;
                    continue;
                }
                else if (point.X == float.NegativeInfinity)
                {
                    key = float.MinValue;
                    continue;
                }
                else 
                {
                    key = point.X;
                }
                if (point.Y == float.PositiveInfinity)
                {
                    value = float.MaxValue;
                    continue;
                }
                else if (point.Y == float.NegativeInfinity)
                {
                    value = float.MinValue;
                    continue;
                }
                else 
                {
                    value = point.Y;
                }
                if (point.X == float.NaN || point.Y == float.NaN) 
                {
                    continue;
                }
                chart1.Series[0].Points.AddXY(Math.Round(key, 3), Math.Round(value, 3));
            }
        }

        private void Form1_StartResize(object sender, EventArgs e)
        {
            if (!_sizeChanged)
            {
                _initialChartWidth = chart1.Width;
                _initialChartHeight = chart1.Height;
                _initialGridViewHeight = dataGridView1.Height;
                _initialFormHeight = this.Height;
                _initialFormWidth = this.Width;
                _initialButtonsPanelOffset = panel1.Top;
                _sizeChanged = true;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (_sizeChanged)
            {
                int deltaHeight = this.Height - _initialFormHeight;
                dataGridView1.Height = _initialGridViewHeight + deltaHeight;
                chart1.Height = _initialChartHeight + deltaHeight;
                panel1.Top = _initialButtonsPanelOffset + deltaHeight;
                int deltaWidth = this.Width - _initialFormWidth;
                chart1.Width = _initialChartWidth + deltaWidth;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = Program.FunctionPoints(_numberOfPoints, _parameterStart, _parameterEnd, Program.FuncOneX, Program.FuncOneY);
            DisplayPoints(data);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var data = Program.FunctionPoints(_numberOfPoints, _parameterStart, _parameterEnd, Program.FuncTwoX, Program.FuncTwoY);
            DisplayPoints(data);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _numberOfPoints = (int)(sender as NumericUpDown).Value;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPeg Image|*.jpg";
            saveFileDialog.Title = "Save Chart As Image File";
            saveFileDialog.FileName = "Sample.png";

            DialogResult result = saveFileDialog.ShowDialog();
            saveFileDialog.RestoreDirectory = true;

            if (result == DialogResult.OK && saveFileDialog.FileName != "")
            {
                try
                {
                    if (saveFileDialog.CheckPathExists)
                    {
                        if (saveFileDialog.FilterIndex == 2)
                        {
                            chart1.SaveImage(saveFileDialog.FileName, ChartImageFormat.Jpeg);
                        }
                        else if (saveFileDialog.FilterIndex == 1)
                        {
                            chart1.SaveImage(saveFileDialog.FileName, ChartImageFormat.Png);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Given Path does not exist");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML file|*.xml";
            saveFileDialog.Title = "Save Chart As XML File";
            saveFileDialog.FileName = "Sample.xml";

            DialogResult result = saveFileDialog.ShowDialog();
            saveFileDialog.RestoreDirectory = true;

            if (result == DialogResult.OK && saveFileDialog.FileName != "")
            {
                try
                {
                    if (saveFileDialog.CheckPathExists)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Point>));
                        TextWriter textWriter = new StreamWriter(saveFileDialog.FileName);
                        serializer.Serialize(textWriter, points);
                        textWriter.Close();
                    }
                    else
                    {
                        MessageBox.Show("Given Path does not exist");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML file|*.xml";
            openFileDialog.Title = "Open Saved Chart Points";
            openFileDialog.FileName = "Sample.xml";

            DialogResult result = openFileDialog.ShowDialog();
            openFileDialog.RestoreDirectory = true;

            if (result == DialogResult.OK && openFileDialog.FileName != "")
            {
                try
                {
                    if (openFileDialog.CheckPathExists)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Point>));
                        TextReader textReader = new StreamReader(openFileDialog.OpenFile());
                        DisplayPoints(serializer.Deserialize(textReader) as List<Point>);
                        textReader.Close();
                    }
                    else
                    {
                        MessageBox.Show("Given Path does not exist");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            float start;
            if (!float.TryParse(textBox.Text, out start) || start >= _parameterEnd)
            {
                errorProvider1.SetError(textBox, "It must be real number and it must be smaller than upper bound.");
                e.Cancel = true;
            }
            else 
            {
                _parameterStart = start;
                errorProvider1.SetError(textBox, string.Empty);
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            float end;
            if (!float.TryParse(textBox.Text, out end) || end <= _parameterStart)
            {
                errorProvider1.SetError(textBox, "It must be real number and it must be bigger than lower bound.");
                e.Cancel = true;
            }
            else
            {
                _parameterEnd = end;
                errorProvider1.SetError(textBox, string.Empty);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
