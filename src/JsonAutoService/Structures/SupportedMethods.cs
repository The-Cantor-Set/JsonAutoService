using Microsoft.AspNetCore.Http;

namespace JsonAutoService.Structures
{
    public struct SupportedMethods
    {
        public static readonly string PUT = HttpMethods.Put;
        public static readonly string POST = HttpMethods.Post;
        public static readonly string HEAD = HttpMethods.Head;
        public static readonly string GET = HttpMethods.Get;
        public static readonly string DELETE = HttpMethods.Delete;
    }
}
