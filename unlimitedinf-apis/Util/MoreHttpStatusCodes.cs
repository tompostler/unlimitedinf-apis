using System.Net;

namespace Unlimitedinf.Apis.Util
{
    public static class MoreHttpStatusCodes
    {
        public static readonly MoreHttpStatusCodeValue ImATeapot = new MoreHttpStatusCodeValue(418);
        public static readonly MoreHttpStatusCodeValue NginxHttpRequestSentToHttpsPort = new MoreHttpStatusCodeValue(497);
    }

    public class MoreHttpStatusCodeValue
    {
        private int statusCode;
        public MoreHttpStatusCodeValue(int statusCode)
        {
            this.statusCode = statusCode;
        }

        public static implicit operator HttpStatusCode(MoreHttpStatusCodeValue toConvert)
        {
            return (HttpStatusCode)toConvert.statusCode;
        }
    }
}