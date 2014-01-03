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
 * Copyright 2004 Klaus Prückl <klaus.prueckl@aon.at>
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Xiph.Interop.VorbisFile
{
    /// <summary>
    /// </summary>
    public enum Seek : int	// src: stdio.h
    {
        /// <summary>
        /// </summary>
        Cur = 1,
        /// <summary>
        /// </summary>
        End = 2,
        /// <summary>
        /// </summary>
        Set = 0
    }

    internal delegate uint read_func_delegate(IntPtr ptr, uint size, uint nmemb,
        SafeFileHandle datasource);
    internal delegate int seek_func_delegate(SafeFileHandle datasource,
        long offset, Seek whence);
    internal delegate int close_func_delegate(SafeFileHandle datasource);
    internal delegate int tell_func_delegate(SafeFileHandle datasource);

    /// <summary>
    /// The function prototypes for the callbacks are basically the same as for
    /// the stdio functions fread, fseek, fclose, ftell. 
    /// The one difference is that the FILE * arguments have been replaced with
    /// a void * - this is to be used as a pointer to whatever internal data
    /// these functions might need. In the stdio case, it's just a FILE * cast
    /// to a void *
    /// </summary>
    /// <remarks>
    /// If you use other functions, check the docs for these functions and
    /// return the right values. For <see cref="ov_callbacks.seek_func"/>, you
    /// *MUST* return -1 if the stream is unseekable.
    /// <para>
    /// See the callbacks and non-stdio I/O document for more detailed
    /// information on required behavior of the various callback functions.
    /// </para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ov_callbacks
    {
        /// <summary>
        /// Delegate to custom data reading function. 
        /// </summary>
        internal read_func_delegate read_func;
        /// <summary>
        /// Delegate to custom data seeking function. If the data source is not
        /// seekable (or the application wants the data source to be treated as
        /// unseekable at all times), the provided seek callback should always
        /// return -1 (failure). 
        /// </summary>
        internal seek_func_delegate seek_func;
        /// <summary>
        /// Delegate to custom data source closure function.
        /// </summary>
        internal close_func_delegate close_func;
        /// <summary>
        /// Delegate to custom data location function.
        /// </summary>
        internal tell_func_delegate tell_func;
    }

    internal enum FileState
    {
        NotOpen = 0,
        PartOpen = 1,
        Opened = 2,
        StreamSet = 3,
        InitSet = 4
    }

    /// <summary>
    /// The OvCallbacks class contains file manipulation function prototypes
    /// necessary for opening, closing, seeking, and location.
    /// </summary>
    /// <remarks>
    /// The OvCallbacks class does not need to be user-defined if you are
    /// working with stdio-based file manipulation; the ov_open() call provides
    /// default callbacks for stdio. OvCallbacks are defined and passed to
    /// <see cref="OggVorbisFile.OpenCallbacks"/> when implementing non-stdio
    /// based stream manipulation (such as playback from a memory buffer).
    /// </remarks>
    public class OvCallbacks
    {
        #region Variables
        internal ov_callbacks ov_callbacks;
        #endregion

        /// <summary>
        /// </summary>
        public OvCallbacks()
        {
            this.ov_callbacks = new ov_callbacks();
            this.ov_callbacks.close_func = new close_func_delegate(this.FileStreamClose);
            this.ov_callbacks.read_func = new read_func_delegate(this.FileStreamRead);
            this.ov_callbacks.seek_func = new seek_func_delegate(this.FileStreamSeek);
            this.ov_callbacks.tell_func = new tell_func_delegate(this.FileStreamTell);
        }

        private uint FileStreamRead(IntPtr ptr, uint size, uint nmemb,
            SafeFileHandle datasource)
        {
            FileStream file = new FileStream(datasource, FileAccess.Read);
            int count = (int)(size * nmemb);
            byte[] buffer = new byte[count];
            int bytes = file.Read(buffer, 0, count);
            Marshal.Copy(buffer, 0, ptr, bytes);
            return (uint)bytes;
        }
        private int FileStreamSeek(SafeFileHandle datasource, long offset,
            Seek whence)
        {
            FileStream file = new FileStream(datasource, FileAccess.Read);
            SeekOrigin so;
            if (!file.CanSeek)
                return -1;
            switch (whence)
            {
                case Seek.Cur:
                    so = SeekOrigin.Current;
                    break;
                case Seek.End:
                    so = SeekOrigin.End;
                    break;
                case Seek.Set:
                    so = SeekOrigin.Begin;
                    break;
                default:
                    so = (SeekOrigin)whence;
                    break;
            }
            try
            {
                file.Seek(offset, so);
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        private int FileStreamClose(SafeFileHandle datasource)
        {
            FileStream file = new FileStream(datasource, FileAccess.Read);
            try
            {
                file.Close();
                return 0;
            }
            catch (Exception)
            {
                return -1;	// = EOF
            }
        }
        private int FileStreamTell(SafeFileHandle datasource)
        {
            FileStream file = new FileStream(datasource, FileAccess.Read);
            try
            {
                return (int)file.Position;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}