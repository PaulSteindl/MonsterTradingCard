using STATUSCODE = HTTPServerCore.Response.StatusCode;

namespace HTTPServerCore.Response.Response
{
    public class Response
    {
        public STATUSCODE.StatusCode StatusCode { get; set; }
        public string Payload { get; set; }
    }
}
