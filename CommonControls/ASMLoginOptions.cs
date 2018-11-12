using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK.Options;

namespace CommonControls
{
	/// <summary>
	/// Stores options for this program
	/// </summary>
	public class ASMLoginOptions : OptionsManager
	{


		public ASMLoginOptions()
		{
			string dir = System.Environment.GetEnvironmentVariable("userprofile");
            dir += "\\Application Data\\Http Black Ops\\ASMLoginOptions";
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
            _optionsDocPath = dir + "\\ASMLoginOptions.xml";
			Load(_optionsDocPath);
		}

		/// <summary>
		/// The ASE Url
		/// </summary>
		public string Url
		{
			get
			{
				object value = GetOption("Url");
				if (value == null) return "";
				return Convert.ToString(value);
			}
			set
			{
				SetSingleValueOption("Url", value);
			}
		}



		/// <summary>
		/// The login user name
		/// </summary>
		public string UserName
		{
			get
			{
				object value = GetOption("UserName");
				if (value == null) return "";
				return Convert.ToString(value);
			}
			set
			{
				SetSingleValueOption("UserName", value);
			}
		}

		/// <summary>
		/// The login password
		/// </summary>
		public string Password
		{
			get
			{
				object value = GetOption("Password");
				if (value == null) return "";
				return Convert.ToString(value);
			}
			set
			{
				SetSingleValueOption("Password", value);
			}
		}

        /// <summary>
        /// The use full name
        /// </summary>
        public string FullName
        {
            get
            {
                object value = GetOption("FullName");
                if (value == null) return "Unspecified";
                return Convert.ToString(value);
            }
            set
            {
                SetSingleValueOption("FullName", value);
            }
        }
    }
}
