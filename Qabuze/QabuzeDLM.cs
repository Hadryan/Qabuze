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
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace Qabuze
{
    public class QabuzeDLM
    {
        private static int amtThreads = 0, activeThreads = 0, maxActiveThreads = 2, finishedThreads = 0;

        public static IDictionary<int, QabuzeDLMThreadStatus> status = new Dictionary<int, QabuzeDLMThreadStatus>();
        private static List<string> logs = new List<string>();
        public enum DLMEvent{
            waiting,
            working,
            loaded,
            tagged,
            done,
            taggingFailed,
            downloadFailed,
            failed,
            other
        }

        public static void clear() {
            List<int> itemsToRemove = new List<int>();
            foreach (KeyValuePair<int, QabuzeDLMThreadStatus> current in status)
            {
                if (current.Value.getStatusAsDLMEvent() == QabuzeDLM.DLMEvent.done
                    || current.Value.getStatusAsDLMEvent() == QabuzeDLM.DLMEvent.failed)
                {
                    itemsToRemove.Add(current.Key);
                }
            }
            foreach (int current in itemsToRemove)
            {
                status.Remove(current);
            }
            updateProgressBar();
            frmDLStat.instance.updateThreadSafe(status);
        }

        private static QabuzeDLMThreadStatus getStatusByThread(int thread) {
            QabuzeDLMThreadStatus tmp;
            try
            {
                tmp = status[thread];
            }
            catch (KeyNotFoundException) {
                tmp = new QabuzeDLMThreadStatus();
                tmp.thread = thread;
            }
            return tmp;
        }

        private static void setAmtSongs(int thread, int songs) {
            QabuzeDLMThreadStatus tmp = getStatusByThread(thread);
            tmp.setAmtSongs(songs);

            while (frmDLStat.instance.updating)
            {
                Thread.Sleep(1);
            }
            status[thread] = tmp;
        }

        private static void updateProgressBar() {
            try
            {
                int totalSongs = 0, doneSongs = 0, failedSongs = 0, percentagePerThread = 100, totalPercentage = 0;
                bool notDoneButFull = false;

                percentagePerThread = (int)Math.Ceiling((double)100 / (double)((status.Count > 0) ? status.Count : 1));

                while (frmDLStat.instance.updating)
                {
                    Thread.Sleep(1);
                }
                foreach (KeyValuePair<int, QabuzeDLMThreadStatus> current in status)
                {
                    totalSongs += current.Value.amtSongs;
                    doneSongs += current.Value.songsDone;
                    failedSongs += current.Value.songsFailed;
                    if (current.Value.getStatusAsDLMEvent() == DLMEvent.done)
                    {
                        totalPercentage += (int)Math.Ceiling((double)current.Value.percentage / (double)100 * (double)percentagePerThread);
                    }
                    else
                    {
                        totalPercentage += (int)Math.Ceiling((double)current.Value.percentage / (double)100 * (double)percentagePerThread);
                    }
                    if (totalSongs > doneSongs + failedSongs) {
                        notDoneButFull = true;
                    }
                }

                totalPercentage = (totalPercentage > 100) ? 100 : totalPercentage;
                totalPercentage = ((totalPercentage >= 100) && notDoneButFull) ? 99 : totalPercentage;

                frmDLStat.instance.setProgressbarThreadSafe(100, totalPercentage);
                frmMain.instance.setProgressbarThreadSafe(100, totalPercentage);
            }
            catch (Exception) { }
        }

        private static void pushStatus(int thread, DLMEvent dlmevent, string logEntry, int song = 0) {
            string prefix = "Thread #" + thread.ToString() + " (" + dlmevent.ToString() + "): ";
            string entry = prefix + logEntry;
            logs.Add(entry);
            Console.WriteLine(entry);

            QabuzeDLMThreadStatus tmp = getStatusByThread(thread);

            if (song == 0) {
                tmp.updateThreadStatus(dlmevent, logEntry);
                if (dlmevent == DLMEvent.done) {
                    string threadDoneEntry = "Total songs: " + tmp.amtSongs.ToString() + " Loaded: " + tmp.songsLoaded.ToString() + " Tagged: " + tmp.songsTagged.ToString() + " Done: " + tmp.songsDone.ToString() + " Failed: " + tmp.songsFailed.ToString();
                    logs.Add(prefix + threadDoneEntry);
                    Console.WriteLine(prefix + threadDoneEntry);
                    tmp.log.Add(threadDoneEntry);
                }
                else if (dlmevent == DLMEvent.failed) {
                    string threadDoneEntry = "Total songs: " + tmp.amtSongs.ToString() + " Loaded: " + tmp.songsLoaded.ToString() + " Tagged: " + tmp.songsTagged.ToString() + " Done: " + tmp.songsDone.ToString() + " Failed: " + tmp.songsFailed.ToString();
                    logs.Add(prefix + threadDoneEntry);
                    Console.WriteLine(prefix + threadDoneEntry);
                    tmp.log.Add(threadDoneEntry);
                }
            } else { 
                tmp.addSongStatus(dlmevent);
            }
            
            while (frmDLStat.instance.updating)
            {
                Thread.Sleep(1);
            }
            status[thread] = tmp;
            
            updateProgressBar();
            frmDLStat.instance.updateThreadSafe(status);

        }

        private static void DownloadAsyncWorker(QabuzeAlbum album)
        {
            #if(WITHDOWNLOAD)
            int currentThread = amtThreads;
            bool tmp = false;
            amtThreads++;
            setAmtSongs(currentThread, album.songs.Count);
            getStatusByThread(currentThread).album = album;
            while (activeThreads >= maxActiveThreads) {
                Thread.Sleep(100);
                if (!tmp)
                {
                    pushStatus(currentThread, DLMEvent.waiting, "Waiting for a slot ...");
                    tmp = true;
                }
            }

            activeThreads++;
            
            WebClient webClient = new WebClient();

            pushStatus(currentThread, DLMEvent.working, "Starting with the download of " + album.songs.Count.ToString() + " songs.");

            KeyValuePair<int, QabuzeSong> kvp_song;
            for (int i = 0; i < album.songs.Count; i++)
            {
                kvp_song = album.songs[i];

                string foldername = (Properties.Settings.Default.outputFolder + "\\" + @Utils.escapeMetaString(@Properties.Settings.Default.folderStructure, album, kvp_song.Value));
                System.IO.Directory.CreateDirectory(foldername);
                if (!System.IO.File.Exists(foldername + "\\folder.jpg"))
                {
                    webClient.DownloadFile(new Uri(album.coverURL), foldername + "\\folder.jpg");
                    pushStatus(currentThread, DLMEvent.working, "Downloaded Cover!");
                } //kvp_song.Value.track_id, kvp_song.Value.getDownloadLink(Properties.Settings.Default.lossless))
                    string filename = Utils.escapeMetaString(Properties.Settings.Default.filenames, album, kvp_song.Value).Replace("\\", "") ;
                    string fullFilename = Utils.truncateIfNecessary(foldername + "\\" + @filename);
                    pushStatus(currentThread, DLMEvent.working, "Now downloading \"" + filename + "\" ...", i + 1);

                    string url = kvp_song.Value.getDownloadLink(true);
                    if (url == null)
                    {
                        pushStatus(currentThread, DLMEvent.downloadFailed, "ERROR with \"" + filename + "\"!", i + 1);
                        continue;            
                    }
                    try
                    {
                        webClient.DownloadFile(new Uri(url), fullFilename);
                        pushStatus(currentThread, DLMEvent.loaded, "Downloading \"" + filename + "\" succeeded!", i + 1);
                    }
                    catch (Exception e)
                    {
                        pushStatus(currentThread, DLMEvent.working, e.InnerException + "\n------\n" + e.Message, i + 1);
                        pushStatus(currentThread, DLMEvent.loaded, "Downloading \"" + filename + "\" failed!", i + 1);
                    }
                    Process proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "metaflac.exe",
                            Arguments = "--set-tag=\"ARTIST=" + kvp_song.Value.artist
                                    + "\" --set-tag=\"TITLE=" + kvp_song.Value.title
                                    + "\" --set-tag=\"ALBUM=" + kvp_song.Value.album
                                    + "\" --set-tag=\"GENRE=" + kvp_song.Value.genre
                                    + "\" --set-tag=\"TRACKNUMBER=" + kvp_song.Value.track_number
                                    + "\" --set-tag=\"ORGANIZATION=" + album.label
                                    + "\" --set-tag=\"TRACKTOTAL=" + album.track_count
                                    + "\" --set-tag=\"ALBUMARTIST=" + album.artist
                                    + "\" --set-tag=\"DISKNUMBER=" + kvp_song.Value.media_number
                                    + "\" --set-tag=\"COMMENT=" + "Created by Quabuze v" + typeof(frmMain).Assembly.GetName().Version
                                    + "\" --import-picture-from=\"" + foldername + "\\folder.jpg" + "\" \"" + fullFilename + "\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };
                    int amtRetries = 0;
                    string buffer = "";
                    bool mayContinue = false;
                    do
                    {
                        proc.Refresh();
                        buffer = "";
                        pushStatus(currentThread, DLMEvent.working, "Now tagging \"" + filename + "\" ...", i + 1);
                        try
                        {
                            proc.Start();
                            while (!proc.StandardOutput.EndOfStream)
                        {
                            buffer += proc.StandardOutput.ReadLine() + "\n";
                        }
                        while (!proc.StandardError.EndOfStream)
                        {
                            buffer += proc.StandardError.ReadLine() + "\n";
                        }
                        Console.Write(buffer);
                        pushStatus(currentThread, DLMEvent.working, "Exitcode of metaflac: " + proc.ExitCode.ToString());
                        amtRetries++;
                        mayContinue = (proc.ExitCode != 0 && amtRetries < 5);
                        }
                        catch (Exception)
                        {
                            pushStatus(currentThread, DLMEvent.taggingFailed, "Tagging \"" + filename + "\" FAILED!", i + 1);
                            mayContinue = false;
                        }

                    } while (mayContinue);
                    try
                    {
                        if (proc.ExitCode == 0)
                        {
                            pushStatus(currentThread, DLMEvent.tagged, "Tagging \"" + filename + "\" done!", i + 1);
                            pushStatus(currentThread, DLMEvent.done, "Done with \"" + filename + "\"!", i + 1);
                        }
                        else
                        {
                            pushStatus(currentThread, DLMEvent.taggingFailed, "Tagging \"" + filename + "\" FAILED!", i + 1);
                        }
                    }
                    catch (Exception)
                    {
                       //If it has to be cached because of proc.ExitCode it already was marked as failed previously. Just catch to prevent a crash.
                    }

                    //pb.Increment(1);

            }
            QabuzeDLMThreadStatus _ct = getStatusByThread(currentThread);
            if (_ct.songsFailed > 0)
            {
                pushStatus(currentThread, DLMEvent.failed, "failure!");
            }
            else
            {
                pushStatus(currentThread, DLMEvent.done, "done!");
            }
            activeThreads--;
            finishedThreads++;
            #endif

        }

        public static void DownloadAsync(QabuzeAlbum album)
        {
            ThreadPool.QueueUserWorkItem(o => DownloadAsyncWorker(album));
        }
    }

    public class QabuzeDLMThreadStatus {

        public bool threadDone = false, threadFailed = false, threadWorking = false, threadWaiting = false;
        public List<string> log = new List<string>();
        public int thread, amtSongs = 0, songsLoaded = 0, songsTagged = 0, songsDone = 0, percentage = 0, songsFailed = 0;
        public QabuzeAlbum album = new QabuzeAlbum();

        public void addSongStatus(QabuzeDLM.DLMEvent dlmevent) {
            switch (dlmevent) {
                case QabuzeDLM.DLMEvent.working:    break; ;
                case QabuzeDLM.DLMEvent.loaded:     songsLoaded++; break;
                case QabuzeDLM.DLMEvent.tagged:     songsTagged++; break;
                case QabuzeDLM.DLMEvent.done:       songsDone++; break;
                case QabuzeDLM.DLMEvent.downloadFailed: //downloadFailed and taggingFailed will result in the same reaction.
                case QabuzeDLM.DLMEvent.taggingFailed: songsFailed++; break;
                default:                            break;
            }
            percentage = (int) Math.Ceiling(((double)songsDone + (double)songsFailed) / (double)amtSongs * (double)100);
            percentage = (percentage > 100) ? 100 : percentage;
        }
        public void setAmtSongs(int amt) {
            amtSongs = amt;        
        }

        public void updateThreadStatus(QabuzeDLM.DLMEvent dlmevent, string log = "") {
            switch (dlmevent)
            {
                case QabuzeDLM.DLMEvent.working:    threadWorking = true; threadDone = false; threadWaiting = false; threadFailed = false; break;
                case QabuzeDLM.DLMEvent.done:       threadWorking = false; threadDone = true; threadWaiting = false; threadFailed = false; break;
                case QabuzeDLM.DLMEvent.waiting:    threadWorking = false; threadDone = false; threadWaiting = true; threadFailed = false; break;
                case QabuzeDLM.DLMEvent.failed:     threadWorking = false; threadDone = true; threadWaiting = false; threadFailed = true; break;
                default:                            break;
            }

            this.log.Add(log);
        }

        public QabuzeDLM.DLMEvent getStatusAsDLMEvent()
        {
            if (threadFailed) {
                return QabuzeDLM.DLMEvent.failed;
            }
            if (threadWorking == true && threadDone == false && threadWaiting == false)
            {
                return QabuzeDLM.DLMEvent.working;
            }
            if (threadWorking == false && threadDone == true && threadWaiting == false)
            {
                return QabuzeDLM.DLMEvent.done;
            }
            if (threadWorking == false && threadDone == false && threadWaiting == true)
            {
                return QabuzeDLM.DLMEvent.waiting;
            }
            return QabuzeDLM.DLMEvent.other;
        }


    }
}
