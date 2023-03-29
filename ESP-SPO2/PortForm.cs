﻿using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Xml;
using System.IO;

namespace lucidcode.LucidScribe.Plugin.ESPSPO2
{
    public partial class PortForm : Form
    {

        public String SelectedPort = "";
        public String Algorithm = "REM Detection";
        public int Threshold = 600;
        public int BlinkInterval = 280;

        private Boolean loaded = false;
        private string m_strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\lucidcode\\Lucid Scribe\\";

        public PortForm()
        {
            InitializeComponent();
        }

        private void PortForm_Load(object sender, EventArgs e)
        {
            LoadPortList();
            LoadSettings();
            loaded = true;
        }

        private void LoadPortList()
        {
            lstPorts.Clear();
            foreach (string strPort in SerialPort.GetPortNames())
            {
                String strPortName = strPort;
                strPortName = strPortName.Replace("a", "");
                strPortName = strPortName.Replace("b", "");
                strPortName = strPortName.Replace("c", "");
                strPortName = strPortName.Replace("d", "");
                strPortName = strPortName.Replace("e", "");
                strPortName = strPortName.Replace("f", "");
                strPortName = strPortName.Replace("g", "");
                strPortName = strPortName.Replace("h", "");
                strPortName = strPortName.Replace("i", "");
                strPortName = strPortName.Replace("j", "");
                strPortName = strPortName.Replace("k", "");
                strPortName = strPortName.Replace("l", "");
                strPortName = strPortName.Replace("m", "");
                strPortName = strPortName.Replace("n", "");
                strPortName = strPortName.Replace("o", "");
                strPortName = strPortName.Replace("p", "");
                strPortName = strPortName.Replace("q", "");
                strPortName = strPortName.Replace("r", "");
                strPortName = strPortName.Replace("s", "");
                strPortName = strPortName.Replace("t", "");
                strPortName = strPortName.Replace("u", "");
                strPortName = strPortName.Replace("v", "");
                strPortName = strPortName.Replace("w", "");
                strPortName = strPortName.Replace("x", "");
                strPortName = strPortName.Replace("y", "");
                strPortName = strPortName.Replace("z", "");

                ListViewItem lstItem = new ListViewItem(strPortName);
                lstItem.ImageIndex = 0;
                lstPorts.Items.Add(lstItem);
            }
        }

        private void LoadSettings()
        {
            XmlDocument xmlSettings = new XmlDocument();

            if (!File.Exists(m_strPath + "Plugins\\SPO2.User.lsd"))
            {
                String defaultSettings = "<LucidScribeData>";
                defaultSettings += "<Plugin>";
                defaultSettings += "<Algorithm>REM Detection</Algorithm>";
                defaultSettings += "<Threshold>600</Threshold>";
                defaultSettings += "<BlinkInterval>280</BlinkInterval>";
                defaultSettings += "</Plugin>";
                defaultSettings += "</LucidScribeData>";
                File.WriteAllText(m_strPath + "Plugins\\SPO2.User.lsd", defaultSettings);
            }

            xmlSettings.Load(m_strPath + "Plugins\\SPO2.User.lsd");

            if (xmlSettings.DocumentElement.SelectSingleNode("//Algorithm") != null)
            {
                cmbAlgorithm.Text = xmlSettings.DocumentElement.SelectSingleNode("//Algorithm").InnerText;
            }
            if (xmlSettings.DocumentElement.SelectSingleNode("//BlinkInterval") != null)
            {
                cmbBlinkInterval.Text = xmlSettings.DocumentElement.SelectSingleNode("//BlinkInterval").InnerText;
            }
            else
            {
                cmbBlinkInterval.Text = "280";
            }
            if (xmlSettings.DocumentElement.SelectSingleNode("//Threshold") != null)
            {
                txtThreshold.Text = xmlSettings.DocumentElement.SelectSingleNode("//Threshold").InnerText;
            }
            else
            {
                txtThreshold.Text = "600";
            }
        }

        private void cmbAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) { return; }
            Algorithm = cmbAlgorithm.Text;
            SaveSettings();
        }

        private void cmbBlinkInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) { return; }
            BlinkInterval = Convert.ToInt32(cmbBlinkInterval.Text);
            SaveSettings();
        }

        private void SaveSettings()
        {
            String defaultSettings = "<LucidScribeData>";
            defaultSettings += "<Plugin>";
            defaultSettings += "<Algorithm>" + cmbAlgorithm.Text + "</Algorithm>";
            defaultSettings += "<Threshold>" + txtThreshold.Text + "</Threshold>";
            defaultSettings += "<BlinkInterval>" + cmbBlinkInterval.Text + "</BlinkInterval>";
            defaultSettings += "</Plugin>";
            defaultSettings += "</LucidScribeData>";
            File.WriteAllText(m_strPath + "Plugins\\SPO2.User.lsd", defaultSettings);
        }

        private void lstPlaylists_MouseMove(object sender, MouseEventArgs e)
        {
            if (lstPorts.GetItemAt(e.X, e.Y) != null)
            {
                lstPorts.Cursor = Cursors.Hand;
            }
            else
            {
                lstPorts.Cursor = Cursors.Default;
            }
        }

        private void lstPlaylists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPorts.SelectedItems.Count > 0)
            {
                SelectedPort = lstPorts.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            LoadPortList();
        }

        private void txtThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!loaded) { return; }
            Threshold = Convert.ToInt32(txtThreshold.Text);
            SaveSettings();
        }

    }
}
