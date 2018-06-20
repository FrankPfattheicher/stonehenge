// ActiveViewModel.cs
//
// Author:
//  Frank Pfattheicher <fpf@ict-baden.de>
//
// Copyright (C)2011-2015 ICT Baden GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Threading;
using IctBaden.Stonehenge.Services;

namespace IctBaden.Stonehenge
{
    using System.Diagnostics;

    public class ActiveViewModel : DynamicObject, ICustomTypeDescriptor, INotifyPropertyChanged
    {
        #region helper classes

        class GetMemberBinderEx : GetMemberBinder
        {
            public GetMemberBinderEx(string name) : base(name, false) { }
            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            { return null; }
        }

        class SetMemberBinderEx : SetMemberBinder
        {
            public SetMemberBinderEx(string name) : base(name, false) { }
            public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
            { return null; }
        }

        private class PropertyDescriptorEx : PropertyDescriptor
        {
            private readonly string _propertyName;
            private readonly PropertyDescriptor _originalDescriptor;
            private readonly bool _readOnly;

            internal PropertyDescriptorEx(string name, PropertyInfo info, bool readOnly)
                : base(name, null)
            {
                _propertyName = name;
                _originalDescriptor = FindOrigPropertyDescriptor(info);
                _readOnly = readOnly;
            }

            public override AttributeCollection Attributes => _originalDescriptor?.Attributes ?? base.Attributes;

            public override object GetValue(object component)
            {
                if (component is DynamicObject dynComponent)
                {
                    if (dynComponent.TryGetMember(new GetMemberBinderEx(_propertyName), out var result))
                        return result;
                }
                return _originalDescriptor?.GetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                if (component is DynamicObject dynComponent)
                {
                    if (dynComponent.TrySetMember(new SetMemberBinderEx(_propertyName), value))
                        return;
                }
                _originalDescriptor?.SetValue(component, value);
            }

            public override bool IsReadOnly => _readOnly || ((_originalDescriptor != null) && _originalDescriptor.IsReadOnly);

            public override Type PropertyType => _originalDescriptor == null ? typeof(object) : _originalDescriptor.PropertyType;

            public override bool CanResetValue(object component)
            {
                return _originalDescriptor != null && _originalDescriptor.CanResetValue(component);
            }

            public override Type ComponentType => _originalDescriptor == null ? typeof(object) : _originalDescriptor.ComponentType;

            public override void ResetValue(object component)
            {
                _originalDescriptor?.ResetValue(component);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return _originalDescriptor != null && _originalDescriptor.ShouldSerializeValue(component);
            }

            private static PropertyDescriptor FindOrigPropertyDescriptor(PropertyInfo propertyInfo)
            {
                return propertyInfo == null ? null : TypeDescriptor.GetProperties(propertyInfo.DeclaringType).Cast<PropertyDescriptor>().FirstOrDefault(propertyDescriptor => propertyDescriptor.Name.Equals(propertyInfo.Name));
            }
        }

        class PropertyInfoEx
        {
            public PropertyInfo Info { get; }
            public object Obj { get; }
            public bool ReadOnly { get; }

            public PropertyInfoEx(PropertyInfo pi, object obj, bool readOnly)
            {
                Info = pi;
                Obj = obj;
                ReadOnly = readOnly;
            }

            public override string ToString()
            {
                return Info.PropertyType + " " + Info.Name;
            }
        }

        #endregion

        #region properties

        private readonly Dictionary<string, List<string>> _dependencies = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        internal List<ActiveModel> ActiveModels = new List<ActiveModel>();
        [Browsable(false)]
        // ReSharper disable once UnusedMember.Global
        internal int Count => GetProperties().Count;

        private void ClearProperties()
        {
            properties = null;
            propertiesAttrib = null;
            _dependencies.Clear();
        }

        [Browsable(false)]
        internal IEnumerable<string> Models => ActiveModels.Select(model => model.GetType().Name);

        [Browsable(false)]
        public AppSession Session;
        [Browsable(false)]
        public bool SupportsEvents;

        // ReSharper disable InconsistentNaming
        [Bindable(false)]
        public string _stonehenge_CommandSenderName_ { get; set; }

        // ReSharper disable once UnusedMember.Global
        public string GetCommandSenderName()
        {
            return _stonehenge_CommandSenderName_;
        }

        protected bool ModelTypeExists(string prefix, object model) { return ActiveModels.FirstOrDefault(m => (m.TypeName == model.GetType().Name) && (m.Prefix == prefix)) != null; }

        #endregion

        // ReSharper disable once UnusedMember.Global
        public ActiveViewModel()
            : this(null)
        {
        }
        public ActiveViewModel(AppSession session)
        {
            SupportsEvents = (session != null);
            Session = session ?? AppSessionCache.NewSession();
        }

