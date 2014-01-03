/********************************************************************
 *                                                                  *
 * THIS FILE IS PART OF THE OggVorbis SOFTWARE CODEC SOURCE CODE.   *
 * USE, DISTRIBUTION AND REPRODUCTION OF THIS LIBRARY SOURCE IS     *
 * GOVERNED BY A BSD-STYLE SOURCE LICENSE INCLUDED WITH THIS SOURCE *
 * IN 'COPYING'. PLEASE READ THESE TERMS BEFORE DISTRIBUTING.       *
 *                                                                  *
 * THE OggVorbis SOURCE CODE IS (C) COPYRIGHT 1994-2001             *
 * by the XIPHOPHORUS Company http://www.xiph.org/                  *

 ********************************************************************

 function: libvorbis codec headers

 ********************************************************************/

/* C#/.NET interop-port
 * 
 * Copyright 2008 Klaus Prückl <klaus.prueckl@live.at>
 */

using System;
using System.Collections;
using System.Runtime.InteropServices;

using Xiph.Interop.Ogg;

namespace Xiph.Interop.Vorbis
{
    /// <summary>
    /// Vorbis ERRORS and return codes
    /// </summary>
    public enum Ov
    {
        /// <summary>
        /// Not true, or no data available 
        /// </summary>
        False = -1,
        /// <summary>
        /// </summary>
        Eof = -2,
        /// <summary>
        /// Vorbisfile encoutered missing or corrupt data in the bitstream.
        /// Recovery is normally automatic and this return code is for
        /// informational purposes only.
        /// </summary>
        Hole = -3,

        /// <summary>
        /// Read error while fetching compressed data for decode.
        /// </summary>
        ERead = -128,
        /// <summary>
        /// Internal inconsistency in decode state. Continuing is likely not possible.
        /// </summary>
        EFault = -129,
        /// <summary>
        /// Feature not implemented.
        /// </summary>
        EImpl = -130,
        /// <summary>
        /// Either an invalid argument, or incompletely initialized argument
        /// passed to libvorbisfile call.
        /// </summary>
        EInval = -131,
        /// <summary>
        /// The given file/data was not recognized as Ogg Vorbis data.
        /// </summary>
        ENotVorbis = -132,
        /// <summary>
        /// The file/data is apparently an Ogg Vorbis stream, but contains a
        /// corrupted or undecipherable header.
        /// </summary>
        EBadHeader = -133,
        /// <summary>
        /// The bitstream format revision of the given stream is not supported.
        /// </summary>
        EVersion = -134,
        /// <summary>
        /// </summary>
        ENotAudio = -135,
        /// <summary>
        /// </summary>
        EBadPacket = -136,
        /// <summary>
        /// The given link exists in the Vorbis data stream, but is not
        /// decipherable due to garbacge or corruption.
        /// </summary>
        EBadLink = -137,
        /// <summary>
        /// The given stream is not seekable.
        /// </summary>
        ENoSeek = -138
    }

    /// <summary>
    /// libvorbis encodes in two abstraction layers; first we perform DSP
    /// and produce a packet (see docs/analysis.txt).  The packet is then
    /// coded into a framed OggSquish bitstream by the second layer (see
    /// docs/framing.txt).  Decode is the reverse process; we sync/frame
    /// the bitstream and extract individual packets, then decode the
    /// packet back into PCM audio.
    /// 
    /// The extra framing/packetizing is used in streaming formats, such as
    /// files.  Over the net (such as with UDP), the framing and
    /// packetization aren't necessary as they're provided by the transport
    /// and the streaming layer is not used
    /// </summary>
    public abstract class Vorbis
    {
        // Buffer management
        static private float[][]   buffer;
        static private int bufferSize = 0;
        static private IntPtr internal_buffer;

        // lots of asserts are thrown if debug-dll is used
		internal const string DllFile = @"libvorbis.dll";

        #region Vorbis PRIMITIVES: general ***************************************

        #region vorbis_granule_time
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern double vorbis_granule_time(
            [In]	ref vorbis_dsp_state v, long granulepos);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="granulepos"></param>
        /// <returns></returns>
        public static double GranuleTime(VorbisDspState v, long granulepos)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");

            return vorbis_granule_time(ref v.vorbis_dsp_state, granulepos);
        }
        #endregion

        #endregion

        #region Vorbis PRIMITIVES: analysis/DSP layer ****************************

        #region vorbis_analysis_init
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_analysis_init(
            [In, Out]	ref vorbis_dsp_state v,
            [In]		ref vorbis_info vi);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vi"></param>
        /// <returns></returns>
        public static int AnalysisInit(VorbisDspState v, VorbisInfo vi)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");
            if (vi == null)
                throw new ArgumentException("VorbisInfo vi cannot be null!");

