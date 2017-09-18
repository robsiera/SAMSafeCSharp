using SamSafeCSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SamSafeCSharp.Components;

namespace SafeCSharp
{
    [Route("api/[controller]")]
    public class ServerController : Controller
    {
        public Safe safe { get; set; }
        public Actions action { get; } // not implemented on the server
        public Model model { get; }
        public State state { get; }
        public View view { get; }
        public Config config { get; set; }
        public string[] serverResponses { get; set; }
        public string v { get; set; }
        public string r { get; set; }
        public string d { get; set; }
        public string a { get; set; }
        public DefaultTimeTraveler timeTraveler { get; }

        public ServerController(Actions action, Model model, State state, View view)
        {
            this.action = action;
            this.model = model;
            this.state = state;
            this.view = view;

            this.safe=new Safe();
            safe.init(action, model, state, view);

            // use default time traveler
            timeTraveler = new DefaultTimeTraveler();
            safe.initTimeTraveler(timeTraveler);

            config = new Config
            {
                port = 5425,
                loginKey = "abcdef0123456789",
                adminDirectory = "./console/bower_components",
                username = "sam",
                password = "nomvc"
            };

            this.v = "/v1";
            this.r = "app";
            this.d = "dev";
            this.a = "api";

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
            safe.dispatcher(app, apis.dispatch);
            timeTraveler.Init(app, apis.timetravel);
        }

        [HttpPost]
        public void Login(object o)
        {
            string username = o.username;
            string password = o.password;
            if (IAutorizor.validateCredentials(username, password))
            {
                //   return true;
            }
            else
                throw new Exception("unknow user");
        }

        [HttpGet]
        public bool Logout(object o)
        {
            IAutorizor.del(Request, Response, "authorized");
            return false;
        }

        [HttpPost]
        public void Present([FromBody] object data)
        {
            model.present(data, "representationFunc");
            /*
             * model.present(data, function(representation) {
                res.status(200).send(representation) ;
             * */
        }

        [HttpGet]
        public void Init(HttpRequest req, HttpResponse res)
        {
            timeTraveler.SaveSnapshot(model, "res.status(200).send(view.init(model))");

            view.Init(model);
        }

        /*
         * // start application 

        app.listen(config.port, function() {
            console.log("registering app on port: "+config.port) ;
            //setTimeout(register(),2000) ; 
        });
        */


    }

    public interface IAutorizor
    {
        void validateCredentials(string username, string password);

        void del(string res, string req, string status);
    }

    public class Config
    {
        public int port { get; set; }
        public string loginKey { get; set; }
        public string adminDirectory { get; set; }
        public string username { get; set; }
        public string password { get; set; }
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