        // ReSharper disable once UnusedMember.Global
        protected void SetParent(ActiveViewModel parent)
        {
            PropertyChanged += (sender, args) => parent.NotifyPropertyChanged(args.PropertyName);
        }

        public object TryGetMember(string name)
        {
            TryGetMember(new GetMemberBinderEx(name), out var result);
            return result;
        }

        public void TrySetMember(string name, object value)
        {
            TrySetMember(new SetMemberBinderEx(name), value);
        }

        [Browsable(false)]
        protected object this[string name]
        {
            get => TryGetMember(name);
            set => TrySetMember(name, value);
        }

        // ReSharper disable once UnusedMember.Global
        public void SetModel(object model, bool readOnly = false)
        {
            SetModel(null, model, readOnly);
        }
        public void SetModel(string prefix, object model, bool readOnly = false)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (ModelTypeExists(prefix, model))
            {
                UpdateModel(prefix, model);
                return;
            }

            ActiveModels.Add(new ActiveModel(prefix, model, readOnly));

            ClearProperties();
            GetProperties();
        }

        // ReSharper disable once UnusedMember.Global
        public void RemoveModel(object model)
        {
            RemoveModel(null, model);
        }

        public void RemoveModel(string prefix, object model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var foundModel = ActiveModels.FirstOrDefault(m => (m.TypeName == model.GetType().Name) && (m.Prefix == prefix));
            if (foundModel == null) return;

            ActiveModels.Remove(foundModel);

            ClearProperties();
            GetProperties();
        }

        // ReSharper disable once UnusedMember.Global
        public void UpdateModel(object model)
        {
            UpdateModel(null, model);
        }
        public void UpdateModel(string prefix, object model)
        {
            if (!ModelTypeExists(prefix, model))
            {
                //throw new ArgumentException(string.Format("No model of type '{0}' is added", model.GetType().Name));
                SetModel(prefix, model);
                return;
            }

            var index = (from m in ActiveModels where (m.TypeName == model.GetType().Name) && (m.Prefix == prefix) select ActiveModels.IndexOf(m)).First();
            ActiveModels[index].Model = model;
            foreach (var prop in model.GetType().GetProperties())
            {
                if (string.IsNullOrEmpty(prefix))
                {
                    NotifyPropertyChanged(prop.Name);
                }
                else
                {
                    NotifyPropertyChanged(prefix + prop.Name);
                }
            }
        }

