using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace FlexNetParse
{
    
    public class FNParse
    {

        private static string _server;
        private static string _lmutilpath;


        public FNParse() { 
        
        }

        public void setServer(string _value){
            _server = _value;
        }

        public void setlmutilpath(string _value) {
            _lmutilpath = _value;
        }


        public string FNData()
        {
            List<String> _features = GetFeatures();

            String _html = "";

            foreach (String _str in _features)
            {

                _html += GetUsage(_str);

            }

            return _html;

        }

        public Dictionary<string, Dictionary<int, string>> FNDataClean()
        {
            List<String> _features = GetFeatures();

            //String _html = "";
            Dictionary<string, Dictionary<int, string>> _data = new Dictionary<string, Dictionary<int, string>>(); 


            foreach (String _str in _features)
            {

                //_html += GetUsageClean(_str);
                _data.Add(_str, GetUsageClean(_str));

            }

            return _data;

        }

        public string FNUsageCount()
        {
            List<String> _features = GetFeatures();

            String _html = "";

            foreach (String _str in _features)
            {

                _html += GetUsageCount(_str);

            }

            return _html;

        }


        public static List<String> GetFeatures()
        {

            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = _lmutilpath;
            p.StartInfo.Arguments = " lmstat -i -c " + _server;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            List<String> _features = new List<string>(50);

            //extract features from the string
            using (StringReader reader = new StringReader(output))
            {
                string line;
                string _out = "";

                while ((line = reader.ReadLine()) != null)
                {

                   

                    if (line.IndexOf("adskflex") != -1)
                    {
                        _out += line + Environment.NewLine;
                        string[] _featuresstr = Regex.Split(line, "     ");
                        _features.Add(_featuresstr[0]);

                    }

                    if (line.IndexOf("ARCGIS") != -1)
                    {
                        _out += line + Environment.NewLine;

                        
                        _features.Add("VIEWER");
                        _features.Add("ACT");
                        _features.Add("TIN");
                        _features.Add("GRID");
                        _features.Add("ARC/INFO");
                        _features.Add("ArcStorm");
                        _features.Add("ArcStormEnable");
                        _features.Add("MrSID");
                        _features.Add("Plotting");
                        _features.Add("TIFFLZW");
                        _features.Add("spatialAnalystP");
                        _features.Add("desktopBasicP");



                        break;

                    }

                    if (line.IndexOf("bmgeo") != -1)
                    {
                        _out += line + Environment.NewLine;

                        string[] _featuresstr = Regex.Split(line, "     ");
                        _features.Add(_featuresstr[0]);
                       
                    }
         


                }



            }

            


            return _features;

        }

        public static String cleanupNames(String _str){

            String _out = _str.Replace("Viewer", "ArcView Desktop Basic")
                              .Replace("ARC/INFO", "ArcView Desktop Advanced")
                              .Replace("TIN", "3D Analyst")
                              .Replace("Grid", "Spatial Analyst")
                              .Replace("spatialAnalystP", "Spatial Analyst Pro")
                              .Replace("desktopBasicP", "ArcView Desktop Basic Pro")
                              ;

            return _out;

        }

        public static String GetUsage(string _featureName)
        {
            string _out = "";

            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = _lmutilpath;
            p.StartInfo.Arguments = " lmstat -c " + _server + " -f " + _featureName;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            //extract usage details from the string
            using (StringReader reader = new StringReader(output))
            {
                string line;


                int _cnt = 1;
                string _head = "", _body = "<table>";

                while ((line = reader.ReadLine()) != null)
                {

                    

                    //Console.WriteLine(_cnt +") "+line);
                    if (line.IndexOf("in use)") != -1)
                    {
                        line = cleanupNames(line);
                        _body += "<caption>" + line + "</caption>" + Environment.NewLine;
                    }

                    if (line.IndexOf("start") != -1)
                    {
                        _body += "<tr><td>" + line + "</td></tr>" + Environment.NewLine;

                    }


                    _cnt++;

                }

                _out += _body + "</table>" + Environment.NewLine;

            }

            return _out;


        }

        public static Dictionary<int,string> GetUsageClean(string _featureName)
        {
            string _out = "";
            Dictionary<int, string> _data = new Dictionary<int, string>(); 

            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = _lmutilpath;
            p.StartInfo.Arguments = " lmstat -c " + _server + " -f " + _featureName;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            //extract usage details from the string
            using (StringReader reader = new StringReader(output))
            {
                string line;

                int _cnt = 1;
                string _head = "", _body = "";

                while ((line = reader.ReadLine()) != null)
                {

                    //Console.WriteLine(_cnt +") "+line);
                    if (line.IndexOf("in use)") != -1)
                    {
                        line = cleanupNames(line);
                        
                        line = line.Replace("Users of ","");
                        _head = line;
                        //_body += "<caption>" + line + "</caption>" + Environment.NewLine;
                    }

                    if (line.IndexOf("start") != -1)
                    {
                        _data.Add(_cnt, line);
                        _body += line + Environment.NewLine;

                    }


                    _cnt++;

                }

                _out += _body + Environment.NewLine;

            }

            return _data;


        }

        public static String GetUsageCount(string _featureName)
        {

            string _str = String.Empty;

            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = _lmutilpath;
            p.StartInfo.Arguments = " lmstat -c " + _server + " -f " + _featureName;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            //extract usage details from the string
            using (StringReader reader = new StringReader(output))
            {
                string line;


                int _cnt = 1;


                while ((line = reader.ReadLine()) != null)
                {


                    //Console.WriteLine(_cnt +") "+line);
                    if (line.IndexOf("in use)") != -1)
                    {
                        line = cleanupNames(line);
                        line = Regex.Replace(line.Replace(_featureName, ""), "[^0-9 ]", "");

                        Console.WriteLine(line);

                        String[] _tmp = Regex.Split(line, "      ");

                        _str += _featureName + ":";
                        _str += _tmp[1] + ":" +_tmp[2].Trim()+ ",";
                        
                        Array.Clear(_tmp,0,_tmp.Length);
                    }

                    if (line.IndexOf("start") != -1)
                    {


                    }


                    _cnt++;

                }



            }

            return _str;

        }

        public String getServerStatus()
        {

            string _out = "";


            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = _lmutilpath;
            p.StartInfo.Arguments = " lmstat -s -c " + _server;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

          

            //extract features from the string
            using (StringReader reader = new StringReader(output))
            {
                string line;
               

                while ((line = reader.ReadLine()) != null)
                {


                    if (line.IndexOf("adskflex:") != -1)
                    {
                        _out += line.Replace(':',' ') + Environment.NewLine;

                    }

                    if (line.IndexOf("ARCGIS:") != -1)
                    {
                        _out += line.Replace(':', ' ') + Environment.NewLine;

                    }

                    if (line.IndexOf("bmgeo:") != -1)
                    {
                        _out += line.Replace(':', ' ') + Environment.NewLine;

                    }

                }



            }

            return _out;


        }

        


        


    }


}
