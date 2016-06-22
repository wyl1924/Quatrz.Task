using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz
{
    /// <summary>
    /// quartz.net接任务调度口实现
    /// 作者：王延领
    /// 时间：2016/5/5
    /// </summary>
    public class JobFactory : IJobFactory
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        private static JobFactory _Instance = new JobFactory();
        public static JobFactory Instance
        {
            get
            {
                return _Instance;
            }
        }
        public JobFactory()
        {
            ssf = new StdSchedulerFactory();
            _scheduler = ssf.GetScheduler();
        }
        ISchedulerFactory ssf;
        IScheduler _scheduler;
        /// <summary>
        /// 任务持久化文件保存地址
        /// 注：默认保存在@"Files\jobs\"文件夹下
        /// 直接地址结尾加"\"
        /// </summary>
        private string _path { get; set; }
        public void SetPath(string path)
        {
            _path = path;
        }
        public string GetPath()
        {
            if (string.IsNullOrEmpty(_path))
                _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.ToString(), @"Files\jobs\");
            return _path;
        }
        //<summary>
        //创建任务
        //</summary>
        //<param name="myjob">任务对象</param>
        public string Build(IQJob qjob)
        {
            IMyJob myjob = new MyJob(qjob);
            if (JobVariables.jobs.Exists(j => j.JobId == myjob.JobId && j.SysCode == myjob.SysCode)) return "任务与存在！！！";
            JobAdd(myjob);
            IJobDetail jobdetail = Create_Jobdetail(myjob);
            ISimpleTrigger trigger = Create_Trigger(myjob);
            _scheduler.ScheduleJob(jobdetail, trigger);
            if (_scheduler.IsShutdown || _scheduler.InStandbyMode)
                _scheduler.Start();
            StandSave(qjob);
            return qjob.Key();
        }
        /// <summary>
        /// 创建jobdetail
        /// </summary>
        /// <param name="qjob">
        /// 默认执行Create_Execute
        /// </param>
        /// <returns></returns>
        protected IJobDetail Create_Jobdetail(IMyJob qjob)
        {
            IJobDetail jobdetail = JobBuilder.Create<Create_Job>()
                .WithIdentity(qjob.JobId, qjob.SysCode)
                .Build();
            return jobdetail;
        }
        /// <summary>
        /// 创建job触发器
        /// </summary>
        /// <param name="qjob"></param>
        /// <returns></returns>
        protected ISimpleTrigger Create_Trigger(IMyJob qjob)
        {
            ISimpleTrigger trigger;
            trigger = (ISimpleTrigger)TriggerBuilder.Create().WithIdentity(qjob.JobId, qjob.SysCode)
                                                 .StartAt(qjob.StartTime).WithSimpleSchedule(x => x.WithIntervalInSeconds(qjob.Seconds)
                                                  .WithRepeatCount(qjob.MaxTimes - 1))
                                                  .Build();
            return trigger;
        }
        /// <summary>
        /// 创建任务执行
        /// </summary>
        public class Create_Job : IJob
        {
            public void Execute(Quartz.IJobExecutionContext context)
            {
                IMyJob myjob = JobFactory.Instance.Find(context.JobDetail.Key);
                myjob.Load();
                if (myjob.State != JobState.Working) return;
                JobFactory.Instance.JobRemove(myjob);
                myjob.Excute();
                JobFactory.Instance.JobAdd(myjob);
            }
        }
        /// <summary>
        /// 从任务列表中删除指定对象
        /// </summary>
        /// <param name="myjob"></param>
        /// <returns></returns>
        bool JobRemove(IMyJob myjob)
        {
            return JobVariables.jobs.Remove(myjob);
        }
        /// <summary>
        /// 向任务列表中添加指定对象
        /// </summary>
        /// <param name="myjob"></param>
        void JobAdd(IMyJob myjob)
        {
            JobVariables.jobs.Insert(0, myjob);
        }
        /// <summary>
        /// 获取MyJob
        /// </summary>
        /// <param name="jobkey">JobDetail.JobKey</param>
        /// <returns></returns>
        IMyJob Find(JobKey jobkey)
        {
            return JobVariables.jobs.SingleOrDefault(j => j.JobId == jobkey.Name && j.SysCode == jobkey.Group);
        }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        public IMyJob Find(string jobkey)
        {
            var job = JobVariables.jobs.SingleOrDefault(j => j.Key() == jobkey);
            return job;
        }
        /// <summary>
        /// 初始化任务
        /// </summary>
        public void Initialize()
        {
            string[] array = XMLProcess.GetFiles();
            if (array == null) return;
            foreach (var path in array)
            {
                IQJob myjob = XMLProcess.Deserialize(typeof(QJob), XMLProcess.ReadXml(path)) as QJob;
                IMyJob qjob = new MyJob(myjob);
                JobFactory.Instance.Build(myjob);
                DateTime nowtime = Convert.ToDateTime(string.Format("{0}:{1}", DateTime.Now.Hour, DateTime.Now.Minute));
                DateTime jobtime = Convert.ToDateTime(string.Format("{0}:{1}", myjob.StartTime.Hour, qjob.StartTime.Minute));
                if (DateTime.Compare(nowtime, Convert.ToDateTime(jobtime)) > 0)
                    DoJob(qjob);
            }
        }
        /// <summary>
        /// 立即执行job
        /// </summary>
        /// <param name="job"></param>
        void DoJob(IMyJob myjob)
        {
            try
            {
                JobRemove(myjob);
                if (myjob.State != JobState.Working) return;
                //获取订阅委托列表
                myjob.Excute();
                JobAdd(myjob);
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        /// 任务持久保存
        /// </summary>
        /// <param name="job"></param>
        protected void StandSave(IQJob job)
        {
            if (File.Exists(job.Path())) return;
            job.State = JobState.Working;
            job.Action = JobAction.NewOne;
            string xml = XMLProcess.Serializer(typeof(QJob), job);
            XMLProcess.Write(xml, job.Path());
        }
        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        public List<IMyJob> FindAll()
        {
            return JobVariables.jobs;
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        public bool Remove(string jobkey)
        {
            var myjob = Find(jobkey); ;
            if (myjob == null) return false;
            return JobsRemove(myjob);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="syscode"></param>
        public void RemoveAll(string syscode)
        {
            for (int i = JobVariables.jobs.Count - 1; i >= 0; i--)
            {
                if (JobVariables.jobs[i].SysCode == syscode)
                    JobsRemove(JobVariables.jobs[i]);
            }
        }
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="myjob">IQjob</param>
        /// <returns></returns>
        bool JobsRemove(IMyJob myjob)
        {
            try
            {
                bool flag = _scheduler.DeleteJob(new JobKey(myjob.JobId, myjob.SysCode));
                if (flag)
                {
                    JobRemove(myjob);
                    myjob.QRemove();
                }
                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        ///  暂停任务
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        public bool Pause(string jobkey)
        {
            try
            {
                var myjob = Find(jobkey); ;
                if (myjob == null) return false;
                return JobsPause(myjob);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="syscode">系统编码</param>
        public void PauseAll(string syscode)
        {
            for (int i = JobVariables.jobs.Count - 1; i >= 0; i--)
            {
                if (JobVariables.jobs[i].SysCode == syscode)
                    JobsPause(JobVariables.jobs[i]);
            }
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="myjob"></param>
        /// <returns></returns>
        bool JobsPause(IMyJob myjob)
        {
            try
            {
                _scheduler.PauseJob(new JobKey(myjob.JobId, myjob.SysCode));
                JobRemove(myjob);
                myjob.QPause();
                JobAdd(myjob);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 继续任务
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        public bool Resume(string jobkey)
        {
            var myjob = Find(jobkey); ;
            if (myjob == null) return false;
            return JobsResume(myjob);
        }
        /// <summary>
        /// 继续任务
        /// </summary>
        /// <param name="syscode">系统编码</param>
        public void ResumeAll(string syscode)
        {
            for (int i = JobVariables.jobs.Count - 1; i >= 0; i--)
            {
                if (JobVariables.jobs[i].SysCode == syscode)
                    JobsResume(JobVariables.jobs[i]);
            }
        }
        bool JobsResume(IMyJob myjob)
        {
            try
            {
                _scheduler.ResumeJob(new JobKey(myjob.JobId, myjob.SysCode));
                JobRemove(myjob);
                myjob.QResume();
                JobAdd(myjob);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 任务是否存在
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        public bool Exists(string jobkey)
        {
            return JobVariables.jobs.Exists(j => j.Key() == jobkey);
        }
        /// <summary>
        /// 任务状态
        /// </summary>
        /// <param name="jobkey">IMyJob.Key()</param>
        /// <returns></returns>
        public JobState State(string jobkey)
        {
            var myjob = Find(jobkey);
            if (myjob == null) return JobState.Empty;
            return myjob.State;
        }
        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <returns>所有任务数量</returns>
        public int JobCount()
        {
            return JobVariables.jobs.Count;
        }
        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <param name="syscode">系统编码</param>
        /// <param name="state">任务状态</param>
        /// <returns>指定任务数量</returns>
        public int JobCount(string syscode, JobState state)
        {
            return JobVariables.jobs.FindAll(j => j.State == state && j.SysCode == syscode).Count;
        }
        public void Trigger(string system_code, Action<IQJob> action)
        {
            var sh = JobVariables.GetHandlers.SingleOrDefault(h => h.systme_code == system_code);
            if (sh == null) sh = new JobExcuteHandler() { systme_code = system_code };
            JobVariables.GetHandlers.Remove(sh);
            sh.detailexcute = action;
            JobVariables.GetHandlers.Add(sh);
        }
        public void Trigger(string system_code, Action action)
        {
            var sh = JobVariables.GetHandlers.SingleOrDefault(h => h.systme_code == system_code);
            if (sh == null) sh = new JobExcuteHandler() { systme_code = system_code };
            JobVariables.GetHandlers.Remove(sh);
            sh.excute = action;
            JobVariables.GetHandlers.Add(sh);
        }
    }
}

