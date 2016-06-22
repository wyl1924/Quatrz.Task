using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz
{
    /// <summary>
    /// quartz.net对象
    /// 作者：王延领
    /// 时间：2016/5/5
    /// </summary>
    public class QJob : IQJob
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public QJob() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="syscode">系统编码</param>
        /// <param name="id">任务id【需系统内唯一】</param>
        /// <param name="name">任务名称</param>
        /// <param name="group">任务群组</param>
        public QJob(string syscode, string id, string name = "", string group = "")
        {
            JobId = id;
            SysCode = syscode;
            Name = name;
            Group = group;
            Seconds = 60 * 60;
            MaxTimes = 0;
            StartTime = DateTime.Now.AddMinutes(1);
            Handler = null;
            DetailHandler = null;
        }
        public string SysCode { get; set; }
        public string JobId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public int Seconds { get; set; }
        public int MaxTimes { get; set; }
        public DateTime StartTime { get; set; }
        public int Times { get; set; }
        public JobState State { get; set; }
        public JobAction Action { get; set; }
        public DateTime LastTime { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public Action Handler { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public Action<IQJob> DetailHandler { get; set; }
        /// 持久化保存
        /// </summary>

        public bool Save()
        {
            try
            {
                string filepath = JobFactory.Instance.GetPath();
                if (!File.Exists(Path())) return false;
                IQJob myjob = new QJob()
                {
                    SysCode = this.SysCode,
                    JobId = this.JobId,
                    Group = this.Group,
                    Name = this.Name,
                    LastTime = this.LastTime,
                    Handler = this.Handler,
                    MaxTimes = this.MaxTimes,
                    Seconds = this.Seconds,
                    State = this.State,
                    Times = this.Times,
                    StartTime = this.StartTime,
                    DetailHandler = this.DetailHandler,
                    Action = this.Action
                };
                string xml = XMLProcess.Serializer(typeof(QJob), myjob);
                XMLProcess.Write(xml, Path());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Load()
        {
            try
            {
                if (!File.Exists(Path())) return false;
                var job = XMLProcess.Deserialize<QJob>(XMLProcess.ReadXml(Path()));
                JobId = job.JobId;
                SysCode = job.SysCode;
                Name = job.Name;
                Group = job.Group;
                Seconds = job.Seconds;
                MaxTimes = job.MaxTimes;
                StartTime = job.StartTime;
                Times = job.Times;
                State = job.State;
                Action = job.Action;
                LastTime = job.LastTime;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 任务的jobkey规则
        /// </summary>
        /// <returns></returns>
        public string Key()
        {
            return SysCode + "_" + JobId;
        }

        /// <summary>
        /// 开始执行任务
        /// </summary>
        public void Start()
        {
            JobFactory.Instance.Build(this);
        }
        /// <summary>
        /// 开始执行任务
        /// </summary>
        /// <param name="starttime">开始执行时间</param>
        /// <param name="internaltimes">时间间隔(s)</param>
        /// <param name="maxtimes">执行次数</param>
        public void Start(DateTime starttime, int internaltimes = 60*60, int maxtimes = 0)
        {
            StartTime = starttime;
            Seconds = internaltimes;
            MaxTimes = maxtimes;
            JobFactory.Instance.Build(this);
        }
        /// <summary>
        /// 下次执行时间
        /// </summary>
        /// <returns></returns>
        public DateTime NextTime()
        {
            return LastTime.AddSeconds(Seconds);
        }
        /// <summary>
        ///任务触发动作
        /// </summary>
        /// <param name="handler">需要任务信息的动作</param>
        /// <returns>IMyJob</returns>
        public IQJob Handle(Action handler)
        {
            Handler = handler;
            return this;
        }
        /// <summary>
        /// 任务触发动作
        /// </summary>
        /// <param name="handler">需要任务信息的动作</param>
        /// <returns>IMyJob</returns>
        public IQJob Handle(Action<IQJob> handler)
        {
            DetailHandler = handler;
            return this;
        }
        /// <summary>
        /// 持久化地址
        /// </summary>
        /// <returns>【例：../job/syscode_name_group_jobid.xml】</returns>
        public string Path()
        {
            return System.IO.Path.Combine(JobFactory.Instance.GetPath(), string.Format("{0}_{1}_{2}_{3}.xml", SysCode, Group, Name, JobId));
        }
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            return JobFactory.Instance.Remove(Key());
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        public void Pause()
        {
            JobFactory.Instance.Pause(Key());
        }
        /// <summary>
        /// 继续执行任务
        /// </summary>
        public void Resume()
        {
            JobFactory.Instance.Resume(Key());
        }
    }
}

