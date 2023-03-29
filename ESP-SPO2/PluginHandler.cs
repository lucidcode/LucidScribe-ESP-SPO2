using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace lucidcode.LucidScribe.Plugin.ESPSPO2
{
    public static class Device
    {
        static bool Initialized;
        static bool InitError;
        static SerialPort serialPort;
        static double irValue;
        static double bpmValue;
        static double avgBpmValue;

        static double irTicks;
        static double bpmTicks;
        static double avgBpmTicks;

        static bool clearIr;
        static bool clearBpm;
        static bool clearAvgBpm;

        public static string Algorithm = "REM Detection";
        public static int Threshold = 600;
        public static int BlinkInterval = 28;

        public static EventHandler<EventArgs> IrChanged;
        public static EventHandler<EventArgs> BpmChanged;
        public static EventHandler<EventArgs> AvgBpmChanged;

        public static Boolean Initialize()
        {
            if (!Initialized & !InitError)
            {
                PortForm formPort = new PortForm();
                if (formPort.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Algorithm = formPort.Algorithm;
                        BlinkInterval = formPort.BlinkInterval / 10;
                        Threshold = formPort.Threshold;

                        // Open the COM port
                        serialPort = new SerialPort(formPort.SelectedPort);
                        serialPort.BaudRate = 9600;
                        serialPort.Parity = Parity.None;
                        //serialPort.DataBits = 8;
                        //serialPort.StopBits = StopBits.One;
                        //serialPort.Handshake = Handshake.None;
                        //serialPort.ReadTimeout = 500;
                        //serialPort.WriteTimeout = 500;
                        serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                        serialPort.Open();
                    }
                    catch (Exception ex)
                    {
                        if (!InitError)
                        {
                            MessageBox.Show(ex.Message, "LucidScribe.InitializePlugin()", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        InitError = true;
                    }
                }
                else
                {
                    InitError = true;
                    return false;
                }

                Initialized = true;
            }
            return true;
        }

        static void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var data = serialPort.ReadExisting();
                var lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                foreach (var line in lines)
                {
                    var columns = line.Split(new string[] { ", " }, StringSplitOptions.None);

                    foreach (var column in columns)
                    {
                        var valuePair = column.Split('=');
                        if (valuePair.Length > 1)
                        {
                            var value = Convert.ToDouble(valuePair[1]);
                            if (valuePair[0] == "IR")
                            {
                                if (clearIr)
                                {
                                    clearIr = false;
                                    irValue = 0;
                                    irTicks = 0;
                                }
                                irValue += value;
                                irTicks++;

                                if (IrChanged != null)
                                {
                                    IrChanged((object)value, null);
                                }
                            }
                            else if (valuePair[0] == "BPM")
                            {
                                if (clearBpm)
                                {
                                    clearBpm = false;
                                    bpmValue = 0;
                                    bpmTicks = 0;
                                }
                                bpmValue += value;
                                bpmTicks++;

                                if (BpmChanged != null)
                                {
                                    BpmChanged((object)value, null);
                                }
                            }
                            else if (valuePair[0] == "Avg BPM")
                            {
                                if (clearAvgBpm)
                                {
                                    clearAvgBpm = false;
                                    avgBpmValue = 0;
                                    avgBpmTicks = 0;
                                }
                                avgBpmValue += value;
                                avgBpmTicks++;

                                if (AvgBpmChanged != null)
                                {
                                    AvgBpmChanged((object)value, null);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    serialPort.DataReceived -= serialPort_DataReceived;
                    serialPort.Close();
                }
                catch (Exception ex2)
                {
                }
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, "SPO2.DataReceived()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Dispose()
        {
            if (Initialized)
            {
                Initialized = false;
            }
        }

        public static Double GetIr()
        {
            if (irTicks == 0) return 0;
            double average = irValue / irTicks;
            clearIr = true;
            return average;
        }

        public static Double GetBpm()
        {
            if (bpmTicks == 0) return 0;
            double average = bpmValue / bpmTicks;
            clearBpm = true;
            return average;
        }

        public static Double GetAvgBpm()
        {
            if (avgBpmTicks == 0) return 0;
            double average = avgBpmValue / avgBpmTicks;
            clearAvgBpm = true;
            return average;
        }
    }
}
