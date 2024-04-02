using UnityEngine.Networking;

namespace DosinisSDK.Rest
{
    public enum Result
    {
        /// <summary>
        ///   <para>The request hasn't finished yet.</para>
        /// </summary>
        InProgress,
        /// <summary>
        ///   <para>The request succeeded.</para>
        /// </summary>
        Success,
        /// <summary>
        ///   <para>Failed to communicate with the server. For example, the request couldn't connect or it could not establish a secure channel.</para>
        /// </summary>
        ConnectionError,
        /// <summary>
        ///   <para>The server returned an error response. The request succeeded in communicating with the server, but received an error as defined by the connection protocol.</para>
        /// </summary>
        ProtocolError,
        /// <summary>
        ///   <para>Error processing data. The request succeeded in communicating with the server, but encountered an error when processing the received data. For example, the data was corrupted or not in the correct format.</para>
        /// </summary>
        DataProcessingError,
    }
    
    public class Response<T>
    {
        public readonly T resultObject;
        public readonly long code;
        public readonly Result result;
        public readonly string error;
        
        public Response(T resultObject, long code, Result result, string error)
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
        public readonly Result result;
        public readonly string error;
        
        public Response(string resultString, long code, Result result, string error)
        {
            this.resultString = resultString;
            this.code = code;
            this.result = result;
            this.error = error;
        }
    }
}
