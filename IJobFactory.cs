using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz
{
    /// <summary>
    /// quartz.net任务调度接口
    /// 作者：王延领
    /// 日期：2016/5/5
    /// </summary>
    public interface IJobFactory
    {
        /// <summary>
        /// 任务触发的动作
        /// </summary>
        /// <param name="syscode">系统编码</param>
        /// <param name="hander">需要参数的触发动作</param>
        void Trigger(string syscode, Action<IQJob> hander);
        /// <summary>
        /// 任务触发的动作
        /// </summary>
        /// <param name="syscode">系统编码</param>
        /// <param name="hander">无需参数的触发动作</param>
        void Trigger(string syscode, Action hander);
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="job"> IQJob</param>
        /// <returns></returns>
        string Build(IQJob job);
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="jobid">IMyJob.Key()</param>
        /// <returns></returns>
        bool Remove(string jobkey);
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        bool Pause(string jobkey);
        /// <summary>
        /// 继续任务
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        bool Resume(string jobkey);
        /// <summary>
        /// 任务是否存在
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        bool Exists(string jobkey);
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="systcode">系统编码</param>
        void RemoveAll(string systcode);
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="syscode">系统编码</param>
        void PauseAll(string syscode);
        /// <summary>
        /// 继续任务
        /// </summary>
        /// <param name="syscode">系统编码</param>
        void ResumeAll(string syscode);
        /// <summary>
        /// 任务数
        /// </summary>
        /// <param name="syscode">系统编码</param>
        /// <param name="state">任务状态</param>
        /// <returns></returns>
        int JobCount(string syscode, JobState state);
        /// <summary>
        /// 任务数
        /// </summary>
        /// <returns></returns>
        int JobCount();
        /// <summary>
        /// 任务状态
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        JobState State(string jobkey);
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        IMyJob Find(string jobkey);
        /// <summary>
        /// 任务初始化
        /// </summary>
        void Initialize();
    }
}
