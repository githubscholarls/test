
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Polly;
using System.Net;
using WebApiTest.Utility;

namespace WebApiTest.Controllers
{

    public class ApiResponse<T>
    {
        public T data { get; set; }
        public int code { get; set; } = 20000;


        public static ApiResponse<T> Success(T t)
        {
            return new ApiResponse<T>()
            {
                data = t,
                code = 20000
            };
        }

        public static ApiResponse<T> Dialog(T t)
        {
            return new ApiResponse<T>()
            {
                data = t,
                code = 101
            };
        }
    }



    [Route("vue-admin-template/user/[action]")]
    public class VueAdminUserTemplate : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult login()
        {
            //return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(new { username = "lishuai", code = 20000 }));
            return new JsonResult(new { data = new { name = "lishuai", token = "this is token" }, code = 20000 });
        }
        [HttpGet]
        public IActionResult info()
        {
            //return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(new { username = "lishuai", code = 20000 }));
            return new JsonResult(new { data = new { name = "lishuai", token = "this is token" }, code = 20000 });
        }
    }


    [Route("vue-admin-template/table/[action]")]
    public class VueAdminTableTemplate : ControllerBase
    {

        private readonly ILogger<VueAdminTableTemplate> logger;
        public VueAdminTableTemplate(ILogger<VueAdminTableTemplate> logger)
        {
            this.logger = logger;
        }

        public static Guid LastId = Guid.Empty;
        public static List<ListItem> items = new List<ListItem>() { new ListItem() { id = Guid.NewGuid(), title = "标题1", author = "上作者" }, new ListItem() { id = Guid.NewGuid(), title = "第一本书", author = "刘建伟" }, new ListItem() { id = Guid.NewGuid(), title = "发顺丰", author = "士大夫", state = 1 } };


        [HttpPost]
        public async Task<ApiResponse<ListData>> list([FromBody] listdto dto)
        {
            var list = items.ToList();
            if (dto.state is not null && dto.state.Count > 0)
            {
                list = list.Where(l => dto.state.Contains(l.state)).ToList();
            }
            return ApiResponse<ListData>.Success(new ListData() { items = list });

        }
        public class listdto
        {
            public List<int>? state { get; set; }
        }

        [HttpPost]
        public ApiResponse<string> ModifyId([FromBody] ListItem item)
        {
            var modify = items.Where(i => i.id == item.id).FirstOrDefault();
            if (modify is null)
            {
                return ApiResponse<string>.Success("不存在");
            }
            modify.title = item.title;
            modify.author = item.author;
            modify.files = item.files;
            items.RemoveAll(i => i.id == item.id);
            items.Add(modify);

            return ApiResponse<string>.Success("修改成功");
        }

        [HttpPost]
        public ApiResponse<string> SetId(Guid id)
        {
            LastId = id;
            return ApiResponse<string>.Success("成功");
        }

        [HttpGet]
        public ApiResponse<Guid> GetId()
        {
            return ApiResponse<Guid>.Success(LastId);
        }

        [HttpPost]
        public ApiResponse<string> DelId(Guid id, bool confirmDelete = false)
        {
            if (!confirmDelete)
            {
                return ApiResponse<string>.Dialog($"请问您确定删除 标题为 {items.Where(i => i.id == id).FirstOrDefault()?.title ?? ""} 的作品吗 ");
            }
            items.RemoveAll(i => i.id == id);
            return ApiResponse<string>.Success("删除成功");
        }

        [HttpPost]
        public ApiResponse<string> Upload2(List<IFormFile> file, Guid id)
        {

            var modify = items.Where(i => i.id == id).FirstOrDefault();
            if (modify is null)
            {
                return ApiResponse<string>.Success("不存在");
            }
            var basePath = Environment.CurrentDirectory;
            var paths = modify.files;
            foreach (var fileItem in file)
            {
                var curPath = Path.Combine(basePath, "wwwroot", fileItem.FileName);
                paths.Add(curPath);
                //创建文件
                using var stream = System.IO.File.Create(curPath);
                fileItem.CopyTo(stream);
            }
            modify.files = paths.Distinct().ToList();
            items.RemoveAll(i => i.id == id);
            items.Add(modify);

            return ApiResponse<string>.Success("上传成功");
        }
        [HttpPost]
        public ApiResponse<List<string>> Upload(List<IFormFile> file)
        {
            var basePath = Environment.CurrentDirectory;
            var paths = new List<string>();
            foreach (var fileItem in file)
            {
                var curPath = Path.Combine(basePath, "wwwroot", fileItem.FileName);
                paths.Add(fileItem.FileName);
                //创建文件
                using var stream = System.IO.File.Create(curPath);
                fileItem.CopyTo(stream);
            }

            return ApiResponse<List<string>>.Success(paths);
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadBigAsync(UploadBigDto dto)
        //{
        //    if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        //    {
        //        ModelState.AddModelError("File",
        //            $"The request couldn't be processed (Error 1).");
        //        // Log error

        //        return BadRequest(ModelState);
        //    }
        //    try
        //    {
        //        //获取参数                    
        //        if (dto.uid.Length == 0 || dto.type.Length == 0)
        //        {
        //            logger.LogError("无效参数");
        //            return BadRequest();
        //        }


        //        //获取上传的文件分片
        //        var file = HttpContext.Request.Form.Files.FirstOrDefault();
        //        if (file == null || file.Length == 0)
        //        {
        //            logger.LogError($"没有文件数据");
        //            return BadRequest();
        //        }

        //        //后缀验证
        //        var ext = Path.GetExtension(file.FileName);
        //        var temp = Path.Combine(dto.physicalPath, "tmp");
        //        if (!Directory.Exists(temp))
        //        {
        //            Directory.CreateDirectory(temp);
        //        }
        //        var chunkFilename = Path.Combine(physicalPath, temporaryFolder, "uploader-" + identifier + "." + chunkNumber);
        //        try
        //        {
        //            using (var fileStream = File.OpenWrite(chunkFilename))
        //            {
        //                var stream = file.OpenReadStream();
        //                stream.CopyTo(fileStream);
        //                fileStream.Flush(true);
        //                countDict.AddOrUpdate(identifier, 1, (key, oldValue) => oldValue + 1);
        //            }

        //            if (chunkNumber == totalChunks)
        //            {
        //                //验证块的完整性
        //                while (true)
        //                {
        //                    if (countDict.GetValueOrDefault(identifier) < totalChunks)
        //                    {
        //                        await Task.Delay(TimeSpan.FromMilliseconds(500));
        //                    }
        //                    else
        //                    {
        //                        countDict.Remove(identifier, out _);
        //                        break;
        //                    }
        //                }

        //                //merge file;
        //                string[] chunkFiles = Directory.GetFiles(
        //                    Path.Combine(_config.PhysicalPath, temporaryFolder),
        //                    "uploader-" + identifier + ".*",
        //                    SearchOption.TopDirectoryOnly);
        //                //var fileUrl = await MergeChunkFiles(payload, ext, chunkFiles); 
        //                var fileUrl = await MergeChunkFiles(context, uid, type, ext, chunkFiles, filename, totalSize);

        //                //await context.Response.WriteAsync(new ApiResponse<string>()
        //                //{
        //                //    result = 200,
        //                //    msg = "请求成功",
        //                //    data = fileUrl
        //                //}.toJson());
        //                var Json = new ApiResponse<Files>(200, "请求成功", fileUrl).toJson();
        //                await context.Response.WriteAsync(Json);
        //            }
        //            else
        //            {
        //                return Ok("片上传完成");
        //            }
        //        }
        //        catch (Exception exp)
        //        {
        //            logger.LogError(exp.ToString());
        //            return BadRequest();
        //        }

        //    }
        //    catch { }

        //}

        // getParams(context, out var chunkNumber, out var chunkSize, out var totalSize, out string identifier,
        //out string filename, out int totalChunks, out string uid, out string type);
        public class UploadBigDto
        {
            public int chunkNumber { get; set; }
            public int chunkSize { get; set; }
            public int totalSize { get; set; }
            public string identifier { get; set; }

            public string filename { get; set; }
            public int totalChunks { get; set; }
            public string uid { get; set; }
            public string type { get; set; }
        }

        public class ListData
        {
            public List<ListItem> items { get; set; } = new();
        }

        public class ListItem
        {

            public Guid id { get; set; }
            public string title { get; set; }

            public string author { get; set; }

            public List<string> files { get; set; } = new();

            public int state { get; set; } = 0;
        }
    }


}
