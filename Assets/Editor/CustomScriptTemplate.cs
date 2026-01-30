using UnityEngine;
using System.IO;
public class CustomScriptTemplate : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset(string newFileMeta)
    {
        if (newFileMeta.EndsWith(".cs")) { return; }
        string newFilePath = newFileMeta.Replace(".meta", "");
        if (!newFilePath.EndsWith(".cs") ){ return; }

        string realPath = Application.dataPath.Replace("Assets", "") + newFilePath; 
        string scriptContent = File.ReadAllText(realPath);
        //这里实现自定义的一些规则
        scriptContent = scriptContent.Replace("#SCRIPTNAME#", Path.GetFileName(newFilePath));
        scriptContent = scriptContent.Replace("#COMPANYNAME#", "CompanyName");
        scriptContent = scriptContent.Replace("#AUTHOR#", "_SourceCode");
        scriptContent = scriptContent.Replace("#VERSION#", "1.0");
        scriptContent = scriptContent.Replace("#UNITYVERSION#", Application.unityVersion);
        scriptContent = scriptContent.Replace("#CREATETIME#", System.DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")); File.WriteAllText(realPath, scriptContent);
    }
}
