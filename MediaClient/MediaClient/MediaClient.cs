using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaAssignment;


namespace MediaClient
{
    public partial class MediaClient : MediaForm
    {
        private const String PROTOCOL_VERSION = "RTSP/1.0";
        private const int RTP_RCV_PORT = 25000; //port where the client will receive the RTP packets

        private String m_ServerPort = "554";
        private String m_ServerHost = "Enter Server Hostname";
        private String m_VideoFileName = "Enter Video Filename";
        /*
         * RTP, UDP is used as the data channel
         *----------------
         * RTP variables:
         */
        //socket to be used to send and receive UDP packets
        private UdpClient m_RTPsocket;
        //buffer used to store data received from the server
        private byte[] m_receiveBuffer;
        // Timeout value, returns from a blocking read after this amount of time
        private const int RTP_RECEIVER_TIMEOUT = 120;

        /*
         * RTSP variables, TCP is used as the control channel
         *----------------
         * rtsp states
         */
        enum RTSP_STATE {INIT,READY,PLAYING};
        //RTSP state == INIT or READY or PLAYING
        private RTSP_STATE m_State;
        //socket used to send/receive RTSP messages
        private TcpClient m_RTSPsocket; 
        // Input/Output streams
        private StreamReader m_RTSPBufferStreamReader = null;
        private StreamWriter m_RTSPBufferStreamWriter = null;
        //Sequence number of RTSP messages within the session
        private int m_RTSPSeqNb = 0;
        //ID of the RTSP session (given by the RTSP Server)
        private int m_RTSPid = 0; 
        // Protocol line terminator
        private const String CRLF = "\r\n";
        /*
         * Video constants:
         *------------------
         */
        private const int MJPEG_TYPE = 26; //RTP payload type for MJPEG video
        /*
         * Client constructor, do all form initialization here
         */
        public MediaClient()
        {
            InitializeComponent();
            /*
             * Display user buttons to prevent confusion
             */
            this.buttonPause.Enabled = false;
            this.buttonPlay.Enabled = false;
            this.buttonTearDown.Enabled = false;
            /*
             * Allocate the receive buffer
             */
            m_receiveBuffer = new byte[15000];
            /*
             * STODO-2
             * Setup the RTSP state 
             */
            m_State = RTSP_STATE.INIT;
        }
        /*
         * Action handlers for all button controls (Setup, Play, Pause, Teardown)
         */
        /*
         * Handler Implementation - Setup
         */
        private void buttonSetup_Click(object sender, EventArgs e)
        {
            if (m_State == RTSP_STATE.INIT) 
	        {
	            //Init non-blocking RTPsocket that will be used to receive data
	            try
                {
                    StartupDlg cDlg = new StartupDlg();
                    if (DialogResult.OK == cDlg.ShowDialog())
                    {
                        m_ServerHost = cDlg.ServerName;
                        m_ServerPort = cDlg.ServerPort;
                        m_VideoFileName = cDlg.Filename;
                        /* 
                         * STODO-3
                         * Create new TCP socket for RTSP and create Reader/Writer Streams
                         */
                        m_RTSPsocket = new TcpClient(m_ServerHost, Convert.ToInt32(m_ServerPort));
                        m_RTSPBufferStreamReader = new StreamReader(m_RTSPsocket.GetStream());
                        m_RTSPBufferStreamWriter = new StreamWriter(m_RTSPsocket.GetStream());



                    }
                    else
                    {
                        textDisplay.Text += "Can't proceed without a server to connect to,..." + CRLF;
                    }

                    /*
                     * STODO-4
                     * Create a new DatagramSocket to receive RTP packets from the server, on port RTP_RCV_PORT
                     * and handle the communication to the server
                     */
                    m_RTPsocket = new UdpClient(RTP_RCV_PORT);
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, RTP_RCV_PORT);


                    /*
                     * STODO-5
                     * Set the Receive TimeOut value of the RTP socket to RTP_RECEIVER_TIMEOUT.
                     */
                    m_RTPsocket.Client.ReceiveTimeout = RTP_RECEIVER_TIMEOUT;


                }
                catch (SocketException se)
                {
                    MessageBox.Show("Socket exception: "+se);
                    return;
                }
                /*
                 * STODO-6
                 * Init RTSP sequence number to 1
                 */
                m_RTSPSeqNb = 1;
                /*
                 * STODO-7
                 * Send SETUP message to the server
                 */
                send_RTSP_request("SETUP", false);
                /*
                 * Wait for the response
                 */
                if (parse_server_response() != 200)
                    textDisplay.Text += "Invalid Server Response" + CRLF;
                else 
                {
                    /* STODO-8
                     * Change RTSP state  
                     */
                    m_State = RTSP_STATE.READY;

                    textDisplay.Text += "New RTSP state: " + m_State + "..." + CRLF;
                    this.buttonSetup.Enabled = false;
                    this.buttonPause.Enabled = true;
                    this.buttonPlay.Enabled = true;
                    this.buttonTearDown.Enabled = true;

                }
            }//else if state != INIT then do nothing
        }
        /*
         * Handler Implementation - Play
         */
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            textDisplay.Text += "Play Button pressed !" + CRLF; 

