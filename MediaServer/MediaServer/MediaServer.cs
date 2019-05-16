using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaAssignment;

namespace MediaServer
{
    public partial class FormMediaServer : MediaForm
    {
        /*
         * RTP variables:
         * ----------------
         */
        private UdpClient m_RTPsocket; //socket to be used to send and receive UDP packets

        private EndPoint m_ClientIPAddr; //Client IP address
        private int m_RTP_dest_port = 0; //destination port for RTP packets  (given by the RTSP Client)

        /*
         * Video variables:
         * ----------------
         */
        private int m_nImagenb = 0; //image nb of the image currently transmitted
        private VideoStream m_cVideo; //VideoStream object used to access video frames
        private const int MJPEG_TYPE = 26; //RTP payload type for MJPEG video
        private const int FRAME_PERIOD = 60; //Frame period of the video to stream, in ms
        private const int VIDEO_LENGTH = 500; //length of the video in frames

        private byte[] buf; //buffer used to store the images to send to the client 

        /* RTSP variables
         * ----------------
         * rtsp states
         */
        enum RTSP_STATES { INIT, READY, PLAYING, SETUP, PLAY, PAUSE, TEARDOWN, DESCRIBE, UNDEFINED }
        private int m_RTSPport = 554;
        //RTSP Server state == INIT or READY or PLAY
        private RTSP_STATES m_State = RTSP_STATES.INIT; 
        TcpClient m_RTSPsocket; //socket used to send/receive RTSP messages
        TcpListener m_listener;
        // Input and output stream filters
        private StreamReader  m_RTSPBufferedReader;
        private StreamWriter m_RTSPBufferedWriter;
        static String m_sVideoFileName; // Video file requested from the client
        static int RTSP_ID = 123456; //ID of the RTSP session
        int RTSPSeqNb = 0; // Sequence number of RTSP messages within the session
  
        private const String CRLF = "\r\n";
        /* 
         * Worker thread controls
         */
        Boolean m_bDone = false;
        Boolean m_bPaused = false;
        AutoResetEvent pauseWorker = new AutoResetEvent(false);

        public FormMediaServer()
        {
            InitializeComponent();
            // Allocate memory for the sending buffer
            buf = new byte[15000];
            //Initiate TCP connection with the client for the RTSP session
            displayText("Started listner using port " + m_RTSPport + CRLF);
            m_listener = new TcpListener(IPAddress.Any, m_RTSPport);
            m_listener.Start();

            if (this.backgroundControlChannel.IsBusy != true)
            {
                this.backgroundControlChannel.RunWorkerAsync();
                displayText("Started background worker thread to receive request. " + CRLF);
            }
        }

        /* ------------------------------------
         * Main worker, processes incoming request and then
         * manages the control channel
         * 
         * ------------------------------------
         */
        public void workerThread() 
        {
            // Wait for the SETUP message from the client
            RTSP_STATES request_type;
            
            while(!m_bDone)
            {
	            request_type = parse_RTSP_request(); //blocking
	
            	if (request_type == RTSP_STATES.SETUP)
	            {
	                m_bDone = true;

	                // Update RTSP state
	                m_State = RTSP_STATES.READY;
                    displayText("New RTSP state: READY" + CRLF);
   
	                // Send response
	                send_RTSP_response();
   
	                /*
                     * Init the VideoStream object, the video files are located above
                     * the Debug and Release directory in "bin", strip off any URL specification
                     * and just use the file name for now.
                     */
                    String [] sFileTokens = m_sVideoFileName.Split('/');
                    if (sFileTokens.Length > 0)
                    {
                        m_cVideo = new VideoStream("../" + sFileTokens.Last());
                    }

	                // Init RTP socket
	                m_RTPsocket = new UdpClient();
	            }
            }
            // Loop to handle RTSP requests
            m_bDone = false;
            while (!m_bDone)
            {
	            //Pparse the request
	            request_type = parse_RTSP_request(); //blocking
	    
	            if ((request_type == RTSP_STATES.PLAY) && (m_State == RTSP_STATES.READY))
	            {
	                // Send back response
	                send_RTSP_response();
                    // If we have paused the stream, restart it
                    if (m_bPaused) m_bPaused = false;
                    pauseWorker.Set();
                    if (this.backgroundSendFile.IsBusy != true)
                    {
                        this.backgroundSendFile.RunWorkerAsync();
                        displayText("Started background worker thread to send file. " + CRLF);
                    }
	                // Update state
	                m_State = RTSP_STATES.PLAYING;
	                displayText("New RTSP state: PLAYING" + CRLF);
	            }
                else if ((request_type == RTSP_STATES.PAUSE) && (m_State == RTSP_STATES.PLAYING))
	            {                    
	                // Send back response
	                send_RTSP_response();
	                /*
                     * Stop the worker thread, this stops the video file from being sent to the client
                     * Let the student fill this in.
                     */
                    m_bPaused = true;
                    pauseWorker.Reset();
	                //update state
	                m_State = RTSP_STATES.READY;
	                displayText("New RTSP state: READY"+CRLF);
	            }
	            else if (request_type == RTSP_STATES.TEARDOWN)
	            {
	                // Send back response
	                send_RTSP_response();
	                /*
                     * Stop the timer service used to send the video chunks to the client
                     * Let the student implement the timer support
                     */
                    m_bDone = true;
                    backgroundSendFile.CancelAsync();
                    backgroundControlChannel.CancelAsync();
	                // Close sockets
	                m_RTSPsocket.Close();
	                m_RTPsocket.Close();
                    /*
                     * Initialize the listener again
                     */
                    request_type = RTSP_STATES.INIT;
                    m_bDone = false;
                    m_nImagenb = 0;

                    return;
	            }
            }
        }

