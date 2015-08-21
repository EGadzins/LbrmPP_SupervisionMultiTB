﻿using System;
using System.Configuration;
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
using System.Diagnostics;
using CBCOMServer;


namespace LbrmPP_SupervisionMultiTB
{
    
   public partial class Form1 : Form
    {

       private BackgroundWorker bgWorkerCB;
       private BackgroundWorker bgWorkerTCP_PUP, bgWorkerTCP_VE1, bgWorkerTCP_VIU, bgWorkerTCP_VC1, bgWorkerTCP_VC2, bgWorkerTCP_VI2, bgWorkerTCP_VE2, bgWorkerTCP_VUE3D, bgWorkerTCP_BISTD;
       private ClassTCP TCPPUP;
       private Color myColorDefault = System.Drawing.Color.Black;
       private Color myColorRed = System.Drawing.Color.Red;
       private Color myColorGreen = System.Drawing.Color.Green;
       private Color myColorOrange = System.Drawing.Color.Orange;
       private int bgWorkerCB_Action = 0;
       struct TestBench
       {
           public string IP;
       }
       private TestBench myTB_PUP, myTB_VE1, myTB_VIU, myTB_VC1, myTB_VC2, myTB_VI2, myTB_VE2, myTB_VUE3D, myTB_BISTD;
        
       public Form1()
        {
            InitializeComponent();

            bgWorkerCB = new BackgroundWorker();
            bgWorkerCB.WorkerReportsProgress = true;
            bgWorkerCB.WorkerSupportsCancellation = true;
            bgWorkerCB.DoWork += bgWorkerCB_Process;
            bgWorkerCB.ProgressChanged += bgWorkerCB_ProgressChanged;
            bgWorkerCB.RunWorkerCompleted += bgWorkerCB_RunWorkerCompleted;

            bgWorkerTCP_PUP = new BackgroundWorker();
            bgWorkerTCP_PUP.WorkerReportsProgress = true;
            bgWorkerTCP_PUP.DoWork += bgWorkerTCP_PUP_Process;
            bgWorkerTCP_PUP.ProgressChanged += bgWorkerTCP_PUP_ProgressChanged;
            bgWorkerTCP_PUP.RunWorkerCompleted += bgWorkerTCP_PUP_RunWorkerCompleted;


            try
            {
                textBox_PathCB.Text = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["PathCB"].ToString();
                textBox_CBProjectName.Text = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["CBProjectName"].ToString();
                textBox_CBApplicationName.Text = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["CBApplicationName"].ToString();
                textBox_CBTaskName.Text = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["CBTaskName"].ToString();
                textBox_PathXML.Text = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["PathXML"].ToString();
                myTB_PUP.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_PUP"].ToString();
                myTB_VE1.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_VE1"].ToString();
                myTB_VIU.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_VIU"].ToString();
                myTB_VC1.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_VC1"].ToString();
                myTB_VC2.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_VC2"].ToString();
                myTB_VI2.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_VI2"].ToString();
                myTB_VE2.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_PVE2"].ToString();
                myTB_VUE3D.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_Vue3D"].ToString();
                myTB_BISTD.IP = LbrmPP_SupervisionMultiTB.Properties.Settings.Default["IP_SimuBiStd"].ToString();
            }
            catch { }
        }

        private void btnStartSup_Click(object sender, EventArgs e)
        {
            btnStartSup.Enabled = false;
            tabControl1.SelectedTab = tabSupTB;

            if (checkBoxCB.Checked) bgWorkerCB.RunWorkerAsync();
            bgWorkerTCP_PUP.RunWorkerAsync();
        }

