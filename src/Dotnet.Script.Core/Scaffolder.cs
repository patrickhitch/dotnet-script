﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Dotnet.Script.Core.Templates;
using Dotnet.Script.DependencyModel.Environment;
using Newtonsoft.Json.Linq;

namespace Dotnet.Script.Core
{
    public class Scaffolder
    {
        public void InitializerFolder(string fileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string vsCodeDirectory = Path.Combine(currentDirectory, ".vscode");
            if (!Directory.Exists(vsCodeDirectory))
            {
                Directory.CreateDirectory(vsCodeDirectory);
            }

            string pathToLaunchFile = Path.Combine(vsCodeDirectory, "launch.json");
            if (!File.Exists(pathToLaunchFile))
            {
                string baseDirectory = Path.GetDirectoryName(new Uri(typeof(Scaffolder).GetTypeInfo().Assembly.CodeBase).LocalPath);
                string csxPath = Path.Combine(baseDirectory, "dotnet-script.dll").Replace(@"\", "/");
                                
                string lauchFileTemplate = TemplateLoader.ReadTemplate("launch.json.template");

                string launchFileContent = lauchFileTemplate.Replace("PATH_TO_DOTNET-SCRIPT", csxPath);
                WriteFile(pathToLaunchFile, launchFileContent);
            }
            
            string pathToOmniSharpJson = Path.Combine(currentDirectory, "omnisharp.json");
            if (!File.Exists(pathToOmniSharpJson))
            {
                var omniSharpFileTemplate = TemplateLoader.ReadTemplate("omnisharp.json.template");
                JObject settings = JObject.Parse(omniSharpFileTemplate);
                settings["script"]["defaultTargetFramework"] = RuntimeHelper.TargetFramework;
                WriteFile(pathToOmniSharpJson, settings.ToString());
            }

            if (Directory.GetFiles(currentDirectory, "*.csx").Any()
                && string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            CreateNewScriptFile(fileName ?? "main.csx");
        }

        public void CreateNewScriptFile(string file)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            var pathToScriptFile = Path.Combine(currentDirectory, file);
            if (!File.Exists(pathToScriptFile))
            {
                var scriptFileTemplate = TemplateLoader.ReadTemplate("helloworld.csx.template");
                WriteFile(pathToScriptFile, scriptFileTemplate);
            }
        }

        private void WriteFile(string path, string content)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(content);
                }
            }
        }
    }
}