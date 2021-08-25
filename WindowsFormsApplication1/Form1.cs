using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]  public static extern void OutputDebugString(string message);

        static ArrayList al = new ArrayList();

        string Cur_Video_Path = "";
        int Cur_Index = 0;
       

        public Form1()
        {
            InitializeComponent();
            this.listView1.Columns.Add("文件路径", 1000, HorizontalAlignment.Left); //一步添加
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string Cur_dir = "";
            Cur_dir = folderBrowserDialog1.SelectedPath;
            this.listView1.Items.Clear();
            //this.listView1.BeginUpdate();
            LoadSongList(Cur_dir);
            //this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制


        }
        private bool LoadSongList(string dPath)
        {
           
            DirectoryInfo TheFolder = new DirectoryInfo(dPath);
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = dPath + "\\" + NextFile.Name;
                string d = lvi.Text;
                d = d.ToLower();
                if (d.IndexOf(".mp4") == -1 && d.IndexOf(".avi") == -1 && d.IndexOf(".mod") == -1 && d.IndexOf(".wmv") == -1 && d.IndexOf(".mkv") == -1)
                {
                    continue;
                }
                this.listView1.Items.Add(lvi);
            }
            return true;
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //C#如何在ListView失去焦点的情况下仍然保持Item高亮
            foreach (ListViewItem itm in this.listView1.Items)
            {
                itm.BackColor = SystemColors.Window;
                itm.ForeColor = Color.Black;

            }
            foreach (ListViewItem itm2 in this.listView1.SelectedItems)
            {
                itm2.BackColor = SystemColors.MenuHighlight;
                itm2.ForeColor = Color.White;
            }

            ListView.SelectedIndexCollection indexes = listView1.SelectedIndices;
            // string pr = "";
             foreach (int index in indexes)
            {
                Cur_Video_Path = listView1.Items[index].Text;
                this.Text = Cur_Video_Path;
                textBox1.Text = Cur_Video_Path;
                axWindowsMediaPlayer1.URL = Cur_Video_Path;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                Cur_Index = index;
                //MessageBox.Show("Line:" + this.listView1.SelectedItems[0]);
                 break;
               //pr += listView1.Items[index].Text;
            }
            //MessageBox.Show(pr);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem item = listView.GetItemAt(e.X, e.Y);
            if (item != null && e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show(listView, e.X, e.Y);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DateTime beforDT = System.DateTime.Now;

            axWindowsMediaPlayer1.Ctlcontrols.stop();

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforDT);  
            Debug.WriteLine("STOP 总共花费ms."+ts.TotalMilliseconds);  

           
            //MessageBox.Show("删除:" + this.Cur_Video_Path);
            string d = this.Cur_Video_Path;
            al.Add(d);
            //File.Delete(d);

            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);  
            Debug.WriteLine("DEL 总共花费ms."+ ts.TotalMilliseconds);

            //this.listView1.BeginUpdate();
            listView1.Items.Remove(this.listView1.SelectedItems[0]);
            afterDT = System.DateTime.Now;
            ts = afterDT.Subtract(beforDT);  
            Debug.WriteLine("remove 总共花费{0}ms."+ ts.TotalMilliseconds);

            //this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("清空");
        }
        
         private static void ThreadProc(object obj)
        {
            while (true)
            {
                Thread.Sleep(1000);
                foreach (string szPath in al)
                {
                    if (File.Exists(szPath))
                    {
                        Debug.WriteLine(szPath);
                        File.Delete(szPath);
                       
                    }
                    al.Remove(szPath);
                    break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //this.listView1.BeginUpdate();
            string dPath = "D:\\code\\2.pycode\\13.douyin\\video";
           
            if (Directory.Exists(dPath) == false)
            {
                string Cur_path = System.IO.Directory.GetCurrentDirectory() + "\\video";
                if (Directory.Exists(Cur_path) == false)
                {
                    Directory.CreateDirectory(Cur_path); 
                }
                dPath = Cur_path;
            }
            folderBrowserDialog1.SelectedPath = dPath;

            //加载目录
            LoadSongList(dPath);

            //显示加载目录
            textBox1.Text = dPath;
            //this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制

            Thread t1 = new Thread(ThreadProc);
            t1.Start(10);            
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                
                int iIndex = 0;
                foreach (ListViewItem var in listView1.SelectedItems)
                {
                    ListViewItem item = var;
                    iIndex = var.Index;
                    
                }
                toolStripMenuItem1_Click(sender, null);
                listView1.Items[iIndex].Selected = true;
            
            }
            //MessageBox.Show("" + e.KeyValue);
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                    //自动播放下一首
                    int iIndex = 0;
                    foreach (ListViewItem var in listView1.SelectedItems)
                    {
                        ListViewItem item = var;
                        iIndex = var.Index;
                    }
                    if (iIndex < listView1.Items.Count) 
                    {
                       // this.listView1.BeginUpdate();

                        if (comboBox1.SelectedIndex == 0)
                        {
                            Debug.WriteLine(axWindowsMediaPlayer1.URL);
                            Debug.WriteLine(this.listView1.Items[iIndex + 1].Text);
                            axWindowsMediaPlayer1.URL = this.listView1.Items[iIndex + 1].Text;
                            listView1.Items[iIndex].Selected = false;
                            listView1.Items[iIndex + 1].Selected = true;
                        }
                        if (comboBox1.SelectedIndex == 1)
                        {
                            Debug.WriteLine(axWindowsMediaPlayer1.URL);
                            Debug.WriteLine(this.listView1.Items[iIndex + 1].Text);
                            axWindowsMediaPlayer1.URL = this.listView1.Items[iIndex ].Text;
                            listView1.Items[iIndex].Selected = false;
                            listView1.Items[iIndex].Selected = true;
                        }

                        
                        
                        //this.listView1.EndUpdate();
                    }
                    
            }
            else if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsReady)     //播放器准备就绪后播放
            {
                if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1)//自动播放下一首
                {
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
               
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (Cur_Index < listView1.Items.Count)
            {
                listView1.Items[Cur_Index].Selected = true;
                Debug.WriteLine("当前焦点:" + Cur_Index);
            }
        }
    }
}
