using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Qabuze
{
    public partial class frmSearch : Form
    {

        //public event FormClosingEventHandler frmSearch_FormClosing;
        private int offset = 0;
        private string query = "";
        public static frmSearch instance = new frmSearch();
        public frmSearch()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSearch_FormClosing);
        }

        public void initSearch(string query, int offset = 0) {
            if (offset == 0) {
                treeView1.Nodes.Clear();
            }
            this.query = query;
            this.Text = "Search on: \"" + this.query + "\"";
            List<QabuzeAlbum> results = (List<QabuzeAlbum>) QabuzeAPI.instance.search(query);

            if (results == null) {
                return;
            }
            foreach (QabuzeAlbum album in results)
            {
                try
                {
                    if (!treeView1.Nodes.ContainsKey(album.artistID.ToString()))
                    {
                        treeView1.Nodes.Add(album.artistID.ToString(), album.artist);
                    }
                    if (!treeView1.Nodes[treeView1.Nodes.IndexOfKey(album.artistID.ToString())].Nodes.ContainsKey(album.id))
                    {
                        treeView1.Nodes[treeView1.Nodes.IndexOfKey(album.artistID.ToString())].Nodes.Add(album.id, album.title);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }

            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode current = treeView1.SelectedNode;
            if (current != null)
            {
                if (current.Level > 0)
                {
                    Console.WriteLine(current.Name);
                    frmMain.instance.setAlbumID(current.Name);
                }
            }
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {

        }

        private void btnTryLM_Click(object sender, EventArgs e)
        {
            offset += 50;
            initSearch(query, offset);
        }

        private void frmSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
