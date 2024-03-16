using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using ZedGraph;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;

namespace BTL_DL
{
    public partial class Form1 : Form
    {
        string Alldata;
        string tempe,humid;
        string tempe_flash,humid_flash;
        string data;
        int len;
        int time;
        double set;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            
            txt_receivetemp.ScrollBars = ScrollBars.Vertical;
           
            txt_receivehumid.ScrollBars = ScrollBars.Vertical;
            txt_tempeflash.ScrollBars = ScrollBars.Vertical;

            txt_humidflash.ScrollBars = ScrollBars.Vertical;
            string[] ports = SerialPort.GetPortNames();
            cBoxCom.Items.AddRange(ports);

            GraphPane MyPanne = zedGraphControl1.GraphPane;
            MyPanne.Title.Text = "Dữ liệu cảm biến";
            MyPanne.YAxis.Title.Text = "Giá trị cảm biến";
            MyPanne.XAxis.Title.Text = "Thời gian";

            RollingPointPairList list1 = new RollingPointPairList(500000);
            RollingPointPairList list2 = new RollingPointPairList(500000);

            LineItem line1 = MyPanne.AddCurve("Temperature", list1, Color.Red, SymbolType.None);
            LineItem line2 = MyPanne.AddCurve("Humidity", list2, Color.Blue, SymbolType.None);

            MyPanne.XAxis.Scale.Min = 0;
            MyPanne.XAxis.Scale.Max = 100;
            MyPanne.XAxis.Scale.MinorStep = 1;
            MyPanne.XAxis.Scale.MajorStep = 5;

            MyPanne.YAxis.Scale.Min = 0;
            MyPanne.YAxis.Scale.Max = 100;
            MyPanne.YAxis.Scale.MinorStep = 2;
            MyPanne.YAxis.Scale.MajorStep = 10;

            zedGraphControl1.AxisChange();

        }
        double tong1 = 0;
        double tong2 = 0;
        public void draw1(double line1)
        {
            LineItem duongline1 = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            if (duongline1 == null )
            {
                return;
            }
            IPointListEdit list1 = duongline1.Points as IPointListEdit;
            if (list1 == null )
            {
                return;
            }
            list1.Add(tong2, line1);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            tong1 += 0.5;

        }
        public void draw2(double line2)
        {
            LineItem duongline2 = zedGraphControl1.GraphPane.CurveList[1] as LineItem;
            if ( duongline2 == null)
            {
                return;
            }
            IPointListEdit list2 = duongline2.Points as IPointListEdit;
            if ( list2 == null)
            {
                return;
            }
            list2.Add(tong2, line2);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            tong2 += 0.5;

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Start();
                serialPort1.PortName = cBoxCom.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort1.Open();
            }
            catch (Exception err) 
            { 
                MessageBox.Show(err.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }    
        }
        int count = 0;
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen) 
            {
             Alldata = "";
          
             Alldata = serialPort1.ReadLine();
                //Data receive if current data, frame data is xx/xx/r/n(6), if flash data, frame data is 1/xx/xx/r/n(8)
             len = Alldata.Length;
            if(len == 6)
             {
                    string[] data = Alldata.Split('/');
                    DateTime currentTime = DateTime.Now;
                    tempe = data[0];
                    humid = data[1];

                    Invoke(new MethodInvoker(() => draw1(Convert.ToDouble(tempe))));
                    Invoke(new MethodInvoker(() => draw2(Convert.ToDouble(humid))));
                    string tempe_1 = currentTime.ToString("HH:mm:ss:  ") + tempe + '\u2103' + "\r\n";
                    string humid_1 = currentTime.ToString("HH:mm:ss:  ") + humid + "%RH" + "\r\n";
                    txt_receivetemp.Invoke(new Action(() => txt_receivetemp.AppendText(tempe_1)));
                    txt_receivehumid.Invoke(new Action(() => txt_receivehumid.AppendText(humid_1)));

             }
            else if(len == 8)
            {
                    string[] data = Alldata.Split('/');

                    tempe_flash = data[1];
                    humid_flash = data[2];

                    string tempeflash_1 = tempe_flash + '\u2103' + "\r\n";
                    string humidflash_1 = humid_flash + "%RH"+ "\r\n";
                    txt_tempeflash.Invoke(new Action(() => txt_tempeflash.AppendText(tempeflash_1)));
                    txt_humidflash.Invoke(new Action(() => txt_humidflash.AppendText(humidflash_1)));
                }
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
           
           //var set = Convert.ToDouble(set_point);
           //Invoke(new MethodInvoker(() => draw1(set)));

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void cBoxCom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            
            string data_send = "1";
            serialPort1.Write(data_send);
          
        }


    }
}
