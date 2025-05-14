using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Nasty.Common.Config;
using Nasty.Common.LoadParams;
using Nasty.Common.Registry;

namespace Nasty.PortalModule.File
{
    public interface IFileService : IAutofacRegister
    {
        public Task<ResultData<string>> SaveFile(IFormFile file);
        public Task<(Stream Response, string ContentType)> GetFile(string key);
    }

    public class FileService : IFileService
    {
        public required IAmazonS3 S3Client { get; set; }

        public async Task<(Stream Response, string ContentType)> GetFile(string key)
        {
            // 从 S3 获取文件
            var response = await S3Client.GetObjectAsync(SuperConfig.GetBucketName(), key);

            // 返回文件类型
            var contentType = response.Headers["Content-Type"];

            // 返回文件流作
            return (response.ResponseStream, contentType);
        }

        public async Task<ResultData<string>> SaveFile(IFormFile file)
        {
            var result = new ResultData<string>();
            if (file == null || file.Length == 0)
            {
                result.IsSuccess = false;
                result.Message = "文件不能为空";
                return result;
            }

            try
            {
                // 生成文件的唯一 Key（可以使用 Guid 或其他逻辑）
                var fileId = Guid.NewGuid().ToString("N");
                var fileKey = $"{fileId}_{file.FileName}";

                // 使用 TransferUtility 上传文件
                using (var transferUtility = new TransferUtility(S3Client))
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = fileStream,
                            Key = fileKey,
                            BucketName = SuperConfig.GetBucketName(),
                            ContentType = file.ContentType
                        };

                        await transferUtility.UploadAsync(uploadRequest);
                    }
                }

                result.IsSuccess = true;
                result.Data = fileKey;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }


    }
}
