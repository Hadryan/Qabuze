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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Qabuze
{
    static class Utils
    {

        static MD5 md5 = MD5.Create();
        public static int sortKeyString(KeyValuePair<string, string> a, KeyValuePair<string, string> b)
        {
            return a.Key.CompareTo(b.Key);
        }
        public static int sortKeyInt(KeyValuePair<int, string> a, KeyValuePair<int, string> b)
        {
            return a.Key.CompareTo(b.Key);
        }
        public static string GetMd5Hash(string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        internal static string escapeAlbumString(string p, QabuzeAlbum album)
        {

            p = Regex.Replace(@p, "%ALBUMARTIST%", album.artist, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%GENRE%", album.genre, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%ORGANIZATION%", album.label, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%ALBUM%", album.title, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%TRACKTOTAL%", album.track_count.ToString(), RegexOptions.IgnoreCase);
            if (album.release_timestamp == null)
            {
                p = Regex.Replace(@p, "%RELEASE%", "no date", RegexOptions.IgnoreCase);
            }
            else
            {
                p = Regex.Replace(@p, "%RELEASE%", UnixTimeStampToDateTime((int)album.release_timestamp).ToShortDateString(), RegexOptions.IgnoreCase);
            }

            return p;

        }
        internal static string escapeSongString(string p, QabuzeSong song)
        {

            p = Regex.Replace(@p, "%ARTIST%", song.artist, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%GENRE%", song.genre, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%TITLE%", song.title, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%ALBUM%", song.album, RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%TRACKNUMBER%", song.track_number.ToString(), RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%DISKNUMBER%", song.media_number.ToString(), RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%DURATION%", song.duration.ToString(), RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "%EXT%", "flac", RegexOptions.IgnoreCase);

            return p;

        }

        internal static string truncateIfNecessary(string p)
        {
            if ((Properties.Settings.Default.outputFolder.Length + p.Length) >= 245)
            {
                p = p.Substring(0, (245 - Properties.Settings.Default.outputFolder.Length)) + ".flac";
            }

            return p;
        }
        internal static string escapeMetaString(string p, QabuzeAlbum album, QabuzeSong song)
        {

            p = escapeAlbumString(@p, album);
            p = escapeSongString(@p, song);

            //remove illegal characters:
            p = Regex.Replace(@p, "/", "", RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, ":", "", RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "'", "", RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, "<", "", RegexOptions.IgnoreCase);
            p = Regex.Replace(@p, ">", "", RegexOptions.IgnoreCase);
            p = @p.Replace("*", "");
            p = @p.Replace("|", "");
            p = @p.Replace("?", "");
            p = @p.Replace("[", "");
            p = @p.Replace("]", "");
            p = @p.Replace("\"", "");

            if ((Properties.Settings.Default.outputFolder.Length + p.Length) >= 245)
            {
                p = p.Substring(0, (245 - Properties.Settings.Default.outputFolder.Length)) + ".flac";
            }

            return p;

        }
    }
}