            int retval = vorbis_analysis_init(ref v.vorbis_dsp_state,
                ref vi.vorbis_info);
            v.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_commentheader_out
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_commentheader_out(
            [In]		ref vorbis_comment vc,
            [In, Out]	ref ogg_packet op);
        /// <summary>
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int CommentheaderOut(VorbisComment vc, OggPacket op)
        {
            if (vc == null)
                throw new ArgumentException("VorbisComment vc cannot be null!");
            if (op == null)
                throw new ArgumentException("OggPacket op cannot be null!");

            int retval = vorbis_commentheader_out(ref vc.vorbis_comment,
                ref op.ogg_packet);
            op.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_analysis_headerout
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_analysis_headerout(
            [In]		ref vorbis_dsp_state v,
            [In]		ref vorbis_comment vc,
            [In, Out]	ref ogg_packet op,
            [In, Out]	ref ogg_packet op_comm,
            [In, Out]	ref ogg_packet op_code);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vc"></param>
        /// <param name="op"></param>
        /// <param name="op_comm"></param>
        /// <param name="op_code"></param>
        /// <returns></returns>
        public static int AnalysisHeaderout(VorbisDspState v, VorbisComment vc,
            OggPacket op, OggPacket op_comm, OggPacket op_code)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");
            if (op == null)
                throw new ArgumentException("OggPacket op cannot be null!");
            if (op_comm == null)
                throw new ArgumentException("OggPacket op_comm cannot be null!");
            if (op_code == null)
                throw new ArgumentException("OggPacket op_code cannot be null!");

            int retval = vorbis_analysis_headerout(ref v.vorbis_dsp_state,
                ref vc.vorbis_comment, ref op.ogg_packet,
                ref op_comm.ogg_packet, ref op_code.ogg_packet);
            op.changed = true;
            op_comm.changed = true;
            op_code.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_analysis_buffer
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr vorbis_analysis_buffer(
            [In, Out] ref vorbis_dsp_state v, int vals);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public static float[][] AnalysisBuffer(VorbisDspState v, int vals)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");

            VorbisInfo vi = v.Vi;

            if( vals != bufferSize )
            {
                buffer = new float[vi.Channels][];
                bufferSize = vals;

                for (int i = 0; i < vi.Channels; i++)
                {
                    buffer[i] = new float[bufferSize];
                }
            }
            
            internal_buffer = vorbis_analysis_buffer(ref v.vorbis_dsp_state, vals);
            v.changed = true;

            return buffer;
        }
        #endregion

        #region vorbis_analysis_wrote
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_analysis_wrote(
            [In, Out]	ref vorbis_dsp_state v, int vals);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public static int AnalysisWrote(VorbisDspState v, int vals)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");

            // Copy our buffer into the vorbis internal buffer
            int[] memory = new int[v.Vi.Channels];
            Marshal.Copy(internal_buffer, memory, 0, v.Vi.Channels);

            for( int i = 0; i < v.Vi.Channels; i++ )
            {
                IntPtr ptr = (IntPtr)memory[i];
                Marshal.Copy( buffer[i], 0, ptr, vals);
            }

            int retval = vorbis_analysis_wrote(ref v.vorbis_dsp_state, vals);
            v.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_analysis_blockout
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_analysis_blockout(
            [In, Out]	ref vorbis_dsp_state v,
            [In, Out]	ref vorbis_block vb);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vb"></param>
        /// <returns></returns>
        public static int AnalysisBlockout(VorbisDspState v, VorbisBlock vb)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");
            if (vb == null)
                throw new ArgumentException("VorbisBlock vb cannot be null!");

            int retval = vorbis_analysis_blockout(ref v.vorbis_dsp_state,
                ref vb.vorbis_block);
            v.changed = true;
            vb.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_analysis
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_analysis(
            [In, Out]	ref vorbis_block vb,
            [In, Out]	ref ogg_packet op);
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_analysis(
            [In, Out]	ref vorbis_block vb,
            [In]		IntPtr ptr);
        /// <summary>
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int Analysis(VorbisBlock vb, OggPacket op)
        {
            if (vb == null)
                throw new ArgumentException("VorbisBlock vb cannot be null!");
            int retval;
            if (op == null)
                retval = vorbis_analysis(ref vb.vorbis_block, IntPtr.Zero);
            else
            {
                retval = vorbis_analysis(ref vb.vorbis_block, ref op.ogg_packet);
                op.changed = true;
            }
            vb.changed = true;
            return retval;
        }
        #endregion

