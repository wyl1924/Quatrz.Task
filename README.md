# Quatrz.Task
 quartz.net插件类库封装
 
 博客园地址：http://www.cnblogs.com/kmonkeywyl/p/5542467.html
 
 
1、前言

　 　最近项目需要做一写任务作业调度的工作，最终选择了quartz.net这个插件，它提供了巨大的灵活性而不牺牲简单性。你能够用它来为执行一个作业而 创建简单的或复杂的调度。它有很多特征，如：数据库支持，集群，插件，支持cron-like表达式等等.对于quartz.net在这就不进行过的介绍 了，下面针对这个插件的封装具体如下。

quartz.net的封装主要包括：

　　　　1.任务的基本操作（创建，删除，暂停，继续，状态查询，数量查询等）

 

　　　　2.任务执行触发动作的回调，其中回调用有两种委托雷响Action,Action<IQjob>

 

　　　　3.持久化的处理，持久化文件保存到xml文件中（一个任务一个xml文件）

 
2、定义对象接口

　　对象分为对位接口（IQJob）和内部操作接口(IMJob).除了对象本身，接口还包括对对象的一些简单操作，比如Remove,pause,Resume等.这样的目的是为了让对象更便与操作。


3、quartz.net具体实现封装接口

　　quartz.net对的封装主要包括：

　　　　1.任务的基本操作（创建，删除，暂停，继续，状态查询，数量查询等）

　　　　2.任务执行触发动作的回调，其中回调用有两种委托雷响Action,Action<IQjob>

　　　　3.持久化的处理，持久化文件保存到xml文件中（一个任务一个xml文件）

4、对象接口的实现　

此接口的实现主要包括两部分：

　1.对外操作的创建，删除，暂停，继续，状态和个数的查询，对象load和sava

   2.执行job操作是进行动作触发的操作。
View Code
5、quartz.net接口实现

1.创建任务的时候主要有散布操作：

　　　　1.创建jobdetail

　　　　2.创建jobtrigger

　　　　3.创建jobexcute

2.对任务的操作都会执行相应的静态变量jobs【list<IMJob>】

3.持久化操作包操作数据保存到xml中
View Code
6、job操作中的静态变量

包括三部分数据：

    1.任务的相关的枚举【状态，和动作】

     2.任务列表

     3.触发动作列表

7、XML序列化

主要包括xml的序列化和反序列化操作


使用说明文档：

使用流程：

  　  1.添加web.config

　　　　

　　 2.在使用处调用类库方法：

　　　　2.1获取任务调度执行时间间隔与开始执行时间（这些值可以在代码中直接赋值）

 

<!--自动任务执行时间间隔（秒）by wyl-->
    <add key="ReDoPaySchedulerTimeBySeconds" value="86400" />
    <!--自动任务执行 by wyl-->
    <add key="ReDoPaySchedulerStartTime" value="3:10"/>

 

　　　　2.2创建任务调度

new QJob("standardsd", "jobid", "轮询缴费", "XX").Handle(DoRePay).Start(startTime, times, 0);

　　　　注：standardsd：系统编码（syscode）

          　　 Jobid：任务id(jobid)

         　　  轮询缴费：任务名称(name)

          　　 山东大学:任务群组名称(group)

          　　 DoRePay：任务调度触发动作。(action)

         　　 startTime：任务开始时间(starttime)

         　　  times：任务间隔时间(internaltimes)

         　　  0:最大执行次数（0表示无限次）(maxtimes)    

　　3.在不停止服务端的情况下暂停与继续任务

　　　　　修改任务持久化文件，默认地址为~/Files/jobs/

　　　　　当State为Working时表示任务正常执行

　　　　　当State为Pending/Empty时表示任务正常执行(注意大小写，写错则按Working执行)
调用说明：

调用说明

对象初始化
	

New QJob(string syscode, string id, string name = "", string group = "")

参数说明
	

Syscode:系统编码

Id:jobid

Name:任务名称

Group:任务群组

 

其他说明
	

Syscode+jobid：标志者任务的唯一性

Name,group：可为空但建议写上，以便于更容易操作任务持久化文件

StartTime：开始时间默认为 DateTime.Now.AddMinutes(1)

Maxtimes:最大执行次数默认为0

Internaltimes:时间间隔默认为1个小时

持久化文件存放地址赋值
	

JobFactory.Instance.SetPath(string path)

 

参数说明
	

文件路径必须以@“\”结尾，不进行赋值默认地址为~\File\jobs\

任务触发动作
	

 Handle(Action handler)和Handle(Action<IQJob> handler)

参数说明
	

两者参数的却别在于一个有回调参数（IQJOB），一个没有参数

创建任务（1）
	

 Start(DateTime starttime, int internaltimes , int maxtimes)

参数说明
	

Starttime：开始时间

internaltimes ：任务执行间隔

maxtimes ：最大执行次数

 

创建任务（2）
	

 Start()

参数说明
	

直接执行创建job任务调度

移除任务
	

IQJob.Remove()

说明
	

根据IQJob进行删除（syscoed与jobid确定其唯一性）

暂停任务
	

IQJob.Pause()

说明
	

根据IQJob进行暂停（syscoed与jobid确定其唯一性）

继续任务
	

IQJob.Resume()

说明
	

根据IQJob进行Resume（syscoed与jobid确定其唯一性）

修改任务触发动作
	

 JobFactory.Instance.Trigger(string system_code, Action action)

 JobFactory.Instance.Trigger(string system_code, Action<IQJob> action)

参数说明
	

system_code：系统编码

Action：触发动作

持久化操作
	

   JobFactory.Instance.Initialize();

说明
	

获取持久化文件，进行反序列化，然后把对象进行任务调度创建。

 

注意：

（1）系统编码和任务id组合必须唯一。

（2）开始执行时间最好在系统操作不频繁时间段（在服务平台和山东大学我用的是3：10）

（3）创建任务调度时，如果当前时间大于开始时间，任务在没创建之前就会事先执行一次。所以希望在创建任务的时候开始时间不要用datetime.now。用指定时间或者用DateTime.Now.AddMinutes(1)

（4）基于（3）的说明，开始时间如果指定为2016-06-06 3：10 ，当在这时间之后如果服务器down掉，重新启动系统时，不管是不是在3：10之后，系统均会执行一次
案例：
	
（1） new QJob("syscode", "jobid", "name", "group").Handle(job_handler).Start(DateTime.Now.AddSeconds(30), 2, 10);
 
 （2） new QJob("syscode", "jobid", "name", "group").Handle(job_detail_handler).Start();
 
（3） new QJob("syscode", "jobid", "name", "group").Remove();
 
（4） new QJob("syscode", "jobid").Remove();
 
（5） new QJob("syscode", "jobid", "name", "group").Pause();
 
（6） new QJob("syscode", "jobid").Pause();
 
（7） new QJob("syscode", "jobid", "name", "group").Resume();
 
（8） new QJob("syscode", "jobid").Resume();

 