            if (m_State == RTSP_STATE.READY)
            {
                /*
                 * STODO-9
	             * Send PLAY message to the server, indicate whether the sequence number
                 * should be incremented
                 */
                send_RTSP_request("PLAY", true);

                /*
                 * Wait for the response
                 */
                if (parse_server_response() != 200)
		            textDisplay.Text += "Invalid Server Response" + CRLF;
	            else 
	            {
                    /*
                     * STODO-10
                     * Change RTSP state 
                     */
                    m_State = RTSP_STATE.PLAYING;
                    textDisplay.Text += "New RTSP state: " + m_State +"..." + CRLF;
                    
                    /*
                     * Display the init, we don't want the user hitting it
                     */
                    this.buttonSetup.Enabled = false;

	                /*
                     * Start the timer, we use this timer to receive the RTP video
                     * stream and display it in the client window
                     */
	                videoTimer.Start();
	            }
	        }//else if state != READY then do nothing
        }
        /*
         * Handler Implementation - Pause
         */
        private void buttonPause_Click(object sender, EventArgs e)
        {
            textDisplay.Text += "Pause Button pressed !" + CRLF;   

            if (m_State == RTSP_STATE.PLAYING) 
	        {
                /*
                 * STODO-11
                 * Send Pause message to the server, indicate whether the sequence number
                 * should be incremented
                 */
                send_RTSP_request("PAUSE", true);
	
	            /*
                 * Wait for the response 
                 */
	            if (parse_server_response() != 200)
		            textDisplay.Text += "Invalid Server Response" + CRLF;
	            else 
	            {
                    /*
                     * STODO-12
                     * Change RTSP state 
                     */
                    m_State = RTSP_STATE.READY;
                    textDisplay.Text += "New RTSP state: " + m_State + "..." + CRLF;
	      
	                //stop the timer
	                videoTimer.Stop();
	            }
	        } //else if state != PLAYING then do nothing
        }
        /*
         * Handler Implementation - Teardown
         */
        private void buttonTearDown_Click(object sender, EventArgs e)
        {
            textDisplay.Text += "Teardown Button pressed !" + CRLF;

            /*
             * STODO-13
             * Send Teardown message to the server, indicate whether the sequence number
             * should be incremented
             */

            send_RTSP_request("TEARDOWN", true);

            /*
             * Wait for the response
             */
            if (parse_server_response() != 200)
	            textDisplay.Text += "Invalid Server Response" + CRLF;
            else 
	        {
                /*
                 * STODO-14
                 * Change RTSP state 
                 */
                m_State = RTSP_STATE.INIT;
                textDisplay.Text += "Closing connection New RTSP state: " + m_State + "..." + CRLF;

	            //Stop the timer
	            videoTimer.Stop();
                this.buttonSetup.Enabled = true;
                this.buttonPause.Enabled = false;
                this.buttonPlay.Enabled = false;
                this.buttonTearDown.Enabled = false;
                /*
                 * STODO-15
                 * Clean up socket resources
                 */
                m_RTSPsocket.Close();
                m_RTPsocket.Close();
            }
        }
        /*
         *....................................
         * Protocol Implementation
         *....................................*/
        /*------------------------------------
         * Send RTSP Request
         *------------------------------------*/
        private void send_RTSP_request(String request_type, Boolean bIncreament)
        {
            try
            {
                if (bIncreament)
                    m_RTSPSeqNb++;
                /*
                 * STODO-16
                 *
                 * Write the request line:
                 * Note: Ignore the correct syntax of the resource specification, which
                 * is rtsp://" + this.m_ServerHost + "/" + this.m_VideoFileName, just send the filename.
                 * For more details, refer to the local wireshark trace of the working client given in the
                 * assignment
                 */
                m_RTSPBufferStreamWriter.Write(request_type + " rtsp://" + this.m_VideoFileName +" "+ PROTOCOL_VERSION + CRLF);

                /*
                 * STODO-17
                 * Write the CSeq line: 
                 */
                m_RTSPBufferStreamWriter.Write("CSeq:" + m_RTSPSeqNb.ToString() + CRLF);
                /*
                 * STODO-18
                 * Check if request_type is equal to "SETUP" and in this case write the "Transport:"
                 * line advertising to the server the port used to receive the RTP packets RTP_RCV_PORT
                 * otherwise, write the Session line using the RTSPid property
                 */
                if (request_type.CompareTo("SETUP") == 0)
                    m_RTSPBufferStreamWriter.Write("Transport: RTP/UDP;unicast;client_port="+RTP_RCV_PORT + "; mode=" + m_State + CRLF);
                else
                    m_RTSPBufferStreamWriter.Write("Session: " + m_RTSPid + CRLF);
                /*
                 * STODO-19
                 * Write the header termination characters
                 */
                m_RTSPBufferStreamWriter.Write(CRLF);
                /* 
                 * STODO-20
                 * Flush the writer stream
                 */
                m_RTSPBufferStreamWriter.Flush();
                

            }
            catch(Exception ex)
            {
	            MessageBox.Show("Exception caught: "+ex);
	            return;
            }
        }
        /*------------------------------------
         * Parse Server Response
         *------------------------------------*/
        private int parse_server_response() 
        {
            int reply_code = 0;

            try
            {
                textDisplay.Text += "RTSP Client - Waiting to Received from Server (Blocking Call):" + CRLF;
                /*
                 * STODO-21
                 * Receive the reply from the server
                 */
                String StatusLine = m_RTSPBufferStreamReader.ReadLine();
                if (StatusLine != null)
                {
                    textDisplay.Text += "RTSP Client - Received from Server:" + CRLF;
                    textDisplay.Text += StatusLine + CRLF;

                    string[] tokens = StatusLine.Split(' ');
                    //Skip over the RTSP version, get the second value in the list
                    reply_code = Convert.ToInt32(tokens[1]);

                    //If reply code is OK, get and print the 2 other lines
                    if (reply_code == 200)
                    {
                        String SeqNumLine = m_RTSPBufferStreamReader.ReadLine();
                        if (SeqNumLine != null)
                        {
                            textDisplay.Text += "Sequence Number : " + SeqNumLine + CRLF;

                            String SessionLine = m_RTSPBufferStreamReader.ReadLine();
                            textDisplay.Text += SessionLine + CRLF;

                            //If state == INIT gets the Session Id from the SessionLine
                            tokens = SessionLine.Split(':'); 
                            //Skip over the Session:
                            m_RTSPid = Convert.ToInt32(tokens[1].Trim());
                        }
                    }
                    // Read the header termination characters
                    String sTerminator = m_RTSPBufferStreamReader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception caught: " + ex);
                return (0);
            }
    
            return(reply_code);
        }
        /*
         * Timer support - timer used to receive data from the UDP socket
         */
        private void videoTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                /*
                 * Receive the DP from the socket:
                 * IPEndPoint object will allow us to read datagrams sent from any source.
                 */
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                /*
                 * Blocks until a message returns on this socket from a remote host.
                 */
                Byte[] receiveBytes = m_RTPsocket.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                /*
                 * 
                 * Create an RTPpacket object from the Datagram packet
                 * 
                 * NOTE: Beware of STODO in RTPpacket constructor for client!
                 */
                RTPpacket rtp_packet = new RTPpacket(receiveBytes, receiveBytes.Length);
                /*
                 * Print important header fields of the RTP packet received: 
                 */
                this.textDisplay.Text += "Got RTP packet with SeqNum # " + rtp_packet.getsequencenumber() + " TimeStamp " + rtp_packet.gettimestamp() + " ms, of type " + rtp_packet.getpayloadtype() + CRLF;
                rtp_packet.printheader(this);
                /*
                 * Get the payload bitstream from the RTPpacket object
                 */
                int payload_length = rtp_packet.getpayload_length();
                byte[] payload = new byte[payload_length];
                /*
                 * STODO-22
                 * Get the payload using the method "getpayload"
                 */
                rtp_packet.getpayload(payload);

                /*
                 * Get and display the image as a bitmap
                 */
                Bitmap bmp = (Bitmap)Bitmap.FromStream(
                              new MemoryStream(payload, 0, payload_length));
                this.movieDisplay.Image = bmp;
            }
            catch (System.Net.Sockets.SocketException sockex)
            {
                /*
                 * Ignore timeout case, this is normal termination (once the client/server are working correctly)
                 */
                if (sockex.ErrorCode != 10060) MessageBox.Show(sockex.Message);
                return;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception caught: " + ex);
                return;
            }

        }
        public override void displayText(String charOut)
        {
            textDisplay.Text += charOut;
        }
    }
}
