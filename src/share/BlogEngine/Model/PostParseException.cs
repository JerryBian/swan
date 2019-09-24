using System;

namespace Laobian.Share.BlogEngine.Model
{
    /// <summary>
    /// Exception occured during parsing post
    /// </summary>
    public class PostParseException : Exception
    {
        /// <summary>
        /// Default constructor of <see cref="PostParseException"/>
        /// </summary>
        public PostParseException()
        {
        }

        /// <summary>
        /// Constructor of <see cref="PostParseException"/>, accepts the message
        /// </summary>
        /// <param name="message"></param>
        public PostParseException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor of <see cref="PostParseException"/>, accepts the message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PostParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
