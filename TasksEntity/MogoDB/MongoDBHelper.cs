using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace TasksEntity.MogoDB
{
    public class MongoDataBaseContext
    {
        public IMongoClient _client = null;
        public IMongoDatabase _database = null;
        public MongoDataBaseContext(MongodbHost host)
        {
            MongoClientSettings mongoSettings = new MongoClientSettings();
            MongoCredential credentials = MongoCredential.CreateCredential(host.DataBase, host.UserName, host.PassWord);//添加用户名、密码
            mongoSettings.Credential = credentials;
            mongoSettings.Server = new MongoServerAddress(host.Host, host.Port);//服务器地址
            mongoSettings.ReadPreference = new ReadPreference(ReadPreferenceMode.Primary);
            _client = new MongoClient(mongoSettings);
            _database = _client.GetDatabase(host.DataBase);
        }
    }

    public class MongodbHost
    {
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; set; }
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the pass word.
        /// </summary>
        /// <value>The pass word.</value>
        public string PassWord { get; set; }
        /// <summary>
        /// Gets or sets the data base.
        /// </summary>
        /// <value>The data base.</value>
        public string DataBase { get; set; }
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public string Table { get; set; }
    }
}