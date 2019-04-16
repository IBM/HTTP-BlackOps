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
using System;

namespace HarSharp
{
    /// <summary>
    /// Represents a HTTP cookie.
    /// <remarks>
    /// <see cref="!:https://dvcs.w3.org/hg/webperf/raw-file/tip/specs/HAR/Overview.html#sec-object-types-cookies" />
    /// </remarks>
    /// </summary>
    public class Cookie : ParameterBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the path pertaining to the cookie.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the host of the cookie.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the cookie expiration time.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is HTTP only.
        /// </summary>
        public bool HttpOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it was transmitted over SSL.
        /// </summary>
        public bool Secure { get; set; }
        #endregion
    }
}
