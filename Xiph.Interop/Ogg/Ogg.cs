/********************************************************************
 *                                                                  *
 * THIS FILE IS PART OF THE OggVorbis SOFTWARE CODEC SOURCE CODE.   *
 * USE, DISTRIBUTION AND REPRODUCTION OF THIS LIBRARY SOURCE IS     *
 * GOVERNED BY A BSD-STYLE SOURCE LICENSE INCLUDED WITH THIS SOURCE *
 * IN 'COPYING'. PLEASE READ THESE TERMS BEFORE DISTRIBUTING.       *
 *                                                                  *
 * THE OggVorbis SOURCE CODE IS (C) COPYRIGHT 1994-2002             *
 * by the Xiph.Org Foundation http://www.xiph.org/                  *
 *                                                                  *
 ********************************************************************

 function: toplevel libogg include

 ********************************************************************/

/* C#/.NET interop-port
 * 
 * Copyright 2004 Klaus Pr�ckl <klaus.prueckl@aon.at>
 */

using System;
using System.Configuration;
using System.Runtime.InteropServices;

namespace Xiph.Interop.Ogg
{
	internal abstract class Ogg 
	{
		internal const string DllFile = @"libogg.dll";
	}
}
