using System.Collections.Generic;
using Unlimitedinf.Apis.Models.Axioms;

namespace Unlimitedinf.Apis.AxiomsUpsert
{
    static class Axioms
    {
        static Axioms()
        {
            Http = new List<HttpAxiomEntity>
            {
                new HttpAxiomEntity
                {
                    Id = "100",
                    Summary = "Continue",
                    Description = "The server has received the request headers and the client should proceed to send the request body (in the case of a request for which a body needs to be sent; for example, a POST request). Sending a large request body to a server after a request has been rejected for inappropriate headers would be inefficient. To have a server check the request's headers, a client must send Expect: 100-continue as a header in its initial request and receive a 100 Continue status code in response before sending the body. The response 417 Expectation Failed indicates the request should not be continued."
                },
                new HttpAxiomEntity
                {
                    Id = "101",
                    Summary = "Switching Protocols",
                    Description = "The requester has asked the server to switch protocols and the server has agreed to do so."
                },
                new HttpAxiomEntity
                {
                    Id = "102",
                    Summary = "Processing (WebDAV)",
                    Description = "A WebDAV request may contain many sub-requests involving file operations, requiring a long time to complete the request. This code indicates that the server has received and is processing the request, but no response is available yet. This prevents the client from timing out and assuming the request was lost."
                },
                new HttpAxiomEntity
                {
                    Id = "200",
                    Summary = "OK",
                    Description = "Standard response for successful HTTP requests. The actual response will depend on the request method used. In a GET request, the response will contain an entity corresponding to the requested resource. In a POST request, the response will contain an entity describing or containing the result of the action."
                },
                new HttpAxiomEntity
                {
                    Id = "201",
                    Summary = "Created",
                    Description = "The request has been fulfilled, resulting in the creation of a new resource."
                },
                new HttpAxiomEntity
                {
                    Id = "202",
                    Summary = "Accepted",
                    Description = "The request has been accepted for processing, but the processing has not been completed. The request might or might not be eventually acted upon, and may be disallowed when processing occurs."
                },
                new HttpAxiomEntity
                {
                    Id = "203",
                    Summary = "Non-Authoritative Information",
                    Description = "The server is a transforming proxy (e.g. a Web accelerator) that received a 200 OK from its origin, but is returning a modified version of the origin's response."
                },
                new HttpAxiomEntity
                {
                    Id = "204",
                    Summary = "No content",
                    Description = "The server successfully processed the request and is not returning any content."
                },
                new HttpAxiomEntity
                {
                    Id = "205",
                    Summary = "Reset Content",
                    Description = "The server successfully processed the request, but is not returning any content. Unlike a 204 response, this response requires that the requester reset the document view."
                },
                new HttpAxiomEntity
                {
                    Id = "206",
                    Summary = "Partial Cotent",
                    Description = "The server is delivering only part of the resource (byte serving) due to a range header sent by the client. The range header is used by HTTP clients to enable resuming of interrupted downloads, or split a download into multiple simultaneous streams."
                },
                new HttpAxiomEntity
                {
                    Id = "207",
                    Summary = "Multi-Status (WebDAV)",
                    Description = "The message body that follows is an XML message and can contain a number of separate response codes, depending on how many sub-requests were made."
                },
                new HttpAxiomEntity
                {
                    Id = "208",
                    Summary = "Already Reported (WebDAV)",
                    Description = "The members of a DAV binding have already been enumerated in a preceding part of the (multistatus) response, and are not being included again."
                },
                new HttpAxiomEntity
                {
                    Id = "226",
                    Summary = "IM Used",
                    Description = "The server has fulfilled a request for the resource, and the response is a representation of the result of one or more instance-manipulations applied to the current instance."
                },
                new HttpAxiomEntity
                {
                    Id = "300",
                    Summary = "Multiple Choices",
                    Description = "Indicates multiple options for the resource from which the client may choose (via agent-driven content negotiation). For example, this code could be used to present multiple video format options, to list files with different filename extensions, or to suggest word-sense disambiguation."
                },
                new HttpAxiomEntity
                {
                    Id = "301",
                    Summary = "Moved Permanently",
                    Description = "This and all future requests should be directed to the given URI."
                },
                new HttpAxiomEntity
                {
                    Id = "302",
                    Summary = "Found",
                    Description = "This is an example of industry practice contradicting the standard. The HTTP/1.0 specification (RFC 1945) required the client to perform a temporary redirect (the original describing phrase was \"Moved Temporarily\"), but popular browsers implemented 302 with the functionality of a 303 See Other. Therefore, HTTP/1.1 added status codes 303 and 307 to distinguish between the two behaviours. However, some Web applications and frameworks use the 302 status code as if it were the 303."
                },
                new HttpAxiomEntity
                {
                    Id = "303",
                    Summary = "See Other",
                    Description = "The response to the request can be found under another URI using a GET method. When received in response to a POST (or PUT/DELETE), the client should presume that the server has received the data and should issue a redirect with a separate GET message."
                },
                new HttpAxiomEntity
                {
                    Id = "304",
                    Summary = "Not Modified",
                    Description = "Indicates that the resource has not been modified since the version specified by the request headers If-Modified-Since or If-None-Match. In such case, there is no need to retransmit the resource since the client still has a previously-downloaded copy."
                },
                new HttpAxiomEntity
                {
                    Id = "305",
                    Summary = "Use Proxy",
                    Description = "The requested resource is available only through a proxy, the address for which is provided in the response. Many HTTP clients (such as Mozilla and Internet Explorer) do not correctly handle responses with this status code, primarily for security reasons."
                },
                new HttpAxiomEntity
                {
                    Id = "306",
                    Summary = "Switch Proxy",
                    Description = "No longer used. Originally meant \"Subsequent requests should use the specified proxy.\""
                },
                new HttpAxiomEntity
                {
                    Id = "307",
                    Summary = "Temporary Redirect",
                    Description = "In this case, the request should be repeated with another URI; however, future requests should still use the original URI. In contrast to how 302 was historically implemented, the request method is not allowed to be changed when reissuing the original request. For example, a POST request should be repeated using another POST request."
                },
                new HttpAxiomEntity
                {
                    Id = "308",
                    Summary = "Permanent Redirect",
                    Description = "The request and all future requests should be repeated using another URI. 307 and 308 parallel the behaviors of 302 and 301, but do not allow the HTTP method to change. So, for example, submitting a form to a permanently redirected resource may continue smoothly."
                },
                new HttpAxiomEntity
                {
                    Id = "400",
                    Summary = "Bad Request",
                    Description = "The server cannot or will not process the request due to an apparent client error (e.g., malformed request syntax, size too large, invalid request message framing, or deceptive request routing)."
                },
                new HttpAxiomEntity
                {
                    Id = "401",
                    Summary = "Unauthorized",
                    Description = "Similar to 403 Forbidden, but specifically for use when authentication is required and has failed or has not yet been provided. The response must include a WWW-Authenticate header field containing a challenge applicable to the requested resource. See Basic access authentication and Digest access authentication. 401 semantically means \"unauthenticated\", i.e. the user does not have the necessary credentials."
                },
                new HttpAxiomEntity
                {
                    Id = "402",
                    Summary = "Payment Required",
                    Description = "Reserved for future use. The original intention was that this code might be used as part of some form of digital cash or micropayment scheme, as proposed for example by GNU Taler, but that has not yet happened, and this code is not usually used. Google Developers API uses this status if a particular developer has exceeded the daily limit on requests."
                },
                new HttpAxiomEntity
                {
                    Id = "403",
                    Summary = "Forbidden",
                    Description = "The request was valid, but the server is refusing action. The user might not have the necessary permissions for a resource, or may need an account of some sort."
                },
                new HttpAxiomEntity
                {
                    Id = "404",
                    Summary = "Not Found",
                    Description = "The requested resource could not be found but may be available in the future. Subsequent requests by the client are permissible."
                },
                new HttpAxiomEntity
                {
                    Id = "405",
                    Summary = "Method Not Allowed",
                    Description = "A request method is not supported for the requested resource; for example, a GET request on a form that requires data to be presented via POST, or a PUT request on a read-only resource."
                },
                new HttpAxiomEntity
                {
                    Id = "406",
                    Summary = "Not Acceptable",
                    Description = "The requested resource is capable of generating only content not acceptable according to the Accept headers sent in the request."
                },
                new HttpAxiomEntity
                {
                    Id = "407",
                    Summary = "Proxy Authentication Required",
                    Description = "The client must first authenticate itself with the proxy."
                },
                new HttpAxiomEntity
                {
                    Id = "408",
                    Summary = "Request Timeout",
                    Description = "The server timed out waiting for the request. According to HTTP specifications: \"The client did not produce a request within the time that the server was prepared to wait. The client MAY repeat the request without modifications at any later time.\""
                },
                new HttpAxiomEntity
                {
                    Id = "409",
                    Summary = "Conflict",
                    Description = "Indicates that the request could not be processed because of conflict in the request, such as an edit conflict between multiple simultaneous updates."
                },
                new HttpAxiomEntity
                {
                    Id = "410",
                    Summary = "Gone",
                    Description = "Indicates that the resource requested is no longer available and will not be available again. This should be used when a resource has been intentionally removed and the resource should be purged. Upon receiving a 410 status code, the client should not request the resource in the future. Clients such as search engines should remove the resource from their indices. Most use cases do not require clients and search engines to purge the resource, and a \"404 Not Found\" may be used instead."
                },
                new HttpAxiomEntity
                {
                    Id = "411",
                    Summary = "Length Required",
                    Description = "The request did not specify the length of its content, which is required by the requested resource."
                },
                new HttpAxiomEntity
                {
                    Id = "412",
                    Summary = "Precondition Failed",
                    Description = "The server does not meet one of the preconditions that the requester put on the request."
                },
                new HttpAxiomEntity
                {
                    Id = "413",
                    Summary = "Payload Too Large",
                    Description = "The request is larger than the server is willing or able to process. Previously called \"Request Entity Too Large\"."
                },
                new HttpAxiomEntity
                {
                    Id = "414",
                    Summary = "URI Too Long",
                    Description = "The URI provided was too long for the server to process. Often the result of too much data being encoded as a query-string of a GET request, in which case it should be converted to a POST request. Called \"Request-URI Too Long\" previously."
                },
                new HttpAxiomEntity
                {
                    Id = "415",
                    Summary = "Unsupported Media Type",
                    Description = "The request entity has a media type which the server or resource does not support. For example, the client uploads an image as image/svg+xml, but the server requires that images use a different format."
                },
                new HttpAxiomEntity
                {
                    Id = "416",
                    Summary = "Range Not Satisfiable",
                    Description = "The client has asked for a portion of the file (byte serving), but the server cannot supply that portion. For example, if the client asked for a part of the file that lies beyond the end of the file. Called \"Requested Range Not Satisfiable\" previously."
                },
                new HttpAxiomEntity
                {
                    Id = "417",
                    Summary = "Expectation Failed",
                    Description = "The server cannot meet the requirements of the Expect request-header field."
                },
                new HttpAxiomEntity
                {
                    Id = "418",
                    Summary = "I'm a teapot",
                    Description = "his code was defined in 1998 as one of the traditional IETF April Fools' jokes, in RFC 2324, Hyper Text Coffee Pot Control Protocol, and is not expected to be implemented by actual HTTP servers. The RFC specifies this code should be returned by teapots requested to brew coffee. This HTTP status is used as an Easter egg in some websites, including Google.com."
                },
                new HttpAxiomEntity
                {
                    Id = "421",
                    Summary = "Misdirected Request",
                    Description = "The request was directed at a server that is not able to produce a response. (for example because of a connection reuse)"
                },
                new HttpAxiomEntity
                {
                    Id = "422",
                    Summary = "Unprocessable Entity (WebDAV)",
                    Description = "The request was well-formed but was unable to be followed due to semantic errors."
                },
                new HttpAxiomEntity
                {
                    Id = "423",
                    Summary = "Locked (WebDAV)",
                    Description = "The resource that is being accessed is locked."
                },
                new HttpAxiomEntity
                {
                    Id = "424",
                    Summary = "Failed Dependency (WebDAV)",
                    Description = "he request failed due to failure of a previous request (e.g., a PROPPATCH)."
                },
                new HttpAxiomEntity
                {
                    Id = "426",
                    Summary = "Upgrade Required",
                    Description = "The client should switch to a different protocol such as TLS/1.0, given in the Upgrade header field."
                },
                new HttpAxiomEntity
                {
                    Id = "428",
                    Summary = "Precondition Required",
                    Description = "The origin server requires the request to be conditional. Intended to prevent the 'lost update' problem, where a client GETs a resource's state, modifies it, and PUTs it back to the server, when meanwhile a third party has modified the state on the server, leading to a conflict."
                },
                new HttpAxiomEntity
                {
                    Id = "429",
                    Summary = "Too Many Requests",
                    Description = "The user has sent too many requests in a given amount of time. Intended for use with rate-limiting schemes."
                },
                new HttpAxiomEntity
                {
                    Id = "431",
                    Summary = "Request Head Fields Too Large",
                    Description = "The server is unwilling to process the request because either an individual header field, or all the header fields collectively, are too large."
                },
                new HttpAxiomEntity
                {
                    Id = "431",
                    Summary = "Unavailable For Legal Reasons",
                    Description = "A server operator has received a legal demand to deny access to a resource or to a set of resources that includes the requested resource. The code 451 was chosen as a reference to the novel Fahrenheit 451."
                },
                new HttpAxiomEntity
                {
                    Id = "500",
                    Summary = "Internal Server Error",
                    Description = "A generic error message, given when an unexpected condition was encountered and no more specific message is suitable."
                },
                new HttpAxiomEntity
                {
                    Id = "501",
                    Summary = "Not Implemented",
                    Description = "The server either does not recognize the request method, or it lacks the ability to fulfil the request. Usually this implies future availability (e.g., a new feature of a web-service API)."
                },
                new HttpAxiomEntity
                {
                    Id = "502",
                    Summary = "Bad Gateway",
                    Description = "The server was acting as a gateway or proxy and received an invalid response from the upstream server."
                },
                new HttpAxiomEntity
                {
                    Id = "503",
                    Summary = "Service Unavailable",
                    Description = "The server is currently unavailable (because it is overloaded or down for maintenance). Generally, this is a temporary state."
                },
                new HttpAxiomEntity
                {
                    Id = "504",
                    Summary = "Gateway Timeout",
                    Description = "The server was acting as a gateway or proxy and did not receive a timely response from the upstream server."
                },
                new HttpAxiomEntity
                {
                    Id = "505",
                    Summary = "HTTP Version Not Supported",
                    Description = "The server does not support the HTTP protocol version used in the request."
                },
                new HttpAxiomEntity
                {
                    Id = "506",
                    Summary = "Variant Also Negotiates",
                    Description = "Transparent content negotiation for the request results in a circular reference."
                },
                new HttpAxiomEntity
                {
                    Id = "507",
                    Summary = "Insufficient Storage (WebDAV)",
                    Description = "The server is unable to store the representation needed to complete the request."
                },
                new HttpAxiomEntity
                {
                    Id = "508",
                    Summary = "Loop Detected (WebDAV)",
                    Description = "The server detected an infinite loop while processing the request (sent in lieu of 208 Already Reported)."
                },
                new HttpAxiomEntity
                {
                    Id = "510",
                    Summary = "Not Extended",
                    Description = "Further extensions to the request are required for the server to fulfil it."
                },
                new HttpAxiomEntity
                {
                    Id = "511",
                    Summary = "Network Authentication Required",
                    Description = "The client needs to authenticate to gain network access. Intended for use by intercepting proxies used to control access to the network (e.g., \"captive portals\" used to require agreement to Terms of Service before granting full Internet access via a Wi-Fi hotspot)."
                },
                //new HttpAxiomEntity
                //{
                //    Id = "",
                //    Summary = "",
                //    Description = ""
                //},
            };

            // apply shared parameters for Http
            foreach (var httpAxiom in Http)
                httpAxiom.Source = "https://en.wikipedia.org/wiki/List_of_HTTP_status_codes";
        }

        public static readonly List<HttpAxiomEntity> Http;
    }
}
