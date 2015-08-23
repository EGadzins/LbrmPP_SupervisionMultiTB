using System;
using System.Threading;
using System.Net.Sockets;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LbrmPP_SupervisionMultiTB
{
    public class ClassTCP
    {
        protected NetworkStream _stream;
        protected TcpClient _connection;
        protected Form1.TestBench _testBench;
        protected ClassTCP connection;
        protected Exception exception;
        protected Form1 _frm = null;
        private int _Status = 0;
        private int _Action = 0;

//        public ClassTCP(string IPAddress, string strPC, BackgroundWorker bgWorker, Form1 f)
        public ClassTCP(Form1.TestBench testBench, Form1 f)
        {
            _testBench = testBench;
            _frm = f;

            Open();
            Action = 0; 
        }

        public int Status
        {
            //Status :
            //2 = End of Init
            //3 = End of HARD.xml
            //4 = End of EACB.xml
            //5 = End of StartEqt

            get
            {
                return _Status;
            }
            set
            {
                if ((value >= -100) && (value <= 100))
                {
                    _Status = value;
                    _testBench.TCP = this;
                    if (_testBench.Worker != null)
                    {
                        _testBench.Worker.ReportProgress(_Status, _testBench);
                    }
                    Thread.Sleep(100);
                }
            }
        }

        public int Action
        {
            //Action :
            //1 = ReConnexion
            //2 = Reload XML
            //3 = 
            //99 = Stop

            get
            {
                return _Action;
            }
            set
            {
                if ((value >= 0) && (value <= 100))
                {
                    _Action = value;
                }
            }
        }

        public void InitRP()
        {
            string strRcv = null;
            strRcv = Send("K?");
            strRcv = Send("KS");
            Status = (strRcv.StartsWith("OK")) ? 2 : -2;
        }

        public void Reconnect()
        {
            string strRcv = null;
            if (_Status > 0)
            {
                strRcv = Send("KZ"); // Reset Conf
                strRcv = Send("Ks"); // Stop
                Close();
            }
            Open();   // Status = (-)1;
            if (_Status > 0)
            {
                InitRP(); // Status = (-)2;
            } 
        }

        
        public void LoadXMLandStart()
        {
            string strRcv = null;
            
            strRcv = Send("KZ"); // Reset Conf

            //= HARD.xml
//            strRcv = Send("KRC:/Documents and Settings/Administrator/Desktop/TB_RP_Docs/HARD_US2.xml");
            strRcv = Send("KRC:/Documents and Settings/Administrator/Desktop/TB_RP_Docs/HARD_UM.xml");
            if (strRcv.StartsWith("OK"))
            {
                Status = 3;
                //= EACB.xml
//                strRcv = Send("KRC:/Documents and Settings/Administrator/Desktop/TB_RP_Docs/EACB_US2.xml");
                strRcv = Send("KRC:/Documents and Settings/Administrator/Desktop/TB_RP_Docs/EACB_UM.xml");
                if (strRcv.StartsWith("OK"))
                {
                    Status = 4;

                    //= StartEqt
//                    strRcv = Send("KACT2_cip");
                    strRcv = Send("KACT1_cip");
                    Status = (strRcv.StartsWith("OK")) ? 5 : -5;
                }
                else Status = -4;
            }
            else Status = -3;
        }

        
        
        public string Send(string strToSend)
        {
            byte[] myWriteBuffer = new byte[0];
            byte[] myReadBuffer = new byte[2048];
            int numberOfBytesRead = 0;
            string strRcv = null;

            myWriteBuffer = Encoding.ASCII.GetBytes(strToSend);

            _frm.Update_RichTextBox(Color.LightSeaGreen, _testBench.richtextbox, ">>" + strToSend);
            _stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
            do
            {
                numberOfBytesRead = _stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                strRcv = Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead);
                _frm.Update_RichTextBox(Color.DarkSlateBlue, _testBench.richtextbox, "<<" + strRcv);
            } while (String.Compare(strRcv, "HB") == 0);

            return strRcv;
        }

        private void Open()
        {
            _connection = new TcpClientWithTimeout(_testBench.IP, 5555, 5000).Connect();  // 5s de timetout
            if (_connection != null)
            {
                _stream = _connection.GetStream();
                Status = 1;
            }
            else Status = -1;
        }

        public void Close()
        {
            _stream.Close(); // workaround for a .net bug: http://support.microsoft.com/kb/821625
            _connection.Close();
            Status = 0;
        }

    
    }
}
