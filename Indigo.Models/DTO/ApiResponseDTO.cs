using System.Net;

namespace Indigo.Models.DTO
{
    public class ApiResponseDTO
    {

        public bool IsSuccess { get; set; } = true;

        public HttpStatusCode StatusCode { get; set; }

        public List<string> ErrorMessages { get; set; } = new List<string>();

        public object? Result { get; set; }


    }
}