        #region DynamicObject

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var names = new List<string>();
            foreach (var model in ActiveModels)
                names.AddRange(from prop in model.GetType().GetProperties() select prop.Name);
            names.AddRange(from elem in _dictionary select elem.Key);
            return names;
        }
        public IEnumerable<string> GetDictionaryNames()
        {
            return _dictionary.Select(e => e.Key);
        }
        public object GetDictionaryValue(string name)
        {
            return _dictionary.ContainsKey(name) ? _dictionary[name] : null;
        }

        private PropertyInfoEx GetPropertyInfoEx(string name)
        {
            var pi = GetType().GetProperty(name);
            if (pi != null)
            {
                return new PropertyInfoEx(pi, this, false);
            }
            foreach (var model in ActiveModels)
            {
                var propName = name;
                if (!string.IsNullOrEmpty(model.Prefix))
                {
                    if (!name.StartsWith(model.Prefix))
                        continue;
                    propName = name.Substring(model.Prefix.Length);
                }
                pi = model.Model.GetType().GetProperty(propName);
                if (pi == null)
                    continue;

                return new PropertyInfoEx(pi, model.Model, model.ReadOnly);
            }
            return null;
        }
        public PropertyInfo GetPropertyInfo(string name)
        {
            var infoEx = GetPropertyInfoEx(name);
            return infoEx?.Info;
        }

        public bool IsPropertyReadOnly(string name)
        {
            var infoEx = GetPropertyInfoEx(name);
            return (infoEx == null) || infoEx.ReadOnly;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var pi = GetPropertyInfoEx(binder.Name);
            if (pi != null)
            {
                var val = pi.Info.GetValue(pi.Obj, null);
                result = val;
                return true;
            }
            return _dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var pi = GetPropertyInfoEx(binder.Name);
            if (pi != null)
            {
                pi.Info.SetValue(pi.Obj, value, null);
                NotifyPropertyChanged(binder.Name);
                return true;
            }
            if ((properties != null) && !_dictionary.ContainsKey(binder.Name))
            {
                var desc = new PropertyDescriptorEx(binder.Name, null, false);
                properties.Add(desc);
            }
            _dictionary[binder.Name] = value;
            NotifyPropertyChanged(binder.Name);
            return true;
        }

        #endregion

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        private PropertyDescriptorCollection properties;
        public PropertyDescriptorCollection GetProperties()
        {
            if (properties != null)
                return properties;

            properties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this, true))
            {
                var pi = GetType().GetProperty(prop.Name);
                var desc = new PropertyDescriptorEx(prop.Name, pi, false);
                properties.Add(desc);
            }
            foreach (var model in ActiveModels)
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(model.Model, true))
                {
                    var pi = model.Model.GetType().GetProperty(prop.Name);
                    var name = prop.Name;
                    if (!string.IsNullOrEmpty(model.Prefix))
                        name = model.Prefix + name;

                    if (properties.Cast<PropertyDescriptor>().Any(pd => pd.Name == name))
                        throw new ArgumentException("Duplicate property", name);

                    var desc = new PropertyDescriptorEx(name, pi, model.ReadOnly);
                    properties.Add(desc);
                }
            }
            foreach (var elem in _dictionary)
            {
                var desc = new PropertyDescriptorEx(elem.Key, null, false);
                properties.Add(desc);
            }

            foreach (PropertyDescriptorEx prop in properties)
            {
                foreach (Attribute attrib in prop.Attributes)
                {
                    if (attrib.GetType() != typeof(DependsOnAttribute))
                        continue;
                    var da = (DependsOnAttribute)attrib;
                    if (!_dependencies.ContainsKey(da.Name))
                    {
                        _dependencies[da.Name] = new List<string>();
                    }
                    _dependencies[da.Name].Add(prop.Name);
                }
            }

            var myMethods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var method in myMethods)
            {
                var dependsOnAttributes = method.GetCustomAttributes(typeof(DependsOnAttribute), true);
                foreach (DependsOnAttribute attrib in dependsOnAttributes)
                {
                    if (!_dependencies.ContainsKey(attrib.Name))
                        _dependencies[attrib.Name] = new List<string>();

                    _dependencies[attrib.Name].Add(method.Name);
                }
            }

            return properties;
        }

        private PropertyDescriptorCollection propertiesAttrib;
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (propertiesAttrib != null)
                return propertiesAttrib;

            propertiesAttrib = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this, true))
                propertiesAttrib.Add(prop);
            foreach (var model in ActiveModels)
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(model, attributes, true))
                    propertiesAttrib.Add(prop);
            }
            return propertiesAttrib;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void ExecuteHandler(PropertyChangedEventHandler handler, string name)
        {
            var dispatcherObject = handler.Target as DispatcherObject;
            var args = new PropertyChangedEventArgs(name);
            // If the subscriber is a DispatcherObject and different thread
            if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
            {
                // Invoke handler in the target dispatcher's thread
                dispatcherObject.Dispatcher.BeginInvoke(handler, DispatcherPriority.DataBind, this, args);
            }
            else // Execute handler as is
            {
                handler(this, args);
            }
        }

        protected void NotifyPropertyChanged(string name)
        {
            GetProperties();
#if DEBUG
            Debug.Assert(name.StartsWith(AppService.PropertyNameId) 
                || (GetPropertyInfo(name) != null) || GetDictionaryNames().Contains(name), 
                "NotifyPropertyChanged for unknown property " + name);
#endif
            var handler = PropertyChanged;
            if (handler == null) return;
            
            ExecuteHandler(handler, name);
            
            if (!_dependencies.ContainsKey(name))
                return;

            foreach (var dependendName in _dependencies[name])
            {
                ExecuteHandler(handler, dependendName);
            }
        }

        protected void NotifyPropertiesChanged(string[] names)
        {
            foreach (var name in names)
            {
                NotifyPropertyChanged(name);
            }
        }

        #endregion

        #region MessageBox

        public string MessageBoxTitle;
        public string MessageBoxText;

        public void MessageBox(string title, string text)
        {
            MessageBoxTitle = title;
            MessageBoxText = text;
            NotifyPropertyChanged(AppService.PropertyNameMessageBox);
        }

        #endregion

        #region Serve site navigation

        public string NavigateToRoute;

        public void NavigateTo(string route)
        {
            Debug.WriteLine("NavigateTo: " + route);
            NavigateToRoute = route;
            NotifyPropertyChanged(AppService.PropertyNameNavigate);
        }

        public void NavigateBack()
        {
            Debug.WriteLine("NavigateBack");
            NavigateToRoute = "_stonehenge_back_";
            NotifyPropertyChanged(AppService.PropertyNameNavigate);
        }

        #endregion

        #region Client site scripting

        public string ClientScript;
        public void ExecuteClientScript(string script)
        {
            ClientScript = script;
            Session.EventAdd(AppService.PropertyNameClientScript);
            //NotifyPropertyChanged(AppService.PropertyNameClientScript);
        }

        #endregion

    }

}
