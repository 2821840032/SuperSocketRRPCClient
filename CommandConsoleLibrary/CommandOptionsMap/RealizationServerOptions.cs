using CommandLine;
using CommandLine.Text;
using SuperSocketAOPClientContainer;
using SuperSocketRRPCClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace CommandConsoleLibrary.CommandOptionsMap
{
    [Verb("IMP", HelpText = "执行远程访问")]
    public class RealizationServerOptions
    {
        [Option('i', "id", Required = false, HelpText = "ID")]
        public string id { get; set; }

        [Option('f', "fullname", Required = false, HelpText = "执行的FullName")]
        public string fullname { get; set; }

        [Option('n', "name", Required = false, HelpText = "函数名称")]
        public string name { get; set; }

        [Option('p', "para", Required = true, HelpText = "参数")]
        public IEnumerable<string> para { get; set; }

        public int Run(List<Type> commandExecutionRPClist, AOPContainer aOPContainer,SocketClientMain client)
        {
            MethodInfo objMethod;
            var objType = commandExecutionRPClist.FirstOrDefault(d => d.FullName.Equals(fullname));
            if (objType == null)
            {
                Console.WriteLine($"没有找到{fullname}的类型");
                return 1;
            }


            objMethod = objType.GetMethods().FirstOrDefault(d => d.Name.Equals(name));
            if (objMethod == null)
            {
                Console.WriteLine($"没有找到{name}函数");
                return 1;
            }

            var paras = objMethod.GetParameters();
            if (para.Count()!= paras.Count())
            {
                Console.WriteLine($"参数数量不对 应为{paras.Count()}");
                return 1;
            }
            List<object> paraList = new List<object>();

            var paraToList = para.ToList();
            try
            {
                for (int i = 0; i < paraToList.Count(); i++)
                {
                    paraList.Add(JsonConvert.DeserializeObject(paraToList[i], paras[i].ParameterType));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("格式化参数失败"+e.Message);
            }

            var obj = aOPContainer.GetServices(client, objType);

            var result =  objMethod.Invoke(obj, paraList.ToArray());

            Console.WriteLine("调用完成:"+JsonConvert.SerializeObject(result));
            return 0;
        }
    }
}