        //------------------------------------
        //Parse RTSP Request
        //------------------------------------
        private RTSP_STATES parse_RTSP_request()
        {
            RTSP_STATES request_type = RTSP_STATES.UNDEFINED;
            try
            {
                // Parse request line and extract the request_type:
                String sRequestLine = m_RTSPBufferedReader.ReadLine();
                displayText("RTSP Server - Received from Client:" + sRequestLine + CRLF);

                String [] tokens = sRequestLine.Split(' ');
                String request_type_string = tokens[0];

                // Convert to request_type structure:
                if (request_type_string.CompareTo("SETUP") == 0)
	                request_type = RTSP_STATES.SETUP;
                else if (request_type_string.CompareTo("PLAY") == 0)
	                request_type = RTSP_STATES.PLAY;
                else if (request_type_string.CompareTo("PAUSE") == 0)
	                request_type = RTSP_STATES.PAUSE;
                else if (request_type_string.CompareTo("TEARDOWN") == 0)
	                request_type = RTSP_STATES.TEARDOWN;

                if (request_type == RTSP_STATES.SETUP)
	            {
	                // Extract VideoFileName from RequestLine
	                m_sVideoFileName = tokens[1];
	            }

                // Parse the SeqNumLine and extract CSeq field
                String SeqNumLine = m_RTSPBufferedReader.ReadLine();
                displayText("Sequence Number : " + SeqNumLine + CRLF);
                tokens = SeqNumLine.Split(':');
                RTSPSeqNb = Convert.ToInt32(tokens[1].Trim());
	
                // Get LastLine
                String sLastLine = m_RTSPBufferedReader.ReadLine();
                displayText("Last Line Recieved :" + sLastLine + CRLF);

                if (request_type == RTSP_STATES.SETUP)
	            {
	                // Extract RTP_dest_port from LastLine
	                tokens = sLastLine.Split(';');
                    String [] sPortToken = tokens[2].Split('=');
	                m_RTP_dest_port = Convert.ToInt32(sPortToken[1].Trim());
	            }
                // else LastLine will be the SessionId line ... do not check for now.
                // Read the header termination CRLF
                String sTerminator = m_RTSPBufferedReader.ReadLine();
            }
            catch (IOException ioex)
            {
                String sError = ioex.Message;
                if (!sError.Contains("forcibly closed"))
                    MessageBox.Show(sError);

                request_type = RTSP_STATES.TEARDOWN;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception caught: " + ex);
                request_type = RTSP_STATES.TEARDOWN;
            }
            return(request_type);
        }

