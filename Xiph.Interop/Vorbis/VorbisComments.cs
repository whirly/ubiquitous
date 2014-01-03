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
 * Copyright 2004 Klaus Prückl <klaus.prueckl@aon.at>
 */

using System;
using System.Runtime.InteropServices;

namespace Xiph.Interop.Vorbis
{
	/// <summary>
	/// </summary>
	public class VorbisComments
	{
		private int comments;
		private IntPtr user_comments;
		private VorbisCommentLengths comment_lengths;

		/// <summary>
		/// </summary>
		public byte[] this [int index]
		{
			get 
			{
				if(index < 0 || index >=comments)
					throw new IndexOutOfRangeException();
				int length = this.comment_lengths[index];
				byte [] retval = new byte[length];
				IntPtr chars = (IntPtr) Marshal.PtrToStructure((IntPtr)((int)
					this.user_comments + index*Marshal.SizeOf(typeof(IntPtr))),
					typeof(IntPtr));
				Marshal.Copy(chars, retval, 0, length);
				return retval;
			}
		}
		/// <summary>
		/// </summary>
		/// <param name="user_comments"></param>
		/// <param name="comment_lengths"></param>
		/// <param name="comments"></param>
		public VorbisComments(IntPtr user_comments, VorbisCommentLengths comment_lengths, int comments) 
		{
			this.user_comments = user_comments;
			this.comment_lengths = comment_lengths;
			this.comments = comments;
		}
	}
}
