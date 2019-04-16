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
    /// Represents the root of the HAR data.
    /// <remarks>
    /// <see cref="!:https://dvcs.w3.org/hg/webperf/raw-file/tip/specs/HAR/Overview.html#sec-object-types-log" />
    /// </remarks>
    /// </summary>
    [DebuggerDisplay("Pages: {Pages.Count} | Entries: {Entries.Count}")]
    public class Log : EntityBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        public Log()
        {
            Pages = new List<Page>();
            Entries = new List<Entry>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the version number of the format.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the log creator application. 
        /// </summary>
        public Creator Creator { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        public Browser Browser { get; set; }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        public IList<Page> Pages { get; private set; }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        public IList<Entry> Entries { get; private set; }
        #endregion
    }
}
