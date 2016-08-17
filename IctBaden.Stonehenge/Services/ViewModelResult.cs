using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Common.Web;
using ServiceStack.Text;

namespace IctBaden.Stonehenge.Services
{
    public class ViewModelResult
    {
        private const string ViewModelContentType = "application/json; charset=utf-8";

        private readonly string _compressionType;
        private readonly AppSession _appSession;
        private readonly object _viewModel;

        public object Result;

        public ViewModelResult(string compressionType, AppSession appSession, object viewModel)
        {
            _compressionType = compressionType;
            _appSession = appSession;
            _viewModel = viewModel;
        }

        public void Build()
        {
            var data = new BlockingCollection<string>();
            var activeVm = _viewModel as ActiveViewModel;
            if (activeVm != null)
            {
                Parallel.ForEach(activeVm.ActiveModels, model => SerializeObject(data, model.Prefix, model.Model));
                //foreach (var model in activeVm.ActiveModels)
                //{
                //    SerializeObject(data, model.Prefix, model.Model);
                //}

                if (!string.IsNullOrEmpty(activeVm.NavigateToRoute))
                {
                    data.Add("\"stonehenge_navigate\":" + JsonSerializer.SerializeToString(activeVm.NavigateToRoute));
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var name in activeVm.GetDictionaryNames())
                {
                    data.Add($"\"{name}\":{JsonSerializer.SerializeToString(activeVm.TryGetMember(name))}");
                }
            }

            SerializeObject(data, null, _viewModel);

            var result = "{" + string.Join(",", data) + "}";

            HttpResult httpResult;
            var contentBytes = Encoding.UTF8.GetBytes(result);

            if (!string.IsNullOrEmpty(_compressionType))
            {
                var compressed = new CompressedResult(contentBytes, _compressionType, ViewModelContentType);
                httpResult = new HttpResult(compressed.Contents, ViewModelContentType);
                httpResult.Headers.Add("CompressionType", _compressionType);
            }
            else
            {
                httpResult = new HttpResult(contentBytes, ViewModelContentType);
            }
            if (!_appSession.CookieSet)
            {
                httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + _appSession.Id);
            }

            Result = httpResult;
        }

        public static Func<object, object> BuildUntypedGetter(Type targetType, PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetGetMethod();

            var exTarget = System.Linq.Expressions.Expression.Parameter(typeof(object), "t");
            var typedTarget = System.Linq.Expressions.Expression.Convert(exTarget, targetType);

            var exBody = System.Linq.Expressions.Expression.Call(typedTarget, methodInfo);
            var exBody2 = System.Linq.Expressions.Expression.Convert(exBody, typeof(object));

            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(exBody2, exTarget);
            // t => Convert(t.get_Foo())

            var action = lambda.Compile();
            return action;
        }

        private static void SerializeObject(BlockingCollection<string> data, string prefix, object obj)
        {
            if (prefix == null)
                prefix = string.Empty;

            Parallel.ForEach(obj.GetType().GetProperties(), prop => SerializeProperty(data, prefix, obj, prop));
            //foreach (var prop in obj.GetType().GetProperties())
            //{
            //    SerializeProperty(data, prefix, obj, prop);
            //}
        }

        private static void SerializeProperty(BlockingCollection<string> data, string prefix, object obj, PropertyInfo prop)
        {
            var bindable = prop.GetCustomAttributes(typeof(BindableAttribute), true);
            if ((bindable.Length > 0) && !((BindableAttribute)bindable[0]).Bindable)
            {
                return;
            }

#if DEBUG
            var time = new Stopwatch();
            time.Start();
#endif
            //var value = prop.GetValue(obj, null);

            var get = BuildUntypedGetter(obj.GetType(), prop);
            var value = get(obj);

            //TODO: evaluate fastest method
            //var arg = System.Linq.Expressions.Expression.Parameter(typeof(object), "x");
            //var targ = System.Linq.Expressions.Expression.Convert(arg, obj.GetType());
            //var expr = System.Linq.Expressions.Expression.Property(targ, prop.Name);
            //var resu = System.Linq.Expressions.Expression.Convert(expr, typeof(object));
            //var propertyResolver = (Func<object, object>)System.Linq.Expressions.Expression.Lambda(resu, arg).Compile();
            //var value = propertyResolver(obj);

            if (value == null)
            {
                return;
            }

#if DEBUG
            Trace.TraceInformation($"GetValue({prop.Name}) {time.ElapsedMilliseconds}ms");
#endif

            string json;
            if (prop.PropertyType.Name == "GraphOptions")
            {
                json = "\"" + prefix + prop.Name + "\":" + value;
            }
            else if (prop.PropertyType.IsValueType && !prop.PropertyType.IsPrimitive &&
                     (prop.PropertyType.Namespace != "System")) // struct
            {
                var structJson = new List<string>();

                foreach (var member in prop.PropertyType.GetProperties())
                {
                    var memberValue = member.GetValue(value, null);
                    if (memberValue != null)
                    {
                        json = "\"" + prefix + member.Name + "\":" +
                               JsonSerializer.SerializeToString(memberValue);
                        structJson.Add(json);
                    }
                }


                json = "\"" + prefix + prop.Name + "\": { " + string.Join(",", structJson) + " }";
            }
            else
            {
                json = "\"" + prefix + prop.Name + "\":" + JsonSerializer.SerializeToString(value);
            }
            data.Add(json);
        }

    }
}