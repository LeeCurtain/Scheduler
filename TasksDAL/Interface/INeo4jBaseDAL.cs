using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Text;
using TasksEntity.Neo4jModels;

namespace TasksDAL.Interface
{
    public interface INeo4jBaseDAL
    {
        IDriver GetDriver();
        bool CheckRelation(string relationshipType, string sourceType, string sourceId, string targetType, string targetId);
        bool AddEmployeeRelation(Guid userId, Guid targetCompanyId, string userTitle, out string message);
        bool AddFollowCompanyRelation(Guid userId, string targetCompanyId, out string message);
        bool AddFriendRelation(Guid userId, Guid targetUserid, out string message);
        bool CreateRelationship(string relationshipType, string sourceType, string sourceId, string targetType, string targetId);
        bool CreateSingleNode(string nodeType, string id);
        bool DeleteFollowCompanyRelation(Guid userId, Guid targetCompanyId, out string message);

        bool AddFollowUserRelation(Guid userId, string targetUserId, out string message);
        bool DeleteFollowUser(Guid userId, Guid targetUserId, out string message);
        bool DeleteFriend(Guid userId, Guid targetUserId, out string message);

        List<NeoEmployeePersonModel> GetEmployeeByCompany(Guid companyId);
        List<string> GetFollowers(Guid userId);
        int GetFollowersCount(Guid userId, string industry, string region, DateTime cursor);
        List<NeoFollowerModel> GetFollowersByPage(Guid userId, DateTime cursor, int skip, int limit);
        List<NeoFollowerModel> GetFollowersByPage(Guid userId, string industry, string region, DateTime cursor, int skip, int limit);
        List<NeoFollowCompanyModel> GetFollowCompanyByPage(Guid userId, DateTime cursor, int skip, int limit);
        List<string> GetFollowUsers(Guid userId);
        int GetFollowsCount(Guid userId, DateTime cursor);
        List<NeoFollowModel> GetFollowsByPage(Guid userId, DateTime cursor, int skip, int limit);
        List<string> GetFollowCompanys(Guid userId);
        List<NeoFriendModel> GetFriendsByPage(Guid userId, DateTime cursor, int skip, int limit);
        List<NeoFriendModel> GetFriends(Guid userId);
        int GetFriendsCount(Guid userId, DateTime cursor);
        List<NeoFriendModel> GetFriendsWithSince(Guid userId);
        bool CheckFriendRelation(Guid userId, Guid friendId);
        List<NeoEmployeeCompanyModel> GetUserCompanies(Guid userId);
        NeoRelationPathModel GetUserPath(Guid sourceId, Guid targetId, int start, int deep, int limit);
        /// <summary>
        /// 获取好友关系路径数量
        /// </summary>
        /// <param name="sourceId">源nodeid</param>
        /// <param name="targetId">目标nodeid</param>
        /// <returns>路径集合</returns>
        int GetUserPathCount(Guid sourceId, Guid targetId, int start, int deep, int limit);

        int GetUserPathCount(Guid sourceId, Guid targetId);
        bool UpdateEmployeeRelationTitle(Guid userId, Guid targetCompanyId, string userTitle, out string message);
        bool UpdateFriendRelationScore(Guid userId, Guid targetUserid, int score, out string message);

        bool AddBlockUserRelation(Guid userId, string targetUserId, out string message);
        bool DeleteBlockUserRelation(Guid userId, string targetUserId, out string message);

        List<string> GetBlockUsers(Guid userId);
        int GetBlocksCount(Guid userId, DateTime cursor);
        List<NeoBlockUserModel> GetBlocksByPage(Guid userId, DateTime cursor, int skip, int limit);

        int GetUserBusinessNetworkInfoCount(Guid userId, int deep);

        List<string> GetUserBusinessNetworkUsers(Guid userId, int start, int deep, string region, string industry);

        int BatchCreateFriendRelation(Guid userId, List<string> ids);
        int BatchCreateUserFieldRelation(Guid userId, List<string> ids);

        List<string> GetUserByIndustry(string industryId);
        List<string> GetUserByRegion(string regionId);

        List<string> GetUserByRegionAndIndustry(string regionId, string industryId);

        /// <summary>
        /// 获取我的好友数量
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        int GetFriendsCount(Guid userId);
    }
}
