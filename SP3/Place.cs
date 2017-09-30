using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System;

namespace SP3
{
    class Place
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }

        public List<Place> Children { get; set; }
        public Place Father { get; set; }

        public bool Traversed { get; set; }


        public Place()
        {

        }

        public void Start()
        {
            TryGetPage();

        }

        //private async Task AnalyzePageAsync()
        //{


        //}

        private void TryGetPage()
        {
            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri(URL));
            client.DownloadStringCompleted += OnDownloaded;
        }

        private void OnDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                TryGetPage();
            }
            else
            {
                OnDownloadSuccess(e.Result);
            }
            throw new NotImplementedException();
        }
        private void OnDownloadSuccess(string result)
        {
            Children = TryGetChildren(result);

        }

        private List<Place> TryGetChildren(string result)
        {



            return null;

        }



        private void OnGetPage()
        {

        }

        //private void On

        public string FullName() => Father != null ? Father.FullName() + Name : Name;
        public string FullName(string split) => Father != null ? Father.FullName() + split + Name : Name;
        public string FullName(char split) => Father != null ? Father.FullName() + split + Name : Name;



    }

}
