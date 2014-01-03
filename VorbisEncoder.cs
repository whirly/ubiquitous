using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Xiph.Interop.Ogg;
using Xiph.Interop.Vorbis;
using Xiph.Interop.VorbisEncode;

namespace Ubiquitous
{
    class VorbisEncoder
    {
        private VorbisInfo vInfo = new VorbisInfo();
        private VorbisComment vComment = new VorbisComment();
        private VorbisDspState vDspState;
        private VorbisBlock vBlock = new VorbisBlock();

        private OggStreamState oggStreamState = new OggStreamState();
        private OggPage oggPage = new OggPage();
        OggPacket packet = new OggPacket();
        
        public VorbisEncoder( int channels, int rate, float quality )
        {            
            VorbisInfo.Init(vInfo);
            vDspState = new VorbisDspState(vInfo);
            VorbisEncode.InitVbr(vInfo, channels, rate, quality);
            VorbisEncode.SetupInit(vInfo);

            VorbisComment.Init(vComment);
            VorbisComment.AddTag(vComment, "ENCODER", System.Text.Encoding.ASCII.GetBytes("Ubiquitous") );
            
            Vorbis.AnalysisInit(vDspState, vInfo);
            VorbisBlock.Init(vDspState, vBlock);

            Random rnd = new Random();
            OggStreamState.Init(oggStreamState, rnd.Next() );
        }

        public void WriteHeader( Stream stream )
        {
            OggPacket header = new OggPacket();
            OggPacket headerComm = new OggPacket();
            OggPacket headerCode = new OggPacket();

            Vorbis.AnalysisHeaderout(vDspState, vComment, header, headerComm, headerCode);

            OggStreamState.Packetin(oggStreamState, header);
            OggStreamState.Packetin(oggStreamState, headerComm);
            OggStreamState.Packetin(oggStreamState, headerCode);

            while (OggStreamState.Flush(oggStreamState, oggPage) > 0)
            {
                stream.Write(oggPage.Header, 0, oggPage.HeaderLen);
                stream.Write(oggPage.Body, 0, oggPage.BodyLen);
            }
        }

        public void WriteAudio( Stream stream, byte[] buffer, int bufferLen )
        {
            int page_out = 0;
            int analysis = 0;
            int packet_in = 0;

            // First we must transcribe our PCM data into an array of float.
            float[][] processingBuffer = Vorbis.AnalysisBuffer(vDspState, bufferLen / 4);

            for (int i = 0; i < bufferLen / 4; i++)
            {
                processingBuffer[0][i] = (( buffer[i*4+1] << 8 ) | (0x00ff & (int) buffer[i*4])) / 32768.0F;
                processingBuffer[1][i] = (( buffer[i*4+3] << 8 ) | (0x00ff & (int) buffer[i*4+2])) / 32768.0F;
            }

            Vorbis.AnalysisWrote(vDspState, bufferLen / 4);

            while (Vorbis.AnalysisBlockout(vDspState, vBlock) == 1)
            {
                Vorbis.Analysis(vBlock, null);
                VorbisBitrate.Addblock(vBlock);
                analysis++;

                while (VorbisBitrate.Flushpacket(vDspState, packet) > 0 )
                {
                    OggStreamState.Packetin(oggStreamState, packet);
                    packet_in++;

                    while (OggStreamState.Pageout(oggStreamState, oggPage) > 0)
                    {
                        stream.Write(oggPage.Header, 0, oggPage.HeaderLen);
                        stream.Write(oggPage.Body, 0, oggPage.BodyLen);
                        page_out++;
                    }
                }
            }
        }

        public void Close()
        {
            OggStreamState.Clear( oggStreamState );
            
            VorbisBlock.Clear(vBlock);
            VorbisDspState.Clear(vDspState);
            VorbisComment.Clear(vComment);
            
            VorbisInfo.Clear(vInfo);
        }
    }
}
