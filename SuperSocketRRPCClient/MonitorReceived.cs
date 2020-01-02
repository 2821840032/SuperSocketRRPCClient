using Newtonsoft.Json;
using SuperSocketRRPCClient.AttributeEntity;
using SuperSocketRRPCClient.Entity;
using SuperSocketRRPCUnity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using System.Linq;
namespace SuperSocketRRPCClient
{
    /// <summary>
    /// 请求监听类
    /// </summary>
   public class MonitorReceived
    {

        /// <summary>
        /// 容器对象
        /// </summary>
        public UnityInIt<SocketClientMain, RequestExecutiveInformation, RequestBaseInfo> unityCon { get; private set; }


        /// <summary>
        /// 请求过滤器类型
        /// </summary>
        Type _RequestFilterAttribte { get; set; }

        SocketClientMain socket { get; set; }

        /// <summary>
        /// 请求监听类
        /// </summary>
        /// <param name="socket">连接对象</param>
        public MonitorReceived(SocketClientMain socket) {
            this.socket = socket;
            Type baseProvideServicesType = typeof(BaseProvideServices);
            unityCon = new UnityInIt<SocketClientMain, RequestExecutiveInformation, RequestBaseInfo>(
                baseProvideServicesType.FullName,
                baseProvideServicesType.GetProperty("Socket"), 
                baseProvideServicesType.GetProperty("Info"),
                baseProvideServicesType.GetProperty("RequestInfo"),
                baseProvideServicesType.GetProperty("Container"),
                baseProvideServicesType.GetProperty("RequestClientSession"));
            _RequestFilterAttribte = typeof(RequestFilterAttribte);
        }


        /// <summary>
        /// 为所有启动的服务器注册服务
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <typeparam name="T"></typeparam>
        public void AddServer<IT, T>()
           where IT : class
           where T : IT
        {
            unityCon.AddServer<IT, T>();
        }

        /// <summary>
        /// 收到请求事件
        /// </summary>
        /// <param name="stringPackageInfo"></param>
        public void onReceived(RequestBaseInfo stringPackageInfo)
        {
            RequestExecutiveInformation info = null;
            try
            {
                info = JsonConvert.DeserializeObject<RequestExecutiveInformation>(stringPackageInfo.bodyMeg);
            }
            catch (Exception e)
            {
                socket.Log("解析失败" + stringPackageInfo.bodyMeg+"。原因："+e.Message, LoggerType.Error);
                return;
            }

            if (info.ReturnValue != null && socket.RemoteCallQueue.GetTaskIDAndSuccess(info.ID, info.ReturnValue))
            {
                //处理完成
            }
            else if (info.ReturnValue != null)
            {
                socket.Log($"收到一个意外的请求 它有结果但是没有找到该任务的信息 ID:{info.ID} FullName:{info.FullName} Return:{info.ReturnValue} 来自于:{socket.RemoteEndpoint.ToString()}", LoggerType.Error);
            }
            else
            {
                ImplementFunc(info, stringPackageInfo);
            }
        }

        /// <summary>
        /// 执行RPC的调用
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="requestInfo">基础信息</param>
        async void ImplementFunc(RequestExecutiveInformation info, RequestBaseInfo requestInfo)
        {
            await Task.Yield();
            //接收RPC的请求
            if (unityCon.GetService(info.FullName, socket, info,requestInfo, socket.UnityContainer, info.RequestClientSession, out object executionObj, out var iServerType))
            {

                var methodType = iServerType.GetMethod(info.MethodName);

                List<object> attribtes = new List<object>();
                attribtes.AddRange(iServerType.CustomAttributes.Select(d => d.Constructor.Invoke(null)).ToArray());
                attribtes.AddRange(methodType.GetCustomAttributes(_RequestFilterAttribte, true));
                //Filter前
                if (!BeforeExecutionAttribte(attribtes, (BaseProvideServices)executionObj))
                {
                    return;
                }

                List<object> paraList = new List<object>();
                var paras = methodType.GetParameters();
                for (int i = 0; i < info.Arguments.Count; i++)
                {
                    paraList.Add(JsonConvert.DeserializeObject(info.Arguments[i], paras[i].ParameterType));
                }

                try
                {
                    var result = methodType.Invoke(executionObj, paraList.ToArray());
                    info.ReturnValue = JsonConvert.SerializeObject(result);
                }
                catch (Exception e)
                {
                    info.ReturnValue = null;
                    Console.WriteLine("处理请求时候出现异常:"+e);
                    socket.Requestexception?.HandleException(e);
                }
                if (AfterxecutionExecutionAttribte(attribtes, (BaseProvideServices)executionObj, info.ReturnValue))
                {
                    var msg = JsonConvert.SerializeObject(info);
                    socket.SendMessage(msg);
                }
            }
            else {
                socket.Log("收到一个未知的请求" +info.FullName, LoggerType.Error);
            }
        }

        /// <summary>
        /// 执行前方法前
        /// </summary>
        /// <param name="attributes">过滤类型</param>
        /// <param name="executionObj">执行对象</param>
        private bool BeforeExecutionAttribte(List<object> attributes, BaseProvideServices executionObj)
        {
            var result = true;
            foreach (RequestFilterAttribte item in attributes)
            {
                if (!item.BeforeExecution(executionObj))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 执行方法后
        /// </summary>
        /// <param name="attributes">过滤类型</param>
        /// <param name="executionObj">执行对象</param>
        /// <param name="impResult">结果</param>
        /// <returns></returns>
        private bool AfterxecutionExecutionAttribte(List<object> attributes,BaseProvideServices executionObj, object impResult) {
            var result = true;
            foreach (RequestFilterAttribte item in attributes)
            {
                if (!item.Afterxecution(executionObj, impResult))
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
