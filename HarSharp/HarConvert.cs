/* 
https://github.com/giacomelli/HarSharp
The MIT License (MIT)

Copyright (c) 2014 Diego Giacomelli

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace HarSharp
{
    /// <summary>
    /// Provides methods for converting between HTTP Archive Format (HAR) and HAR entities.
    /// </summary>
    public static class HarConvert
    {
        #region Public methods
        /// <summary>
        /// Deserialize HAR content to a HAR entity.
        /// </summary>
        /// <param name="har">The HAR content to be deserialized.</param>
        /// <returns>The HAR entity.</returns>
        public static Har Deserialize(string har)
        {
            if (string.IsNullOrWhiteSpace(har))
            {
                throw new ArgumentNullException("har");
            }

            var result = JsonConvert.DeserializeObject<Har>(har);

            TransformPartialRedirectUrlToFull(result);

            return result;
        }

        /// <summary>
        /// Deserialize a HAR file to a HAR entity.
        /// </summary>
        /// <param name="fileName">The HAR file name to be deserialized.</param>
        /// <returns>The HAR entity.</returns>
        public static Har DeserializeFromFile(string fileName)
        {
            return Deserialize(File.ReadAllText(fileName));
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Transform the partial redirect URL to a full one.
        /// </summary>
        /// <param name="har">The HAR.</param>
        private static void TransformPartialRedirectUrlToFull(Har har)
        {
            var responsesWithPartialRedirectUrl = har.Log.Entries
                .Where(e => e.Response.RedirectUrl != null && e.Response.RedirectUrl.OriginalString.StartsWith("/", StringComparison.OrdinalIgnoreCase));

            foreach (var entry in responsesWithPartialRedirectUrl)
            {
                var requestUrl = entry.Request.Url;
                entry.Response.RedirectUrl = new Uri(String.Format("{0}{1}",
                    requestUrl.GetLeftPart(UriPartial.Authority),
                    entry.Response.RedirectUrl.OriginalString));
            }
        }
        #endregion
    }
}
