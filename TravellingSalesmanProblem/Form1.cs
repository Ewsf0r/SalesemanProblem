using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace TravellingSalesmanProblem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonAddRandom_Click(object sender, EventArgs e)
        {
            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < (int)numericUpDownQuantity.Value; i++)
            {
                dataGridViewEnter.Rows.Add();
                dataGridViewEnter.Rows[dataGridViewEnter.RowCount - 2].Cells["ColumnLocationName"].Value = dataGridViewEnter.RowCount - 1;
                dataGridViewEnter.Rows[dataGridViewEnter.RowCount - 2].Cells["ColumnX"].Value = r.NextDouble() + r.Next(1000);
                dataGridViewEnter.Rows[dataGridViewEnter.RowCount - 2].Cells["ColumnY"].Value = r.NextDouble() + r.Next(1000);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            dataGridViewEnter.Rows.Clear();
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            dataGridViewResult.Rows.Clear();
            if (dataGridViewEnter.RowCount > 1)
            {
                string[] locationName = new string[dataGridViewEnter.RowCount - 1];
                double[] locationX = new double[dataGridViewEnter.RowCount - 1];
                double[] locationY = new double[dataGridViewEnter.RowCount - 1];
                for (int i = 0; i < dataGridViewEnter.RowCount - 1; i++)
                {
                    locationName[i] = dataGridViewEnter.Rows[i].Cells["ColumnLocationName"].Value.ToString();
                    locationX[i] = Convert.ToDouble(dataGridViewEnter.Rows[i].Cells["ColumnX"].Value);
                    locationY[i] = Convert.ToDouble(dataGridViewEnter.Rows[i].Cells["ColumnY"].Value);
                }
                Locations allLocations = new Locations(locationName, locationX, locationY);
                Locations.Result[] result = allLocations.NearestNeighbour(locationName);
                if (comboBoxMethod.SelectedItem.ToString().Contains("перебор"))
                    result = allLocations.BruteForce(locationName);
                for (int i= 0; i< result.Length; i++)
                {
                    dataGridViewResult.Rows.Add();
                    dataGridViewResult.Rows[i].Cells["ColumnResultName"].Value = result[i].ResultName;
                    dataGridViewResult.Rows[i].Cells["ColumnResultX"].Value = result[i].ResultX;
                    dataGridViewResult.Rows[i].Cells["ColumnResultY"].Value = result[i].ResultY;
                    dataGridViewResult.Rows[i].Cells["ColumnDistance"].Value = result[i].ResultDistanceToPrevious;
                }
                dataGridViewResult.Rows.Add();
                dataGridViewResult.Rows[dataGridViewResult.RowCount - 2].Cells["ColumnDistance"].Value
                    = result[0].ResultTotalDistance;
                dataGridViewResult.Rows[dataGridViewResult.RowCount - 2].Cells["ColumnResultName"].Value
                    = "Общая длина маршрута";
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            using (StreamReader sr = new StreamReader(myStream))
                            {
                                string line = sr.ReadToEnd();
                                string subString = "";
                                string separators = " -/\\\n\t\0";
                                bool isPreviosSeparator = false, isLocationName = true, isX = false, isY = false;
                                foreach(char c in line)
                                {
                                    if (separators.Contains(c))
                                    {
                                        if (!isPreviosSeparator)
                                        {
                                            if (isLocationName)
                                            {
                                                dataGridViewEnter.Rows.Add();
                                                dataGridViewEnter.Rows[dataGridViewEnter.RowCount - 2].Cells["ColumnLocationName"].Value = subString;
                                                isX = true;
                                                isLocationName = false;
                                            }
                                            else if (isX)
                                            {
                                                dataGridViewEnter.Rows[dataGridViewEnter.RowCount - 2].Cells["ColumnX"].Value = subString;
                                                isY = true;
                                                isX = false;
                                            }
                                            else if (isY)
                                            {
                                                dataGridViewEnter.Rows[dataGridViewEnter.RowCount - 2].Cells["ColumnY"].Value = subString;
                                                isLocationName = true;
                                                isY = false;
                                            }
                                        }
                                        subString = "";
                                        isPreviosSeparator = true;
                                    }
                                    else
                                    {
                                        subString += c;
                                        isPreviosSeparator = false;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
    }
}
