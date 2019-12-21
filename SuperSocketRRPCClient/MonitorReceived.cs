using Newtonsoft.Json;
using SuperSocketRRPCClient.Entity;
using SuperSocketRRPCUnity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
        public UnityInIt unityCon { get; private set; }

        SocketClientMain socket { get; set; }

        #region 反射设置值的对象列表

        /// <summary>
        ///  BaseProvideServices FulleName 
        /// </summary>
        string BaseProvideServicesFullName { get; set; }

        /// <summary>
        /// Socket PropertyInfo
        /// </summary>
        PropertyInfo socketPropertyInfo { get; set; }

        /// <summary>
        /// info PropertyInfo
        /// </summary>
        PropertyInfo infoPropertyInfo { get; set; }

        /// <summary>
        /// requestInfo PropertyInfo
        /// </summary>
        PropertyInfo requestInfoPropertyInfo { get; set; }

        #endregion


        /// <summary>
        /// 请求监听类
        /// </summary>
        /// <param name="socket">连接对象</param>
        public MonitorReceived(SocketClientMain socket) {
            this.socket = socket;
            Type baseProvideServicesType = typeof(BaseProvideServices);
            BaseProvideServicesFullName = baseProvideServicesType.FullName;
            unityCon = new UnityInIt(BaseProvideServicesFullName);
            socketPropertyInfo = baseProvideServicesType.GetProperty("Socket");
            infoPropertyInfo = baseProvideServicesType.GetProperty("Info");
            requestInfoPropertyInfo = baseProvideServicesType.GetProperty("RequestInfo");
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
                Console.WriteLine("解析失败" + stringPackageInfo.bodyMeg+"。原因："+e.Message);
                return;
            }

            if (info.ReturnValue != null && socket.RemoteCallQueue.GetTaskIDAndSuccess(info.ID, info.ReturnValue))
            {
                //处理完成
            }
            else if (info.ReturnValue != null)
            {
                Console.WriteLine($"收到一个意外的请求 它有结果但是没有找到该任务的信息 ID:{info.ID} FullName:{info.FullName} Return:{info.ReturnValue} 来自于:{socket.RemoteEndpoint.ToString()}");
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
        void ImplementFunc(RequestExecutiveInformation info, RequestBaseInfo requestInfo)
        {
            //接收RPC的请求
            if (unityCon.GetService(info.FullName, out object executionObj, out var iServerType, out bool isMatchingFullName))
            {
                if (isMatchingFullName)
                {
                    socketPropertyInfo.SetValue(executionObj, socket);
                    infoPropertyInfo.SetValue(executionObj, info);
                    requestInfoPropertyInfo.SetValue(executionObj, requestInfo);
                }

                var methodType = iServerType.GetMethod(info.MethodName);
                List<object> paraList = new List<object>();
                for (int i = 0; i < info.Arguments.Count; i++)
                {
                    var paras = methodType.GetParameters();
                    paraList.Add(JsonConvert.DeserializeObject(info.Arguments[i], paras[i].ParameterType));
                }
                info.ReturnValue = JsonConvert.SerializeObject(methodType.Invoke(executionObj, paraList.ToArray()));
                var msg = JsonConvert.SerializeObject(info);
                socket.SendMessage(msg);
            }
            else {
                Console.WriteLine("收到一个未知的请求"+info.FullName);
            }

          
        }
    }
}
