using System;
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
        //private BackgroundWorker bgWorkerCB;
        readonly Color myColorDefault = System.Drawing.Color.Black;
        readonly Color myColorRed = System.Drawing.Color.Red;
        readonly Color myColorGreen = System.Drawing.Color.Green;
        readonly Color myColorOrange = System.Drawing.Color.Orange;
        private int bgWorkerCB_Action = 0;

        public struct TestBench
        {
            public BackgroundWorker Worker;
            public ClassTCP TCP;
            public string IP;
            public string ID;
            public Label labelConn;
            public Label labelHard;
            public Label labelEacb;
            public Label labelStartEqt;
            public Label labelgroupBox;
            public Button buttonReconn;
            public Button buttonReload;
            public Button buttonReinit;
            public RichTextBox richtextbox;
        }
        private TestBench myTB_PUP, myTB_VE1, myTB_VIU, myTB_VC1, myTB_VC2, myTB_VI2, myTB_VE2, myTB_VUE3D, myTB_BISTD, myTB_SUP;
       
        public Form1()
        {
            InitializeComponent();

            myTB_SUP.Worker = new BackgroundWorker();
            myTB_SUP.Worker.WorkerReportsProgress = true;
            myTB_SUP.Worker.WorkerSupportsCancellation = true;
            myTB_SUP.Worker.DoWork += bgWorkerCB_Process;
            myTB_SUP.Worker.ProgressChanged += bgWorkerCB_ProgressChanged;
            myTB_SUP.Worker.RunWorkerCompleted += bgWorkerCB_RunWorkerCompleted;

            myTB_PUP.Worker = new BackgroundWorker();
            myTB_PUP.Worker.WorkerReportsProgress = true;
            myTB_PUP.Worker.DoWork += bgWorkerTCP_Process;
            myTB_PUP.Worker.ProgressChanged += bgWorkerTCP_ProgressChanged;
            myTB_PUP.Worker.RunWorkerCompleted += bgWorkerTCP_RunWorkerCompleted;
            myTB_PUP.buttonReconn = buttonPUPReconn;
            myTB_PUP.buttonReinit = buttonPUPReinit;
            myTB_PUP.buttonReload = buttonPUPReload;
            myTB_PUP.labelConn = labelPUPConn;
            myTB_PUP.labelEacb = labelPUPEacb;
            myTB_PUP.labelgroupBox = labelgroupBoxPUP;
            myTB_PUP.labelHard = labelPUPHard;
            myTB_PUP.labelStartEqt = labelPUPStartEqt;
            myTB_PUP.richtextbox = richTextBoxPUP;

            myTB_VE1.Worker = new BackgroundWorker();
            myTB_VE1.Worker.WorkerReportsProgress = true;
            myTB_VE1.Worker.DoWork += bgWorkerTCP_Process;
            myTB_VE1.Worker.ProgressChanged += bgWorkerTCP_ProgressChanged;
            myTB_VE1.Worker.RunWorkerCompleted += bgWorkerTCP_RunWorkerCompleted;
            myTB_VE1.buttonReconn = buttonVE1Reconn;
            myTB_VE1.buttonReinit = buttonVE1Reinit;
            myTB_VE1.buttonReload = buttonVE1Reload;
            myTB_VE1.labelConn = labelVE1Conn;
            myTB_VE1.labelEacb = labelVE1Eacb;
            myTB_VE1.labelgroupBox = labelgroupBoxVE1;
            myTB_VE1.labelHard = labelVE1Hard;
            myTB_VE1.labelStartEqt = labelVE1StartEqt;
            myTB_VE1.richtextbox = richTextBoxVE1;
            
            myTB_VIU.Worker = new BackgroundWorker();
            myTB_VIU.Worker.WorkerReportsProgress = true;
            myTB_VIU.Worker.DoWork += bgWorkerTCP_Process;
            myTB_VIU.Worker.ProgressChanged += bgWorkerTCP_ProgressChanged;
            myTB_VIU.Worker.RunWorkerCompleted += bgWorkerTCP_RunWorkerCompleted;
            myTB_VIU.buttonReconn = buttonVIUReconn;
            myTB_VIU.buttonReinit = buttonVIUReinit;
            myTB_VIU.buttonReload = buttonVIUReload;
            myTB_VIU.labelConn = labelVIUConn;
            myTB_VIU.labelEacb = labelVIUEacb;
            myTB_VIU.labelgroupBox = labelgroupBoxVIU;
            myTB_VIU.labelHard = labelVIUHard;
            myTB_VIU.labelStartEqt = labelVIUStartEqt;
            myTB_VIU.richtextbox = richTextBoxVIU;
            
            myTB_VC1.Worker = new BackgroundWorker();
            myTB_VC1.Worker.WorkerReportsProgress = true;
            myTB_VC1.Worker.DoWork += bgWorkerTCP_Process;
            myTB_VC1.Worker.ProgressChanged += bgWorkerTCP_ProgressChanged;
            myTB_VC1.Worker.RunWorkerCompleted += bgWorkerTCP_RunWorkerCompleted;
            myTB_VC1.buttonReconn = buttonVC1Reconn;
            myTB_VC1.buttonReinit = buttonVC1Reinit;
            myTB_VC1.buttonReload = buttonVC1Reload;
            myTB_VC1.labelConn = labelVC1Conn;
            myTB_VC1.labelEacb = labelVC1Eacb;
            myTB_VC1.labelgroupBox = labelgroupBoxVC1;
            myTB_VC1.labelHard = labelVC1Hard;
            myTB_VC1.labelStartEqt = labelVC1StartEqt;
            myTB_VC1.richtextbox = richTextBoxVC1;

            myTB_VC2.Worker = new BackgroundWorker();
            myTB_VC2.Worker.WorkerReportsProgress = true;
            myTB_VC2.Worker.DoWork += bgWorkerTCP_Process;
            myTB_VC2.Worker.ProgressChanged += bgWorkerTCP_ProgressChanged;
            myTB_VC2.Worker.RunWorkerCompleted += bgWorkerTCP_RunWorkerCompleted;
            myTB_VC2.buttonReconn = buttonVC2Reconn;
            myTB_VC2.buttonReinit = buttonVC2Reinit;
            myTB_VC2.buttonReload = buttonVC2Reload;
            myTB_VC2.labelConn = labelVC2Conn;
            myTB_VC2.labelEacb = labelVC2Eacb;
            myTB_VC2.labelgroupBox = labelgroupBoxVC2;
            myTB_VC2.labelHard = labelVC2Hard;
            myTB_VC2.labelStartEqt = labelVC2StartEqt;
            myTB_VC2.richtextbox = richTextBoxVC2;

            myTB_VI2.Worker = new BackgroundWorker();
            myTB_VI2.Worker.WorkerReportsProgress = true;
            myTB_VI2.Worker.DoWork += bgWorkerTCP_Process;
            myTB_VI2.Worker.ProgressChanged += bgWorkerTCP_ProgressChanged;
            myTB_VI2.Worker.RunWorkerCompleted += bgWorkerTCP_RunWorkerCompleted;
            myTB_VI2.buttonReconn = buttonVI2Reconn;
            myTB_VI2.buttonReinit = buttonVI2Reinit;
            myTB_VI2.buttonReload = buttonVI2Reload;
            myTB_VI2.labelConn = labelVI2Conn;
            myTB_VI2.labelEacb = labelVI2Eacb;
            myTB_VI2.labelgroupBox = labelgroupBoxVI2;
            myTB_VI2.labelHard = labelVI2Hard;
            myTB_VI2.labelStartEqt = labelVI2StartEqt;
            myTB_VI2.richtextbox = richTextBoxVI2;

            myTB_VE2.Worker = new BackgroundWorker();
            myTB_VE2.Worker.WorkerReportsProgress = true;
            myTB_VE2.Worker.DoWork += bgWorkerTCP_Process;
            myTB_VE2.Worker.ProgressChanged += bgWorkerTCP_ProgressChanged;
            myTB_VE2.Worker.RunWorkerCompleted += bgWorkerTCP_RunWorkerCompleted;
            myTB_VE2.buttonReconn = buttonVE2Reconn;
            myTB_VE2.buttonReinit = buttonVE2Reinit;
            myTB_VE2.buttonReload = buttonVE2Reload;
            myTB_VE2.labelConn = labelVE2Conn;
            myTB_VE2.labelEacb = labelVE2Eacb;
            myTB_VE2.labelgroupBox = labelgroupBoxVE2;
            myTB_VE2.labelHard = labelVE2Hard;
            myTB_VE2.labelStartEqt = labelVE2StartEqt;
            myTB_VE2.richtextbox = richTextBoxVE2;

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
            myTB_PUP.ID = "PUP";
            myTB_VE1.ID = "VE1";
            myTB_VIU.ID = "VIU";
            myTB_VC1.ID = "VC1";
            myTB_VC2.ID = "VC2";
            myTB_VI2.ID = "VI2";
            myTB_VE2.ID = "VE2";
            myTB_VUE3D.ID = "VUE3D";
            myTB_BISTD.ID = "BISTD";
        }

        private void btnStartSup_Click(object sender, EventArgs e)
        {
            btnStartSup.Enabled = false;
            tabControl1.SelectedTab = tabSupTB;

            if (checkBoxCB.Checked) myTB_SUP.Worker.RunWorkerAsync();
            else Update_RichTextBox(Color.Blue, richTextBoxSUP, "Partie ControlBuild désactivé (cf onglet \"Configuration\")");

            myTB_PUP.Worker.RunWorkerAsync(myTB_PUP);
            myTB_VE1.Worker.RunWorkerAsync(myTB_VE1);
            myTB_VIU.Worker.RunWorkerAsync(myTB_VIU);
            myTB_VC1.Worker.RunWorkerAsync(myTB_VC1);
            myTB_VC2.Worker.RunWorkerAsync(myTB_VC2);
            myTB_VI2.Worker.RunWorkerAsync(myTB_VI2);
            myTB_VE2.Worker.RunWorkerAsync(myTB_VE2);
        }

        public void Update_RichTextBox(Color msgColor, RichTextBox richtextbox, string Msg)
        {
            CultureInfo culture = new CultureInfo("en-gb");
            string timeStamp = DateTime.Now.ToString("HH:mm:ss", culture);
            richtextbox.SelectionColor = msgColor;
            richtextbox.AppendText(timeStamp + " " + Msg + "\n");
            richtextbox.SelectionStart = richTextBoxPUP.Text.Length;
            richtextbox.ScrollToCaret();
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

            Update_RichTextBox(myColorOrange, richTextBoxSUP, "## ControlBuild: Lancement de l'application ControlBuild (Monitor)...");
            Process.Start(@"C:\Pack\ControlBuild\tooldir\tool.c\bin\msw32\ControlBuild.exe", "-monitor");
            Thread.Sleep(15000);
            MessageBox.Show("Valider une fois qu'une instance visible de ControlBuild Monitor est lancée.", "Laborame Supervision QDG", MessageBoxButtons.OK);

            myCB = new CBCOMServer.CBCOMServer();
            if (myCB != null)
            {
                //MessageBox.Show(":)");
                bgWorker.ReportProgress(1);
                myCBProject = myCB.OpenProject(textBox_CBProjectName.Text, textBox_PathCB.Text);
            }
            else { if (canIContinue) bgWorker.ReportProgress(-1); canIContinue = false; }

            if (myCBProject != null && canIContinue)
            {
                bgWorker.ReportProgress(2);
                myCBApplication = myCBProject.OpenApplication(textBox_CBApplicationName.Text);
            }
            else { if (canIContinue) bgWorker.ReportProgress(-2); canIContinue = false; }

            if (myCBApplication != null && canIContinue)
            {
               bgWorker.ReportProgress(3);
               try
               {
                   myCBFunctionalTask = myCBApplication.OpenFunctionalTask(textBox_CBTaskName.Text);
               }
               catch (Exception)
               {
                   canIContinue = false;
                   Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Application mal générée. Impossible de continuer.");
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
                catch(Exception)
                {
                    canIContinue = false;
                    Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Application mal générée. Impossible de continuer.");
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
            string[] results = (string[])e.UserState;
            
            switch (e.ProgressPercentage)
            {
                case 0:
                    break;
                case -1:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Erreur à l'initialisation de l'objet CBCOMServer. Problème de dépendance ou incohérence avec la version CB installée");
                    break;
                case -2:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Erreur à l'ouverture du projet ControlBuild.");
                    break;
                case 3:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorGreen; }));
                    break;
                case -3:
                    Invoke(new Action(() => { labelSUPCB1.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Erreur à l'ouverture de l'application.");
                    break;
                case 4:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorOrange; }));
                    break;
                case -4:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Erreur à l'ouverture de la vue Fonctionnel.");
                    break;
                case 5:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { labelSUPCB3.ForeColor = myColorOrange; }));
                    Invoke(new Action(() => { labelSUPCB4.ForeColor = myColorOrange; }));
                    break;
                case -5:
                    Invoke(new Action(() => { labelSUPCB2.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Erreur à l'ouverture de la conf.");
                    break;
                case -6:
                    Invoke(new Action(() => { labelSUPCB3.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, richTextBoxSUP, "## ControlBuild: Erreur sur l'un des objets Target (tTB_PUP_sil0, tTB_VE1_sil0, tTB_VIU_sil0, tTB_VC1_sil0, tTB_VC2_sil0, tTB_VI2_sil0, tTB_VE2_sil0).");
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
            Update_RichTextBox(myColorRed, richTextBoxSUP, "## Fin du Worker.");
            if (e != null && e.Cancelled)
            {
                startTheWorker(myTB_SUP.Worker);
                return;
            }
        }

        #endregion

        #region bgWorkerTCP
        /// <summary>
        /// Thread pour l'interface TCP
        /// </summary>

        private void bgWorkerTCP_Process(object sender, DoWorkEventArgs e)
        {
            TestBench testBench = (TestBench)e.Argument;
            //BackgroundWorker bgWorker = (BackgroundWorker)sender;
            testBench.TCP = new ClassTCP(testBench, this);
            if (testBench.TCP.Status > 0) 
            {
                testBench.TCP.InitRP();
                testBench.TCP.LoadXMLandStart();
            }

            while (testBench.TCP.Action < 100)
            {
                //1 = ReConnexion; 2 = Reload XML; 90 = Stop; 100 = Exit the loop
                switch (testBench.TCP.Action)
                {
                    case 1: //1 = ReConnexion
                        Invoke(new Action(() => { testBench.buttonReconn.Enabled = false; }));
                        Invoke(new Action(() => { testBench.buttonReload.Enabled = false; }));
                        Invoke(new Action(() => { testBench.labelConn.ForeColor = myColorOrange; }));
                        testBench.TCP.Reconnect();
                        testBench.TCP.Action = 0;
                        break;
                    case 2: //2 = Reload XML
                        testBench.TCP.LoadXMLandStart();
                        testBench.TCP.Action = 0;
                        break;
                    default:
                        testBench.TCP.Action = 0;
                        break;
                }
                Thread.Sleep(10);
            }

            if (testBench.TCP.Status > 0)
            {
                testBench.TCP.Close();
            }
        }

        private void bgWorkerTCP_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Status : 2 = End of Init; 3 = End of HARD.xml; 4 = End of EACB.xml; 5 = End of StartEqt
            TestBench testBench = (TestBench)e.UserState;

            if (testBench.ID == "PUP") Invoke(new Action(() => { myTB_PUP = testBench; }));
            if (testBench.ID == "VE1") Invoke(new Action(() => { myTB_VE1 = testBench; }));
            if (testBench.ID == "VIU") Invoke(new Action(() => { myTB_VIU = testBench; }));
            if (testBench.ID == "VC1") Invoke(new Action(() => { myTB_VC1 = testBench; }));
            if (testBench.ID == "VC2") Invoke(new Action(() => { myTB_VC2 = testBench; }));
            if (testBench.ID == "VI2") Invoke(new Action(() => { myTB_VI2 = testBench; }));
            if (testBench.ID == "VE2") Invoke(new Action(() => { myTB_VE2 = testBench; }));
                    

            switch (e.ProgressPercentage)
            {
                case 0:
                    break;
                case 1:
                    Invoke(new Action(() => { testBench.labelConn.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { testBench.labelHard.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { testBench.labelEacb.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { testBench.labelStartEqt.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { testBench.labelgroupBox.ForeColor = myColorDefault; }));
                    Invoke(new Action(() => { testBench.buttonReconn.Enabled = false; }));
                    Invoke(new Action(() => { testBench.buttonReload.Enabled = false; }));
                    Invoke(new Action(() => { testBench.buttonReinit.Enabled = false; }));
                    break;
                case -1:
                    Invoke(new Action(() => { testBench.labelConn.ForeColor = myColorRed; }));
                    Update_RichTextBox(myColorRed, testBench.richtextbox, "## Probleme de connexion. Vérifier que TB_RP_Server est démarré sur le PC distant (" + testBench.IP + "). Utiliser le bouton 'Reconnexion' pour retenter, puis 'Reload XML'.");
                    Invoke(new Action(() => { testBench.buttonReconn.Enabled = true; }));
                    break;
                case 2 :
                    Invoke(new Action(() => { testBench.buttonReconn.Enabled = true; }));
                    Invoke(new Action(() => { testBench.buttonReload.Enabled = true; }));
                    break;
                case 3:
                    Invoke(new Action(() => { testBench.labelHard.ForeColor = myColorGreen; }));
                    break;
                case -3:
                    Invoke(new Action(() => { testBench.labelHard.ForeColor = myColorRed; }));
                    break;
                case 4:
                    Invoke(new Action(() => { testBench.labelEacb.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { testBench.labelStartEqt.ForeColor = myColorOrange; }));
                    break;
                case -4:
                    Invoke(new Action(() => { testBench.labelEacb.ForeColor = myColorRed; }));
                    break;
                case 5:
                    Invoke(new Action(() => { testBench.labelStartEqt.ForeColor = myColorGreen; }));
                    Invoke(new Action(() => { testBench.buttonReload.Enabled = true; }));
                    Update_RichTextBox(myColorGreen, testBench.richtextbox, "== Configuration terminée.");
                    Invoke(new Action(() => { testBench.labelgroupBox.ForeColor = myColorGreen; }));
                    break;
                case -5:
                    Invoke(new Action(() => { testBench.labelStartEqt.ForeColor = myColorRed; }));
                    Invoke(new Action(() => { testBench.buttonReload.Enabled = true; }));
                    break;
                default:
                    break;
            }
            if (e.ProgressPercentage < 0)
            {
                if (testBench.labelgroupBox.ForeColor != myColorRed) testBench.labelgroupBox.ForeColor = myColorRed;
            }
        
        }

        void bgWorkerTCP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                MessageBox.Show("bgWorkerTCP Canceled");
            }
            else
            {
                // Finally, handle the case where the operation  
                // succeeded.
                MessageBox.Show(e.Result.ToString());;
            }
            //Update_RichTextBox(myColorOrange, "PUP", "##  Fin du Worker TCPPUP ##");
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnSUPReInit_Click(object sender, EventArgs e)
        {
            Invoke(new Action(() => { btnSUPReInit.Enabled = false; })); 
            bgWorkerCB_Action = 99;
            myTB_SUP.Worker.CancelAsync();
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

        private void buttonPUPReconn_Click(object sender, EventArgs e)
        {
            myTB_PUP.TCP.Action = 1;
        }

        private void buttonPUPReload_Click(object sender, EventArgs e)
        {
            myTB_PUP.TCP.Action = 2;
        }
        
        private void buttonVE1Reconn_Click(object sender, EventArgs e)
        {
            myTB_VE1.TCP.Action = 1;
        }

        private void buttonVE1Reload_Click(object sender, EventArgs e)
        {
            myTB_VE1.TCP.Action = 2;
        }

        private void buttonVIUReconn_Click(object sender, EventArgs e)
        {
            myTB_VIU.TCP.Action = 1;
        }

        private void buttonVIUReload_Click(object sender, EventArgs e)
        {
            myTB_VIU.TCP.Action = 2;
        }

        private void buttonVC1Reconn_Click(object sender, EventArgs e)
        {
            myTB_VC1.TCP.Action = 1;
        }

        private void buttonVC1Reload_Click(object sender, EventArgs e)
        {
            myTB_VC1.TCP.Action = 2;
        }

        private void buttonVC2Reconn_Click(object sender, EventArgs e)
        {
            myTB_VC2.TCP.Action = 1;
        }

        private void buttonVC2Reload_Click(object sender, EventArgs e)
        {
            myTB_VC2.TCP.Action = 2;
        }

        private void buttonVI2Reconn_Click(object sender, EventArgs e)
        {
            myTB_VI2.TCP.Action = 1;
        }

        private void buttonVI2Reload_Click(object sender, EventArgs e)
        {
            myTB_VI2.TCP.Action = 2;
        }

        private void buttonVE2Reconn_Click(object sender, EventArgs e)
        {
            myTB_VE2.TCP.Action = 1;
        }

        private void buttonVE2Reload_Click(object sender, EventArgs e)
        {
            myTB_VE2.TCP.Action = 2;
        }
    }
}
