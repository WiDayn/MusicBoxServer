using Microsoft.AspNetCore.Mvc;

namespace MusicBoxServer.Utils
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public ApiResponse(int statusCode, T data = default, string message = null)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }
    }

    public class ApiResponseController : Controller
    {
        // 成功响应
        public IActionResult Success<T>(T data, string message = "Success")
        {
            return Ok(new ApiResponse<T>(StatusCodes.Status200OK, data, message));
        }

        // 创建资源成功响应
        public IActionResult CreatedResponse<T>(T data = default, string message = "Created")
        {
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<T>(StatusCodes.Status201Created, data, message));
        }

        // 无内容响应
        public IActionResult NoContentResponse(string message = "No Content")
        {
            return StatusCode(StatusCodes.Status204NoContent, new ApiResponse<object>(StatusCodes.Status204NoContent, null, message));
        }

        // 错误请求响应
        public IActionResult BadRequestResponse(string message = "Bad Request")
        {
            return BadRequest(new ApiResponse<object>(StatusCodes.Status400BadRequest, null, message));
        }

        // 未授权响应
        public IActionResult UnauthorizedResponse(string message = "Unauthorized")
        {
            return Ok(StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse<object>(StatusCodes.Status401Unauthorized, null, message)));
        }

        // 禁止访问响应
        public IActionResult ForbiddenResponse(string message = "Forbidden")
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse<object>(StatusCodes.Status403Forbidden, null, message));
        }

        // 未找到响应
        public IActionResult NotFoundResponse(string message = "Not Found")
        {
            return StatusCode(StatusCodes.Status404NotFound, new ApiResponse<object>(StatusCodes.Status404NotFound, null, message));
        }

        // 内部服务器错误响应
        public IActionResult InternalServerErrorResponse(string message = "Internal Server Error")
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>(StatusCodes.Status500InternalServerError, null, message));
        }

        // 自定义响应
        public IActionResult CustomResponse(int statusCode, string message)
        {
            return StatusCode(statusCode, new ApiResponse<object>(statusCode, null, message));
        }
    }
}
