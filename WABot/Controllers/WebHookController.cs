using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WABot.Api;
using WABot.Helpers.Json;

namespace WABot.Controllers
{
    /// <summary>
    /// Controller for processing requests coming from chat-api.com
    /// </summary>
    [ApiController]
    [Route("/")]
    public class WebHookController : ControllerBase
    {
        /// <summary>
        /// A static object that represents the API for a given controller.
        /// </summary>
        /// 
        private static readonly WaApi api = new WaApi("https://eu5.chat-api.com/instance180795/", "nvpohfhzexbp53sl");

        /// <summary>
        /// Handler of post requests received from chat-api
        /// </summary>
        /// <param name="data">Serialized json object</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Post(Answer data)
        {
            foreach (var message in data.Messages)
            {
                if (message.FromMe)
                    continue;

                switch (message.Body.Split()[0].ToLower())
                {
                    case "chatid":
                        return await api.SendMessage(message.ChatId, $"Your ID: {message.ChatId}");
                    case "file":
                        var texts = message.Body.Split();
                        if (texts.Length > 1)
                            return await api.SendFile(message.ChatId, texts[1]);
                        break;
                    case "ogg":
                        return await api.SendOgg(message.ChatId);
                    case "geo":
                        return await api.SendGeo(message.ChatId, "", "");
                    case "group":
                        return await api.CreateGroup(message.Author);
                    default:
                        return await api.SendMessage(message.ChatId, message.Body);
                }
            }
            return "";
        }

        [HttpPost]
        [Route("enviararchivo")]
        public async Task<string> EnviarArchivo(Answer data)
        {
            string retornar = "";
            foreach (var message in data.Messages)
            {
                retornar = await api.SendFile(message.ChatId, message.Body);
            }
            return retornar;
        }


        [HttpPost]
        [Route("enviarmensaje")]
        public async Task<string> EnviarMensaje(Answer data)
        {
            string retornar = "";
            foreach (var message in data.Messages)
            {
                retornar = await api.SendMessage(message.ChatId, message.Body);
            }
            return retornar;
        }
        [HttpPost]
        [Route("enviarubicacion")]
        public async Task<string> Enviarubicacion(Answer data)
        {
            string retornar = "";
            foreach (var message in data.Messages)
            {
                retornar = await api.SendGeo(message.ChatId, message.Author,message.Id);
            }
            return retornar;
        }

        [HttpGet]
        public async Task<Answer> GetMensajes(string chatid)
        {
            return await api.GetRequestGet("messages", chatid);
        }

        [HttpGet]
        [Route("messagesHistory")]
        public async Task<Answer> GetMensajesmessagesHistory()
        {
            return await api.GetRequestmessagesHistory("messagesHistory");
        }
    }
}
