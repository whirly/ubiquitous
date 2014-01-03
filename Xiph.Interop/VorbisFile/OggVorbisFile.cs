/********************************************************************
 *                                                                  *
 * THIS FILE IS PART OF THE OggVorbis SOFTWARE CODEC SOURCE CODE.   *
 * USE, DISTRIBUTION AND REPRODUCTION OF THIS LIBRARY SOURCE IS     *
 * GOVERNED BY A BSD-STYLE SOURCE LICENSE INCLUDED WITH THIS SOURCE *
 * IN 'COPYING'. PLEASE READ THESE TERMS BEFORE DISTRIBUTING.       *
 *                                                                  *
 * THE OggVorbis SOURCE CODE IS (C) COPYRIGHT 1994-2001             *
 * by the XIPHOPHORUS Company http://www.xiph.org/                  *
 *                                                                  *
 ********************************************************************

 function: stdio-based convenience library for opening/seeking/decoding

 ********************************************************************/

/* C#/.NET interop-port
 * 
 * Copyright 2008 Klaus Prückl <klaus.prueckl@live.at>
 */

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

using Xiph.Interop.Ogg;
using Xiph.Interop.Vorbis;

namespace Xiph.Interop.VorbisFile
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct OggVorbis_File
    {
        /// <summary>Pointer to a FILE *, etc.</summary>
        internal IntPtr datasource;	// void *
        internal int seekable;
        internal long offset;
        internal long end;
        internal ogg_sync_state oy;

        /* If the FILE handle isn't seekable (eg, a pipe), only the current
                   stream appears */
        internal int links;
        internal IntPtr offsets;	// ogg_int64_t *
        internal IntPtr dataoffsets;	// ogg_int64_t *
        internal IntPtr serialnos;	// long *
        /// <summary>
        /// overloaded to maintain binary
        /// compatability; x2 size, stores both
        /// beginning and end values
        /// </summary>
        internal IntPtr pcmlengths;	// ogg_int64_t *
        internal IntPtr vi;
        internal IntPtr vc;

        /* Decoding working state local storage */
        internal long pcm_offset;
        internal int ready_state;
        internal int current_serialno;
        internal int current_link;

        internal double bittrack;
        internal double samptrack;

        /// <summary>
        /// take physical pages, weld into a logical
        /// stream of packets
        /// </summary>
        internal ogg_stream_state os;
        /// <summary>
        /// central working state for the packet->PCM decoder
        /// </summary>
        internal vorbis_dsp_state vd;
        /// <summary>
        /// local working space for packet->PCM decode
        /// </summary>
        internal vorbis_block vb;

        internal ov_callbacks callbacks;
    }

    /// <summary>
    /// The OggVorbisFile class defines an Ogg Vorbis file. 
    /// </summary>
    /// <remarks>
    /// This structure is used in all libvorbisfile routines. Before it can be
    /// used, it must be initialized by <see cref="Open"/> or
    /// <see cref="OpenCallbacks"/>.
    /// <para>
    /// After use, the OggVorbisFile class object must be deallocated with a
    /// call to <see cref="Clear"/>.
    /// </para>
    /// <para>
    /// Once a file or data source is opened successfully by libvorbisfile
    /// (using <see cref="Open"/> or <see cref="OpenCallbacks"/>), it is owned
    /// by libvorbisfile. The file should not be used by any other applications
    /// or functions outside of the libvorbisfile API. The file must not be
    /// closed directly by the application at any time after a successful open;
    /// libvorbisfile expects to close the file within <see cref="Clear"/>. 
    /// </para>
    /// <para>
    /// If the call to <see cref="Open"/> or <see cref="OpenCallbacks"/>
    /// <b>fails</b>, libvorbisfile does <b>not</b> assume ownership of the
    /// file and the application is expected to close it if necessary.
    /// </para>
    /// </remarks>
    public class OggVorbisFile
    {
        #region Variables
        internal OggVorbis_File OggVorbis_File = new OggVorbis_File();
        internal bool changed = true;
        //		private VorbisInfo vi;
        //		private VorbisComment vc;
        #endregion

        #region Properties
        //		public vorbis_info		vi
        //		{
        //			get { return (vorbis_info) Marshal.PtrToStructure(this.pvi, typeof(vorbis_info)); }
        //		}
        //		public vorbis_comment	vc 
        //		{
        //			get { return (vorbis_comment) Marshal.PtrToStructure(this.pvc, typeof(vorbis_comment)); }
        //		}
        //		/// <summary>
        //		/// Read-only int indicating whether file is seekable. E.g., a physical
        //		/// file is seekable, a pipe isn't. 
        //		/// </summary>
        //		public int Seekable 
        //		{
        //			get { return this.OggVorbis_File.seekable; }
        //		}
        //		/// <summary>
        //		/// Read-only int indicating the number of logical bitstreams within
        //		/// the physical bitstream. 
        //		/// </summary>
        //		public int Links 
        //		{
        //			get { return this.OggVorbis_File.links; }
        //		}
        #endregion

        #region Functions
        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_clear([In, Out] ref OggVorbis_File vf);
        /// <summary>
        /// </summary>
        /// <param name="vf"></param>
        /// <returns></returns>
        public static int Clear(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        // TODO ! new in Vobis 1.2.0
        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_fopen([In] string path, [In, Out] ref OggVorbis_File vf);

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_open(SafeFileHandle f,
            [In, Out]	ref OggVorbis_File vf, IntPtr initial, int ibytes); // char *
        private static int Open(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_open_callbacks(IntPtr datasource, [In, Out] ref OggVorbis_File vf,
          IntPtr initial, int ibytes, ov_callbacks callbacks); // char *
        private static int OpenCallbacks(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_test(SafeFileHandle f, [In, Out] ref OggVorbis_File vf, IntPtr initial, int ibytes); // char *
        private static int Test(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_test_callbacks(IntPtr datasource, [In, Out] ref OggVorbis_File vf,
          IntPtr initial, int ibytes, ov_callbacks callbacks); // char *
        private static int TestCallbacks(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_test_open([In, Out] ref OggVorbis_File vf);
        private static int TestOpen(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_bitrate([In] ref OggVorbis_File vf, int i);
        private static int Bitrate(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_bitrate_instant([In, Out] ref OggVorbis_File vf);
        private static int BitrateInstant(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_streams([In] ref OggVorbis_File vf);
        private static int Streams(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_seekable([In] ref OggVorbis_File vf);
        private static int Seekable(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_serialnumber([In] ref OggVorbis_File vf, int i);
        private static int Serialnumber(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern long ov_raw_total([In] ref OggVorbis_File vf, int i);
        private static int RawTotal(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern long ov_pcm_total([In] ref OggVorbis_File vf, int i);
        private static int PcmTotal(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern double ov_time_total([In] ref OggVorbis_File vf, int i);
        private static int TimeTotal(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_raw_seek([In, Out] ref OggVorbis_File vf, long pos);
        private static int RawSeek(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_pcm_seek([In, Out] ref OggVorbis_File vf, long pos);
        private static int PcmSeek(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_pcm_seek_page([In, Out] ref OggVorbis_File vf, long pos);
        private static int PcmSeekPage(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_time_seek([In, Out] ref OggVorbis_File vf, double pos);
        private static int TimeSeek(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_time_seek_page([In, Out] ref OggVorbis_File vf, double pos);
        private static int TimeSeekPage(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_raw_seek_lap([In, Out] ref OggVorbis_File vf, long pos);
        private static int RawSeekLap(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_pcm_seek_lap([In, Out] ref OggVorbis_File vf, long pos);
        private static int PcmSeekLap(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_pcm_seek_page_lap([In, Out] ref OggVorbis_File vf, long pos);
        private static int PcmSeekPageLap(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_time_seek_lap([In, Out] ref OggVorbis_File vf, double pos);
        private static int TimeSeekLap(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_time_seek_page_lap([In, Out] ref OggVorbis_File vf, double pos);
        private static int TimeSeekPageLap(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern long ov_raw_tell([In] ref OggVorbis_File vf);
        private static int RawTell(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern long ov_pcm_tell([In] ref OggVorbis_File vf);
        private static int PcmTell(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern double ov_time_tell([In] ref OggVorbis_File vf);
        private static int TimeTell(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ov_info([In] ref OggVorbis_File vf, int link); // vorbis_info *
        private static int Info(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ov_comment([In] ref OggVorbis_File vf, int link); // vorbis_comment *
        private static int Comment(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_read_float([In, Out] ref OggVorbis_File vf, IntPtr pcm_channels, int samples, // float ***
          IntPtr bitstream);	// int *
        private static int ReadFloat(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_read([In, Out] ref OggVorbis_File vf, IntPtr buffer, int length,	// char *
          int bigendianp, int word, int sgned, IntPtr bitstream);	// int *
        private static int Read(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_crosslap([In, Out] ref OggVorbis_File vf1, [In, Out] ref OggVorbis_File vf2);
        private static int Crosslap(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_halfrate([In, Out] ref OggVorbis_File vf, int flag);
        private static int Halfrate(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        [DllImport(VorbisFile.DllFile, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ov_halfrate_p([In] ref OggVorbis_File vf);
        private static int HalfrateP(OggVorbisFile vf)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}