        public void Update_RichTextBox(Color msgColor, string RichTextBox, string Msg)
        {
            RichTextBox myRichTextBox = null;
            CultureInfo culture = new CultureInfo("en-gb");
            string timeStamp = DateTime.Now.ToString("HHmmss", culture);

            switch (RichTextBox){
                case "PUP": myRichTextBox = richTextBoxPUP;
                    break;
                case "VE1": myRichTextBox = richTextBoxVE1;
                    break;
                case "VIU": myRichTextBox = richTextBoxVIU;
                    break;
                case "VC1": myRichTextBox = richTextBoxVC1;
                    break;
                case "VC2": myRichTextBox = richTextBoxVC2;
                    break;
                case "VI2": myRichTextBox = richTextBoxVI2;
                    break;
                case "VE2": myRichTextBox = richTextBoxVE2;
                    break;
                default: myRichTextBox = richTextBoxSUP;
                    break;
            }

            Invoke(new Action(() =>
            {
                myRichTextBox.SelectionColor = msgColor;
                myRichTextBox.AppendText(timeStamp + " " + Msg + "\n");
                myRichTextBox.SelectionStart = richTextBoxPUP.Text.Length;
                myRichTextBox.ScrollToCaret();
            }
            ));
        }

        #region bgWorkerCB
        /// <summary>
        /// Thread pour l'interface CB
        /// </summary>

        private void bgWorkerCB_Process(object sender, DoWorkEventArgs e)
        {
            bool canIContinue = true;
            BackgroundWorker bgWorker = (BackgroundWorker)sender;
            CBCOMServer.CBCOMServer myCB = null;
            CBCOMProject myCBProject = null;
            CBCOMApplication myCBApplication = null;
            CBCOMFunctionalTask myCBFunctionalTask = null;
            CBCOMEmbeddedConfiguration myCBConf = null;
            CBCOMInstance myCBInstance = null;
            CBCOMTarget myCBTargetPUP = null, myCBTargetVE1 = null, myCBTargetVIU = null, myCBTargetVC1 = null, myCBTargetVC2 = null, myCBTargetVI2 = null, myCBTargetVE2 = null;
            CBCOMComponent myCBComponent = null;
            CBCOMInstanciatedScenario myCBScenario = null;

            Update_RichTextBox(myColorOrange, "SUP", "## ControlBuild: Lancement de l'application ControlBuild (Monitor)...");
            Process.Start(@"C:\Pack\ControlBuild\tooldir\tool.c\bin\msw32\ControlBuild.exe", "-monitor");
            Thread.Sleep(15000);
            MessageBox.Show("Valider une fois qu'une instance visible de ControlBuild Monitor est lancée.", "Laborame Supervision QDG", MessageBoxButtons.OK);

            myCB = new CBCOMServer.CBCOMServer();
            if (myCB != null)
            {
                //MessageBox.Show(":)");
                bgWorker.ReportProgress(1);
                myCBProject = myCB.OpenProject("CP_Laborame.cbp", @"C:\REGIOLIS\CP_Laborame3");
            }
            else { if (canIContinue) bgWorker.ReportProgress(-1); canIContinue = false; }

            if (myCBProject != null && canIContinue)
            {
                bgWorker.ReportProgress(2);
                myCBApplication = myCBProject.OpenApplication("cp_laborame");
            }
            else { if (canIContinue) bgWorker.ReportProgress(-2); canIContinue = false; }

            if (myCBApplication != null && canIContinue)
            {
               bgWorker.ReportProgress(3);
               try
               {
                   myCBFunctionalTask = myCBApplication.OpenFunctionalTask("cp_laborame");
               }
               catch (Exception ex)
               {
                   canIContinue = false;
                   Update_RichTextBox(myColorRed, "SUP", "## ControlBuild: Application mal générée. Impossible de continuer.");
               }

            }
            else { if (canIContinue) bgWorker.ReportProgress(-3); canIContinue = false; }

            if (myCBFunctionalTask != null && canIContinue)
            {
                bgWorker.ReportProgress(4);
                try
                {
                    myCBConf = myCBFunctionalTask.OpenEmbeddedConfiguration("conf");
                    myCBInstance = myCBFunctionalTask.RootInstance();
                }
                catch(Exception ex)
                {
                    canIContinue = false;
                    Update_RichTextBox(myColorRed, "SUP", "## ControlBuild: Application mal générée. Impossible de continuer.");
                }
            }
            else { if (canIContinue) bgWorker.ReportProgress(-4); canIContinue = false; }

            if (myCBConf != null && myCBInstance != null && canIContinue)
            {
                bgWorker.ReportProgress(5);
                myCBTargetPUP = myCBConf.TargetOfName("tTB_PUP_sil0");
                myCBTargetVE1 = myCBConf.TargetOfName("tTB_VE1_sil0");
                myCBTargetVIU = myCBConf.TargetOfName("tTB_VIU_sil0");
                myCBTargetVC1 = myCBConf.TargetOfName("tTB_VC1_sil0");
                myCBTargetVC2 = myCBConf.TargetOfName("tTB_VC2_sil0");
                myCBTargetVI2 = myCBConf.TargetOfName("tTB_VI2_sil0");
                myCBTargetVE2 = myCBConf.TargetOfName("tTB_VE2_sil0");
            }
            else { if (canIContinue) bgWorker.ReportProgress(-5); canIContinue = false; }

            if (myCBTargetPUP != null && myCBTargetVE1 != null && myCBTargetVIU != null && myCBTargetVC1 != null && myCBTargetVC2 != null && myCBTargetVI2 != null && myCBTargetVE2 != null && canIContinue)
            {
                bgWorker.ReportProgress(6);
                myCBTargetPUP.Connect();
                myCBTargetVE1.Connect();
                myCBTargetVIU.Connect();
                myCBTargetVC1.Connect();
                myCBTargetVC2.Connect();
                myCBTargetVI2.Connect();
                myCBTargetVE2.Connect();
                bgWorker.ReportProgress(7);
                myCBComponent = myCBApplication.ComponentOfName("cp_laborame");
                myCBScenario = myCBInstance.ScenarioOfName("a01_bancs_connect");
                myCBScenario.Run(true);
                Thread.Sleep(4000);
                myCBScenario = myCBInstance.ScenarioOfName("a01_config_train");
                myCBScenario.Run(true);
                Thread.Sleep(4000); 
                myCBScenario = myCBInstance.ScenarioOfName("a01_modele_state");
                myCBScenario.Run(true);
                Thread.Sleep(4000);
                myCBScenario = myCBInstance.ScenarioOfName("a01_fermeture_cc");
                myCBScenario.Run(true);
                Thread.Sleep(4000);
                myCBScenario = myCBInstance.ScenarioOfName("acu");
                myCBScenario.Run(true);
                Thread.Sleep(4000);
                myCBScenario = myCBInstance.ScenarioOfName("contournement_85");
                myCBScenario.Run(true);
                Thread.Sleep(4000);
                bgWorker.ReportProgress(8);
            }
            else { if (canIContinue) bgWorker.ReportProgress(-6); canIContinue = false; }

            Invoke(new Action(() => { btnSUPReInit.Enabled = true; }));

            while (canIContinue)
            {
                if (bgWorkerCB_Action >= 99) canIContinue = false;
                Thread.Sleep(100);
            }
         }

