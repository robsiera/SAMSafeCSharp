﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SamSafeCSharp.Components;

namespace SafeCSharp
{
    [Route("api/[controller]")]
    public class SamSafeController : Controller
    {
        private readonly Actions _action; // not implemented on the server
        private readonly Model _model;
        private readonly State _state;
        private readonly View _samView;

        private Config _config;
        private readonly DefaultTimeTraveler _timeTraveler;
        private string _finalRepresantion;

        public SamSafeController(IHostingEnvironment hostingEnvironment)
        {
            this._action = new Actions();
            this._model = new Model();
            this._state = new State();
            this._samView = new View();

            TemplateRenderingService.Init(hostingEnvironment);
            var safe = new Safe();
            safe.Init(_action, _model, _state, _samView);

            // use default time traveler
            _timeTraveler = new DefaultTimeTraveler();
            safe.InitTimeTraveler(_timeTraveler);

            _config = new Config
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

            var app = new App();

            // add SAFE's APIs
            Safe.Dispatcher(app, apis.dispatch, "");
            _timeTraveler.Init(app, apis.timetravel, "");
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
            _timeTraveler.SaveSnapshot(_model, "");
            return _samView.Init(_model);
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


    /*
     * To Implement in Controller? 
     * 
     * var serverResponses = {
            tooBusy: function(req,res) {
                res.writeHead(429, { 'Content-Type': 'text/plain' });
                res.end("Server is too busy, please try again later") ;
            },
    
            unauthorized: function(req,res) {
                res.header('Content-Type', 'text/html') ;
                res.status(401).send('<htnl><body>Unauthorized access</body></html>') ;  
            },
    
            serverError: function(req,res) {
                res.header('Content-Type', 'text/html') ;
                res.status(500).send('<htnl><body>Server Error</body></html>') ;  
            }
        } ;

        var authnz = {

            authorized: function (cookies) {
                if (cookies.authorized > 0) {
                    return true ;
                }
                return false ;
            },
    
            del: function(req, res, cookie) { 
                if (cookie !== undefined) {
                    res.clearCookie(cookie);  
                }
            },
    
            set: function(req, res, cookie) { 
                if (cookie !== undefined) {
                     res.cookie(cookie, +new Date(), { maxAge: 3600000, path: '/' }); 
                }
            },
    
            isSet: function(req, res, cookie) { 
                if (cookie !== undefined) {
                     return res.cookies[cookie]; 
                }
            },
    
            validateCredentials: function(username,password) {
                return ((username === config.username) && (password === config.password)) ;
            }

        } ;*/


}
