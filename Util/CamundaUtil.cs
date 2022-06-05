#pragma warning disable CS1591

using Camunda.Api.Client;
using Camunda.Api.Client.History;
using Camunda.Api.Client.Message;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client.User;
using Camunda.Api.Client.UserTask;
using KBC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KBC.Util
{
    public class CamundaUtil
    {
        private const string camundaEngineUri = "http://localhost:8080/engine-rest";
        private static CamundaClient client = CamundaClient.Create(camundaEngineUri);
        private const string processKey = "PrijavaPregleda";

        private const string applyMessage = "StvoriTermin";


        public static async Task<string> KreirajPrijavuPregleda(int MBO)
        {
            var parameters = new Dictionary<string, object>();
            parameters["MBO"] = MBO.ToString();
            var processInstanceId = await StartProcess(parameters);
            return processInstanceId;
        }

        private static async Task<string> StartProcess(Dictionary<string, object> processParameters)
        {
            var client = CamundaClient.Create(camundaEngineUri);
            StartProcessInstance parameters = new StartProcessInstance();
            foreach (var param in processParameters)
            {
                parameters.SetVariable(param.Key, param.Value);
            }
            
            var definition = client.ProcessDefinitions.ByKey(processKey);
            ProcessInstanceWithVariables processInstance = await definition.StartProcessInstance(parameters);
            return processInstance.Id;
        }

        public static async Task PreuzmiPregled(string pid, string user)
        {
            var message = new CorrelationMessage()
            {
                ProcessInstanceId = pid,
                MessageName = applyMessage,
                All = true,
                BusinessKey = null
            };
            message.ProcessVariables.Set("Doktor", user);
            await client.Messages.DeliverMessage(message);
        }

        public static async Task<bool> IsUserInGroup(string user, string group)
        {
            var list = await client.Users
                                    .Query(new UserQuery
                                    {
                                        Id = user,
                                        MemberOfGroup = group
                                    })
                                    .List();
            return list.Count > 0;
        }

        public static async Task PickTask(string taskId, string user)
        {
            await client.UserTasks[taskId].Claim(user);
        }

        public static async Task ZavrsiZadatak(string taskId)
        {
            await client.UserTasks[taskId].Complete(new CompleteTask());
        }


        public static async Task DodijeliDoktora(string taskId, string doktor)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["Doktor"] = VariableValue.FromObject(doktor);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }
        
        public static async Task PotvrdiTermin(string taskId, bool odluka)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["Prihvat"] = VariableValue.FromObject(odluka);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            });
        }

        public static async Task ZavrsiPrijavu(string taskId, string datum)
        {
            var variables = new Dictionary<string, VariableValue>();
            variables["Datum"] = VariableValue.FromObject(datum);
            await client.UserTasks[taskId].Complete(new CompleteTask()
            {
                Variables = variables
            }
            );
        }

        public static async Task<string> GetXmlDefinition()
        {
            var client = CamundaClient.Create(camundaEngineUri);
            var definition = client.ProcessDefinitions.ByKey(processKey);
            ProcessDefinitionDiagram diagram = await definition.GetXml();
            return diagram.Bpmn20Xml;
        }

        //FFU: Za pojedinačni status
        //public static async Task<ReviewInfo> GetInstanceActivities(string instanceId)
        //{
        //  var info = new ReviewInfo();
        //  var instanceHistory = await client.History.ProcessInstances[instanceId].Get();
        //  info.StartTime = instanceHistory.StartTime;
        //  var userTasks = await client.UserTasks.Query(new Camunda.Api.Client.UserTask.TaskQuery() { ProcessInstanceId = instanceId }).List();
        //  var activities = await client.History.ActivityInstances.Query(new Camunda.Api.Client.History.HistoricActivityInstanceQuery
        //  {
        //    ProcessInstanceId = instanceId
        //  }).List();
        //  return info;
        //}

        public static async Task<List<PrijavaInfo>> GetPrijave()
        {
            var list = await client.ProcessInstances.Query(new ProcessInstanceQuery { ProcessDefinitionKey = processKey }).List();
            var historyList = await client.History.ProcessInstances.Query(new HistoricProcessInstanceQuery { ProcessDefinitionKey = processKey }).List();
            var prijave = historyList.OrderBy(p => p.StartTime)
                                     .Select(p => new PrijavaInfo
                                     {
                                         //StartTime = p.StartTime,
                                         //EndTime = p.State == ProcessInstanceState.Completed ? p.EndTime : new DateTime?(),
                                         Zavrsio = p.State == ProcessInstanceState.Completed,
                                         PID = p.Id
                                     })
                                     .ToList();

            var tasks = new List<Task>();
            foreach (var prijava in prijave)
            {
                tasks.Add(LoadInstanceVariables(prijava));
            }
            await Task.WhenAll(tasks);

            return prijave;
        }

        public static async Task<List<TaskInfo>> GetTasks(string username)
        {
            var userTasks = await client.UserTasks
                                        .Query(new TaskQuery
                                        {
                                            Assignee = username,
                                            ProcessDefinitionKey = processKey
                                        })
                                        .List();

            var list = userTasks.OrderBy(t => t.Created)
                                .Select(t => new TaskInfo
                                {
                                    TID = t.Id,
                                    TaskName = t.Name,
                                    TaskKey = t.TaskDefinitionKey,
                                    PID = t.ProcessInstanceId,
                                    //StartTime = t.Created.Value,
                                })
                                .ToList();

            var tasks = new List<Task>();
            foreach (var task in list)
            {
                tasks.Add(LoadTaskVariables(task));
            }
            await Task.WhenAll(tasks);
            return list;
        }

        public static async Task<List<TaskInfo>> UnAssignedGroupTasks(string groupName)
        {
            var userTasks = await client.UserTasks
                                        .Query(new TaskQuery
                                        {
                                            Assigned = false,
                                            CandidateGroup = groupName,
                                            ProcessDefinitionKey = processKey
                                        })
                                        .List();

            var list = userTasks.OrderBy(t => t.Created)
                                .Select(t => new TaskInfo
                                {
                                    TID = t.Id,
                                    TaskName = t.Name,
                                    TaskKey = t.TaskDefinitionKey,
                                    PID = t.ProcessInstanceId,
                                    // StartTime = t.Created.Value,
                                })
                                .ToList();

            var tasks = new List<Task>();
            foreach (var task in list)
            {
                tasks.Add(LoadTaskVariables(task));
            }
            await Task.WhenAll(tasks);
            return list;
        }
        
        public static async Task<List<TaskInfo>> AssignedTasks(string user)
        {
            var userTasks = await client.UserTasks
                .Query(new TaskQuery
                {
                    Assigned = true,
                    Assignee = user,
                    ProcessDefinitionKey = processKey
                })
                .List();

            var list = userTasks.OrderBy(t => t.Created)
                .Select(t => new TaskInfo
                {
                    TID = t.Id,
                    TaskName = t.Name,
                    TaskKey = t.TaskDefinitionKey,
                    PID = t.ProcessInstanceId,
                    // StartTime = t.Created.Value,
                })
                .ToList();

            var tasks = new List<Task>();
            foreach (var task in list)
            {
                tasks.Add(LoadTaskVariables(task));
            }
            await Task.WhenAll(tasks);
            return list;
        }

        private static async Task LoadTaskVariables(TaskInfo task)
        {
            var variables = await client.UserTasks[task.TID].Variables.GetAll();

            if (variables.TryGetValue("MBO", out VariableValue value))
            {
                task.MBO = value.GetValue<int>();
            }
            if (variables.TryGetValue("Doktor", out VariableValue value1))
            {
                task.Doktor = value1.GetValue<string>();
            }
            if (variables.TryGetValue("Datum", out VariableValue value2))
            {
                task.DatumPregleda = value2.GetValue<string>();
            }

        }

        private static async Task LoadInstanceVariables(PrijavaInfo prijava)
        {
            var list = await client.History.VariableInstances.Query(new HistoricVariableInstanceQuery { ProcessInstanceId = prijava.PID }).List();
            prijava.MBO = list.Where(v => v.Name == "MBO")
                                    .Select(v => Convert.ToInt32(v.Value))
                                    .FirstOrDefault();

            var doktor = list.Where(v => v.Name == "Doktor")
                                 .Select(v => v.Value as string)
                                 .FirstOrDefault();
            prijava.Doktor = doktor;

            var timePassed = list.Where(v => v.Name == "TimePassed")
                                  .Select(v => v.Value)
                                  .FirstOrDefault();


            prijava.MozeLiSePreuzeti = string.IsNullOrWhiteSpace(doktor) && (timePassed == null || !Convert.ToBoolean(timePassed));
        }
    }
}
