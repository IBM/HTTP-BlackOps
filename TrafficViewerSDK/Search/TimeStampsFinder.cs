using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK.Search
{

	/// <summary>
	/// This class is used in log syncronization. It will locate a set of texts that match
	/// the specified datetime
	/// </summary>
	public class TimeStampsFinder
	{
		private const int ESTIMATED_LINE_SIZE = 1024;

		private FileStream _currentLog;
		private TimeStampsPositionsCache _cache;

		private const string TIME_REGEX = @"(\d{1,2}:\d{2}:\d{2}(?:\.\d+)?(?:\sAM|\sPM)?)";
		private const string EXCLUSIONS = "Date:|Set-Cookie|expiry";

		/// <summary>
		/// Finds time stamps in the specified file
		/// </summary>
		/// <param name="logPath"></param>
		public TimeStampsFinder(string logPath)
		{
			_currentLog = File.Open(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			_cache = new TimeStampsPositionsCache();

		}

		/// <summary>
		/// Find all the entries in the specified range of the given timestamp
		/// </summary>
		/// <param name="timeStamp"></param>
		/// <param name="range">Range in seconds</param>
		/// <returns>List of timestamp snapshots in byte array format (as they were read from the stream)</returns>
		public List<ByteArrayBuilder> Find(DateTime timeStamp, int range)
		{
			long rangeTicks = range * 1000 * 10000; //1ms = 10000ticks
			DateTime startStamp = new DateTime(timeStamp.Ticks - rangeTicks);
			DateTime endStamp = new DateTime(timeStamp.Ticks + rangeTicks);


			_currentLog.Position = _cache.FindBestEntry(startStamp);

			DateTime cacheEntry = default(DateTime);

			byte[] bytes;
			List<ByteArrayBuilder> result = new List<ByteArrayBuilder>();

			ByteArrayBuilder snapShot = null;
			DateTime currExtractedTime = startStamp; //last time extracted from the line read

			long lastPos = _currentLog.Position; //used to keep track of the log position before read

			while ((bytes = Utils.ReadLine(_currentLog, ESTIMATED_LINE_SIZE)) != null)
			{

				DateTime time = GetTimeStamp(bytes, timeStamp);

				if (time != default(DateTime))
				{
					if (time.CompareTo(endStamp) <= 0)
					{
						//if we are inside the range
						if (time.CompareTo(startStamp) >= 0)
						{
							//if the current search was not cached
							if (cacheEntry == default(DateTime))
							{
								//we save the first match to the cache
								_cache.Add(currExtractedTime, lastPos);
								cacheEntry = currExtractedTime;
							}

							currExtractedTime = time;

							if (snapShot != null)
							{
								//save the current snapshot
								result.Add(snapShot);
							}

							snapShot = new ByteArrayBuilder();
						}
					}
					else
					{
						//the current extracted time is higher than the upper range limit
						if (result.Count > 0 || (snapShot != null && snapShot.Length > 0))
						{
							//if we have already extracted snapshots then we stop here
							break;
							//otherwise we keep looking;
						}
					}
				}

				//save the last read position
				lastPos = _currentLog.Position;

				//here we add bytes we read to the current snapshot
				if (snapShot != null)
				{
					snapShot.AddChunkReference(bytes, bytes.Length);
					snapShot.AddChunkReference(Constants.NewLineBytes, Constants.NewLineBytes.Length);
				}

			}//end read loop

			//check if there is anything left to add
			if (snapShot != null && snapShot.Length > 0)
			{
				result.Add(snapShot);
			}

			return result;
		}

		/// <summary>
		/// Tries to match several time formats to the line
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="day">The value of the day portion of the date</param>
		/// <returns></returns>
		private DateTime GetTimeStamp(byte[] bytes, DateTime day)
		{
			DateTime result = default(DateTime);
			string line = Constants.DefaultEncoding.GetString(bytes);

			if (!Utils.IsMatch(line, EXCLUSIONS))
			{
				string ts = Utils.RegexFirstGroupValue(line, TIME_REGEX);

				if (ts != String.Empty && DateTime.TryParse(ts, out result))
				{
					result = new DateTime(day.Year, day.Month, day.Day, result.Hour, result.Minute, result.Second, result.Millisecond);
				}
			}

			return result;
		}

	}
}
