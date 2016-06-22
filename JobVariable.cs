using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quartz
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum JobState
    {
        Working = 0,
        Pending = 1,
        Empty = 2
    }
    /// <summary>
    /// 任务动作
    /// </summary>
    public enum JobAction
    {

        NewOne = 0,
        Excute = 1,
        Delete = 2,
        Pend = 3,
        Resume = 4
    }
    public class JobVariables
    {
        /// <summary>
        /// 任务调度集合
        /// </summary>
        private static List<IMyJob> _jobs = new List<IMyJob>();
        public static List<IMyJob> jobs
        {
            get
            {
                return _jobs;
            }
        }
        /// <summary>
        /// 任务触发动作集合
        /// </summary>
        private static List<JobExcuteHandler> _excutehandlers = new List<JobExcuteHandler>();
        public static List<JobExcuteHandler> GetHandlers
        {
            get
            {
                return _excutehandlers;
            }
        }
    }
    public class JobExcuteHandler
    {
        public string systme_code { get; set; }
        public Action<IQJob> detailexcute { get; set; }
        public Action excute { get; set; }
    }
}
