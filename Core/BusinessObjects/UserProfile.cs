/*
===========================================================================
Copyright (c) 2010 BrickRed Technologies Limited

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
===========================================================================

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brickred.SocialAuth.NET.Core.BusinessObjects
{
    /// <summary>
    /// Contains information about context user
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Gets ID of user at provider
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// Gets first name of user
        /// </summary>
        public string FirstName { get; internal set; }
        /// <summary>
        /// Gets last name of user
        /// </summary>
        public string LastName { get; internal set; }
        /// <summary>
        /// Gets Email ID of user
        /// </summary>
        public string Email { get; internal set; }
        /// <summary>
        /// Gets profile picture URL of user
        /// </summary>
        public string ProfilePictureURL { get; internal set; }
        /// <summary>
        /// Gets language of user
        /// </summary>
        public string Language { get; internal set; }
        /// <summary>
        /// Gets country of user
        /// </summary>
        public string Country { get; internal set; }
        /// <summary>
        /// Gets public profile URL of user
        /// </summary>
        public string ProfileURL { get; internal set; }
        /// <summary>
        /// Gets date of birth of user
        /// </summary>
        public string DateOfBirth { get; internal set; }
        /// <summary>
        /// Gets Gender of user
        /// </summary>
        public string Gender { get; internal set; }
    }
}
