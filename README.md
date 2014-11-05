#**Qabuze** [![Build status](https://ci.appveyor.com/api/projects/status/hmkn8xkhfjydnp96/branch/master)](https://ci.appveyor.com/project/butterkekstorte/qabuze/branch/master)
---

A Music-data-gatherer.

Qabuze is a simple Data-gatherer using the Qobuz-API with optional functionality to download songs.

For license and disclaimer, please refer to [LICENSE.md](https://github.com/butterkekstorte/Qabuze/blob/master/LICENSE.md).

##**How to use**
---

 1. **Initial setup**
  - create a file called 'config.json' in the application directory or a 'qabuze.json' in the userfolder (C:\Users\&lt;Your Username&gt;\qabuze.json).
  - use this skeleton to configure it to your needs.
    <code>{<br/>
      "qabuze": {<br/>
        "accounts": [<br/>
          {<br/>
            "name": "1st account",<br/>
            "token": "This token will be used for authentication",<br/>
            "password": null<br/>
          },<br/>
          {<br/>
            "name": "2nd account",<br/>
            "token": "LOGIN",<br/>
            "password": "this password will be used to get a token"<br/>
          }<br/>
        ],<br/>
        "appId": "&lt;own app-id or 214748364&gt;",<br/>
        "appSecret": "&lt;own app-secret or 6fdcbccb7a073f35fbd16a193cdef6c4&gt;",<br/>
        "apiURL": "http://www.qobuz.com/api.json/0.2/",<br/>
        "fileScheme": "%disknumber% - %tracknumber% - %title% - %album% - %artist%.%ext%",<br/>
        "folderScheme": "%ALBUMARTIST%\\%album%",<br/>
        "outputFolder": "C:\\Users\\&lt;Your Username&gt;\\QabuzeDownloads\\"<br/>
      }<br/>
    }</code>

 2. **Search**
  - Enter your search query into the upper-left textbox and click "Search"
  - A new window will open
  - Click on any artist and select an album
  - The album info will automatically be loaded

 
 3. **Grab album info**
  - You can replicate the loading of album-info with the button "Grab Album info" and the textbox to the left of it.
  - When you know a album-id of an album you can enter it into the textbox and click on the button to load it.

  
 4. **Download** *
  - In order to download an album make sure the relevant settings are set.
  - Select an album as described in 2 or 3
  - Click on the download button.
 

 5. **Download status** *
  - To see the exact status of your downloads you can use the "Download status"-button.
  - Once clicked a pop-up will open with a detailed view.
  - Within that view you can clear failed and done threads with the button in the bottom-left.
     - **Note**: This will only remove them from the view, but *not* from the hard-drive


* *Only necessary/possible with download-enabled version*
