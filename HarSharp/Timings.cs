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
    /// Describes various phases within request-response round trip.
    /// <remarks>
    /// All times are specified in milliseconds.
    /// <see cref="!:https://dvcs.w3.org/hg/webperf/raw-file/tip/specs/HAR/Overview.html#sec-object-types-timings" />
    /// </remarks>
    /// </summary>
    [DebuggerDisplay("Blocked:{Blocked} | Dns:{Dns} | Connect:{Connect} | Send:{Send} | Wait:{Wait} | Receive:{Receive} | Ssl:{Ssl}")]
    public class Timings : EntityBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the time spent in a queue waiting for a network connection.
        /// </summary>
        public double? Blocked { get; set; }

        /// <summary>
        /// Gets or sets the DNS resolution time. The time required to resolve a host name. 
        /// </summary>
        public double? Dns { get; set; }

        /// <summary>
        /// Gets or sets the time required to create TCP connection.
        /// </summary>
        public double? Connect { get; set; }

        /// <summary>
        /// Gets or sets the time required to send HTTP request to the server.
        /// </summary>
        public double Send { get; set; }

        /// <summary>
        /// Gets or sets the waiting for a response from the server.
        /// </summary>
        public double Wait { get; set; }

        /// <summary>
        /// Gets or sets the time required to read entire response from the server (or cache).
        /// </summary>
        public double Receive { get; set; }

        /// <summary>
        /// Gets or sets the time required for SSL/TLS negotiation.
        /// </summary>
        public double? Ssl { get; set; }
        #endregion
    }
}
