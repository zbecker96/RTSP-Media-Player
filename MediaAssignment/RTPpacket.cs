using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaAssignment
{
    /*
     * To allow protocol packet headers to be displayed, we
     * create a new form class here and allow our client and
     * server applications to derive form it, exposing the 
     * "displayText" class.
     * 
     */
    public class MediaForm : Form
    {
        public virtual void displayText(String sMessage){ }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MediaForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MediaForm";
            this.Load += new System.EventHandler(this.MediaForm_Load);
            this.ResumeLayout(false);

        }

        private void MediaForm_Load(object sender, EventArgs e)
        {

        }
    }
    /*
     * RTP packet format, refer to RFC 1889 for format
     *------------------
     */
    public class RTPpacket
    {
        //size of the RTP header:
        static int HEADER_SIZE = 12;
        //Fields that compose the RTP header
        public int Version;
        public int Padding;
        public int Extension;
        public int CC;
        public int Marker;
        public int PayloadType;
        public int SequenceNumber;
        public int TimeStamp;
        public int Ssrc;
        //Bitstream of the RTP header
        public byte[] header;
        //size of the RTP payload
        public int payload_size;
        //Bitstream of the RTP payload
        public byte[] payload;
        /*--------------------------
         * Constructor of an RTPpacket object from header fields and payload bitstream
         * used by the server
         *--------------------------
         */
        public RTPpacket(int PType, int Framenb, int Time, byte[] data, int data_length)
        {
            // Fill by default header fields:
            Version = 2;
            Padding = 0;
            Extension = 0;
            CC = 0;
            Marker = 0;
            Ssrc = 0;
            // Fill changing header fields:
            SequenceNumber = Framenb;
            TimeStamp = Time;
            PayloadType = PType;
            // Build the header bistream:
            //--------------------------
            header = new byte[HEADER_SIZE];
            //fill the payload bitstream:
            //--------------------------
            payload_size = data_length;
            //fill payload array of byte from data (given in parameter of the constructor)
            payload = new byte[data_length];
            //fill the header array of byte with RTP header fields

            header[0] = (byte)Version;
            header[0] = (byte)(header[0] | Padding << 2);
            header[0] = (byte)(header[0] | Extension << 3);
            header[0] = (byte)(header[0] | CC << 4);

            header[1] = (byte)Marker;
            header[1] = (byte)(header[1] | PayloadType << 1);

            header[2] = (byte)(SequenceNumber >> 8);

            header[3] = (byte)(SequenceNumber & 0xFF);

            header[4] = (byte)(TimeStamp >> 24);

            header[5] = (byte)(TimeStamp >> 16);

            header[6] = (byte)(TimeStamp >> 8);

            header[7] = (byte)(TimeStamp & 0xFF);

            header[8] = (byte)(Ssrc >> 24);

            header[9] = (byte)(Ssrc >> 16);

            header[10] = (byte)(Ssrc >> 8);

            header[11] = (byte)(Ssrc & 0xFF);

            //fill the payload bitstream:
            //--------------------------
            payload_size = data_length;
            payload = new byte[data_length];

            //fill payload array of byte from data (given in parameter of the constructor)
            payload = data;

        }
        /*--------------------------
         * Constructor of an RTPpacket object from the packet bistream, used by the client 
         *--------------------------
         */
        public RTPpacket(byte[] packet, int packet_size)
        {
            //Fill default fields:
            Version = 2;
            Padding = 0;
            Extension = 0;
            CC = 0;
            Marker = 0;
            Ssrc = 0;

            // Check if total packet size is lower than the header size
            if (packet_size >= HEADER_SIZE)
            {
                // Get the header bitsream:
                header = new byte[HEADER_SIZE];
                for (int i = 0; i < HEADER_SIZE; i++)
                    header[i] = packet[i];

                // Get the payload bitstream:
                payload_size = packet_size - HEADER_SIZE;
                payload = new byte[payload_size];
                for (int i = HEADER_SIZE; i < packet_size; i++)
                    payload[i - HEADER_SIZE] = packet[i];

                /*
                 * STODO-1
                 * Interpret the changing fields of the header:
                 * PayloadType, SequenceNumber and TimeStamp
                 */
                PayloadType = (header[1] & 127) >> 1;
                SequenceNumber = (header[3] & 0xFF) + ((header[2] & 0xFF) << 8);
                TimeStamp = (header[7] & 0xFF) + ((header[6] & 0xFF) << 8) + ((header[5] & 0xFF) << 16) + ((header[4] & 0xFF) << 24);

            }
        }
        /*--------------------------
         * getpayload: return the payload bistream of the RTPpacket and its size
         *--------------------------
         */
        public int getpayload(byte[] data)
        {
            for (int i = 0; i < payload_size; i++)
                data[i] = payload[i];

            return (payload_size);
        }
        /*--------------------------
         * getpayload_length: return the length of the payload
         *--------------------------
         */
        public int getpayload_length()
        {
            return (payload_size);
        }
        /*--------------------------
         * getlength: return the total length of the RTP packet
         *--------------------------
         */
        public int getlength()
        {
            return (payload_size + HEADER_SIZE);
        }
        /*--------------------------
         * getpacket: returns the packet bitstream and its length
         *--------------------------
         */
        public int getpacket(byte[] packet)
        {
            //Construct the packet = header + payload
            for (int i = 0; i < HEADER_SIZE; i++)
                packet[i] = header[i];
            for (int i = 0; i < payload_size; i++)
                packet[i + HEADER_SIZE] = payload[i];

            // Return total size of the packet
            return (payload_size + HEADER_SIZE);
        }
        /*--------------------------
         * gettimestamp
         *--------------------------
         */
        public int gettimestamp()
        {
            return (TimeStamp);
        }
        /*--------------------------
         * getsequencenumber
         *--------------------------
         */
        public int getsequencenumber()
        {
            return (SequenceNumber);
        }

        /*--------------------------
         * getpayloadtype
         *--------------------------
         */
        public int getpayloadtype()
        {
            return (PayloadType);
        }
        /*--------------------------
         * Print headers without the SSRC,
         * uses the MediaForm class to expose
         * the displayText extensions in the 
         * client and server implementations.
         *--------------------------
         */
        public void printheader(MediaForm cForm)
        {
            for (int i=0; i < (HEADER_SIZE-4); i++)
            {
                for (int j = 7; j>=0 ; j--)
                    if (((1<<j) & header[i] ) != 0)
                        cForm.displayText("1");
                    else
                        cForm.displayText("0");
                    cForm.displayText(" ");
            }

            cForm.displayText("\r\n");
        }

        //return the unsigned value of 8-bit integer nb
        static int unsigned_int(int nb)
        {
            if (nb >= 0)
                return (nb);
            else
                return (256 + nb);
        }
    }
}
