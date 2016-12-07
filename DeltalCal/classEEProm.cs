using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeltalCal {
    class classEEProm {
        // this reads and writes the RAMBo EEPROM table.

        const String eepromRegEx = @"EPR:(\d+) (\d+) ([^\s]+) (.+)";

        public bool doneReading = false;

        class classEEPromData {
            public String dataType;
            public String position;
            public String origValue;
            public String value;
            public String description;

            public classEEPromData(string dataType, string position, string origValue, string value, string description) {
                this.dataType = dataType;
                this.position = position;
                this.origValue = origValue;
                this.value = value;
                this.description = description;
            }
        
        }

        List<classEEPromData> eepromData;

        SerialPort serialPort;
        public classEEProm(SerialPort serialPort) {
            this.serialPort = serialPort;
           

        }


        void readEEProm() {
            // wire up the event handler.
            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataRx);
            serialPort.DiscardInBuffer(); // throw out anything that's there.

            this.serialPort.WriteLine("M205"); // request the EEPROM data from the host.
            
        }

        void dataRx(object sender, SerialDataReceivedEventArgs e) {
            String inData = serialPort.ReadLine();
            classEEPromData workData;
            if (inData.Contains("EPR")) {
                // break it apart and save it!
                string[] values = Regex.Split(inData, eepromRegEx);
                if (values.Length > 1) {
                    workData = new DeltalCal.classEEProm.classEEPromData(values[0], values[1], values[2], values[3],
                        values[4]);
                    eepromData.Add(workData);
                }
            }
            if (inData.Contains("wait")) {
                // we're done, disconnect event handler.
                serialPort.DataReceived -= dataRx;

                doneReading = true;

            }
        }
    }
}
