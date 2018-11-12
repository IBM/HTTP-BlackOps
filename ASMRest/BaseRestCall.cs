using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace ASMRest
{
	public abstract class BaseRestCall<T> : BaseRestHttpClient
	{

		private const string API_BASE_PATH = "/ase/api";
		public abstract string Path { get; }

        private string _extraFilter;
        /// <summary>
        /// Additional filter for the query 
        /// </summary>
        public string ExtraFilter
        {
            get { return _extraFilter; }
            set { _extraFilter = Utils.UrlEncode(value); }
        }

        public BaseRestCall(ASMRestSettings settings):base(settings)
        {
        }


		public virtual T[] Fetch()
		{
			return this.Fetch(null);
		}

		public virtual T[] Fetch(Dictionary<string, string> queryParamList = null)
		{
			string path = String.Format("{0}/{1}", API_BASE_PATH, Path);

            if (!String.IsNullOrWhiteSpace(_extraFilter))
            {
                if (queryParamList != null)
                {
                    if (queryParamList.ContainsKey("query"))
                    {
                        queryParamList["query"] = queryParamList["query"] +Utils.UrlEncode(",")+ _extraFilter;
                    }
                    else
                    {
                        queryParamList.Add("query", _extraFilter);
                    }
                }
                else
                {
                    queryParamList = new Dictionary<string, string>();
                    queryParamList.Add("query", _extraFilter);
                }
            }

			HttpResponseInfo resp = SendRequest("GET", path, queryParamList);

			string jsonData = resp.ResponseBody.ToString();

            JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 1024 * 1024 * 1024;
            T[] result = ser.Deserialize<T[]>(jsonData);

			string contentRange = resp.Headers["Content-Range"];
			//check to see if we got all
			if (result != null && contentRange != null)
			{
				//parse content range
				string totalString = Utils.RegexFirstGroupValue(contentRange, @"/(\d+)");
				int total;
				if (int.TryParse(totalString, out total) && total > result.Length)
				{
					//get the rest
					string range = String.Format("items={0}-{1}", result.Length, total);
					resp = SendRequest("GET", path, queryParamList, null, range);
					jsonData = resp.ResponseBody.ToString();
					T[] remaining = ser.Deserialize<T[]>(jsonData);
					List<T> resultList = new List<T>();
					resultList.AddRange(result);
					resultList.AddRange(remaining);

					result = resultList.ToArray();

				}

			}

			return result;
		}

		public virtual T Get()
		{
			return Get(String.Empty);
		}

		public virtual T Get(string id)
		{
			string path = "";

			if (String.IsNullOrWhiteSpace(id))
			{
				path = String.Format("{0}/{1}", API_BASE_PATH, Path);
			}
			else
			{
				path = String.Format("{0}/{1}/{2}", API_BASE_PATH, Path, id);
			}

			HttpResponseInfo resp = SendRequest("GET", path, null);
			string jsonData = resp.ResponseBody.ToString();
			JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 1024 * 1024 * 1024;
			return ser.Deserialize<T>(jsonData);
		}

		public virtual T Get(long id)
		{
			return Get(id.ToString());

		}

		public virtual T Update(T obj, string id)
		{
			string path = String.Format("{0}/{1}/{2}", API_BASE_PATH, Path, id);

			JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 1024 * 1024 * 1024;
			string jsonData = ser.Serialize(obj);
			HttpResponseInfo resp = SendRequest("PUT", path, null, jsonData);

			if (resp.Status > 300)
			{
				throw new Exception(Utils.HtmlDecode(resp.ResponseBody.ToString()));
			}

			return ser.Deserialize<T>(jsonData);
		}

		public virtual T Add(T obj)
		{
			string path = String.Format("{0}/{1}", API_BASE_PATH, Path);

			JavaScriptSerializer ser = new JavaScriptSerializer();
            ser.MaxJsonLength = 1024 * 1024 * 1024;
			string jsonData = ser.Serialize(obj);
			HttpResponseInfo resp = SendRequest("PUT", path, null, jsonData);
			return ser.Deserialize<T>(jsonData);
		}
	}
}
