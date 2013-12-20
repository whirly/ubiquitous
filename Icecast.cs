using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ubiquitous
{
    class Icecast
    {
        private HttpWebRequest outReq;
        public Icecast( String server, String inPoint, String outPoint, String password )
        {
            outReq = InitializeOutStream( server, outPoint, password );            
        }

        public void Stream( )
        {
            Stream outStream = outReq.GetRequestStream();

            byte[]  buffer = new byte[ 1024 ];

            while( true ) 
            {
                outStream.Write(buffer, 0, 1024);
            }

        }

        private HttpWebRequest InitializeOutStream( String server, String outPoint, String password )
        {
            
            String outURL = "http://" + server + "/" + outPoint + ".ogg";
            HttpWebRequest client = (HttpWebRequest) WebRequest.Create( outURL );

            String encodedPassword = 
                System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes( "source:" + password));

            client.Method = "SOURCE";
            client.ContentType = "audio/mpeg";
            client.Headers.Add("Authorization", "Basic " + encodedPassword );
            client.Headers.Add("ice-name", "Ubiquitous Sound " + outPoint );
            client.Headers.Add("ice-url", outURL );
            client.Headers.Add("ice-genre", "Rock");
            client.Headers.Add("ice-description", "a simple application to dual stream.");
            client.Headers.Add("ice-audio-info", "ice-samplerate=44100;ice-bitrate=192;ice-channels=2");
            client.Headers.Add("ice-public", "1");
            client.Headers.Add("ice-private", "0");

            Console.Write("Connect to end point " + outURL );
            WebResponse response = client.GetResponse();
            Console.Write(" OK\n");

            return client;
        }
    }
}
