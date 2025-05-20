using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Nasty.Core.Attributes;
using Nasty.PortalModule.File;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;

namespace Nasty.PortalModule.Areas.File.Controllers
{
    [Area("Portal")]
    [Route("[Area]/[controller]/[action]"), ApiExplorerSettings(GroupName = "Portal")]
    [ApiController]
    [NastyAuthorize(AuthenticationSchemes = "Bearer,Cookies")]
    public class FileController : ControllerBase
    {
        private readonly IFileService m_FileService;
        private readonly ILogger<FileController> _logger;
        private readonly IAmazonS3 m_AmazonS3;
        public FileController(ILogger<FileController> logger, IFileService p_FileService, IAmazonS3 p_AmazonS3)
        {
            _logger = logger;
            m_FileService = p_FileService;
            m_AmazonS3 = p_AmazonS3;
        }

        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile file)
        {
            var res = await m_FileService.SaveFile(file);
            if (res.IsSuccess)
            {
                return Ok(res.Data);
            }
            else
            {
                return BadRequest(res.Message);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Preview([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("文件 Key 不能为空");
            }

            try
            {
                // 生成 ETag（可以使用文件的哈希值或其他唯一标识符）
                var eTag = $"\"{key.GetHashCode()}\"";

                // 检查客户端是否发送了 If-None-Match 头
                if (Request.Headers.TryGetValue("If-None-Match", out var clientETag) && clientETag == eTag)
                {
                    // 如果 ETag 匹配，返回 304 Not Modified
                    return StatusCode(304);
                }

                var res = await m_FileService.GetFile(key);
                if (!res.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("仅支持图片格式预览");
                }

                // 设置缓存头
                Response.Headers["Cache-Control"] = "public, max-age=3600"; // 缓存 1 小时
                Response.Headers["ETag"] = eTag;
                Response.Headers["Last-Modified"] = DateTime.UtcNow.ToString("R"); // 设置最后修改时间

                return File(res.Response, res.ContentType);
            }
            catch (AmazonS3Exception ex)
            {
                return NotFound("文件不存在或无法访问");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "文件预览失败");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Download([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("文件 Key 不能为空");
            }

            try
            {
                // 调用 IFileService 获取文件流和内容类型
                var res = await m_FileService.GetFile(key);

                // 设置文件下载的 Content-Disposition 响应头
                var contentDisposition = new ContentDisposition
                {
                    FileName = key, // 使用 key 作为文件名，或根据需求自定义
                    Inline = false  // 设置为 false 表示下载而不是直接显示
                };

                Response.Headers["Content-Disposition"] = contentDisposition.ToString();

                // 返回文件流
                return File(res.Response, res.ContentType);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "文件下载失败");
                return NotFound("文件不存在或无法访问");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件下载失败");
                return StatusCode(500, "文件下载失败");
            }


        }

    }
}
