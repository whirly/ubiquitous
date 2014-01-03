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
	[StructLayout(LayoutKind.Sequential)]
	internal struct alloc_chain
	{
		internal IntPtr	ptr;	// void *
		internal IntPtr	next;
	}

	/// <summary>
	/// </summary>
	public class AllocChain
	{
		internal alloc_chain alloc_chain = new alloc_chain();

//		internal alloc_chain next 
//		{
//			get { return (alloc_chain) Marshal.PtrToStructure(this.pnext, typeof(alloc_chain));	}
//		}
	}
}
