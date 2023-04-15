namespace Lambot.Adapters.OneBot;

/// <summary>
/// 通知类型
/// </summary>
public enum NoticeType
{
    /// <summary>
    /// 群文件上传
    /// </summary>
    GroupUpload,
    /// <summary>
    /// 群管理变更
    /// </summary>
    GroupAdmin,
    /// <summary>
    /// 群成员减少
    /// </summary>
    GroupDecrease,
    /// <summary>
    /// 群成员增加
    /// </summary>
    GroupIncrease,
    /// <summary>
    /// 群成员禁言
    /// </summary>
    GroupBan,
    /// <summary>
    /// 好友添加
    /// </summary>
    FriendAdd,
    /// <summary>
    /// 群消息撤回
    /// </summary>
    GroupRecall,
    /// <summary>
    /// 好友消息撤回
    /// </summary>
    FriendRecall,
    /// <summary>
    /// 群名片变更
    /// </summary>
    GroupCard,
    /// <summary>
    /// 离线文件上传
    /// </summary>
    OfflineFile,
    /// <summary>
    /// 客户端状态变更
    /// </summary>
    ClientStatus,
    /// <summary>
    /// 精华消息
    /// </summary>
    Essence,
    /// <summary>
    /// 系统通知
    /// </summary>
    Notify
}
