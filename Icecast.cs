using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using NAudio.Utils;
using NAudio.Wave;

namespace Ubiquitous
{
    class Icecast
    {
        private HttpWebRequest inReq;

        private WaveInEvent capture;
        private WaveOutEvent output;

        private BufferedWaveProvider buffer;
        private Stream outputStream;

        VorbisEncoder encoder = new VorbisEncoder(2, 44100, 0.6F);
        private Stream fileStream;

        public Icecast( String server, String inPoint, String outPoint, String password )
        {
            outputStream = InitializeOutStream( server, outPoint, password );
            fileStream = File.Open("machin.ogg", FileMode.Create);
            
            capture = InitializeCapture();
            // output = InitializeOutput();
        }

        public void Stream( )
        {
            encoder.WriteHeader(fileStream);

            capture.StartRecording();
            // output.Play();

            while( true ) 
            {
            }

        }

        private WaveInEvent InitializeCapture()
        {
            WaveInEvent capture = new WaveInEvent();
            capture.WaveFormat = new WaveFormat(44100, 2);
            capture.BufferMilliseconds = 1000;
            capture.DataAvailable += new EventHandler<WaveInEventArgs>(SendCaptureSample);

            return capture;
        }

        private WaveOutEvent InitializeOutput()
        {
            WaveOutEvent output = new WaveOutEvent();
            buffer = new BufferedWaveProvider(capture.WaveFormat);

            output.Init(buffer);

            return output;
        }

        private Stream InitializeOutStream( String server, String outPoint, String password )
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

            client.SendChunked = true;
            client.KeepAlive = true;

            Stream output = client.GetRequestStream();
            
            //WebResponse response = client.GetResponse();
            Console.Write(" OK\n");
            
            return output;
        }

        private void SendCaptureSample( object sender, WaveInEventArgs e )
        {
            Console.WriteLine("Byte recorded : {0}", e.BytesRecorded);
            encoder.WriteAudio(fileStream, e.Buffer, e.BytesRecorded);
        }
    }
}
