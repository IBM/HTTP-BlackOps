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

namespace HarSharp
{
    /// <summary>
    /// A base class for HTTP messages.
    /// </summary>
    public abstract class MessageBase : EntityBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase"/> class.
        /// </summary>
        protected MessageBase()
        {
            Cookies = new List<Cookie>();
            Headers = new List<Header>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the HTTP Version.
        /// </summary>
        public string HttpVersion { get; set; }

        /// <summary>
        /// Gets the list of cookie objects.
        /// </summary>
        public IList<Cookie> Cookies { get; private set; }

        /// <summary>
        /// Gets the list of header objects.
        /// </summary>
        public IList<Header> Headers { get; private set; }

        /// <summary>
        /// Gets or sets the total number of bytes from the start of the HTTP request message until (and including) the double CRLF before the body.
        /// </summary>
        public int HeadersSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the request body (POST data payload) in bytes. 
        /// </summary>
        public int BodySize { get; set; }
        #endregion
    }
}
