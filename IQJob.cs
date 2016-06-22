using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quartz
{
    /// <summary>
    /// quartz.net接口对象
    /// 作者：王延领
    /// 时间：2016/5/5
    /// </summary>
    public interface IQJob
    {
        /// <summary>
        /// 系统代码
        /// </summary>
        string SysCode { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        string JobId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 任务分组
        /// </summary>
        string Group { get; set; }
        /// <summary>
        /// 间隔时间
        /// </summary>
        int Seconds { get; set; }
        /// <summary>
        /// 做多执行次数，如果是<1，则无限循环
        /// </summary>
        int MaxTimes { get; set; }
        /// <summary>
        /// 开始执行时间
        /// </summary>
        DateTime StartTime { get; set; }
        /// <summary>
        /// 任务处理器
        /// </summary>
        Action Handler { get; set; }
        /// <summary>
        /// 任务处理器
        /// </summary>
        Action<IQJob> DetailHandler { get; set; }
        /// <summary>
        /// 当前执行的第几次
        /// </summary>
        int Times { get; set; }
        /// <summary>
        /// 接口执行时间
        /// </summary>
        DateTime LastTime { get; set; }
        /// <summary>
        /// 任务的当前状态
        /// </summary>
        JobState State { get; set; }
        /// <summary>
        /// 本次任务执行的动作
        /// </summary>
        JobAction Action { get; set; }
        /// <summary>
        /// 开始执行任务
        /// </summary>
        void Start();
        /// <summary>
        /// 开始执行任务
        /// </summary>
        /// <param name="starttime">任务开始时间</param>
        /// <param name="internaltimes">间隔时间（s）</param>
        /// <param name="maxtimes">执行次数</param>
        void Start(DateTime starttime, int internaltimes = 60*60, int maxtimes = 0);
        /// <summary>
        /// 任务触发动作
        /// 无需参数
        /// </summary>
        /// <param name="action">触发的动作</param>
        /// <returns></returns>
        IQJob Handle(Action handler);
        /// <summary>
        /// 任务触发动作
        /// </summary>
        /// <param name="action">触发的动作</param>
        /// <returns></returns>
        IQJob Handle(Action<IQJob> handler);
        string Key();
        bool Load();
        bool Save();
        /// <summary>
        /// 下次运行时间
        /// </summary>
        DateTime NextTime();
        /// <summary>
        /// 获取job文件地址
        /// </summary>
        /// <returns></returns>
        string Path();
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        bool Remove();
        /// <summary>
        /// 挂起
        /// </summary>
        void Pause();
        /// <summary>
        ///继续执行
        /// </summary>
        void Resume();
    }
}
