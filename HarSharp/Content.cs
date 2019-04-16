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
using System.Diagnostics;

namespace HarSharp
{
    /// <summary>
    /// Describes details about response content.
    /// <remarks>
    /// <see cref="!:https://dvcs.w3.org/hg/webperf/raw-file/tip/specs/HAR/Overview.html#sec-object-types-content" />
    /// </remarks>
    /// </summary>
    [DebuggerDisplay("{MimeType}: {Text}")]
    public class Content : EntityBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the length of the returned content in bytes.
        /// <remarks>
        /// Should be equal to response.BodySize if there is no compression and bigger when the content has been compressed.
        /// </remarks>
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes saved.
        /// </summary>
        public int? Compression { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the response text (value of the Content-Type response header). 
        /// <remarks>
        /// The charset attribute of the MIME type is included (if available).
        /// </remarks>
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the response body sent from the server or loaded from the browser cache.
        /// <remarks>
        /// This field is populated with textual content only. The text field is either HTTP decoded text or a encoded (e.g. "base64") representation of the response body.
        /// </remarks>
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the encoding used for response text field.
        /// </summary>
        public string Encoding { get; set; }
        #endregion
    }
}
