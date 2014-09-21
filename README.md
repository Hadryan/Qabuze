#**Qabuze** [![Build status](https://ci.appveyor.com/api/projects/status/hmkn8xkhfjydnp96/branch/master)](https://ci.appveyor.com/project/butterkekstorte/qabuze/branch/master)
---

A Music-data-gatherer.

Qabuze is a simple Data-gatherer using the Qobuz-API with optional functionality to download songs.

For license and disclaimer, please refer to [LICENSE.md](https://github.com/butterkekstorte/Qabuze/blob/master/LICENSE.md).

##**How to use**
---

 1. **Initial setup**
  - In the upper-right corner click on "Settings"
  - Enter your App-Id, App-Secret and User-auth-token
  - Set your downloadlocation and your folder- and file-structure *
  - You can use the following placeholders (case-insensitive): *

     - %ALBUMARTIST% - Album Artist (eg. "Various Artists" if a compilation or same as %artist%)
     - %ARTIST% - Song Artist
     - %GENRE% - Genre (not always accurate, complain to Qobuz)
     - %ORGANIZATION% - Album label / Recording studio
     - %ALBUM% - Album name
     - %TRACKTOTAL% - Total amount of tracks in the album
     - %RELEASE% - Release-date (again, not always accurate, because of partially incorrect Qobuz-DB-data)
     - %TITLE% - Song title
     - %TRACKNUMBER% - Track number on disc
     - %DISKNUMBER% - Disc number
     - %DURATION% - Song duration in seconds
     - %EXT% - resolves to "flac"
     
  -  Hit save and restart the application

 2. **Search**
  - Enter your search query into the upper-left textbox and click "Search"
  - A new window will open
  - Click on any artist and select an album
  - The album info will automatically be loaded

 
 3. **Grab album info**
  - You can replicate the loading of album-info with the button "Grab Album info" and the textbox to the left of it.
  - When you know a album-id of an album you can enter it into the textbox and click on the button to load it.

  
 4. **Download ***
  - In order to download an album make sure the relevant settings are set.
  - Select an album as described in 2 or 3
  - Click on the download button.
 

 5. **Download status ***
  - To see the exact status of your downloads you can use the "Download status"-button.
  - Once clicked a pop-up will open with a detailed view.
  - Within that view you can clear failed and done threads with the button in the bottom-left.
     - **Note**: This will only remove them from the view, but *not* from the hard-drive


**Only necessary/possible with download-enabled version*