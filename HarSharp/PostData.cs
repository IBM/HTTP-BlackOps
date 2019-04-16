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
using System.Collections.Generic;
using System.Diagnostics;

namespace HarSharp
{
    /// <summary>
    /// Describes posted data.
    /// <remarks>
    /// <see cref="!:https://dvcs.w3.org/hg/webperf/raw-file/tip/specs/HAR/Overview.html#sec-object-types-postData" />
    /// </remarks>
    /// </summary>
    [DebuggerDisplay("{MimeType}: {Text}")]
    public class PostData : EntityBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PostData"/> class.
        /// </summary>
        public PostData()
        {
            Params = new List<PostDataParameter>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mime type of posted data.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets the list of posted parameters (in case of URL encoded parameters).
        /// </summary>
        public IList<PostDataParameter> Params { get; private set; }

        /// <summary>
        /// Gets or sets the plain text posted data.
        /// </summary>
        public string Text { get; set; }
        #endregion
    }
}