        /// <summary>
        /// New in Vorbis 1.1.0
        /// </summary>
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_bitrate_addblock(
            [In, Out]	ref vorbis_block vb);

        /// <summary>
        /// New in Vorbis 1.1.0
        /// </summary>
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_bitrate_flushpacket(
            [In, Out]	ref vorbis_dsp_state vd,
            [In, Out]	ref ogg_packet op);

        #endregion

        #region Vorbis PRIMITIVES: synthesis layer *******************************

        #region vorbis_synthesis_idheader
        // TODO ! new in Vobis 1.2.0
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_idheader([In] ref ogg_packet op);
        #endregion

        #region vorbis_synthesis_headerin
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_headerin(
            [In, Out]	ref vorbis_info vi,
            [In, Out]	ref vorbis_comment vc,
            [In]		ref ogg_packet op);
        /// <summary>
        /// </summary>
        /// <param name="vi"></param>
        /// <param name="vc"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int SynthesisHeaderin(VorbisInfo vi, VorbisComment vc,
            OggPacket op)
        {
            if (vi == null)
                throw new ArgumentException("VorbisInfo vi cannot be null!");

            int retval = vorbis_synthesis_headerin(ref vi.vorbis_info,
                ref vc.vorbis_comment, ref op.ogg_packet);
            vi.changed = true;
            vc.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_synthesis_init
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_init(
            [In, Out]	ref vorbis_dsp_state v,
            [In]		ref vorbis_info vi);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vi"></param>
        /// <returns></returns>
        public static int SynthesisInit(VorbisDspState v, VorbisInfo vi)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");
            if (vi == null)
                throw new ArgumentException("VorbisInfo vi cannot be null!");
            int retval = vorbis_synthesis_init(ref v.vorbis_dsp_state,
                ref vi.vorbis_info);
            v.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_synthesis_restart
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_restart(
            [In, Out]	ref vorbis_dsp_state v);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int SynthesisRestart(VorbisDspState v)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");
            int retval = vorbis_synthesis_restart(ref v.vorbis_dsp_state);
            v.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_synthesis
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis(
            [In, Out]	ref vorbis_block vb,
            [In]		ref ogg_packet op);
        /// <summary>
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int Synthesis(VorbisBlock vb, OggPacket op)
        {
            if (vb == null)
                throw new ArgumentException("VorbisBlock vb cannot be null!");
            if (op == null)
                throw new ArgumentException("OggPacket op cannot be null!");
            int retval = vorbis_synthesis(ref vb.vorbis_block, ref op.ogg_packet);
            vb.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_synthesis_trackonly
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_trackonly(
            [In, Out]	ref vorbis_block vb,
            [In]		ref ogg_packet op);
        /// <summary>
        /// </summary>
        /// <param name="vb"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int SynthesisTrackonly(VorbisBlock vb, OggPacket op)
        {
            if (vb == null)
                throw new ArgumentException("VorbisBlock vb cannot be null!");
            int retval = vorbis_synthesis_trackonly(ref vb.vorbis_block, ref op.ogg_packet);
            vb.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_synthesis_blockin
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_blockin(
            [In, Out]	ref vorbis_dsp_state v,
            [In]		ref vorbis_block vb);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vb"></param>
        /// <returns></returns>
        public static int SynthesisBlockin(VorbisDspState v, VorbisBlock vb)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");

            System.Diagnostics.Trace.Indent();
            //			System.Diagnostics.Debug.Assert(v.Vi.Channels == 2, "Blockin vor Aufruf");
            System.Diagnostics.Trace.WriteLine(">Blockin vor Aufruf: " + v.Vi.Channels);
            int retval = vorbis_synthesis_blockin(ref v.vorbis_dsp_state,
                ref vb.vorbis_block);
            v.changed = true;
            //			System.Diagnostics.Debug.Assert(v.Vi.Channels == 2, "Blockin nach Aufruf");
            System.Diagnostics.Trace.WriteLine("<Blockin nach Aufruf: " + v.Vi.Channels);
            System.Diagnostics.Trace.Unindent();
            return retval;
        }
        #endregion

        #region vorbis_synthesis_pcmout
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_pcmout(
            [In]		ref vorbis_dsp_state v, ref IntPtr pcm); // float ***
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="pcm"></param>
        /// <returns></returns>
        public static int SynthesisPcmout(VorbisDspState v, out float[][] pcm)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");

            IntPtr ppcm = IntPtr.Zero;
            System.Diagnostics.Trace.Indent();
            //			System.Diagnostics.Debug.Assert(v.Vi.Channels == 2, "Pcmout vor Aufruf");
            System.Diagnostics.Trace.WriteLine(">Pcmout vor Aufruf: " + v.Vi.Channels);
            int samples = vorbis_synthesis_pcmout(ref v.vorbis_dsp_state,
                ref ppcm);
            //			System.Diagnostics.Debug.Assert(v.Vi.Channels == 2, "Pcmout nach Aufruf");
            System.Diagnostics.Trace.WriteLine("<Pcmout nach Aufruf: " + v.Vi.Channels);
            System.Diagnostics.Trace.Unindent();
            int channels = v.Vi.Channels;
            pcm = new float[channels][];
            int[] pch = new int[channels];
            if (ppcm != IntPtr.Zero)
            {
                Marshal.Copy(ppcm, pch, 0, channels);
                for (int i = 0; i < channels; i++)
                {
                    pcm[i] = new float[samples];
                    Marshal.Copy((IntPtr)pch[i], pcm[i], 0, samples);
                }
            }
            return samples;
        }
        #endregion

        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_lapout(
            [In, Out]	vorbis_dsp_state v, ref IntPtr pcm); // float ***
        //		private static int SynthesisLapout(VorbisDspState v, VorbisInfo vi) 
        //		{
        //			return vorbis_synthesis_lapout(v.vorbis_dsp_state, ...);
        //		}

        #region vorbis_synthesis_read
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_read(
            [In, Out]	ref vorbis_dsp_state v, int samples);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="samples"></param>
        /// <returns></returns>
        public static int SynthesisRead(VorbisDspState v, int samples)
        {
            if (v == null)
                throw new ArgumentException("VorbisDspState v cannot be null!");
            int retval = vorbis_synthesis_read(ref v.vorbis_dsp_state, samples);
            v.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_packet_blocksize
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_packet_blocksize(
            [In]	ref vorbis_info vi,
            [In]	ref ogg_packet op);
        /// <summary>
        /// </summary>
        /// <param name="vi"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int PacketBlocksize(VorbisInfo vi, OggPacket op)
        {
            if (vi == null)
                throw new ArgumentException("VorbisInfo vi cannot be null!");
            if (op == null)
                throw new ArgumentException("OggPacket op cannot be null!");
            int retval = vorbis_packet_blocksize(ref vi.vorbis_info, ref op.ogg_packet);
            return retval;
        }
        #endregion

        #region vorbis_synthesis_halfrate
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_halfrate(
            [In]	ref vorbis_info v, int flag);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static int SynthesisHalfrate(VorbisInfo v, int flag)
        {
            if (v == null)
                throw new ArgumentException("VorbisInfo v cannot be null!");
            return vorbis_synthesis_halfrate(ref v.vorbis_info, flag);
        }
        #endregion

        #region vorbis_synthesis_halfrate_p
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_synthesis_halfrate_p(
            [In]	ref vorbis_info v);
        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int SynthesisHalfrateP(VorbisInfo v)
        {
            if (v == null)
                throw new ArgumentException("VorbisInfo v cannot be null!");
            return vorbis_synthesis_halfrate_p(ref v.vorbis_info);
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// </summary>
    public abstract class VorbisBitrate
    {
        #region Vorbis PRIMITIVES: analysis/DSP layer ****************************

        #region vorbis_bitrate_addblock
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_bitrate_addblock(
            [In, Out]	ref vorbis_block vb);
        /// <summary>
        /// </summary>
        /// <param name="vb"></param>
        /// <returns></returns>
        public static int Addblock(VorbisBlock vb)
        {
            if (vb == null)
                throw new ArgumentException("VorbisBlock vb cannot be null!");
            int retval = vorbis_bitrate_addblock(ref vb.vorbis_block);
            vb.changed = true;
            return retval;
        }
        #endregion

        #region vorbis_bitrate_flushpacket
        [DllImport(Vorbis.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int vorbis_bitrate_flushpacket(
            [In]		ref vorbis_dsp_state vd,
            [In, Out]	ref ogg_packet op);
        /// <summary>
        /// </summary>
        /// <param name="vd"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static int Flushpacket(VorbisDspState vd, OggPacket op)
        {
            if (vd == null)
                throw new ArgumentException("VorbisDspState vd cannot be null!");
            if (op == null)
                throw new ArgumentException("OggPacket op cannot be null!");
            int retval = vorbis_bitrate_flushpacket(ref vd.vorbis_dsp_state, ref op.ogg_packet);
            op.changed = true;
            return retval;
        }
        #endregion

        #endregion
    }
}