        //------------------------------------
        //Send RTSP Response
        //------------------------------------
        private void send_RTSP_response()
        {
            try
            {
                m_RTSPBufferedWriter.Write("RTSP/1.0 200 OK"+CRLF);
                m_RTSPBufferedWriter.Write("CSeq: "+RTSPSeqNb+CRLF);
                m_RTSPBufferedWriter.Write("Session: "+RTSP_ID+CRLF);
                m_RTSPBufferedWriter.Write(CRLF);
                m_RTSPBufferedWriter.Flush();
                displayText("RTSP Server - Sent response to Client." + CRLF);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception caught: " + ex);
            }
        }
        private void backgroundControlChannel_DoWork(object sender, DoWorkEventArgs e)
        {
            // Initiate TCP connection with the client for the RTSP session
            m_RTSPsocket = m_listener.AcceptTcpClient();
            m_RTSPBufferedReader = new StreamReader(m_RTSPsocket.GetStream());
            m_RTSPBufferedWriter = new StreamWriter(m_RTSPsocket.GetStream());
            // Get Client IP address
            m_ClientIPAddr = m_RTSPsocket.Client.RemoteEndPoint;
            displayText("Received a client connection from " + m_ClientIPAddr + " using port " + m_RTSPport + CRLF);

            // Initiate RTSPstate
            m_State = RTSP_STATES.INIT;
            workerThread();
        }
        /*
         * Thread support for display
         */
        private delegate void displayDelegate(String sMessage);
        public override void displayText(String sMessage)
        {
            if (textDisplay.InvokeRequired)
            {
                textDisplay.Invoke(new displayDelegate(this.displayText), sMessage);
            }
            else
            {
                textDisplay.Text += sMessage;
            }
        }
        private void backgroundControlChannel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            String sStatus = "In Progress";
            if (e.Cancelled == true) sStatus = "Canceled!";
            else if (!(e.Error == null)) sStatus = ("Error: " + e.Error.Message);
            else sStatus = "Done!";

            displayText("Background worker completed client request with results (" + sStatus + ")" + CRLF);
            /*
             * Wait for the background task to complete,...
             */
            while (this.backgroundControlChannel.IsBusy == true) Thread.Sleep(20);
            this.backgroundControlChannel.RunWorkerAsync();
            displayText("Started background worker thread to receive request. " + CRLF);

        }
        /*
         * Background routine used to send the video file the client request
         */
        private int sendFile()
        {
            /*
             * It we hit a "teardown" while waiting, just return,..
             */
            if (m_bDone) return (0); 
            /*
             * Make sure we can send (not paused)
             */
            if (m_bPaused)
                pauseWorker.WaitOne();
            //if the current image nb is less than the length of the video
            if (m_nImagenb < VIDEO_LENGTH)
            {
                //Uupdate current imagenb
                m_nImagenb++;

                try
                {
                    // Get next frame to send from the video, as well as its size
                    int image_length = m_cVideo.getnextframe(buf);

                    // Builds an RTPpacket object containing the frame
                    RTPpacket rtp_packet = new RTPpacket(MJPEG_TYPE, m_nImagenb, m_nImagenb * FRAME_PERIOD, buf, image_length);

                    // Get to total length of the full rtp packet to send
                    int packet_length = rtp_packet.getlength();

                    // Retrieve the packet bitstream and store it in an array of bytes
                    byte[] packet_bits = new byte[packet_length];
                    rtp_packet.getpacket(packet_bits);

                    // Send the packet as a DatagramPacket over the UDP socket 
                    IPAddress ipAddress = ((IPEndPoint)m_ClientIPAddr).Address;
                    /*
                     * If you need to resolve IP host names, use the following:
                     * IPAddress ipAddress = Dns.Resolve("<Hostname>").AddressList[0];
                     */
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, m_RTP_dest_port);	

                    m_RTPsocket.Send(packet_bits, packet_bits.Length, ipEndPoint);

                    // Print the header bitstream
                    rtp_packet.printheader(this);

                    // Update GUI
                    displayText("Send frame #" + m_nImagenb + CRLF);
                    /*
                     * Return status indicating more to do
                     */
                    return (1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception caught: " + ex);
                    displayText("Received exception, stopping timer at frame " + m_nImagenb + CRLF);
                    return(0);
                }
            }
            return (0);
        }

        private void backgroundSendFile_DoWork(object sender, DoWorkEventArgs e)
        {
            // While we haven't reached the end of the video file, keep sending           
            while (sendFile() != 0) Thread.Sleep(FRAME_PERIOD);

            backgroundSendFile.CancelAsync();
        }

        private void textDisplay_TextChanged(object sender, EventArgs e) {

        }
    }
}
