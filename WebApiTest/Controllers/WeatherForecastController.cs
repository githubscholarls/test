using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebApiTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("[action]")]
        public Object GetWTCodeMenuXml()
        {
            var s = "》》物流公司\r\n/member/UpdateCompInfo.aspx\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageNotice.aspx\r\n/member/ManageNoticewithWLLine.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendWuLiuCity.aspx\r\n/member/ManageWuLiuCity.aspx\r\n/member/SendWlLine.aspx\r\n/member/ManageWlLeasedLine.aspx\r\n/member/ManageBiddingLines.aspx\r\n/member/WLLinesGreenChannel.aspx\r\n/member/ManageGnCargo.aspx\r\n/member/ManageBinddingHuo.aspx\r\n/member/ManageInterWlHuoPan.aspx\r\n/member/FaHuoQiYeKu.aspx\r\n/member/CreateWayBill.aspx\r\n/member/ManageWayBill.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n/member/SendAgent.aspx\r\n/member/ManageAgent.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n》》配货信息部\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageNotice.aspx\r\n/member/ManageNoticewithWLLine.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendWlLine.aspx\r\n/member/ManageWlLeasedLine.aspx\r\n/member/WLLinesGreenChannel.aspx\r\n/member/ManageBiddingLines.aspx\r\n/member/ManageSearchcheline.aspx\r\n/member/ManageSearchhuo.aspx\r\n/member/SendCarLinePeiHuoBu.aspx\r\n/member/ManageCarLine.aspx\r\n/member/ManageBiddingCheLines.aspx\r\n/member/ManageGnCargo.aspx\r\n/member/ManageBinddingHuo.aspx\r\n/member/ManageInterWlHuoPan.aspx\r\n/member/FaHuoQiYeKu.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n/member/SendAgent.aspx\r\n/member/ManageAgent.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n》》车主\r\n/member/UpdateCheZhuInfo.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/FaHuoQiYeKu.aspx\r\n/member/CustomerCompany.aspx\r\n/member/SendCarLineCheZhuNew.aspx\r\n/member/ManageCarLineCheZhu.aspx\r\n/member/CheDui.aspx\r\n/member/SendCityCarLine.aspx\r\n/member/ManageCityCarLine.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n/member/SendAgent.aspx\r\n/member/ManageAgent.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n》》货源提供商\r\n/member/UpdateHuoZhuInfo.aspx\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdateRunArea.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageContacts.aspx\r\n/member/ManageCommonGoods.aspx\r\n/member/CustomerCar.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageGnCargo.aspx\r\n/member/ManageBinddingHuo.aspx\r\n/member/ManageInterWlHuoPan.aspx\r\n/member/SendExpress.aspx\r\n/member/ManageExpress.aspx\r\n/member/SendBanJiaInfo.aspx\r\n/member/ManageBanJiaInfo.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n/member/SendAgent.aspx\r\n/member/ManageAgent.aspx\r\n国际物流\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageNotice.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendInerLineZX.aspx\r\n/member/ManageInterLine.aspx\r\n/member/ManageInterWlHuoPan.aspx\r\n/member/ManageGnCargo.aspx\r\n/member/SendInterCangChu.aspx\r\n/member/ManageInterCangChu.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n/member/SendAgent.aspx\r\n/member/ManageAgent.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n》》快递公司\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageNotice.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendkjCity.aspx\r\n/member/Managekjcity.aspx\r\n/member/SendkjLeasedline.aspx\r\n/member/ManagekjLeasedline.aspx\r\n/member/ManageBinddingKuaidi.aspx\r\n/member/sendcarline.aspx\r\n/member/ManageCarLine.aspx\r\n/member/managecar.aspx\r\n/member/ManageGnCargo.aspx\r\n/member/ManageInterWlHuoPan.aspx\r\n/member/SendExpress.aspx\r\n/member/ManageExpress.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n》》搬家公司\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageNotice.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendBanJiaSite.aspx\r\n/member/ManageBanJiaSite.aspx\r\n/member/ManageBanJiaJingJia.aspx\r\n/member/SendBanJiaInfo.aspx\r\n/member/ManageBanJiaInfo.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n/member/SendAgent.aspx\r\n/member/ManageAgent.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n》》物流设备\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/UpdateRunArea.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendSupply.aspx\r\n/member/ManageSupply.aspx\r\n/member/SendBuy.aspx\r\n/member/ManageBuy.aspx\r\n/member/ManageGnCargo.aspx\r\n/member/ManageBinddingHuo.aspx\r\n/member/ManageInterWlHuoPan.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n/member/SendBizInfo.aspx\r\n/member/ManageBizInfo.aspx\r\n》》物流园区\r\n/member/UpdateCompInfo.aspx\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageNotice.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendLPMerchants.aspx\r\n/member/ManageLPMerchants.aspx\r\n/member/SendLPHouseRent.aspx\r\n/member/ManageLPHouseRent.aspx\r\n/member/SendLPWearHouse.aspx\r\n/member/ManageLPWearHouse.aspx\r\n/member/SendLPAutoRepair.aspx\r\n/member/ManageLPAutoRepair.aspx\r\n/member/SendLPCarPark.aspx\r\n/member/ManageLPCarPark.aspx\r\n/member/SendLPCatering.aspx\r\n/member/ManageLPCatering.aspx\r\n/member/SendLPHotel.aspx\r\n/member/ManageLPHotel.aspx\r\n/member/SendLPLogisticsCom.aspx\r\n/member/ManageLPLogisticsCom.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n》》停车场\r\n/member/UpdateCompInfo.aspx\r\n/member/RealnameAuthentication.aspx\r\n/member/UpdatePassword.aspx\r\n/member/BindingWechatAccount.aspx\r\n/member/ManageComPicture.aspx\r\n/member/SelectWebTemplet.aspx\r\n/member/ManageDecorate.aspx\r\n/member/MemberInvite.aspx\r\n/member/ManageNotice.aspx\r\n/member/CustomerCar.aspx\r\n/member/SendLPMerchants.aspx\r\n/member/ManageLPMerchants.aspx\r\n/member/SendLPHouseRent.aspx\r\n/member/ManageLPHouseRent.aspx\r\n/member/SendLPWearHouse.aspx\r\n/member/ManageLPWearHouse.aspx\r\n/member/SendLPAutoRepair.aspx\r\n/member/ManageLPAutoRepair.aspx\r\n/member/SendLPCarPark.aspx\r\n/member/ManageLPCarPark.aspx\r\n/member/SendLPCatering.aspx\r\n/member/ManageLPCatering.aspx\r\n/member/SendLPHotel.aspx\r\n/member/ManageLPHotel.aspx\r\n/member/SendLPLogisticsCom.aspx\r\n/member/ManageLPLogisticsCom.aspx\r\n/member/ManageReceipt.aspx\r\n/member/ManageEvaluate.aspx\r\n/member/ManageReceipt.aspx\r\n/member/QueryAccountCaiwu.aspx\r\n/member/WutongPay.aspx\r\n/member/ManageFx.aspx\r\n/member/LuckyDrawRecord.aspx\r\n/member/ManageBack.aspx\r\n/member/WutongPay.aspx\r\n/member/SendWliuZbiao.aspx\r\n/member/ManageWliuZbiao.aspx\r\n";
            var urls = s.Split("\r\n").ToList();

            Dictionary<string, List<string>> dic = new();
            int lastIndex = 0;
            var curKey = "";
            foreach (var url in urls)
            {
                if (url.StartsWith("》》"))
                {
                    curKey=url.Substring(2);
                    dic.Add(url.Substring(2), new());
                }
                if (!ContainerItem(dic, url))
                {
                    dic[curKey].Add(url);
                }
            }

            StringBuilder res = new();
            foreach (var key in dic.Keys)
            {
                foreach (var it in dic[key])
                {
                    Console.WriteLine(it);
                    res.Append(it);
                }
            }

            return dic;
        }

        private bool ContainerItem(Dictionary<string, List<string>> dic,string item) 
        {
            foreach (var key in dic.Keys)
            {
                if (dic[key].Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}