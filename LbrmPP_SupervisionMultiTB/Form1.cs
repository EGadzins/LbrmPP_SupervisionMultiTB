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

        public void Update_RichTextBox(bool SndRcv, string RichTextBox, string Msg)
        {
            CultureInfo culture = new CultureInfo("en-gb");
            string timeStamp = DateTime.Now.ToString("G", culture);
            Invoke(new Action(() =>
            {
                richTextBoxPUP.AppendText(">" + timeStamp + Msg + "\n");
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
            int ReportProgress = 0;
            BackgroundWorker bgWorker = (BackgroundWorker)sender;

            // connect with a 5 second timeout on the connection
            TcpClient connection = new TcpClientWithTimeout("10.23.154.14", 5555, 5000).Connect();
            NetworkStream stream = connection.GetStream();

            Invoke(new Action(() => { labelPUPConn.ForeColor = System.Drawing.Color.LawnGreen; }));
            bgWorker.ReportProgress(ReportProgress++);

            string strToSend = null;
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("K?");
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
            byte[] myReadBuffer = new byte[1024];
            int numberOfBytesRead = 0;
            numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
            string contents = null;
            contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);

            strToSend = "KS";
            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);
            Update_RichTextBox(true, "PUP", strToSend);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                Update_RichTextBox(true, "PUP", contents);
            } while (String.Compare(contents, "HB") == 0);
            bgWorker.ReportProgress(ReportProgress++);

            strToSend = "KRC:/Documents and Settings/Administrator/Desktop/TB_RP_Docs/HARD_US2.xml";
            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);
            Update_RichTextBox(true, "PUP", strToSend);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                Update_RichTextBox(false, "PUP", contents);
            } while (String.Compare(contents, "HB") == 0);
            bgWorker.ReportProgress(ReportProgress++);
            if (contents.StartsWith("OK"))
                {
                    Invoke(new Action(() => { labelPUPHard.ForeColor = System.Drawing.Color.LawnGreen; }));
                }
                else
                {
                    Invoke(new Action(() => { labelPUPHard.ForeColor = System.Drawing.Color.DarkRed; }));
                }

            strToSend = "KRC:/Documents and Settings/Administrator/Desktop/TB_RP_Docs/EACB_US2.xml"; 
            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);
            Update_RichTextBox(true, "PUP", strToSend);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length); 
            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                Update_RichTextBox(false, "PUP", contents);
            } while (String.Compare(contents, "HB") == 0);
            bgWorker.ReportProgress(ReportProgress++);
            if (contents.StartsWith("OK"))
            {
                Invoke(new Action(() => { labelPUPEacb.ForeColor = System.Drawing.Color.LawnGreen; }));
            }
            else
            {
                Invoke(new Action(() => { labelPUPEacb.ForeColor = System.Drawing.Color.DarkRed; }));
            }

            strToSend = "KACT2_cip"; 
            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);
            Update_RichTextBox(true, "PUP", strToSend);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length); 
            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                Update_RichTextBox(false, "PUP", contents);
            } while (String.Compare(contents, "HB") == 0);
            bgWorker.ReportProgress(ReportProgress++);

            strToSend = "LE"; //List Equipements
            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);
            Update_RichTextBox(true, "PUP", strToSend);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length); 
            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                Update_RichTextBox(false, "PUP", contents);
            } while (String.Compare(contents, "HB") == 0);
            bgWorker.ReportProgress(ReportProgress++);

            strToSend = "KZ"; // Reset Conf
            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);
            Update_RichTextBox(true, "PUP", strToSend);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length); 
            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                Update_RichTextBox(false, "PUP", contents);
            } while (String.Compare(contents, "HB") == 0);
            bgWorker.ReportProgress(ReportProgress++);

            strToSend = "Ks"; //Stop
            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);
            Update_RichTextBox(true, "PUP", strToSend);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length); 
            do
            {
                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                contents = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                Update_RichTextBox(false, "PUP", contents);
            } while (String.Compare(contents, "HB") == 0);
            bgWorker.ReportProgress(ReportProgress++);

            // Disconnect nicely
            stream.Close(); // workaround for a .net bug: http://support.microsoft.com/kb/821625
            connection.Close();
            bgWorker.ReportProgress(ReportProgress++);

            Thread.Sleep(100);
        }

        private void bgWorkerTCP_PUP_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //if (e.ProgressPercentage >= 20) labelSUPCB1.ForeColor = System.Drawing.Color.Green;
        }

        void bgWorkerTCP_PUP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //btnStartSup.Enabled = true;
        }
        #endregion

        private void richTextBoxPUP_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
