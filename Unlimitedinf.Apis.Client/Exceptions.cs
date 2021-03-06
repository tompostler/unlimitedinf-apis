﻿using System;
using System.Net;
using System.Runtime.Serialization;

namespace Unlimitedinf.Apis.Client
{
    /// <summary>
    /// Represents errors that occur when performing operations against the API.
    /// </summary>
    [Serializable]
    public abstract class ApiException : Exception
    {
        /// <summary>
        /// The HTTP status code we got back from the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        private ApiException() { }

        internal ApiException(HttpStatusCode statusCode)
            : base() { this.StatusCode = statusCode; }
        internal ApiException(HttpStatusCode statusCode, string message)
            : base(message) { this.StatusCode = statusCode; }
        internal ApiException(HttpStatusCode statusCode, string message, Exception e)
            : base(message, e) { this.StatusCode = statusCode; }

        /// <summary>
        /// See <see cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)"/>.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("statcode", this.StatusCode);
        }
    }

    #region HTTP 4xx Client Errors

    /// <summary>
    /// Represents an HTTP 4xx error.
    /// </summary>
    [Serializable]
    public class ClientErrorException : ApiException
    {
        private const HttpStatusCode statusCode = HttpStatusCode.BadRequest;

        internal ClientErrorException()
            : base(statusCode) { }
        internal ClientErrorException(string message)
            : base(statusCode, message) { }
        internal ClientErrorException(string message, Exception e)
            : base(statusCode, message, e) { }

        internal ClientErrorException(HttpStatusCode statusCode)
            : base(statusCode) { }
        internal ClientErrorException(HttpStatusCode statusCode, string message)
            : base(statusCode, message) { }
        internal ClientErrorException(HttpStatusCode statusCode, string message, Exception e)
            : base(statusCode, message, e) { }
    }

    /// <summary>
    /// Represents an HTTP 401 Unauthorized error.
    /// </summary>
    [Serializable]
    public class UnauthorizedException : ClientErrorException
    {
        private const HttpStatusCode statusCode = HttpStatusCode.Unauthorized;

        internal UnauthorizedException()
            : base(statusCode) { }
        internal UnauthorizedException(string message)
            : base(statusCode, message) { }
        internal UnauthorizedException(string message, Exception e)
            : base(statusCode, message, e) { }
    }

    /// <summary>
    /// Represents an HTTP 404 Not Found error.
    /// </summary>
    [Serializable]
    public class NotFoundException : ClientErrorException
    {
        private const HttpStatusCode statusCode = HttpStatusCode.NotFound;

        internal NotFoundException()
            : base(statusCode) { }
        internal NotFoundException(string message)
            : base(statusCode, message) { }
        internal NotFoundException(string message, Exception e)
            : base(statusCode, message, e) { }
    }

    /// <summary>
    /// Represents an HTTP 409 Conflict error.
    /// </summary>
    [Serializable]
    public class ConflictException : ClientErrorException
    {
        private const HttpStatusCode statusCode = HttpStatusCode.Conflict;

        internal ConflictException()
            : base(statusCode) { }
        internal ConflictException(string message)
            : base(statusCode, message) { }
        internal ConflictException(string message, Exception e)
            : base(statusCode, message, e) { }
    }

    /// <summary>
    /// Represents an HTTP 418 I'm a teapot error.
    /// </summary>
    [Serializable]
    public class TeapotException : ClientErrorException
    {
        private const HttpStatusCode statusCode = (HttpStatusCode)418;

        internal TeapotException()
            : base(statusCode) { }
        internal TeapotException(string message)
            : base(statusCode, message) { }
        internal TeapotException(string message, Exception e)
            : base(statusCode, message, e) { }
    }

    #endregion HTTP 4xx Client Errors
    
    #region HTTP 5xx Client Errors

    /// <summary>
    /// Represents an HTTP 5xx error.
    /// </summary>
    [Serializable]
    public class ServerErrorException : ApiException
    {
        private const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

        internal ServerErrorException()
            : base(statusCode) { }
        internal ServerErrorException(string message)
            : base(statusCode, message) { }
        internal ServerErrorException(string message, Exception e)
            : base(statusCode, message, e) { }

        internal ServerErrorException(HttpStatusCode statusCode)
            : base(statusCode) { }
        internal ServerErrorException(HttpStatusCode statusCode, string message)
            : base(statusCode, message) { }
        internal ServerErrorException(HttpStatusCode statusCode, string message, Exception e)
            : base(statusCode, message, e) { }
    }

    #endregion HTTP 5xx Client Errors

    /// <summary>
    /// A helper class to create the proper exception.
    /// </summary>
    public static class ExceptionCreator
    {
        /// <summary>
        /// Translate a HttpStatusCode into an exception type.
        /// </summary>
        public static ApiException Create(HttpStatusCode statusCode, string message)
        {
            int stat = (int)statusCode;

            if (stat >= 400 && stat < 500)
            {
                switch (statusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        return new UnauthorizedException(message);
                    case HttpStatusCode.NotFound:
                        return new NotFoundException(message);
                    case HttpStatusCode.Conflict:
                        return new ConflictException(message);
                    case HttpStatusCode.BadRequest:
                    default:
                        if (stat == 418)
                            return new TeapotException(message);
                        else
                            return new ClientErrorException(statusCode, message);
                }
            }
            else if (stat >= 500 && stat < 600)
            {
                return new ServerErrorException(statusCode, message);
            }

            return null;
        }

        /// <summary>
        /// Instead of just creating the exception, throw it if there is one!
        /// </summary>
        public static void ThrowMaybe(HttpStatusCode statusCode, string message)
        {
            var e = Create(statusCode, message);
            if (e != null)
                throw e;
        }
    }
}
