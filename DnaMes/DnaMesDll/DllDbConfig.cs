﻿// ****************************************************
//  Author: Charles Ma
//  Date: 2018/02/09 15:11
// ****************************************************
//  Copyright © DNA Studio 2018. All rights reserved.
// ****************************************************

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DnaMesModel.Config;
using DnaMesModel.Global;
using SqlSugar;

namespace DnaMesDll
{
    /// <summary>
    ///     DllDataConfig类
    /// </summary>
    public static class DllDbConfig
    {
        #region 私有字段

        private static readonly Configuration Config =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        #endregion

        #region 公有属性

        #endregion

        #region 私有方法

        private static List<DbInfo> GetDbInfos()
        {
            return (Config.GetSection("DbConfig") as DbConfig)?.DbCollection.Cast<DbInfo>().ToList();
        }

        #endregion

        #region 公有方法

        #region DataBase.config

        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public static DbInfo GetDbInfo(DbInfoName name)
        {
            return GetDbInfos().Find(db => db.Name == name) ?? new DbInfo();
        }

        /// <summary>
        /// 保存数据库配置
        /// </summary>
        /// <param name="dbInfo"></param>
        public static void SetDbInfo(this DbInfo dbInfo)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var dbConfig = (DbConfig) config.GetSection("DbConfig");
            dbConfig.DbCollection.Remove(dbInfo.Name);
            dbConfig.DbCollection.Add(dbInfo);
            config.Save();
        }

        /// <summary>
        /// Retrieves a connection string by name.
        /// Returns null if the name is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete("暂不使用该方法", true)]
        public static string GetConnectionString(DbInfoName name)
        {
            // Assume failure.
            string returnValue = null;
            // Look for the name in the connectionStrings section.
            var settings = ConfigurationManager.ConnectionStrings[name.ToString()];
            // If found, return the connection string.
            if (settings != null)
                returnValue = settings.ConnectionString;
            return returnValue;
        }

        ///<summary>
        ///更新连接字符串
        ///</summary>
        ///<param name="newName">连接字符串名称</param>
        ///<param name="newConString">连接字符串内容</param>
        ///<param name="newProviderName">数据提供程序名称</param>
        [Obsolete("暂不使用该方法", true)]
        public static void UpdateConnectionString(DbInfoName newName, string newConString,
            string newProviderName = "System.Data.SqlClient")
        {
            //记录该连接串是否已经存在
            var isModified = ConfigurationManager.ConnectionStrings[newName.ToString()] != null;
            //新建一个连接字符串实例
            var mySettings = new ConnectionStringSettings(newName.ToString(), newConString, newProviderName);
            // 打开可执行的配置文件*.exe.config
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // 如果连接串已存在，首先删除它
            if (isModified)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(newName.ToString());
            }

            // 将新的连接串添加到配置文件中.
            config.ConnectionStrings.ConnectionStrings.Add(mySettings);
            // 保存对配置文件所作的更改
            config.Save(ConfigurationSaveMode.Modified);
            // 强制重新载入配置文件的ConnectionStrings配置节
            ConfigurationManager.RefreshSection(nameof(ConfigurationManager.ConnectionStrings));
        }

        #endregion

        #endregion
    }
}