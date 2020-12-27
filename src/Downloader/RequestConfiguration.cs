﻿using System;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Downloader
{
    public class RequestConfiguration
    {
        public RequestConfiguration()
        {
            KeepAlive = false; // Please keep this in false. Because of an error (An existing connection was forcibly closed by the remote host)
            AllowAutoRedirect = true;
            AutomaticDecompression = DecompressionMethods.None;
            UserAgent = $"{nameof(Downloader)}/{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}";
            ProtocolVersion = HttpVersion.Version10;
            UseDefaultCredentials = true;
            MaximumAutomaticRedirections = 50;
            ClientCertificates = new X509CertificateCollection();
            Pipelined = true;
        }


        /// <summary>
        ///     A <see cref="System.DateTime"/> that contains the contents of the HTTP If-Modified-Since header.
        ///     The default value is the current date and time of the system.
        /// <remarks>
        ///     Note: For additional information see section 14.25 of IETF RFC 2616 - HTTP/1.1.
        /// </remarks> 
        /// </summary>
        public DateTime? IfModifiedSince { get; set; }

        /// <summary>
        ///     A <see cref="string"/> that contains the contents of the HTTP Expect header. The default value is null.
        /// <exception cref="ArgumentException">
        ///     The value specified for a set operation is "100-continue". This value is case insensitive.
        /// </exception>
        /// </summary>
        public string Expect { get; set; }

        /// <summary>
        ///     An application uses <see cref="System.Net.HttpWebRequest.KeepAlive"/> to
        ///     indicate a preference for persistent connections. When this property is true,
        ///     the application makes persistent connections to the servers that support them.
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        ///     A <see cref="Int32"/> value that indicates the maximum number of redirection responses that the current instance will follow.
        ///     The default value is implementation-specific.
        /// <exception cref="ArgumentException">
        ///     The value specified for a set operation is less than or equal to zero.
        /// </exception>
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; }

        /// <summary>
        ///     A <see cref="string"/> that identifies the media type of the current request.
        ///     The default value is null.
        /// </summary>
        /// <remarks>
        ///     The value of this property affects the <seealso cref="System.Net.HttpWebResponse.CharacterSet"/> property.
        ///     When this property is set in the current instance,
        ///     the corresponding media type is chosen from the list of
        ///     character sets returned in the response HTTP Content-type header.
        /// </remarks>
        public string MediaType { get; set; }

        /// <summary>
        ///     A <see cref="ICredentials"/> object containing the authentication
        ///     credentials associated with the current instance. The default is null.
        /// </summary>
        /// <remarks>
        ///     The <seealso cref="System.Net.HttpWebRequest.Credentials"/> property contains authentication
        ///     information to identify the client making the request. The <see cref="System.Net.HttpWebRequest.Credentials"/> property
        ///     can be either an instance of NetworkCredential, in which case the user, password,
        ///     and domain information contained in the NetworkCredential instance is used to authenticate the request,
        ///     or it can be an instance of CredentialCache, in which case the uniform resource identifier (URI)
        ///     of the request is used to determine the user, password, and domain information to use to authenticate the request.
        /// </remarks>
        public ICredentials Credentials { get; set; }

        /// <summary>
        ///     Gets or sets a Boolean value that indicates whether to send HTTP preauthentication header
        ///     information with current instance without waiting for an authentication challenge
        ///     from the requested resource.
        ///     true to send a HTTP WWW-authenticate header with the current instance
        ///     without waiting for an authentication challenge from the requested resource;
        ///     otherwise, false . The default is false .
        /// </summary>
        public bool PreAuthenticate { get; set; }

        /// <summary>
        ///     A <see cref="System.Version"/> that represents the HTTP version to use for the request.
        ///     The default is <see cref="System.Net.HttpVersion.Version10"/>.
        ///     The <seealso cref="System.Net.HttpWebRequest"/> class supports only versions 1.0 and 1.1 of HTTP.
        ///     Setting <see cref="System.Net.HttpWebRequest.ProtocolVersion"/> to a different version
        ///     causes a ArgumentException exception to be thrown.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     The HTTP version is set to a value other than 1.0 or 1.1.
        /// </exception>
        public Version ProtocolVersion { get; set; }

        /// <summary>
        ///     The <see cref="System.Net.HttpWebRequest.Proxy"/> property identifies
        ///     the WebProxy instance to use to communicate with the destination server.
        ///     To specify that no proxy should be used, set the <see cref="System.Net.HttpWebRequest"/>.
        ///     Default value is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">A set operation was requested and the specified value was null .</exception>
        /// <exception cref="InvalidOperationException">A set operation was requested but data has already been sent to the request stream.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission for the requested operation.</exception>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        ///     A <see cref="string"/> containing the value of the HTTP Referer header. The default value is null.
        ///     Note: For additional information see section 14.36 of IETF RFC 2616 - HTTP/1.1.
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        ///     When System.Net.HttpWebRequest.SendChunked is true , the request sends data to the destination in segments.
        ///     The destination server is required to support receiving chunked data. The default value is false.
        ///     Set this property to true only if the server specified by the System.Net.HttpWebRequest.
        ///     Address property of the current instance accepts chunked data (i.e. is HTTP/1.1 or greater in compliance).
        ///     If the server does not accept chunked data, buffer all data to be written and send a HTTP Content-Length header with the buffered data.
        /// </summary>
        /// <exception cref="InvalidOperationException">A set operation was requested but data has already been written to the request data stream.</exception>
        public bool SendChunked { get; set; }

        /// <summary>
        ///     A String that contains the value of the HTTP Transfer-encoding header. The default value is null.
        ///     Clearing <see cref="System.Net.HttpWebRequest.TransferEncoding"/> by setting it to null has no effect
        ///     on the value of <see cref="System.Net.HttpWebRequest.SendChunked"/>.
        ///     Values assigned to the <see cref="System.Net.HttpWebRequest.TransferEncoding"/> property replace any existing contents.
        ///     For additional information see section 14.41 of IETF RFC 2616 - HTTP/1.1.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="System.Net.HttpWebRequest.TransferEncoding"/> is set when <see cref="System.Net.HttpWebRequest.SendChunked"/> is false.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <see cref="System.Net.HttpWebRequest.TransferEncoding"/> is set to the value "Chunked". This value is case insensitive.
        /// </exception>
        public string TransferEncoding { get; set; }

        /// <summary>
        ///     An application uses this property to indicate a preference for pipelined connections.
        ///     If <see cref="System.Net.HttpWebRequest.Pipelined"/> is true ,
        ///     an application makes pipelined connections to servers that support them. The default is true.
        ///     Pipelined connections are made only when the <seealso cref="System.Net.HttpWebRequest.KeepAlive"/> property is true.
        /// </summary>
        public bool Pipelined { get; set; }

        /// <summary>
        ///     A <see cref="string"/> containing the value of the HTTP Accept header. The default value of this property is null.
        ///     Note: For additional information see section 14.1 of IETF RFC 2616 - HTTP/1.1.
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        ///     Set <see cref="System.Net.HttpWebRequest.AllowAutoRedirect"/> to true to allow the current request to automatically
        ///     follow HTTP redirection headers to the new location of a resource. Default value is true.
        ///     The maximum number of redirections to follow is set by the <see cref="System.Net.HttpWebRequest.MaximumAutomaticRedirections"/> property.
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        ///     true if the default credentials are used; otherwise, false. The default value is true.
        ///     Set this property to true when requests made by this <see cref="HttpWebRequest"/> object should,
        ///     if requested by the server, be authenticated using the credentials of the currently logged on user.
        ///     For client applications, this is the desired behavior in most scenarios. For middle-tier applications,
        ///     such as ASP.NET applications, instead of using this property, you would typically
        ///     set the <c>Credentials</c> property to the credentials of the client on whose behalf the request is made.
        /// </summary>
        /// <exception cref="InvalidOperationException">You attempted to set this property after the request was sent.</exception>
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        ///     A <see cref="string"/> containing the value of the HTTP User-agent header.
        ///     The default value is "<seealso cref="Downloader"/>/{<seealso cref="Version"/>}".
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        ///     The <see cref="X509CertificateCollection"/> that contains the security certificates associated with this request.
        /// </summary>
        /// <remarks>
        ///     The Framework caches SSL sessions as they are created and attempts to reuse a cached session for a new request, if possible.
        ///     When attempting to reuse an SSL session, the Framework uses the first element of <see cref="ClientCertificates"/> (if there is one),
        ///     or tries to reuse an anonymous sessions if <see cref="ClientCertificates"/> is empty.
        ///     For performance reasons, you shouldn't add a client certificate to a
        ///     <see cref="HttpWebRequest"/> unless you know the server will ask for it.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The value specified for a set operation is null.</exception>
        public X509CertificateCollection ClientCertificates { get; set; }

        /// <summary>
        ///     A <see cref="DecompressionMethods"/> object that indicates the type of decompression that is used.
        ///     Default value is None;
        /// </summary>
        /// <exception cref="InvalidOperationException">The object's current state does not allow this property to be set.</exception>
        public DecompressionMethods AutomaticDecompression { get; set; }
    }
}
