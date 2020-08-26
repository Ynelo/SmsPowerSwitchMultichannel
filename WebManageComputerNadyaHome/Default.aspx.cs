namespace WebManageComputerNadyaHome
{
    using System;
    using System.Linq;

    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string errorText = null;
            string status = null;

            //текст справки, адаптированный к текущему адресу
            var thisUrl = this.ResolveUrl(".");
            var helpResponse = String.Format(@"Help:
Краткие сведения:
====================================================================================================
Исходник на github здесь (учётка vovlite1): https://github.com/VovaLite1/SmsPowerSwitchMultichannel.git
Цель: Включить компьютер дома удаленно.
Как работает:
 1) Выпорлняем запрос в браузере: http://{0}?set=1 устанавливаем в текстовом файле state.txt единичку, этим мы подаем команду на включение компа дома для iskrajs
       -выполнить можно с телефона или компа другого

 2) iskrajs дома помстоянно опрашивает адрес и читает ответ: http://{0}?get
       - как только получит 1, то
           - включит комп  (ИЛИ УСЫПИТ, ЕСЛИ РАБОТАЛ!!!! - ТУТ СМОТРИ РАЗДЕЛ 4 - РАЗВИТИЕ - анализировать ATX-блок питания на предмет уже включенного состояния (ДВА ПИНА ЗАМКНУТЫ))
           - выполнит запрос на сброс 1-ки послав чуть измененный запрос: http://{0}?set=1

 3) Всё, комп включен (наверное :)) 

 4) Развитие: 
     - можно потом подумать над расширением функционала по опросу состояния компа искрой по какому то косвенному другому признаку - например, замкнутым контактам на АТХ-блоке питания
     - можно придумать команду set=2 - которая скажет искре УДЕРЖИВАТЬ КНОПКУ БОЛЕЕ 3 СЕКУНД - и комп если подвис - то выключится.

Команды:
=====================================================================================================
\t{0}?set=1  - установить значение в 1
\t{0}?set=0  - сбросить значение в 0
\t{0}?get  - прочитать текущее значение 0 или 1 (ранее установленное через set) и вернуть в ответ

Возможные ответы:
=====================================================================================================
\t1) 1, 0 - значение, ранее установленное по set и запрошенное сейчас по get
\t1) Help:... - справка
\t2) Error:... - что то пошло не так :)

", thisUrl);

            try
            {
                //в ответ пишем чистый текст
                Response.Clear();
                Response.ClearHeaders();
                Response.AddHeader("Content-Type", "text/plain");

                bool helpCommandExists = Request.Params.AllKeys.Contains("help", StringComparer.InvariantCultureIgnoreCase) ||
                                         Request.Params.AllKeys.Length == 0;

                if (helpCommandExists)
                {
                    Response.Write(helpResponse);
                    return;
                }

                //отработаем команду set
                bool setCommandExists = Request.Params.AllKeys.Contains("set");
                if (setCommandExists)
                {
                    string setCommand = Request.Params.Get("set");
                    if (setCommand != "1" && setCommand != "0")
                    {
                        //некорректное значение передали, направим на справку
                        Response.Write(helpResponse);
                        return;
                    }
                }

                //отработаем команду get
                bool getCommandExists = Request.Params.AllKeys.Contains("get");
                if (getCommandExists)
                {
                    var thisFolder = Server.MapPath(this.ResolveUrl("."));
                    Response.Write(thisFolder);
                    return;
                }

                //на все остальные команды возвращаем help
                Response.Write(helpResponse);
                return;
            }
            catch (Exception exception)
            {
                status = exception.ToString();
                Response.Write("Error:"+status);
            }
            finally
            {
                Response.Flush();
                Response.End();
            }
        }
    }
}
