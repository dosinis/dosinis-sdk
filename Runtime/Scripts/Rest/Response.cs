using UnityEngine.Networking;

namespace DosinisSDK.Rest
{
    public class Response<T>
    {
        public readonly T resultObject;
        public readonly long code;
        public readonly UnityWebRequest.Result result;
        public readonly string error;
        
        public Response(T resultObject, long code, UnityWebRequest.Result result, string error)
        {
            this.resultObject = resultObject;
            this.code = code;
            this.result = result;
            this.error = error;
        }
    }

    public class Response
    {
        public string resultString;
        public readonly long code;
        public readonly UnityWebRequest.Result result;
        public readonly string error;
        
        public Response(string resultString, long code, UnityWebRequest.Result result, string error)
        {
            this.resultString = resultString;
            this.code = code;
            this.result = result;
            this.error = error;
        }
    }
}
