namespace SamSafeCSharp.Components
{
    public class DefaultTimeTraveler
    {
        private readonly DefaultSnapshotStore _snapshotStore;
        private readonly int _cursor;

        /* JS code
         *  let display =(res,representation) => {
                res.status(200).send(representation) ;
            } ; 
         * */

        public DefaultTimeTraveler(DefaultSnapshotStore store = null)
        {
            _snapshotStore = store ?? new DefaultSnapshotStore();
            _cursor = -1;
        }


        public void Init(App app, string path, string next)
        {
            /*display = next || display;

            // API : returns a given snapshot or all if index is negative 
            app.get(path + '/:snapshot', function(req, res) {

                let index = req.params.snapshot;
                res.setHeader("Content-Type", "application/json");
                display(res, JSON.stringify(safe.getSnapshot(index)));

            }) ;

            app.get(path, function(req, res) {
                res.setHeader("Content-Type", "application/json");
                display(res, JSON.stringify(safe.getSnapshot()));

            }) ;

            // travel back
            app.post(path + '/:snapshot', function(req, res) {

                let dis = (representation) => {
                    let resp = res;
                    display(resp, representation);
                }


                let index = req.params.snapshot;

                if (index >= 0)
                {
                    let snapshot = snapshotStore.retrieve(index);
                    if ((index >= 0) && (index < snapshotStore.length()))
                    {
                        cursor = index;
                    }
                    let m = safe.deepCopy(snapshot.store);
                    m.__token = req.cookies['safe_token'] || '';

                    let modelProperties = Object.getOwnPropertyNames(safe.model).filter(function(p) {
                        return typeof safe.model[p] !== 'function';
                    }) ;

                    let snapShotProperties = Object.getOwnPropertyNames(m);

                    modelProperties.forEach(function(p) {
                        delete safe.model[p];
                    });

                    snapShotProperties.forEach(function(p) {
                        safe.model[p] = m[p];
                    });

                    safe.render(safe.model, dis, false);
                }
            }) ;*/
        }

        public int SaveSnapshot(Model model, string dataset)
        {
            // TODO check this
            string snapshot = _snapshotStore.Retrieve(_cursor);
            //if (dataset != null)
            //{
            //    cursor++;
            //    snapshot = null;
            //    snapshot = this.DeepCopy(dataset);
            //}

            //if (model != null)
            //    snapshot.store = this.DeepCopy(model);

            return _snapshotStore.Store(_cursor, snapshot);
        }

        public string GetSnapshot(int i)
        {
            if (i == 0)
                i = -1;

            if (i >= 0)
            {
                return _snapshotStore.Retrieve(i);
            }
            else
            {
                return _snapshotStore.RetrieveAll().ToString();
            }
        }

        public string DisplayTimeTravelControls(string representation)
        {
            /*JS CODE
             * 
             * return (representation + 
        '          <br>\n<br>\n<hr>\n<div class="mdl-cell mdl-cell--6-col">\n'+
        '                      <input id="__snapshot" type="text" class="form-control"><br>\n'+
        '                      <button id="__travel" onclick="JavaScript:return travel({\'snapshot\':document.getElementById(\'__snapshot\').value});"> TimeTravel </button>\n'+
        '          </div><br><br>\n') ;
             * 
             * 
             * */
            return null;
        }
    }
}