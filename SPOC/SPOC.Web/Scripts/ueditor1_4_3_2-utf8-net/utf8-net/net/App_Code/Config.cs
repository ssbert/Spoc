using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Config 的摘要说明
/// </summary>
public static class Config
{
    private static bool noCache = true;
    private static JObject BuildItems()
    {
        var json = File.ReadAllText(HttpContext.Current.Server.MapPath("config.json"));
        var config = JsonConvert.DeserializeObject<UeConfig>(json);
        var host = "http://" + HttpContext.Current.Request.Url.Authority;
        config.imageUrlPrefix = host;
        config.scrawlUrlPrefix = host;
        config.snapscreenUrlPrefix = host;
        config.catcherUrlPrefix = host;
        config.videoUrlPrefix = host;
        config.fileUrlPrefix = host;
        config.imageManagerUrlPrefix = host;
        config.fileManagerUrlPrefix = host;
        json = JsonConvert.SerializeObject(config);
        return JObject.Parse(json);
    }

    public static JObject Items
    {
        get
        {
            if (noCache || _Items == null)
            {
                _Items = BuildItems();
            }
            return _Items;
        }
    }
    private static JObject _Items;


    public static T GetValue<T>(string key)
    {
        return Items[key].Value<T>();
    }

    public static String[] GetStringList(string key)
    {
        return Items[key].Select(x => x.Value<String>()).ToArray();
    }

    public static String GetString(string key)
    {
        return GetValue<String>(key);
    }

    public static int GetInt(string key)
    {
        return GetValue<int>(key);
    }
}

/// <summary>
/// config.js的序列化
/// </summary>
public class UeConfig
{
    public string imageActionName { get; set; }
    public string imageFieldName { get; set; }
    public int imageMaxSize { get; set; }
    public List<string> imageAllowFiles { get; set; }
    public bool imageCompressEnable { get; set; }
    public int imageCompressBorder { get; set; }
    public string imageInsertAlign { get; set; }
    public string imageUrlPrefix { get; set; }
    public string imagePathFormat { get; set; }
    public string scrawlActionName { get; set; }
    public string scrawlFieldName { get; set; }
    public string scrawlPathFormat { get; set; }
    public int scrawlMaxSize { get; set; }
    public string scrawlUrlPrefix { get; set; }
    public string scrawlInsertAlign { get; set; }
    public string snapscreenActionName { get; set; } /* 执行上传截图的action名称 */
    public string snapscreenPathFormat { get; set; } /* 上传保存路径{get;set;}可以自定义保存路径和文件名格式 */
    public string snapscreenUrlPrefix { get; set; } /* 图片访问路径前缀 */
    public string snapscreenInsertAlign { get; set; } /* 插入的图片浮动方式 */
    public List<string> catcherLocalDomain { get; set; }
    public string catcherActionName { get; set; } /* 执行抓取远程图片的action名称 */
    public string catcherFieldName { get; set; } /* 提交的图片列表表单名称 */
    public string catcherPathFormat { get; set; } /* 上传保存路径{get;set;}可以自定义保存路径和文件名格式 */
    public string catcherUrlPrefix { get; set; } /* 图片访问路径前缀 */
    public int catcherMaxSize { get; set; } /* 上传大小限制，单位B */
    public List<string> catcherAllowFiles { get; set; } /* 抓取图片格式显示 */
    public string videoActionName { get; set; } /* 执行上传视频的action名称 */
    public string videoFieldName { get; set; } /* 提交的视频表单名称 */
    public string videoPathFormat { get; set; } /* 上传保存路径{get;set;}可以自定义保存路径和文件名格式 */
    public string videoUrlPrefix { get; set; } /* 视频访问路径前缀 */
    public int videoMaxSize { get; set; } /* 上传大小限制，单位B，默认100MB */
    public List<string> videoAllowFiles { get; set; } /* 上传视频格式显示 */

    public string fileActionName { get; set; } /* controller里{get;set;}执行上传视频的action名称 */
    public string fileFieldName { get; set; } /* 提交的文件表单名称 */
    public string filePathFormat { get; set; } /* 上传保存路径{get;set;}可以自定义保存路径和文件名格式 */
    public string fileUrlPrefix { get; set; } /* 文件访问路径前缀 */
    public int fileMaxSize { get; set; } /* 上传大小限制，单位B，默认50MB */
    public List<string> fileAllowFiles { get; set; } /* 上传文件格式显示 */
    public string imageManagerActionName { get; set; } /* 执行图片管理的action名称 */
    public string imageManagerListPath { get; set; } /* 指定要列出图片的目录 */
    public int imageManagerListSize { get; set; } /* 每次列出文件数量 */
    public string imageManagerUrlPrefix { get; set; } /* 图片访问路径前缀 */
    public string imageManagerInsertAlign { get; set; } /* 插入的图片浮动方式 */
    public List<string> imageManagerAllowFiles { get; set; } /* 列出的文件类型 */
    public string fileManagerActionName { get; set; } /* 执行文件管理的action名称 */
    public string fileManagerListPath { get; set; } /* 指定要列出文件的目录 */
    public string fileManagerUrlPrefix { get; set; } /* 文件访问路径前缀 */
    public int fileManagerListSize { get; set; } /* 每次列出文件数量 */
    public List<string> fileManagerAllowFiles { get; set; } /* 列出的文件类型 */
}