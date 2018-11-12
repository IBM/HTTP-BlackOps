using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace TrafficViewerControls.Browsing
{
	class ProxySettings
	{
		public void SetIEProxy(string proxyStr, string exceptions)
		{
			int optionCount = 1;
			InternetConnectionOption[] options = new InternetConnectionOption[3];
			options[0].m_Option = PerConnOption.INTERNET_PER_CONN_FLAGS;
			if (string.IsNullOrEmpty(proxyStr))
			{
				options[0].m_Value.m_Int = (int)PerConnFlags.PROXY_TYPE_DIRECT;
			}
			else
			{
				options[0].m_Value.m_Int = (int)(PerConnFlags.PROXY_TYPE_DIRECT | PerConnFlags.PROXY_TYPE_PROXY);
			}

			if (!string.IsNullOrEmpty(proxyStr))
			{
				options[1].m_Option = PerConnOption.INTERNET_PER_CONN_PROXY_SERVER;
				options[1].m_Value.m_StringPtr = Marshal.StringToHGlobalAuto(proxyStr);
				optionCount = 2;
				if (!string.IsNullOrEmpty(exceptions))
				{
					options[2].m_Option = PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS;
					if (exceptions == "null")
					{
						options[2].m_Value.m_StringPtr = Marshal.StringToHGlobalAuto(null);
					}
					else
					{
						options[2].m_Value.m_StringPtr = Marshal.StringToHGlobalAuto(exceptions);
					}
					optionCount = 3;
				}
			}

			InternetPerConnOptionList InternetPerConnList = new InternetPerConnOptionList();
			InternetPerConnList.dwSize = Marshal.SizeOf(InternetPerConnList);
			InternetPerConnList.dwOptionError = 0;
			InternetPerConnList.szConnection = IntPtr.Zero;



			int optSize = Marshal.SizeOf(typeof(InternetConnectionOption));
			IntPtr optionsPtr = Marshal.AllocCoTaskMem(optSize * optionCount);
			for (int i = 0; i < optionCount; ++i)
			{
				if (options[i].m_Option > 0)
				{
					IntPtr opt = new IntPtr(optionsPtr.ToInt32() + (i * optSize));
					Marshal.StructureToPtr(options[i], opt, false);
				}
			}
			InternetPerConnList.dwOptionCount = optionCount;
			InternetPerConnList.options = optionsPtr;

			IntPtr ipcoListPtr = Marshal.AllocCoTaskMem((Int32)InternetPerConnList.dwSize);
			Marshal.StructureToPtr(InternetPerConnList, ipcoListPtr, false);

			NativeMethods.InternetSetOption(IntPtr.Zero,
											InternetOption.INTERNET_OPTION_PER_CONNECTION_OPTION,
											ipcoListPtr,
											InternetPerConnList.dwSize);

			Marshal.FreeCoTaskMem(optionsPtr);
			Marshal.FreeCoTaskMem(ipcoListPtr);

		}
		public string ProxyExceptions
		{
			get
			{
				InternetConnectionOption ico = GetIEProxy(PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS);
				string exceptions = Marshal.PtrToStringAnsi(ico.m_Value.m_StringPtr);
				return exceptions;
			}
		}

		public string ProxyServer
		{
			get
			{
				InternetConnectionOption ico = GetIEProxy(PerConnOption.INTERNET_PER_CONN_PROXY_SERVER);
				string Server = Marshal.PtrToStringAnsi(ico.m_Value.m_StringPtr);
				return Server;
			}
		}

		public PerConnFlags ConnectionType
		{
			get
			{
				InternetConnectionOption ico = GetIEProxy(PerConnOption.INTERNET_PER_CONN_FLAGS);
				PerConnFlags Types = (PerConnFlags)ico.m_Value.m_Int;
				return Types;
			}
		}


		public InternetConnectionOption GetIEProxy(PerConnOption perConnOption)
		{

			InternetPerConnOptionList InternetPerConnList = new InternetPerConnOptionList();
			int listSize = Marshal.SizeOf(InternetPerConnList);
			InternetPerConnList.dwSize = Marshal.SizeOf(InternetPerConnList);
			InternetPerConnList.dwOptionCount = 1;
			InternetPerConnList.dwOptionError = 0;
			InternetPerConnList.szConnection = IntPtr.Zero;

			InternetConnectionOption ico = new InternetConnectionOption();
			GCHandle gch = GCHandle.Alloc(ico, GCHandleType.Pinned);
			ico.m_Option = perConnOption;
			int icoSize = Marshal.SizeOf(ico);
			InternetPerConnList.options = Marshal.AllocCoTaskMem(icoSize);

			IntPtr optionListPtr = InternetPerConnList.options;
			Marshal.StructureToPtr(ico, optionListPtr, false);

			if (NativeMethods.InternetQueryOption(IntPtr.Zero, 75, ref InternetPerConnList, ref listSize) == true)
			{
				ico = (InternetConnectionOption)Marshal.PtrToStructure(InternetPerConnList.options, typeof(InternetConnectionOption));
			}
			Marshal.FreeCoTaskMem(InternetPerConnList.options);
			gch.Free();

			return ico;
		}



	}

	#region WinInet structures
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct InternetPerConnOptionList
	{
		public int dwSize;               // size of the INTERNET_PER_CONN_OPTION_LIST struct
		public IntPtr szConnection;         // connection name to set/query options
		public int dwOptionCount;        // number of options to set/query
		public int dwOptionError;           // on error, which option failed
		//[MarshalAs(UnmanagedType.)]
		public IntPtr options;
	};

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct InternetConnectionOption
	{
		static readonly int Size;
		public PerConnOption m_Option;
		public InternetConnectionOptionValue m_Value;
		static InternetConnectionOption()
		{
			InternetConnectionOption.Size = Marshal.SizeOf(typeof(InternetConnectionOption));
		}

		// Nested Types
		[StructLayout(LayoutKind.Explicit)]
		public struct InternetConnectionOptionValue
		{
			// Fields
			[FieldOffset(0)]
			public System.Runtime.InteropServices.ComTypes.FILETIME m_FileTime;
			[FieldOffset(0)]
			public int m_Int;
			[FieldOffset(0)]
			public IntPtr m_StringPtr;
		}
	}
	#endregion

	#region WinInet enums
	//
	// options manifests for Internet{Query|Set}Option
	//
	public enum InternetOption : uint
	{
		INTERNET_OPTION_PER_CONNECTION_OPTION = 75
	}

	//
	// Options used in INTERNET_PER_CONN_OPTON struct
	//
	public enum PerConnOption
	{
		INTERNET_PER_CONN_FLAGS = 1,
		INTERNET_PER_CONN_PROXY_SERVER = 2,
		INTERNET_PER_CONN_PROXY_BYPASS = 3,
		INTERNET_PER_CONN_AUTOCONFIG_URL = 4

	}

	//
	// PER_CONN_FLAGS
	//
	[Flags]
	public enum PerConnFlags
	{
		PROXY_TYPE_DIRECT = 0x00000001,
		PROXY_TYPE_PROXY = 0x00000002,
		PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,
		PROXY_TYPE_AUTO_DETECT = 0x00000008
	}
	#endregion

	internal class NativeMethods
	{
		[DllImport("WinInet.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InternetSetOption(IntPtr hInternet, InternetOption dwOption, IntPtr lpBuffer, int dwBufferLength);
		[DllImport("WinInet.dll", EntryPoint = "InternetQueryOption", SetLastError = true)]
		public static extern bool InternetQueryOption(IntPtr hInternet, int dwOption, ref InternetPerConnOptionList optionsList, ref int bufferLength);

	}
}
