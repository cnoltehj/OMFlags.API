using System.Net;

namespace OMFlags.API.Models.Common
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Response = new ResponseAndCode();
            Message = new MessageAndID();
        }

        public MessageAndID Message { get; set; }
        public ResponseAndCode Response { get; set; }

        public bool Succeeded()
        {
            return Response.Code == 0;
        }

        public bool Failed()
        {
            return Response.Code != 0;
        }
    }

    public class StatusCodeAndMessage
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
    }
}
