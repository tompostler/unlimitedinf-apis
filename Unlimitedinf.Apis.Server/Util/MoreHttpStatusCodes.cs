using System.Net;

namespace Unlimitedinf.Apis.Server.Util
{
    public static class MoreHttpStatusCodes
    {
        public static readonly MoreHttpStatusCodeValue ImATeapot = new MoreHttpStatusCodeValue(418);
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

        public static implicit operator int(MoreHttpStatusCodeValue toConvert)
        {
            return toConvert.statusCode;
        }
    }
}