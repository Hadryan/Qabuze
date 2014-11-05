/* 
 * Qabuze: A Music-data-gatherer
 * Copyright (C) 2014 Hendrik 'Xendo' Meyer <code@xendosoft.de>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.IO;
using System.Data;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Qabuze
{


    public class QabuzeAPI
    {

        public class Config {

            private static Config instance = null;

            public static Config getInstance() {
                if (Config.instance == null) {
                    Config.instance = new Config();
                }
                return Config.instance;
            }

            public List<Account> accounts = new List<Account>();

            public string appId, appSecret, apiURL, fileScheme, folderScheme, outputFolder;
            
        }

        public class Account {

            private string name, token;

            public string getToken() { return this.token; }

            public string getName() { return this.name; }

            public Account setToken(string token) { this.token = token; return this; }

            public Account setName(string name) { this.name = name; return this; }

            public Account(string name, string token) {
                this.token = token;
                this.name = name;
            }

        }

        public static QabuzeAPI instance = null;

        public static void init() {
            QabuzeAPI.instance = new QabuzeAPI();
        }

        public string AppId, AppSecret, UserAuthToken, baseURL;

        public string BuildRequest(String APICall, List<KeyValuePair<string,string>> data, Boolean noAuth = false, int credIndex = 0) {
            //Builds a signed request using the given data.
            this.AppId = QabuzeAPI.Config.getInstance().appId;
            this.AppSecret = QabuzeAPI.Config.getInstance().appSecret;
            try
            {
                this.UserAuthToken = QabuzeAPI.Config.getInstance().accounts.AsReadOnly()[credIndex].getToken();
            } catch (Exception e) {
                this.UserAuthToken = "";
            }

            this.baseURL = QabuzeAPI.Config.getInstance().apiURL; 
            
            data.Sort(Utils.sortKeyString);
            string sig = APICall.Replace("/", "");
            foreach (KeyValuePair<string, string> pair in data)
            {
                sig += pair.Key + pair.Value;
            }
            int timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            sig += timestamp.ToString();
            sig += AppSecret;
            sig = Utils.GetMd5Hash(sig);

            data.Add(new KeyValuePair<string, string>("app_id", AppId));
            if (!noAuth)
            {
                data.Add(new KeyValuePair<string, string>("user_auth_token", UserAuthToken));
                data.Add(new KeyValuePair<string, string>("request_ts", timestamp.ToString()));
                data.Add(new KeyValuePair<string, string>("request_sig", sig));
            }

            string url = baseURL + APICall + "?";
            foreach (KeyValuePair<string, string> pair in data)
            {
                url += pair.Key + "=" + pair.Value + "&";
            }

            return url;
        }

        public QabuzeAPI() {

            string json = "";
            JObject obj = null;
            if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\qabuze.json")) {
                json = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\qabuze.json");
            } else { 
                try {
                    json = System.IO.File.ReadAllText("config.json");
		        } catch (Exception) {                		
                        Console.WriteLine("No valid configfile found!");
                        return;
	            }
            } 
            try {
                obj = JObject.Parse(json);
                JObject config = (JObject)obj["qabuze"];
                if (config == null)
                {
                    throw new Exception();
                }

                QabuzeAPI.Config.getInstance().appId = (string)config["appId"];
                QabuzeAPI.Config.getInstance().appSecret = (string)config["appSecret"];
                QabuzeAPI.Config.getInstance().apiURL = (string)config["apiURL"];
                QabuzeAPI.Config.getInstance().fileScheme = (string)config["fileScheme"];
                QabuzeAPI.Config.getInstance().folderScheme = (string)config["folderScheme"];
                QabuzeAPI.Config.getInstance().outputFolder = (string)config["outputFolder"];

                #if DEBUG
                Console.WriteLine("appId: " + QabuzeAPI.Config.getInstance().appId);
                Console.WriteLine("appSecret: " + QabuzeAPI.Config.getInstance().appSecret);
                Console.WriteLine("apiURL: " + QabuzeAPI.Config.getInstance().apiURL);
                Console.WriteLine("fileScheme: " + QabuzeAPI.Config.getInstance().fileScheme);
                Console.WriteLine("folderScheme: " + QabuzeAPI.Config.getInstance().folderScheme);
                Console.WriteLine("outputFolder: " + QabuzeAPI.Config.getInstance().outputFolder);
                Console.WriteLine("\n");
                #endif

                Console.WriteLine("=== Accounts ===");
                foreach (JObject acc in config["accounts"])
                {
                    Console.WriteLine("Name: " + acc["name"]);
                    if ((string)acc["token"] == "LOGIN")
                    {
                        Console.WriteLine("Aquiring Usertoken...");
                        List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();

                        acc["password"] = Utils.GetMd5Hash((string)acc["password"]);

                        data.Add(new KeyValuePair<string, string>("username", (string)acc["name"]));
                        data.Add(new KeyValuePair<string, string>("password", (string)acc["password"]));

                        string response = (new WebClient()).DownloadString(BuildRequest("user/login", data, true));

                        JObject resp = JObject.Parse(response);
#if DEBUG
                        System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\qabuze_account_" + (string)acc["name"] + ".json", resp.ToString());
#endif
                        acc["token"] = (string)resp["user_auth_token"];
                        Console.WriteLine((string)acc["token"]);
                        acc["password"] = null;
                        QabuzeAPI.Config.getInstance().accounts.Add(new QabuzeAPI.Account((string)acc["name"], (string)acc["token"]));

                    }
                    else {
                    QabuzeAPI.Config.getInstance().accounts.Add(new QabuzeAPI.Account((string)acc["name"], (string)acc["token"]));
                    //Console.WriteLine(acc["token"]);
                    }
                }
                Console.WriteLine("=== Accounts ===\n");

                #if DEBUG
                foreach (QabuzeAPI.Account acc in QabuzeAPI.Config.getInstance().accounts) {

                    Console.WriteLine(acc.getName() + ": " + acc.getToken());
                                    
                }
                #endif

                if (QabuzeAPI.Config.getInstance().accounts.Count == 0) {
                    MessageBox.Show("Please add an account to the config file!", "No account configured");
                    frmMain.instance.Close();
                }

                #if DEBUG
                Console.WriteLine(obj.ToString());
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\qabuze.json", obj.ToString());
                #endif

            } catch (Exception) {
                Console.WriteLine("Configfile invalid!");
                Console.WriteLine(obj.ToString());
            }
            instance = this;
            #if WITHDOWNLOAD
            if (!System.IO.File.Exists("metaflac.exe"))
            {
                MessageBox.Show("Metaflac could not be found. Files will NOT be tagged!", "Metaflac not found");
            }
            #endif
        }

        public static Object PerformRequest(string url) {
            string response;
            try
            {
                response = (new WebClient()).DownloadString(url);
            }
            catch (WebException e) {
                HttpWebResponse _response = (System.Net.HttpWebResponse)e.Response;
                Console.WriteLine("URL \"" + url + "\" has come back with the following error: " + _response.StatusCode);
                return null;
            }
            JObject obj = JObject.Parse(response);

            #region Album
            if (obj["tracks_count"] != null)
            {
                QabuzeAlbum objReturn;
                Console.WriteLine("Is Album");
                 objReturn = new QabuzeAlbum();
                //QabuzeAlbum objReturn = new QabuzeAlbum();
                objReturn.title =               (string)obj["title"];
                objReturn.id =                  (string)obj["id"];
                objReturn.genre =               (string)obj["genre"]["name"];
                objReturn.artist =              (string)obj["artist"]["name"];
                objReturn.artistID =            (int)obj["artist"]["id"];
                objReturn.description =         (string)obj["description"];
                objReturn.coverURL =            (string)obj["image"]["large"];
                objReturn.label =               (string)obj["label"]["name"];
                objReturn.track_count =         (int)obj["tracks_count"];
                objReturn.duration =            (int?)obj["duration"];
                objReturn.release_timestamp =   (int?)obj["released_at"];
                int amt_tracks =                (int)obj["tracks"]["total"];

                for (int i = 0; i < amt_tracks; i++)
                {
                    JObject track = (JObject) obj["tracks"]["items"][i];
                    try {

                        Console.WriteLine("Adding Track #" + i + " with ID " + track["id"] + "...");
                    QabuzeSong song = new QabuzeSong(
                        (string)    track["id"],
                        (string)    track["title"],
                        (string)    track["performer"]["name"],
                        (string)    obj["id"],
                        (int)       track["duration"],
                        (int)       track["track_number"],
                        (int)       track["media_number"],
                        (string)    obj["title"],
                        (string)    obj["genre"]["name"]
                        );
                    KeyValuePair<int, QabuzeSong> kvp = new KeyValuePair<int, QabuzeSong>(i, song);
                    objReturn.songs.Add(kvp);
                    } catch (Exception e){
                        Console.Error.WriteLine("Whoops. Track #" + i + " with ID " + track["id"] + " could not be added");
                    }
                }

                string start_date = "9999-12-31", end_date = "9999-12-31";
              /*  try
                {
                        start_date = (string)obj["rights"]["stream"][0]["start_date"];

                }
                catch (Exception)
                {*/
                    try
                    {
                        start_date = Utils.UnixTimeStampToDateTime((int)obj["streamable_at"]).ToString();

                    }
                    catch (Exception) { }
               /* }
                try
                {
                        end_date = (string)obj["rights"]["stream"][0]["end_date"];

                }
                catch (Exception) { }
                */
                DateTime d_start = DateTime.Parse(start_date).ToUniversalTime();
                DateTime d_end = DateTime.Parse(end_date).ToUniversalTime();
                DateTime d_curr = DateTime.UtcNow;

                bool c_start = d_start > d_curr;
                bool c_end = d_end > d_curr;

                if (c_start)
                {
                    objReturn.availableForStreaming = false;
                }
                else
                {
                    objReturn.availableForStreaming = true;
                }

                if (!c_end){
                    objReturn.availableForStreaming = false;
                }

                Console.WriteLine("May stream from " + start_date + " until " + end_date);
                objReturn.availableFrom = d_start;
                objReturn.availableUntil = d_end;

                return objReturn;
            #endregion
             #region Album from Search
            }
            else if (obj["albums"] != null)
            {
                Console.WriteLine("Is Album Search");
                List<QabuzeAlbum> objReturn = new List<QabuzeAlbum>();
                //QabuzeAlbum objReturn = new QabuzeAlbum();
                int limit = (int)obj["albums"]["limit"];
                int offset =(int)obj["albums"]["offset"];
                int total = (int)obj["albums"]["total"];
                int upperLimit = ((offset + limit) > total) ? total : (offset + limit);

                for (int i = 0; i < upperLimit - offset; i++)
                {
                    JObject jalbum = (JObject)obj["albums"]["items"][i];

                    QabuzeAlbum album = new QabuzeAlbum();
                    album.title = (string)jalbum["title"];
                    album.id = (string)jalbum["id"];
                    album.genre = (string)jalbum["genre"]["name"];
                    album.artist = (string)jalbum["artist"]["name"];
                    album.artistID = (int?)jalbum["artist"]["id"];
                    album.description = (string)jalbum["description"];
                    album.coverURL = (string)jalbum["image"]["large"];
                    album.label = (string)jalbum["label"]["name"];
                    album.track_count = (int)jalbum["tracks_count"];
                    
                    objReturn.Add(album);
                }
                return objReturn;
                
            #endregion
            #region Track
            } else if (obj["track_number"] != null) {
                Console.WriteLine("Is Track");
                QabuzeSong objReturn = new QabuzeSong(
                        (string)obj["id"],
                        (string)obj["title"],
                        (string)obj["performer"]["name"],
                        (string)obj["album"]["id"],
                        (int)   obj["duration"],
                        (int)   obj["track_number"],
                        (int)obj["media_number"],
                        (string)obj["album"]["title"],              
                        (string)obj["album"]["genre"]["name"]
                        );
                return objReturn;
            #endregion
            #region Artist
            } else if (obj["albums_as_primary_artist_count"] != null)
            {
                Console.WriteLine("Is Artist");
                QabuzeArtist objReturn = new QabuzeArtist();

                objReturn.albums_as_primary_artist_count = (int) obj["albums_as_primary_artist_count"];
                objReturn.id = (int)obj["id"];
                objReturn.name = (string)obj["name"];
                objReturn.imageURL = (string)obj["image"]["mega"];
                if (obj["biography"] != null)
                {
                    objReturn.biography = (string)obj["biography"]["content"];
                }

                return objReturn;
            }
            #endregion

           // Console.WriteLine(response);
            return (null);
        }
        public List<QabuzeAlbum> search(string query, int offset = 0, int credIndex = 0)
        {

            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("query", query));
            data.Add(new KeyValuePair<string, string>("type", "albums"));
            data.Add(new KeyValuePair<string, string>("limit", (50 + offset).ToString()));
            data.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
            return (List<QabuzeAlbum>)QabuzeAPI.PerformRequest(QabuzeAPI.instance.BuildRequest("catalog/search", data, false, credIndex));

        }


        private readonly object _sync = new object();

    }
    public class QabuzeSong {

        public string title = "", artist, track_id, albumID, album, genre;
        public int? duration = 0, track_number, media_number;

        public QabuzeSong(string id, string title, string artist, string albumID, int duration, int track_number, int media_number, string album, string genre)
        {
            this.track_id = id;
            this.title = title;
            this.artist = artist;
            this.albumID = albumID;
            this.album = album;
            this.duration = duration;
            this.track_number = track_number;
            this.media_number = media_number;
            this.genre = genre;
        }

        public QabuzeSong() {            
        }

        public static QabuzeSong querySong(string id) {

            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("track_id", id));
            for (int credIndex = 0; credIndex < QabuzeAPI.Config.getInstance().accounts.Count; credIndex++)
            {
                QabuzeSong tmp = (QabuzeSong) QabuzeAPI.PerformRequest(QabuzeAPI.instance.BuildRequest("track/get", data, false, credIndex));
                if(tmp != null){
                    return tmp;
                }
            }
            return null;
        }

        public string getDownloadLink(bool lossless) {
            try
            {
                for (int credIndex = 0; credIndex < QabuzeAPI.Config.getInstance().accounts.Count; credIndex++)
                {

                    List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();

                    data.Add(new KeyValuePair<string, string>("format_id", "6"));
                    data.Add(new KeyValuePair<string, string>("intent", "stream"));
                    data.Add(new KeyValuePair<string, string>("track_id", this.track_id));

                    string response = (new WebClient()).DownloadString(QabuzeAPI.instance.BuildRequest("track/getFileUrl", data, false, credIndex));
                    JObject obj = JObject.Parse(response);

                    int format_id = 0, isSample = 2;
                    List<string> restrictions = new List<string>();

                    try
                    {
                        format_id = ((int)obj["format_id"]);

                    }
                    catch (Exception) { }
                    try
                    {
                        isSample = ((bool)obj["sample"]) ? 1 : 0;

                    }
                    catch (Exception) { }
                    try
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            restrictions.Add((string)obj["restrictions"][i]["code"]);
                        }

                    }
                    catch (Exception) { }

                    if (format_id < 6 || isSample == 1)
                    {

                        Console.WriteLine("Track #" + track_id + " not to be downloaded via account '" + QabuzeAPI.Config.getInstance().accounts[credIndex].getName() + "' because:");
                        foreach (string code in restrictions)
                        {
                            Console.WriteLine("  - " + code);
                        }
                        Console.WriteLine("  - The format is " + ((format_id < 5) ? "NOT " : "") + "FLAC");
                        Console.WriteLine((isSample == 1) ? "  - The file is a sample" : "  - The file is (probaly) not a sample");
                        //Console.WriteLine("No success via account '" + QabuzeAPI.Config.getInstance().accounts[credIndex].getName() + "'...");
                    }
                    else
                    {
                        Console.WriteLine("Got the link via account '" + QabuzeAPI.Config.getInstance().accounts[credIndex].getName() + "'...");

                        #if DEBUG
                        Console.WriteLine((string)obj["url"]);
                        #endif

                        return (string)obj["url"];
                    }
                }
                return null;
            }
            catch (Exception) {
                return null;
            }
        }

    }
    public class QabuzeArtist
    {

        public string name, biography, imageURL;
        public int id, albums_as_primary_artist_count;

        public QabuzeArtist() { }

        public static QabuzeArtist queryArtist(string id) {

            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("artist_id", id));
            for (int credIndex = 0; credIndex < QabuzeAPI.Config.getInstance().accounts.Count; credIndex++)
            {
                QabuzeArtist tmp = (QabuzeArtist)QabuzeAPI.PerformRequest(QabuzeAPI.instance.BuildRequest("artist/get", data, false, credIndex));
                if (tmp != null)
                {
                    return tmp;
                }
            }
            return null;
        }

    }
    public class QabuzeAlbum
    {

        public static List<QabuzeAlbum> albums = new List<QabuzeAlbum>();

        public string id, genre, title, description, coverURL, label, artist;
        public int? duration, track_count, release_timestamp, artistID;
        public List<KeyValuePair<int, QabuzeSong>> songs = new List<KeyValuePair<int, QabuzeSong>>();
        public bool availableForStreaming = false;
        public DateTime availableFrom, availableUntil;

        public QabuzeAlbum() {}
        public static QabuzeAlbum QueryAlbumById(string id)
        {

            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string,string>("album_id", id));
            for (int credIndex = 0; credIndex < QabuzeAPI.Config.getInstance().accounts.Count; credIndex++)
            {
                QabuzeAlbum tmp = (QabuzeAlbum)QabuzeAPI.PerformRequest(QabuzeAPI.instance.BuildRequest("album/get", data, false, credIndex));
                if (tmp != null)
                {
                    return tmp;
                }
            }
            return null;
        }

        public static QabuzeAlbum queryAlbum(string id)
        {

            foreach (QabuzeAlbum current in QabuzeAlbum.albums)
            {
                if (current.id == id) { 
                    return current;
                }
            }

            return QueryAlbumById(id);
        }

    }
}
