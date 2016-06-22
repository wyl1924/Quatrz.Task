using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Quartz
{
    public class MyJob : QJob, IMyJob
    {
        public MyJob() { }
        public MyJob(IQJob qjob)
        {
            this.Action = qjob.Action;
            this.SysCode = qjob.SysCode;
            this.JobId = qjob.JobId;
            this.Group = qjob.Group;
            this.Name = qjob.Name;
            this.LastTime = qjob.LastTime;
            this.MaxTimes = qjob.MaxTimes;
            this.Seconds = qjob.Seconds;
            this.State = qjob.State;
            this.Times = qjob.Times;
            this.StartTime = qjob.StartTime;
            this.DetailHandler = qjob.DetailHandler;
            this.Handler = qjob.Handler;
        }
        /// <summary>
        ///任务执行时触发动作
        /// </summary>
        public void Excute()
        {
            try
            {
                Times++;
                LastTime = DateTime.Now;
                Action = JobAction.Excute;
                if (MaxTimes == 1)
                {
                    XMLProcess.Delete(Path());
                    MaxTimes = 0;
                    Trigger();
                    return;
                }
                if (MaxTimes != 0)
                    MaxTimes--;
                Save();
                Trigger();
            }
            catch (Exception ex)
            { }
        }
        /// <summary>
        ///任务暂停时触发动作
        /// </summary>
        public void QPause()
        {
            Action = JobAction.Pend;
            State = JobState.Pending;
            Save();
            Trigger();
        }
        /// <summary>
        /// 任务继续时触发动作
        /// </summary>
        public void QResume()
        {
            Action = JobAction.Resume;
            State = JobState.Working;
            Save();
            Trigger();
        }
        /// <summary>
        /// 任务移除触发动作
        /// </summary>
        public void QRemove()
        {
            XMLProcess.Delete(Path());
            Action = JobAction.Delete;
            Trigger();
        }   /// <summary>
        /// <summary>
        /// 动作触发
        /// </summary>
        /// <param name="myjob">JobDetail</param>
        void Trigger()
        {
            if (Handler != null) { Handler(); return; }
            if (DetailHandler != null) { DetailHandler(this); return; }
            //获取订阅委托列表
            var sh = JobVariables.GetHandlers.SingleOrDefault(h => h.systme_code == SysCode);
            if (sh.detailexcute != null)
            {
                sh.detailexcute(this);
                return;
            }
            if (sh.excute != null)
                sh.excute();
        }
    }
}
