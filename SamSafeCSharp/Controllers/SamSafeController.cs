using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SamSafeCSharp.Components;
using SamSafeCSharp.Helpers;

namespace SafeCSharp
{
    [Route("api/[controller]")]
    public class SamSafeController : Controller
    {
        private readonly Safe _safe = new Safe();

        private readonly Model _model;
        private readonly View _samView;

        private readonly DefaultTimeTraveler _timeTraveler;
        private string _finalRepresantion;

        public SamSafeController(IHostingEnvironment hostingEnvironment)
        {
            // Business Logic - SAM Pattern
            Actions actions = new Actions();
            State state = new State();
            this._model = new Model();
            this._samView = new View();

            TemplateRenderingService.Init(hostingEnvironment);

            _safe.Init(actions, _model, state, _samView);

            // use default time traveler
            if (true == false) // change this to enable timetraveling
            {
                _timeTraveler = new DefaultTimeTraveler();
                _safe.InitTimeTraveler(_timeTraveler);
            }

            var config = new Config
            {
                Port = 5425,
                LoginKey = "abcdef0123456789",
                AdminDirectory = "./console/bower_components",
                Username = "sam",
                Password = "nomvc"
            };

            const string r = "api";
            const string v = "/samsafe";
            const string d = "dev";

            var apis = new
            {
                login = $"/{r}{v}/login",
                logout = $"/{r}{v}/logout",
                present = $"/{r}{v}/present",
                init = $"/{r}{v}/init",
                dispatch = $"/{r}{v}/dispatch",
                timetravel = $"/{d}{v}/timetravel/snapshots"
            };

            // add SAFE's APIs
            _timeTraveler?.Init(apis.timetravel, null);
        }


        [HttpPost]
        public void Login(object o)
        {
            //TODO : create user model class 
            //string username = o.username;
            //string password = o.password;
            //if (IAutorizor.validateCredentials(username, password))
            //{
            //    //   return true;
            //}
            //else
            //    throw new Exception("unknow user");
        }

        [HttpPost]
        public bool Logout(object o)
        {
            //IAutorizor.del(Request, Response, "authorized");
            return false;
        }

        [HttpPost]
        public string Present([FromBody] dynamic data)
        {
            _model.Present((PresenterModel)data, PushRepresentation);
            return _finalRepresantion;
        }

        /// <summary>
        /// Pushes the representation to the client
        /// </summary>
        private void PushRepresentation(string representation)
        {
            _finalRepresantion = representation;

            //todo consider using signalR to push new representation to the client
        }

        [HttpGet("init")]
        public string Init()
        {
            _timeTraveler?.SaveSnapshot(_model, "");
            return _samView.Init(_model);
        }

        [HttpPost("dispatch")]
        public string Dispatch([FromBody] PresenterModel data)
        {
            Request.Cookies.TryGetValue("safe_token", out var safeToken);
            data.__token = safeToken;

            Action<string> nap = PushRepresentation;
            _safe.Dispatch(data.__action, data, nap);

            return _finalRepresantion;
        }

    }

    public interface IAutorizor
    {
        void ValidateCredentials(string username, string password);

        void Del(string res, string req, string status);
    }

    public class Config
    {
        public int Port { get; set; }
        public string LoginKey { get; set; }
        public string AdminDirectory { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
