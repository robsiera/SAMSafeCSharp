using Microsoft.AspNetCore.Mvc;
using SamSafeCSharp.Components;

namespace SafeCSharp
{
    [Route("api/[controller]")]
    public class SamSafeController : Controller
    {
        private Safe Safe { get; }
        private Actions Action { get; } // not implemented on the server
        private Model Model { get; }
        private State State { get; }
        private View SamView { get; }
        private Config Config { get; }
        public string[] ServerResponses { get; set; }

        public DefaultTimeTraveler TimeTraveler { get; }

        public SamSafeController()
        {
            this.Action = new Actions();
            this.Model = new Model();
            this.State = new State();
            this.SamView = new View();

            this.Safe = new Safe();
            Safe.Init(Action, Model, State, SamView);

            // use default time traveler
            TimeTraveler = new DefaultTimeTraveler();
            Safe.InitTimeTraveler(TimeTraveler);

            Config = new Config
            {
                Port = 5425,
                LoginKey = "abcdef0123456789",
                AdminDirectory = "./console/bower_components",
                Username = "sam",
                Password = "nomvc"
            };

            var R = "api";
            var V = "/samsafe";
            var D = "dev";

            var apis = new
            {
                login = $"/{R}{V}/login",
                logout = $"/{R}{V}/logout",
                present = $"/{R}{V}/present",
                init = $"/{R}{V}/init",
                dispatch = $"/{R}{V}/dispatch",
                timetravel = $"/{D}{V}/timetravel/snapshots"
            };

            var app = new App();

            // add SAFE's APIs
            Safe.Dispatcher(app, apis.dispatch, "");
            TimeTraveler.Init(app, apis.timetravel, "");
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
        public void Present([FromBody] dynamic data)
        {

            Model.Present((PresenterModel)data, "representationFunc");
            /*
             * model.present(data, function(representation) {
                res.status(200).send(representation) ;
             * */
        }

        [HttpGet("init")]
        public string Init()
        {

            //timeTraveler.SaveSnapshot(model, "res.status(200).send(view.init(model))");

            return SamView.Init(Model);
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
