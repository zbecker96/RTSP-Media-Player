using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MediaServer
{
    public class VideoStream
    {
        BinaryReader m_bReader;

        /*-----------------------------------
         * constructor
         * -----------------------------------
         */
        public VideoStream(String filename)
        {
            // Init variables
            FileStream filestream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            m_bReader = new BinaryReader(filestream);
        }

        /*-----------------------------------
         * getnextframe
         * returns the next frame as an array of byte and the size of the frame
         *-----------------------------------
         */
        public int getnextframe(byte[] frame)
        {
            int length = 0;
            String length_string;
            byte[] frame_length = new byte[5];

            // Read current frame length
            m_bReader.Read(frame_length, 0, 5);

            // Transform frame_length to integer
            length_string = Encoding.ASCII.GetString(frame_length);
            length = Convert.ToInt32(length_string);

            return (m_bReader.Read(frame, 0, length));
        }
    }
}
