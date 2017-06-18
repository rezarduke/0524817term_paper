using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using System.IO;
using BotSkype.Model;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BotSkype.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //接收訊息
            var activity = await result as Activity;
            //解析訊息
            var msg = LUISParsing(activity.Text);
            //回傳訊息
            await context.PostAsync(msg);
            context.Wait(MessageReceivedAsync);
        }
        /// <summary>
        /// 透過LUIS解析訊息
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private string LUISParsing(string queryString)
        {
            string result = "無法識別的內容";
            //LUIS Endpoint url
            string strLuisUrl = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/5facbde4-4c49-40a4-a5f8-815edfb7ee9a?subscription-key=2394d8fa12364faf8743eb384b0a1c8b&verbose=true&timezoneOffset=0&q=" + queryString;
            //解析json格式轉為 LUISResponse 物件
            LUISResponse objLUISRes = JsonConvert.DeserializeObject<LUISResponse>(JsonParsing(strLuisUrl));
            if (objLUISRes.intents.Count() > 0)
            {
                //句字分類
                switch (objLUISRes.intents.First().intent)
                {
                    case "溢領":
                        result = "您好，溢領中老津貼將收到縣府或公所的行政處分(公文)，請盡速返還繳回溢領款項，若逾期未返還縣府將移送行政執行。";
                        break;
                    case "津貼互斥":
                        result = "您好，中老津貼與身障補助、老農津貼、國民年金老年基本保證年金、國民年金身障基本保證年金、國民年金老年年金A式給付...互斥，僅得擇一領取，申請中老津貼後原互斥補助將停止發放，詳情請洽戶籍所在地公所。";
                        break;
                    case "全家人口":
                        result = "您好，全家人口包括申請人及其配偶，負有扶養義務之子女及其配偶，前款之人所扶養之無工作能力子女，無負有扶養義務之子女及其配偶但實際負擔扶養義務之孫子女，認列綜合所得稅扶養親屬免稅額之納稅義務人；其餘排除列計人口請洽戶籍所在地公所。";
                        break;
                    case "了解其他津貼":
                        result = "您好，除了中老津貼以外，目前尚有老農津貼、國民年金老年相關給付、勞保老年年金等老人補助，以及身心障礙補助，如有疑問，可洽各補助主管機關。";
                        break;
                    case "了解中老津貼":
                        result = "您好，凡年滿65歲以上，設籍並實際居住於屏東縣，最近一年居住於國內183天，未接受政府公費收容安置，未入獄服刑、因案羈押或依法拘禁，家庭平均收入、動產、不動產符合法定標準，均可向戶籍所在地公所提出申請。";
                        break;
                    case "中老津貼發放":
                        result = "您好，屏東縣中老津貼於次月10日轉帳撥付申請人郵局帳戶，若10日遇國定例假日則提早至最近一個政府上班日入帳；申請人帳戶遭執行可申請專戶或現金發放。";
                        break;
                    case "None":
                        result = "您好，這是屏東縣中低收入老人生活津貼對話機器人，請輸入中低收入老人生活津貼(中老津貼)的問題或關鍵字，以利機器人辨識，謝謝。";
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 解析json格式
        /// </summary>
        /// <param name="xUrl"></param>
        /// <returns></returns>
        private string JsonParsing(string xUrl)
        {
            string _JsonResult = "";
            try
            {
                WebRequest request = WebRequest.Create(xUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                _JsonResult = reader.ReadToEnd();
                reader.Dispose();
                dataStream.Dispose();
                response.Close();
            }
            catch (Exception e)
            {
                _JsonResult = e.ToString();
            }
            return _JsonResult;
        }

    }
}