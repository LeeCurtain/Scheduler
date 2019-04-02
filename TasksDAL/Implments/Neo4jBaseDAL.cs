using Neo4j.Driver.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TasksCommon;
using TasksDAL.Interface;
using TasksEntity.Neo4jModels;

namespace TasksDAL.Implments
{
    public class Neo4jBaseDAL : INeo4jBaseDAL
    {
        private readonly IDriver _graphDatabase;

        public Neo4jBaseDAL(IDriver graphDatabase)
        {
            _graphDatabase = graphDatabase;
        }

        public IDriver GetDriver()
        {
            return _graphDatabase;
        }

        /// <summary>
        /// 创建node
        /// </summary>
        /// <param name="nodeType">node类型</param>
        /// <param name="id">id</param>
        /// <returns>bool 是否创建成功</returns>
        public bool CreateSingleNode(string nodeType, string id)
        {
            string query = string.Format("CREATE (n:{0} ", nodeType) + @"{id: $id})";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.NodesCreated == 1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 创建关系
        /// </summary>
        /// <param name="relationshipType">关系类型</param>
        /// <param name="sourceType">源node类型</param>
        /// <param name="sourceId">nodeid</param>
        /// <param name="targetType">目标node类型</param>
        /// <param name="targetId">目标node类型</param>
        /// <returns>bool 是否创建成功</returns>
        public bool CreateRelationship(string relationshipType, string sourceType, string sourceId, string targetType, string targetId)
        {
            string relationshipPara = "";
            switch (relationshipType)
            {
                case NeoRelationConst.Friend:
                    relationshipPara = "r:" + relationshipType + @"{since: $date,score: 0}";
                    break;
                case NeoRelationConst.Follow:
                case NeoRelationConst.Block:
                case NeoRelationConst.Employee:
                    relationshipPara = "r:" + relationshipType + @"{since: $date})";
                    break;
                default:
                    relationshipPara = "r:" + relationshipType + @"{since: $date,score: 0}";
                    break;
            }
            string query = string.Format(@"match (n:{0}),(m:{1}) where n.id=$source and m.id=$target create (n)-[{2}]->(m);", sourceType, targetType, relationshipPara);
            using (var session = _graphDatabase.Session())
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { date = DateTime.Now, source = sourceId, target = targetId }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取我的好友列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public PaginatedList<string> GetFriendsByPage(Guid userId, PaginationBase parameters)
        {
            int skip = (parameters.Page - 1) * parameters.PageSize;
            int limit = parameters.PageSize;
            string query = string.Format("match (n:Person)-[r:Friend]-(m:Person) where n.id=$id return m.id ORDER BY r.since DESC SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return new PaginatedList<string>(parameters.Page, parameters.PageSize, 0, resultList);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 获取我的好友列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public List<NeoFriendModel> GetFriends(Guid userId)
        {
            string query = string.Format("match (n:Person)-[r:Friend]-(m:Person) where n.id=$id return m.id,r.since ORDER BY r.since DESC ");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString() }));
                    List<NeoFriendModel> resultList = new List<NeoFriendModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoFriendModel() { UserID = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取我的好友列表与成为好友时间
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public List<NeoFriendModel> GetFriendsWithSince(Guid userId)
        {
            string query = string.Format("match (n:Person)-[r:Friend]-(m:Person) where n.id=$id return m.id,r.since ORDER BY r.since DESC ");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString() }));
                    List<NeoFriendModel> resultList = new List<NeoFriendModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoFriendModel() { UserID = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the friends by page.
        /// </summary>
        /// <returns>The friends by page.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="limit">Limit.</param>
        public List<NeoFriendModel> GetFriendsByPage(Guid userId, DateTime cursor, int skip, int limit)
        {
            string query = string.Format("match(n: Person)-[r:Friend]-(m: Person) where n.id = $id and r.since<$cur return m.id,r.since ORDER BY r.since DESC SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor }));
                    List<NeoFriendModel> resultList = new List<NeoFriendModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoFriendModel() { UserID = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the friends count.
        /// </summary>
        /// <returns>The friends count.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        public int GetFriendsCount(Guid userId, DateTime cursor)
        {
            int count = 0;
            string query = string.Format("match(n: Person)-[r:Friend]-(m: Person) where n.id = $id and r.since<$cur return count(m) ");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString(), cur = cursor }));
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取我的好友数量
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public int GetFriendsCount(Guid userId)
        {
            int count = 0;
            string query = string.Format("match (n:Person)-[r:Friend]-(m:Person)  where n.id=$id  return count(*)  ");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString() }));
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool CheckFriendRelation(Guid userId, Guid friendId)
        {
            string query = string.Format("match (n:Person)-[r:Friend]-(m:Person) where n.id=$id and m.id=$fid return count(*)");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), fid = friendId.ToString().ToUpper() }));
                    //IResultSummary resultSummary = result.Summary;
                    int count = 0;
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 添加好友关系
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="targetUserid">添加的目标用户id</param>
        /// <returns></returns>
        public bool AddFriendRelation(Guid userId, Guid targetUserid, out string message)
        {
            message = "";
            var query = "MATCH (n:Person),(m:Person) WHERE n.id =$id AND m.id =$targetid CREATE(n) -[r: Friend{ since: $since,score:0}]->(m) RETURN r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetUserid.ToString().ToUpper(), since = DateTime.Now }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    //添加日志记录
                    message = "添加好友关系失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 更新用户好友关系
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="targetUserid">好友id</param>
        /// <returns></returns>
        public bool UpdateFriendRelationScore(Guid userId, Guid targetUserid, int score, out string message)
        {
            message = "";
            if (!CheckRelation(NeoRelationConst.Friend, NeoNodeConst.Person, userId.ToString().ToUpper(), NeoNodeConst.Person, targetUserid.ToString().ToUpper()))
            {
                message = "非好友关系，无法更新亲密度";
                return false;
            }
            var query = "match (m:Person) -[r:Friend]- (n:Person)  where m.id = $id and n.id=$targetid set r.score=r.score+$sc";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetUserid.ToString().ToUpper(), sc = score }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.ContainsUpdates;
                }
                catch (Exception ex)
                {
                    message = "更新好友亲密度失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="targetUserId">删除的好友id</param>
        /// <returns></returns>
        public bool DeleteFriend(Guid userId, Guid targetUserId, out string message)
        {
            message = "";
            string query = "match (n:Person)-[r:Friend]-(m:Person) where n.id=$id and m.id=$targetid delete r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetUserId.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsDeleted == 1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 获取所有关注的用户
        /// </summary>
        /// <param name="userId">用户</param>
        /// <returns></returns>
        public List<string> GetFollowUsers(Guid userId)
        {
            string query = string.Format("match (n:Person)-[r:Follow]->(m:Person) where n.id=$id return m.id ORDER BY r.since DESC");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the follows count.
        /// </summary>
        /// <returns>The follows count.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        public int GetFollowsCount(Guid userId, DateTime cursor)
        {
            int count = 0;
            string query = string.Format("match(n:Person)-[r:Follow]->(m:Person) where n.id = $id and r.since<$cur return count(m)");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor }));
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the follow users by page.
        /// </summary>
        /// <returns>The follow users by page.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="limit">Limit.</param>
        public List<NeoFollowModel> GetFollowsByPage(Guid userId, DateTime cursor, int skip, int limit)
        {
            string query = string.Format("match(n:Person)-[r:Follow]->(m:Person) where n.id = $id and r.since<$cur return m.id,r.since ORDER BY r.since DESC SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor }));
                    List<NeoFollowModel> resultList = new List<NeoFollowModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoFollowModel() { UserID = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<string> GetFollowCompanys(Guid userId)
        {
            string query = string.Format("match (n:Person)-[r:Follow]->(m:Company) where n.id=$id return m.id ORDER BY r.since DESC");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the follow company by page.
        /// </summary>
        /// <returns>The follow company by page.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="limit">Limit.</param>
        public List<NeoFollowCompanyModel> GetFollowCompanyByPage(Guid userId, DateTime cursor, int skip, int limit)
        {
            string query = string.Format("match(n:Person)-[r:Follow]->(m:Company) where n.id = $id and r.since<$cur return m.id,r.since ORDER BY r.since DESC SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor }));
                    List<NeoFollowCompanyModel> resultList = new List<NeoFollowCompanyModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoFollowCompanyModel() { Id = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool AddFollowUserRelation(Guid userId, string targetUserId, out string message)
        {
            message = "";
            var query = "MATCH (n:Person),(m:Person) WHERE n.id =$id AND m.id =$targetid CREATE (n) -[r: Follow{ since: $since}]->(m) RETURN r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetUserId.ToString().ToUpper(), since = DateTime.Now }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    message = "关注用户失败，请稍后再试";
                    return false;
                }
            }
        }


        /// <summary>
        /// 取消对用户的关注
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="targetUserId">取消关注的用户</param>
        /// <returns></returns>
        public bool DeleteFollowUser(Guid userId, Guid targetUserId, out string message)
        {
            message = "";
            string query = "match (n:Person)-[r:Follow]->(m:Person) where n.id=$id and m.id=$targetid delete r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetUserId.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsDeleted == 1;
                }
                catch (Exception ex)
                {
                    message = "关注用户失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 添加对企业的关注
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="targetCompanyId">企业id</param>
        /// <returns></returns>
        public bool AddFollowCompanyRelation(Guid userId, string targetCompanyId, out string message)
        {
            message = "";
            var query = "MATCH (n:Person),(m:Company) WHERE n.id =$id AND m.id =$targetid CREATE (n) -[r: Follow{ since: $since}]->(m) RETURN r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetCompanyId.ToString().ToUpper(), since = DateTime.Now }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    message = "添加Follow关系失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 取消对企业的关注
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="targetCompanyId">企业id</param>
        /// <returns></returns>
        public bool DeleteFollowCompanyRelation(Guid userId, Guid targetCompanyId, out string message)
        {
            message = "";
            string query = "match (n:Person)-[r:Follow]->(m:Company) where n.id=$id and m.id=$targetid delete r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetCompanyId.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsDeleted == 1;
                }
                catch (Exception ex)
                {
                    message = "关注用户失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 添加人员与企业关系
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="targetCompanyId">企业Id</param>
        /// <param name="userTitle">用户Title</param>
        /// <returns></returns>
        public bool AddEmployeeRelation(Guid userId, Guid targetCompanyId, string userTitle, out string message)
        {
            message = "";
            if (CheckRelation(NeoRelationConst.Employee, NeoNodeConst.Person, userId.ToString().ToUpper(), NeoNodeConst.Company, targetCompanyId.ToString().ToUpper()))
            {
                message = "与企业关系已存在";
                return false;
            }
            var query = "MATCH (n:Person),(m:Company) WHERE n.id =$id AND m.id =$targetid CREATE (n) -[r: Employee{ since: $since,title: $title}]->(m) RETURN r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetCompanyId.ToString().ToUpper(), since = DateTime.Now, title = userTitle.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    message = "关注企业失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 更新用户在企业中的Title
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="targetCompanyId">公司id</param>
        /// <param name="userTitle">title</param>
        /// <returns></returns>
        public bool UpdateEmployeeRelationTitle(Guid userId, Guid targetCompanyId, string userTitle, out string message)
        {
            message = "";
            if (!CheckRelation(NeoRelationConst.Friend, NeoNodeConst.Person, userId.ToString().ToUpper(), NeoNodeConst.Company, targetCompanyId.ToString().ToUpper()))
            {
                message = "非雇佣关系，无法更新Title";
                return false;
            }
            var query = "match (m:Person) -[r:Employee]- (n:Company)  where m.id = $id and n.id=$targetid set r.title=$title";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetCompanyId.ToString().ToUpper(), since = DateTime.Now, title = userTitle.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.ContainsUpdates;
                }
                catch (Exception ex)
                {
                    message = "更新雇佣关系失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取所有粉丝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetFollowers(Guid userId)
        {
            string query = string.Format("match (n:Person)-[r:Follow]->(m:Person) where m.id=$id return n.id ORDER BY r.since DESC");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the followers count.
        /// </summary>
        /// <returns>The followers count.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        public int GetFollowersCount(Guid userId, string industry, string region, DateTime cursor)
        {
            int count = 0;
            string query = string.Format("match(n:Person)-[r:Follow]->(m:Person) where m.id = $id and r.since<$cur with n");
            if (!string.IsNullOrWhiteSpace(industry))
            {
                query = query + string.Format(" match (n)-[ri:Belong]->(i:Industry) where i.id=$inid with n");
            }

            if (!string.IsNullOrWhiteSpace(industry))
            {
                query = query + string.Format(" match (n)-[rr:Belong]->(re:Region) where re.id=$rid with n");
            }
            query = query + string.Format(" return count(n.id)");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor, inid = industry, rid = region }));
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<NeoFollowerModel> GetFollowersByPage(Guid userId, string industry, string region, DateTime cursor, int skip, int limit)
        {
            string query = string.Format("match(n:Person)-[r:Follow]->(m:Person) where m.id = $id and r.since<$cur with n,r");
            if (!string.IsNullOrWhiteSpace(industry))
            {
                query = query + string.Format(" match (n)-[ri:Belong]->(i:Industry) where i.id=$inid with n,r");
            }

            if (!string.IsNullOrWhiteSpace(industry))
            {
                query = query + string.Format(" match (n)-[rr:Belong]->(re:Region) where re.id=$rid with n,r");
            }
            query = query + string.Format(" return n.id,r.since ORDER BY r.since DESC SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor, inid = industry, rid = region }));
                    List<NeoFollowerModel> resultList = new List<NeoFollowerModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoFollowerModel() { UserID = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the followers by page.
        /// </summary>
        /// <returns>The followers by page.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="limit">Limit.</param>
        public List<NeoFollowerModel> GetFollowersByPage(Guid userId, DateTime cursor, int skip, int limit)
        {
            string query = string.Format("match(n:Person)-[r:Follow]->(m:Person) where m.id = $id and r.since<$cur return n.id,r.since ORDER BY r.since DESC SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor }));
                    List<NeoFollowerModel> resultList = new List<NeoFollowerModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoFollowerModel() { UserID = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 获取好友关系路径
        /// </summary>
        /// <param name="sourceId">源nodeid</param>
        /// <param name="targetId">目标nodeid</param>
        /// <returns>路径集合</returns>
        public NeoRelationPathModel GetUserPath(Guid sourceId, Guid targetId, int start, int deep, int limit)
        {
            string query = string.Format("match p=(n:Person)-[:Friend*{0}..{1}]-(m:Person) where n.id=$sourceid and m.id=$targetid return p order by p limit {2}", start, deep, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { sourceid = sourceId.ToString().ToUpper(), targetid = targetId.ToString().ToUpper() }));
                    var rows = result.ToList();
                    NeoRelationPathModel neoRelationPath = new NeoRelationPathModel() { PathCount = rows.Count(), Nodes = new List<NeoPathModel>() };
                    foreach (var row in rows)
                    {
                        NeoPathModel neoPath = new NeoPathModel() { NodeList = new List<string>() };
                        //每个数据行都包含多个数据列
                        var columns = row.Values;
                        foreach (var column in columns)
                        {
                            //每个数据列，可能是一个节点，也可能是一个标量值
                            if (column.Key == "p")
                            {
                                var nodes = ((IPath)column.Value).Nodes;
                                foreach (var node in nodes)
                                {
                                    foreach (var property in node.Properties)
                                    {
                                        neoPath.NodeList.Add(property.Value.ToString());
                                    }
                                }
                            }
                        }
                        neoRelationPath.Nodes.Add(neoPath);
                    }
                    return neoRelationPath;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取好友关系路径数量
        /// </summary>
        /// <param name="sourceId">源nodeid</param>
        /// <param name="targetId">目标nodeid</param>
        /// <returns>路径集合</returns>
        public int GetUserPathCount(Guid sourceId, Guid targetId, int start, int deep, int limit)
        {
            string query = string.Format("match p=(n:Person)-[:Friend*{0}..{1}]-(m:Person) where n.id=$sourceid and m.id=$targetid return p order by p limit {2}", start, deep, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { sourceid = sourceId.ToString().ToUpper(), targetid = targetId.ToString().ToUpper() }));
                    var rows = result.ToList();
                    int count = 0;
                    count = rows.Count();
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public int GetUserPathCount(Guid sourceId, Guid targetId)
        {
            string query = "match p=(n:Person)-[:Friend*]-(m:Person) where n.id=$sourceid and m.id=$targetid return count(p)";
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { sourceid = sourceId.ToString().ToUpper(), targetid = targetId.ToString().ToUpper() }));
                    int count = 0;
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取公司的雇员
        /// </summary>
        /// <param name="companyId">公司id</param>
        /// <returns>雇员列表</returns>
        public List<NeoEmployeePersonModel> GetEmployeeByCompany(Guid companyId)
        {
            string query = "match (n:Person)-[r:Employee]-(c:Company) where c.id=$id return n,r";
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = companyId.ToString().ToUpper() }));
                    List<NeoEmployeePersonModel> resultList = new List<NeoEmployeePersonModel>();

                    foreach (var record in result)
                    {
                        var nodeProps = JsonConvert.SerializeObject(record[0].As<INode>().Properties);
                        var relationProps = JsonConvert.SerializeObject(record[1].As<IRelationship>().Properties);
                        resultList.Add(new NeoEmployeePersonModel()
                        {
                            User = JsonConvert.DeserializeObject<NeoPersonModel>(nodeProps),
                            Employee = JsonConvert.DeserializeObject<NeoEmployeeRelationModel>(relationProps)
                        });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 查询用户有关联的公司
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public List<NeoEmployeeCompanyModel> GetUserCompanies(Guid userId)
        {
            string query = "match (n:Person)-[r:Employee]->(c:Company) where n.id=$id return c,r";
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper() }));
                    List<NeoEmployeeCompanyModel> resultList = new List<NeoEmployeeCompanyModel>();

                    foreach (var record in result)
                    {
                        var nodeProps = JsonConvert.SerializeObject(record[0].As<INode>().Properties);
                        var relationProps = JsonConvert.SerializeObject(record[1].As<IRelationship>().Properties);
                        resultList.Add(new NeoEmployeeCompanyModel()
                        {
                            Company = JsonConvert.DeserializeObject<NeoCompanyModel>(nodeProps),
                            Employee = JsonConvert.DeserializeObject<NeoEmployeeRelationModel>(relationProps)
                        });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 查询共同好友
        /// </summary>
        /// <param name="userId1">用户id1</param>
        /// <param name="userId2">用户id2</param>
        /// <returns></returns>
        public PaginatedList<string> GetUserMutualFriends(Guid userId1, Guid userId2, PaginationBase parameters)
        {
            int skip = (parameters.Page - 1) * parameters.PageSize;
            int limit = parameters.PageSize;
            string query = string.Format("match (m:Person) -[r:Friend]- (n1)  where m.id =$userId1  with n1 match (m: Person) -[r: Friend] - (n2) where m.id = $userId2 and n1 = n2  return n2.id SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { userId1 = userId1.ToString().ToUpper(), userId2 = userId2.ToString().ToUpper() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return new PaginatedList<string>(parameters.Page, parameters.PageSize, 0, resultList);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public bool AddBlockUserRelation(Guid userId, string targetUserId, out string message)
        {
            message = "";
            var query = "MATCH (n:Person),(m:Person) WHERE n.id =$id AND m.id =$targetid CREATE (n) -[r: Block{ since: $since}]->(m) RETURN r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetUserId.ToString().ToUpper(), since = DateTime.Now }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    message = "屏蔽用户失败，请稍后再试";
                    return false;
                }
            }
        }

        public bool DeleteBlockUserRelation(Guid userId, string targetUserId, out string message)
        {
            message = "";
            string query = "match (n:Person)-[r:Block]->(m:Person) where n.id=$id and m.id=$targetid delete r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), targetid = targetUserId.ToString().ToUpper(), since = DateTime.Now }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsDeleted == 1;
                }
                catch (Exception ex)
                {
                    message = "取消屏蔽失败，请稍后再试";
                    return false;
                }
            }
        }

        public List<string> GetBlockUsers(Guid userId)
        {
            string query = string.Format("match (n:Person)-[r:Block]->(m:Person) where n.id=$id return m.id ORDER BY r.since DESC");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the blocks count.
        /// </summary>
        /// <returns>The blocks count.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        public int GetBlocksCount(Guid userId, DateTime cursor)
        {
            int count = 0;
            string query = string.Format("match(n: Person)-[r:Block]->(m: Person) where n.id = $id and r.since<$cur return count(m)");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor }));
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets the blocks by page.
        /// </summary>
        /// <returns>The blocks by page.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="cursor">Cursor.</param>
        /// <param name="skip">Skip.</param>
        /// <param name="limit">Limit.</param>
        //public List<string> GetBlocksByPage(Guid userId, string cursor, int skip, int limit)
        //{
        //    string query = string.Format("match(n: Person)-[r:Block]->(m: Person) where n.id = $id with m ORDER BY r.since DESC,m.id DESC");
        //    if (cursor != "")
        //    {
        //        query += string.Format("  where m.id < $curid ");
        //    }
        //    query += string.Format(" return m.id SKIP {0} LIMIT {1}", skip, limit);

        //    using (var session = _graphDatabase.Session(AccessMode.Read))
        //    {
        //        try
        //        {
        //            var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), curid = cursor.ToUpper() }));
        //            List<string> resultList = new List<string>();
        //            foreach (var record in result)
        //            {
        //                resultList.Add(record[0].ToString());
        //            }
        //            return resultList;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //}


        public List<NeoBlockUserModel> GetBlocksByPage(Guid userId, DateTime cursor, int skip, int limit)
        {
            string query = string.Format("match(n: Person)-[r:Block]->(m: Person) where n.id = $id and r.since<$cur return m.id,r.since ORDER BY r.since DESC SKIP {0} LIMIT {1}", skip, limit);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToUpper(), cur = cursor }));
                    List<NeoBlockUserModel> resultList = new List<NeoBlockUserModel>();
                    foreach (var record in result)
                    {
                        resultList.Add(new NeoBlockUserModel() { UserID = record[0].ToString(), Since = DateTime.Parse(record[1].ToString()) });
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public bool CheckRelation(string relationshipType, string sourceType, string sourceId, string targetType, string targetId)
        {
            string relationshipPara = "";
            switch (relationshipType)
            {
                case NeoRelationConst.Friend:
                case NeoRelationConst.Employee:
                    relationshipPara = "-[r:" + relationshipType + "]-";
                    break;
                case NeoRelationConst.Follow:
                case NeoRelationConst.Block:
                case NeoRelationConst.Belong:
                    relationshipPara = "-[r:" + relationshipType + "]->";
                    break;
                default:
                    relationshipPara = "-[r:" + relationshipType + "]->";
                    break;
            }
            string query = string.Format(@"match (n:{0}){1}(m:{2}) where n.id=$source and m.id=$target return r", sourceType, relationshipPara, targetType);
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(tx => tx.Run(query, new { source = sourceId.ToUpper(), target = targetId.ToUpper() }));
                    return result.Count() > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 添加用户所在地区关系
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="regionId">地区id</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool AddUserRegionRelation(Guid userId, string regionId, out string message)
        {
            message = "";
            if (CheckRelation(NeoRelationConst.Belong, NeoNodeConst.Person, userId.ToString().ToUpper(), NeoNodeConst.Region, userId.ToString().ToUpper()))
            {
                message = "用户与地区关系已存在";
                return false;
            }
            var query = "MATCH (n:Person),(m:Region) WHERE n.id =$id AND m.id =$regionid CREATE (n) -[r: Belong]->(m) RETURN r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), regionid = regionId.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    message = "添加用户所在地区失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除用户地区关系
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="regionId">地区id</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool DeleteUserRegionRelation(Guid userId, string regionId, out string message)
        {
            message = "";
            if (!CheckRelation(NeoRelationConst.Belong, NeoNodeConst.Person, userId.ToString().ToUpper(), NeoNodeConst.Region, regionId.ToString().ToUpper()))
            {
                message = "不存在该用户与该地区的关系";
                return false;
            }
            var query = "match (n:Person)-[r:Belong]->(m:Region) where n.id=$id and m.id=$regionid delete r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), regionid = regionId.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    message = "删除用户所在地区失败，请稍后再试";
                    return false;
                }
            }
        }

        public int GetUserBusinessNetworkInfoCount(Guid userId, int deep)
        {
            string query = string.Format("match (n:Person)-[r:Friend*..$deepcount]-(m:Person{id:$id}) return count(distinct(n.id))");
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { deepcount = deep.ToString().ToUpper(), id = userId.ToString().ToUpper() }));
                    int count = 0;
                    foreach (var record in result)
                    {
                        count = int.Parse(record[0].ToString());
                    }
                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<string> GetUserBusinessNetworkUsers(Guid userId, int start, int deep, string region, string industry)
        {
            //"match  (n:Person)-[r:Friend*2..6]-(m:Person{id:'ADEC6617-5170-490B-8A57-A876C7C580AD'}) with n match (n)-[r1:Belong*..3]->(o:Region{id:'310000'}) with n match (n)-[r2:Belong*..3]->(p:Industry{id:'2_1'}) return distinct(n)"
            string query = string.Format("match (n:Person)-[r:Friend*{0}..{1}]-(m:Person) where n.id=$id ", start, deep);
            if (region != "")
            {
                query = query + " with m match (m)-[r1:Belong*..3]->(o:Region{id:'" + region + "'})";
            }
            if (industry != "")
            {
                query = query + " with m match (m)-[r2:Belong*..3]->(p:Industry{id:'" + industry + "'})";
            }
            query = query + " return distinct(m.id)";

            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = userId.ToString().ToLower() }));
                    List<string> users = new List<string>();
                    foreach (var record in result)
                    {
                        users.Add(record[0].ToString());
                    }
                    return users;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public int BatchCreateFriendRelation(Guid userId, List<string> ids)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("f", userId.ToString().ToUpper());
            param.Add("t", ids);
            param.Add("since", DateTime.Now);
            var query = "MATCH(from: Person) WHERE from.id = $f MATCH(to: Person) where to.id in $t create(from) -[r:Friend{since:$since,score:0}]->(to)";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(rx => rx.Run(query, param));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 建立用户行业关系
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="industryId">行业id</param>
        /// <returns></returns>
        public bool AddUserIndustryBelongRelation(Guid userId, string industryId, out string message)
        {
            message = "";
            if (CheckRelation(NeoRelationConst.Belong, NeoNodeConst.Person, userId.ToString().ToUpper(), NeoNodeConst.Industry, userId.ToString().ToUpper()))
            {
                message = "关系已存在";
                return false;
            }
            var query = "MATCH (n:Person),(m:Industry) WHERE n.id =$id AND m.id =$industryid CREATE (n) -[r: Belong]->(m) RETURN r";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(tx => tx.Run(query, new { id = userId.ToString().ToUpper(), industryid = industryId.ToString().ToUpper() }));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated == 1;
                }
                catch (Exception ex)
                {
                    message = "添加用户所在地区失败，请稍后再试";
                    return false;
                }
            }
        }

        /// <summary>
        /// 根据行业查找用户
        /// </summary>
        /// <param name="industryId">行业id</param>
        /// <returns></returns>
        public List<string> GetUserByIndustry(string industryId)
        {
            var query = "match p=(n:Person)-[r:Belong*..3]-(m:Industry) where m.id=$id return distinct(n.id)";
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = industryId.ToString().ToUpper() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 根据地区查找用户
        /// </summary>
        /// <param name="regionId">地区id</param>
        /// <returns></returns>
        public List<string> GetUserByRegion(string regionId)
        {
            var query = "match p=(n:Person)-[r:Belong*..4]-(m:Region) where m.id=$id return distinct(n.id)";
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { id = regionId.ToString().ToUpper() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 根据地区，行业信息查询用户
        /// </summary>
        /// <param name="regionId">地区id</param>
        /// <param name="industryId">行业id</param>
        /// <returns></returns>
        public List<string> GetUserByRegionAndIndustry(string regionId, string industryId)
        {
            string query = string.Format("match (n:Person)-[r:Belong*..3]->(m:Region) where m.id=$regionid  ");
            query = query + " with n match (n)-[r2:Belong*..3]->(p:Industry) where p.id=$industryid";
            query = query + " return distinct(n.id)";
            using (var session = _graphDatabase.Session(AccessMode.Read))
            {
                try
                {
                    var result = session.ReadTransaction(rx => rx.Run(query, new { regionid = regionId.ToString(), industryid = industryId.ToString() }));
                    List<string> resultList = new List<string>();
                    foreach (var record in result)
                    {
                        resultList.Add(record[0].ToString());
                    }
                    return resultList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        /// <summary>
        /// 创建用户熟悉领域关系
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int BatchCreateUserFieldRelation(Guid userId, List<string> ids)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("f", userId.ToString().ToUpper());
            param.Add("t", ids);
            var query = "MATCH(from: Person) WHERE from.id = $f MATCH(to: Industry) where to.id in $t create(from) -[r:Field]->(to)";
            using (var session = _graphDatabase.Session(AccessMode.Write))
            {
                try
                {
                    var result = session.WriteTransaction(rx => rx.Run(query, param));
                    IResultSummary rs = result.Summary;
                    return rs.Counters.RelationshipsCreated;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}