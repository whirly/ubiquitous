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
	public class VorbisCommentLengths
	{
		private int comments;
		private IntPtr comment_lengths;

		/// <summary>
		/// </summary>
		public int this [int index]
		{
			get 
			{
				if(index < 0 || index >=this.comments)
					throw new IndexOutOfRangeException();
				return (int) Marshal.PtrToStructure((IntPtr)((int)
					this.comment_lengths + index*Marshal.SizeOf(typeof(int))),
					typeof(int));
			}
		}
		/// <summary>
		/// </summary>
		/// <param name="comment_lengths"></param>
		/// <param name="comments"></param>
		public VorbisCommentLengths(IntPtr comment_lengths, int comments) 
		{
			this.comment_lengths = comment_lengths;
			this.comments = comments;
		}
	}
}
