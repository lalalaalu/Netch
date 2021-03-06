﻿using System;
using System.IO;
using System.Linq;
using WindowsFirewallHelper;

namespace Netch.Utils
{
    public static class Firewall
    {
        private const string Netch = "Netch";
        private static readonly string[] ProgramPath =
        {
            "bin/NTT.exe",
            "bin/Privoxy.exe",
            "bin/Shadowsocks.exe",
            "bin/ShadowsocksR.exe",
            "bin/Trojan.exe",
            "bin/tun2socks.exe",
            "bin/xray.exe",
            "Netch.exe"
        };

        /// <summary>
        ///     Netch 自带程序添加防火墙
        /// </summary>
        public static void AddNetchFwRules()
        {
            try
            {
                var rule = FirewallManager.Instance.Rules.FirstOrDefault(r => r.Name == Netch);
                if (rule != null)
                {
                    if (rule.Name.StartsWith(Global.NetchDir))
                        return;
                    RemoveNetchFwRules();
                }

                foreach (var p in ProgramPath)
                {
                    var path = Path.GetFullPath(p);
                    if (File.Exists(path))
                        AddFwRule(Netch, path);
                }
            }
            catch (Exception e)
            {
                Logging.Warning("添加防火墙规则错误(如已关闭防火墙则可无视此错误)\n" + e);
            }
        }

        /// <summary>
        ///     清除防火墙规则 (Netch 自带程序)
        /// </summary>
        public static void RemoveNetchFwRules()
        {
            try
            {
                foreach (var rule in FirewallManager.Instance.Rules.Where(r => r.Name == Netch))
                    FirewallManager.Instance.Rules.Remove(rule);
            }
            catch (Exception e)
            {
                Logging.Warning("清除防火墙规则错误\n" + e);
            }
        }

        #region 封装

        private static void AddFwRule(string ruleName, string exeFullPath)
        {
            var rule = FirewallManager.Instance.CreateApplicationRule(ruleName, FirewallAction.Allow, exeFullPath);
            rule.Direction = FirewallDirection.Inbound;

            FirewallManager.Instance.Rules.Add(rule);
        }

        #endregion
    }
}