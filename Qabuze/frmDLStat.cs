using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Qabuze
{
    public partial class frmDLStat : Form
    {

        public static frmDLStat instance = new frmDLStat();

        public bool updating = false;
        public frmDLStat()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDLStat_FormClosing);
        }

        private void frmDLStat_Load(object sender, EventArgs e)
        {
            this.Text = "Download status: 0%";
        }

        public void setProgressbarThreadSafe(int max, int value)
        {

            ThreadPool.QueueUserWorkItem(o => this.setProgressbar(max, value));

        }

        public void setProgressbarThreadSafe(int value)
        {

            ThreadPool.QueueUserWorkItem(o => this.setProgressbar(value));

        }

        private void setProgressbar(int max, int value)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.progressBar1.Maximum = max;
            setProgressbar(value);

        }
        private void setProgressbar(int value)
        {

            Control.CheckForIllegalCrossThreadCalls = false;
            this.progressBar1.Value = value;
            this.Text = "Download status: " + value.ToString() + "%";

        }
        private void frmDLStat_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void updateView(IDictionary<int, QabuzeDLMThreadStatus> status)
        {
            while (updating) {
                Thread.Sleep(1);
            }
            updating = true;
            lock (QabuzeDLM.status)
            {

                listView1.Items.Clear();

                foreach (KeyValuePair<int, QabuzeDLMThreadStatus> current in QabuzeDLM.status)
                {

                listView1.Items.Add(new ListViewItem(new [] {
                    current.Key.ToString(),
                    current.Value.percentage.ToString(),
                    current.Value.getStatusAsDLMEvent().ToString(),
                    current.Value.album.track_count.ToString(),
                    current.Value.songsLoaded.ToString(),
                    current.Value.songsTagged.ToString(),
                    current.Value.songsDone.ToString(),
                    current.Value.songsFailed.ToString(),
                    current.Value.album.title,
                    current.Value.album.artist
                }));

            } 

            }
            updating = false;
            
        }

        public void updateThreadSafe(IDictionary<int, QabuzeDLMThreadStatus> status)
        {

            ThreadPool.QueueUserWorkItem(o => this.updateView(status));

        }
    }
}
