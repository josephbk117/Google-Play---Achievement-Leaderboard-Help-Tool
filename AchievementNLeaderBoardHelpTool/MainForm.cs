using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MetroFramework.Forms;

namespace AchievementNLeaderBoardHelpTool
{


    public partial class MainForm : MetroForm
    {
        OpenFileDialog ofd;
        string[] achievements;
        List<int> achievementsChecked;
        List<int> checkedACH;


        public MainForm()
        {            
            InitializeComponent();            
        }
        
        void setUpAction() {
            listView1.Items.Clear();
            listView2.Items.Clear();


            string resourceData = richTextBox1.Text;          
            
            int endIndex = resourceData.LastIndexOf('}');
            int firstIndex = resourceData.IndexOf('{') + 1;
            resourceData = resourceData.Substring(firstIndex, endIndex - firstIndex);
            resourceData = resourceData.Replace("// <GPGSID>", "");
            resourceData = resourceData.Replace("        ", "");
            resourceData = resourceData.Replace(";", "");
            // Console.WriteLine("New resource : " + Environment.NewLine + resourceData + Environment.NewLine);
            int numberOfResources = 0;
            resourceData = resourceData.Trim();
            char[] temp = resourceData.ToCharArray();
            for (int i = 0; i < resourceData.Length; i++)
            {
                if (temp[i] == '\n')
                    numberOfResources++;
            }

            Console.WriteLine("Lines = " + numberOfResources);
            string[] seperate_res = resourceData.Split('\n', '=');

            int numberOfACH = 0;
            int numberOfLEA = 0;
            for (int i = 0; i < seperate_res.Length; i++)
            {
                if (seperate_res[i].Contains("achievement"))
                    numberOfACH++;
                if (seperate_res[i].Contains("leaderboard"))
                    numberOfLEA++;
            }

            string[,] achievements = new string[numberOfACH, 2];
            string[,] leaderBoard = new string[numberOfLEA, 2];
            achievementsChecked = new List<int>(numberOfACH);

            int _numACH = 0, _numLEA = 0;

            for (int i = 0; i < seperate_res.Length; i++)
            {
                if (seperate_res[i].Contains("achievement") || seperate_res[i].Contains("leaderboard"))
                {
                    seperate_res[i] = seperate_res[i].Remove(0, 20);
                    if (seperate_res[i].Contains("achievement"))
                    {
                        achievements[_numACH, 0] = seperate_res[i];
                        achievements[_numACH, 1] = seperate_res[i + 1];
                        _numACH++;
                    }
                    else if (seperate_res[i].Contains("leaderboard"))
                    {
                        leaderBoard[_numLEA, 0] = seperate_res[i];
                        leaderBoard[_numLEA, 1] = seperate_res[i + 1];
                        _numLEA++;
                    }

                }
            }


            Console.WriteLine("All The Achievements : ");
            for (int i = 0; i < numberOfACH; i++)
                Console.WriteLine("::" + achievements[i, 0]);
            Console.WriteLine("All The Leaderboards : ");
            for (int i = 0; i < numberOfLEA; i++)
                Console.WriteLine("::" + leaderBoard[i, 0]);

            for (int i = 0; i < numberOfACH; i++)
            {
                ListViewItem item = new ListViewItem(achievements[i, 0].Remove(0, 12).Replace("_", " "));
                item.SubItems.Add(achievements[i, 1].Replace("\"", ""));
                listView1.Items.Add(item);
            }
            for (int i = 0; i < numberOfLEA; i++)
            {
                ListViewItem item = new ListViewItem(leaderBoard[i, 0].Remove(0, 12).Replace("_", " "));
                item.SubItems.Add(leaderBoard[i, 1].Replace("\"", ""));
                listView2.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                setUpAction();
            }
            catch {
                MessageBox.Show("Error : No File Opened");
            }           
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            DialogResult result = MessageBox.Show("Are You Sure", "Leaving The Application",MessageBoxButtons.YesNo);
            if(result==DialogResult.Yes)
                Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK) {
                richTextBox1.Text = System.IO.File.ReadAllText(ofd.FileName);                
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "GPH File(*.pgh;)|*.GPH;";
                string[] content = new string[achievementsChecked.Count + 1];
                if (ofd.FileName != null)
                    content[0] = ofd.FileName;


                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    int j = 1;
                    foreach (var item in achievementsChecked)
                    {
                        content[j] = item.ToString();
                        j++;
                    }
                    System.IO.File.WriteAllLines(sfd.FileName, content);
                }
            }
            catch
            {
                MessageBox.Show("You Have Not Yet Opened A File");
            }            
        }

        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            MessageBox.Show("Checked : " + e.Index);
            achievementsChecked.Add(e.Index);            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog svofd = new OpenFileDialog();
            svofd.Filter = "GPH File(*.pgh;)|*.GPH;";
            if (svofd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                int numOfChecks = 0;
                using (StreamReader sr = new StreamReader(svofd.FileName))
                {
                    while (sr.Peek() >= 0)
                    {
                        sb.Append(sr.ReadLine() + '\n');                        
                    }
                }
                for (int i = 0; i < sb.Length; i++) {
                    if (sb.ToString().ToCharArray()[i] == '\n')
                        numOfChecks++;
                }
                string[] path = sb.ToString().Split('\n');
                Console.WriteLine("FIle ::" + path[0] + " number of lines (checks) : " + numOfChecks);
                richTextBox1.Text = System.IO.File.ReadAllText(path[0]);
                setUpAction();
                
                
                checkedACH = new List<int>(numOfChecks);
                for (int i = 1; i < numOfChecks; i++)
                {
                    checkedACH.Add(int.Parse(path[i]));
                }
                foreach (int i in checkedACH){
                    Console.WriteLine("checks : " + i);
                }

                Console.WriteLine("Number of list items = " + listView1.Items.Count);

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if(checkedACH.Contains(i))
                        listView1.Items[i].Checked = true;
                }

            }
        }

        private void ascendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView2.Sorting = SortOrder.Ascending;
        }

        private void descendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView2.Sorting = SortOrder.Descending;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView2.Sorting = SortOrder.None;
            setUpAction();
        }
    }
}
