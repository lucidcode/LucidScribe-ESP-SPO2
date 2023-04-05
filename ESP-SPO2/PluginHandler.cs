using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace lucidcode.LucidScribe.Plugin.ESPSPO2
{
    public static class Device
    {
        static bool Initialized;
        static bool InitError;
        static SerialPort serialPort;
        static double irValue;
        static double spo2Value;
        static double bpmValue;
        static double avgBpmValue;

        static double irTicks;
        static double spo2Ticks;
        static double bpmTicks;
        static double avgBpmTicks;

        static bool clearIr;
        static bool clearSpo2;
        static bool clearBpm;
        static bool clearAvgBpm;

        public static string Algorithm = "REM Detection";
        public static int Threshold = 600;
        public static int BlinkInterval = 28;

        public static EventHandler<EventArgs> IrChanged;
        public static EventHandler<EventArgs> Spo2Changed;
        public static EventHandler<EventArgs> BpmChanged;
        public static EventHandler<EventArgs> AvgBpmChanged;

        public static PortForm formPort;

        public static Boolean Initialize()
        {
            if (!Initialized & !InitError)
            {
                formPort = new PortForm();
                if (formPort.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Algorithm = formPort.Algorithm;
                        BlinkInterval = formPort.BlinkInterval / 10;
                        Threshold = formPort.Threshold;
                        var selectedPort = formPort.SelectedPort;

                        formPort = new PortForm();
                        formPort.Show();

                        // Open the COM port
                        serialPort = new SerialPort(selectedPort);
                        serialPort.BaudRate = 115200;
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
                formPort.UpdateData(data);
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
                            else if (valuePair[0] == "SPO2")
                            {
                                if (clearSpo2)
                                {
                                    clearSpo2 = false;
                                    spo2Value = 0;
                                    spo2Ticks = 0;
                                }
                                spo2Value += value;
                                spo2Ticks++;

                                if (Spo2Changed != null)
                                {
                                    Spo2Changed((object)value, null);
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

        public static Double GetSpo2()
        {
            if (spo2Ticks == 0) return 0;
            double average = spo2Value / spo2Ticks;
            clearSpo2 = true;
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