        private void bgWorkerCB_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                    break;
                case -1:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, "SUP", "## ControlBuild: Erreur à l'initialisation de l'objet CBCOMServer. Problème de dépendance ou incohérence avec la version CB installée");
                    break;
                case -2:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, "PUP", "## ControlBuild: Erreur à l'ouverture du projet ControlBuild.");
                    break;
                case 3:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorGreen; }));
                    break;
                case -3:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, "SUP", "## ControlBuild: Erreur à l'ouverture de l'application.");
                    break;
                case 4:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorOrange; }));
                    break;
                case -4:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, "SUP", "## ControlBuild: Erreur à l'ouverture de la vue Fonctionnel.");
                    break;
                case 5:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { labelSUPCB3.ForeColor = myColorOrange; }));
                    Invoke(new Action(() => { labelSUPCB4.ForeColor = myColorOrange; }));
                    break;
                case -5:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, "SUP", "## ControlBuild: Erreur à l'ouverture de la conf.");
                    break;
                case -6:
                    Invoke(new Action(() => { labelSUPCB3.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, "SUP", "## ControlBuild: Erreur sur l'un des objets Target (tTB_PUP_sil0, tTB_VE1_sil0, tTB_VIU_sil0, tTB_VC1_sil0, tTB_VC2_sil0, tTB_VI2_sil0, tTB_VE2_sil0).");
                    break;
                case 7:
                    Invoke(new Action(() => { labelSUPCB3.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { labelSUPCB4.ForeColor = myColorOrange; }));
                    break;
                case 8:
                    Invoke(new Action(() => { labelSUPCB4.ForeColor = myColorGreen; }));
                    break;
                default:
                    break;
            }
        }

        void bgWorkerCB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Update_RichTextBox(myColorRed, "SUP", "## Fin du Worker.");
            if (e != null && e.Cancelled)
            {
                startTheWorker(bgWorkerCB);
                return;
            }
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

            TCPPUP = new ClassTCP("127.0.0.1", "PUP", bgWorker, this);
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
                        Invoke(new Action(() => { labelPUPConn.ForeColor = myColorOrange; }));
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
                    Invoke(new Action(() => { labelPUPConn.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { labelPUPHard.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { labelPUPEacb.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { labelgroupBoxPUP.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { buttonPUPReconn.Enabled = false; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = false; }));
                    Invoke(new Action(() => { buttonPUPReinit.Enabled = false; }));
                    break;
                case -1:
                    Invoke(new Action(() => { labelPUPConn.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, "PUP", "## Probleme de connexion. Vérifier que TB_RP_Server est démarré sur le PC distant.");
                    Update_RichTextBox(myColorRed, "PUP", "## Utiliser le bouton 'Reconnexion' pour retenter, puis 'Reload XML'.");
                    Invoke(new Action(() => { buttonPUPReconn.Enabled = true; }));
                    break;
                case 2 :
                    Invoke(new Action(() => { buttonPUPReconn.Enabled = true; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = true; }));
                    break;
                case 3:
                    Invoke(new Action(() => { labelPUPHard.ForeColor = myColorGreen; }));
                    break;
                case -3:
                    Invoke(new Action(() => { labelPUPHard.ForeColor = myColorRed; }));
                    break;
                case 4:
                    Invoke(new Action(() => { labelPUPEacb.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = myColorOrange; }));
                    break;
                case -4:
                    Invoke(new Action(() => { labelPUPEacb.ForeColor = myColorRed; }));
                    break;
                case 5:
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = true; }));
                    Update_RichTextBox(myColorGreen, "PUP", "== Configuration terminée.");
                    Invoke(new Action(() => { labelgroupBoxPUP.ForeColor = myColorGreen; }));
                    break;
                case -5:
                    Invoke(new Action(() => { labelPUPStartEqt.ForeColor = myColorRed; }));
                    Invoke(new Action(() => { buttonPUPReload.Enabled = true; }));
                    break;
                default:
                    break;
            }
            if (e.ProgressPercentage < 0)
            {
                if (labelgroupBoxPUP.ForeColor != myColorRed) labelgroupBoxPUP.ForeColor = myColorRed;
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
            Update_RichTextBox(myColorOrange, "PUP", "##  Fin du Worker TCPPUP ##");
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
        }

        private void btnSUPReInit_Click(object sender, EventArgs e)
        {
            Invoke(new Action(() => { btnSUPReInit.Enabled = false; })); 
            bgWorkerCB_Action = 99;
            bgWorkerCB.CancelAsync();
        }

        private void startTheWorker(BackgroundWorker worker)
        {
            if (worker == null)
                throw new Exception("How come this is null?");
            worker.RunWorkerAsync();
        }

        private void buttonMajValTS_Click(object sender, EventArgs e)
        {
            
            LbrmPP_SupervisionMultiTB.Properties.Settings.Default["PathCB"] = textBox_PathCB.Text.ToString();
            LbrmPP_SupervisionMultiTB.Properties.Settings.Default["CBProjectName"] = textBox_CBProjectName.Text.ToString();
            LbrmPP_SupervisionMultiTB.Properties.Settings.Default["CBApplicationName"] = textBox_CBApplicationName.Text.ToString();
            LbrmPP_SupervisionMultiTB.Properties.Settings.Default["CBTaskName"] = textBox_CBTaskName.Text.ToString();
            LbrmPP_SupervisionMultiTB.Properties.Settings.Default["PathXML"] = textBox_PathXML.Text.ToString();
            LbrmPP_SupervisionMultiTB.Properties.Settings.Default.Save();
        }
    }
}
