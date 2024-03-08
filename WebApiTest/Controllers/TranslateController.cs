using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApiTest.Controllers
{

    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class TranslateController : Controller
    {
        [HttpGet]
        public IActionResult Index(string from,string to, string query)
        {
            // 原文
            string q = query;
            // 源语言
            //string from = from;
            // 目标语言
            //string to = "zh";
            // 改成您的APP ID
            string appId = "20220517001218662";
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            // 改成您的密钥
            string secretKey = "xiwODo3NPGa2U0ZtAUaz";
            string sign = EncryptString(appId + q + salt + secretKey);
            string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";

            url += "q=" +   HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;


            var client = new RestClient(url);
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("content-type", "text/html;charset=UTF-8");
            var response = client.Execute(request);
            var enc = response.ContentEncoding;
            if (response is null || !response.IsSuccessful ||string.IsNullOrEmpty(response.Content))
            {
                return Ok();
            }
            var res = System.Text.RegularExpressions.Regex.Unescape(response.Content);


            //{ "from":"zh","to":"ru","trans_result":[{ "src":"发生的发色人情味儿","dst":"Происходящий человеческий запах."}]}
            return Ok(res);
        }
        // 计算MD5值
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

    }
}
