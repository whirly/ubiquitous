using System;
using System.Runtime.InteropServices;

namespace Xiph.Interop.VorbisEncode
{
	/// <summary>New in Vorbis 1.1.0</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ovectl_ratemanage2_arg 
	{
		internal int    management_active;

		internal int   bitrate_limit_min_kbps;
		internal int   bitrate_limit_max_kbps;
		internal int   bitrate_limit_reservoir_bits;
		internal double bitrate_limit_reservoir_bias;

		internal int   bitrate_average_kbps;
		internal double bitrate_average_damping;
	}

	/// <summary>
	/// Zusammenfassung für OvectlRatemanage2Arg.
	/// </summary>
	public class OvectlRatemanage2Arg
	{
        /// <summary>
        /// </summary>
		public OvectlRatemanage2Arg()
		{
			//
			// TODO: Fügen Sie hier die Konstruktorlogik hinzu
			//
		}
	}
}
