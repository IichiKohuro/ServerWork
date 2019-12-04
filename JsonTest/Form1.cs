using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JsonTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            Obj obj = new Obj()
            {
                data = new List<Data>()
                {
                    new Data()
                    {
                        ID = 0001,
                        Name = "Test0001",
                        Input220 = 1,
                        Ethernet = 1,
                        PhisicEthernet = 0,
                        WorkEthernet = 0
                    },
                    new Data()
                    {
                        ID = 0002,
                        Name = "Test0002",
                        Input220 = 0,
                        Ethernet = 0,
                        PhisicEthernet = 0,
                        WorkEthernet = 0
                    }
                }
            };

            txtJsonOutput.Text = JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }

    public class Data
    {
        [JsonProperty("device_id")]
        public int ID { get; set; }

        [JsonProperty("device_name")]
        public string Name { get; set; }

        [JsonProperty("220 Input")]
        public int Input220 { get; set; }

        [JsonProperty("ETHERNET")]
        public int Ethernet { get; set; }

        [JsonProperty("Phisic ETHERNET")]
        public int PhisicEthernet { get; set; }

        [JsonProperty("Work ETHERNET")]
        public int WorkEthernet { get; set; }
    }

    public class Obj
    {
        public List<Data> data { get; set; }
    }
}
