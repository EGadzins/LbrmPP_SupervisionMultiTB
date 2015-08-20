using System;
using System.Threading;
using System.Net.Sockets;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LbrmPP_SupervisionMultiTB
{
    
   public partial class Form1 : Form
    {

       private BackgroundWorker bgWorkerCB;
       private BackgroundWorker bgWorkerTCP_PUP;
       private ClassTCP TCPPUP;
       private Color ColorDefault = System.Drawing.Color.Black;
       private Color ColorRed = System.Drawing.Color.Red;
       private Color ColorGreen = System.Drawing.Color.Green;
       private Color ColorOrange = System.Drawing.Color.Orange;
        
       public Form1()
        {
            InitializeComponent();

            bgWorkerCB = new BackgroundWorker();
            bgWorkerCB.WorkerReportsProgress = true;
            bgWorkerCB.DoWork += bgWorkerCB_Process;
            bgWorkerCB.ProgressChanged += bgWorkerCB_ProgressChanged;
            bgWorkerCB.RunWorkerCompleted += bgWorkerCB_RunWorkerCompleted;

            bgWorkerTCP_PUP = new BackgroundWorker();
            bgWorkerTCP_PUP.WorkerReportsProgress = true;
            bgWorkerTCP_PUP.DoWork += bgWorkerTCP_PUP_Process;
            bgWorkerTCP_PUP.ProgressChanged += bgWorkerTCP_PUP_ProgressChanged;
            bgWorkerTCP_PUP.RunWorkerCompleted += bgWorkerTCP_PUP_RunWorkerCompleted;
        }

        private void btnStartSup_Click(object sender, EventArgs e)
        {
            btnStartSup.Enabled = false;
            tabControl1.SelectedTab = tabSupTB;
            
            bgWorkerTCP_PUP.RunWorkerAsync();
        }

        public void Update_RichTextBox(Color msgColor, string RichTextBox, string Msg)
        {
            CultureInfo culture = new CultureInfo("en-gb");
            string timeStamp = DateTime.Now.ToString("G", culture);
            Invoke(new Action(() =>
            {
                richTextBoxPUP.SelectionColor = msgColor;
                richTextBoxPUP.AppendText(timeStamp + " " + Msg + "\n");
                richTextBoxPUP.SelectionStart = richTextBoxPUP.Text.Length;
                richTextBoxPUP.ScrollToCaret();
            }
            ));
        }

        #region bgWorkerCB
        /// <summary>
        /// Thread pour l'interface CB
        /// </summary>

        private void bgWorkerCB_Process(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = (BackgroundWorker)sender;
            for (var i = 0; i < 100; i++)
            {
                bgWorker.ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        private void bgWorkerCB_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //if (e.ProgressPercentage >= 20) labelSUPCB1.ForeColor = System.Drawing.Color.Green;
            //if (e.ProgressPercentage >= 40) labelSUPCB2.ForeColor = System.Drawing.Color.Green;
            //if (e.ProgressPercentage >= 60) labelSUPCB3.ForeColor = System.Drawing.Color.Green;
            //if (e.ProgressPercentage >= 80) labelSUPCB4.ForeColor = System.Drawing.Color.Green;
        }

        void bgWorkerCB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //btnStartSup.Enabled = true;
        }
        #endregion

        #region bgWorkerTCP_PUP
        /// <summary>
        /// Thread pour l'interface TCP avec le PC PUP
        /// </summary>

        private void bgWorkerTCP_PUP_Process(object sender, DoWorkEventArgs e)
        {
            //string strRcv = null;
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            TCPPUP = new ClassTCP("10.23.154.58", "PUP", bgWorker, this);
            if (TCPPUP.Status > 0) 
            {
                TCPPUP.InitRP();
                TCPPUP.LoadXMLandStart();
            }

            while (TCPPUP.Action<100)
            {
                //1 = ReConnexion
                //2 = Reload XML
                //90 = Stop
                //100 = Exit the loop
                switch (TCPPUP.Action)
                {
                    case 1: //1 = ReConnexion
                        Invoke(new Action(() => { buttonPUPReconn.Enabled = false; }));
                        Invoke(new Action(() => { buttonPUPReload.Enabled = false; }));
                        Invoke(new Action(() => { labelPUPConn.ForeColor = ColorOrange; }));
                        TCPPUP.Reconnect();
                        TCPPUP.Action = 0;
                        break;
                    case 2: //2 = Reload XML
                        TCPPUP.LoadXMLandStart();
                        TCPPUP.Action = 0;
                        break;
                    default:
                        TCPPUP.Action = 0;
                        break;
                }
            }

            if (TCPPUP.Status > 0)
            {
                //Thread.Sleep(3000);
                TCPPUP.Close();
            }
        }

        private void bgWorkerTCP_PUP_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Status :
            //2 = End of Init
            //3 = End of HARD.xml
            //4 = End of EACB.xml
            //5 = End of StartEqt
            
            switch (e.ProgressPercentage)
            {
                case 0:
                    break;
                case 1:
                    Invoke(new Action(() => { labelPUPConn.ForeColor = ColorGreen; }));
                    Invoke(new Action(() => { labelPUPHard.ForeColor = ColorDefault; }));
                    Invoke(new Action(() => { labelPUPEacb.ForeColor = ColorDefault; }));
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = ColorDefault; }));
                    Invoke(new Action(() => { buttonPUPReconn.Enabled = false; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = false; }));
                    Invoke(new Action(() => { buttonPUPReinit.Enabled = false; }));
                    break;
                case -1:
                    Invoke(new Action(() => { labelPUPConn.ForeColor = ColorRed; }));
                    Update_RichTextBox(Color.Red, "PUP", "## Probleme de connexion. Vérifier que TB_RP_Server est démarré sur le PC distant.");
                    Update_RichTextBox(Color.Red, "PUP", "## Utiliser le bouton 'Reconnexion' pour retenter, puis 'Reload XML'.");
                    Invoke(new Action(() => { buttonPUPReconn.Enabled = true; }));
                    break;
                case 2 :
                    Invoke(new Action(() => { buttonPUPReconn.Enabled = true; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = true; }));
                    break;
                case 3:
                    Invoke(new Action(() => { labelPUPHard.ForeColor = ColorGreen; }));
                    break;
                case -3:
                    Invoke(new Action(() => { labelPUPHard.ForeColor = ColorRed; }));
                    break;
                case 4:
                    Invoke(new Action(() => { labelPUPEacb.ForeColor = ColorGreen; }));
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = ColorOrange; }));
                    break;
                case -4:
                    Invoke(new Action(() => { labelPUPEacb.ForeColor = ColorRed; }));
                    break;
                case 5:
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = ColorGreen; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = true; }));
                    Update_RichTextBox(ColorGreen, "PUP", "== Configuration terminée.");
                    break;
                case -5:
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = ColorRed; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = true; }));
                    break;
                default:
                    break;
            }
        }

        void bgWorkerTCP_PUP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown. 
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled the operation. 
                // Note that due to a race condition in the DoWork event handler, the Cancelled flag may not have been set, even though CancelAsync was called.
                MessageBox.Show("bgWorkerTCP_PUP Canceled");
            }
            else
            {
                // Finally, handle the case where the operation  
                // succeeded.
                MessageBox.Show(e.Result.ToString());;
            }
            Update_RichTextBox(Color.Orange, "PUP", "##  Fin du Worker TCPPUP ##");
        }
        #endregion

        private void buttonPUPReconn_Click(object sender, EventArgs e)
        {
            TCPPUP.Action = 1;
        }

        private void buttonPUPReload_Click(object sender, EventArgs e)
        {
            TCPPUP.Action = 2;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            TCPPUP.Action = 100;
        }

        private void richTextBoxPUP_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
