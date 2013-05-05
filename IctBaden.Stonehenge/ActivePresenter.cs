﻿// DynamicViewModel.cs
//
// Author:
//  Frank Pfattheicher <fpf@ict-baden.de>
//
// Copyright (C)2011-2013 ICT Baden GmbH
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

namespace IctBaden.Stonehenge
{
  public class ActivePresenter : DynamicObject, ICustomTypeDescriptor, INotifyPropertyChanged
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

    class PropertyDescriptorEx : PropertyDescriptor
    {
      private readonly string propertyName;
      private readonly PropertyDescriptor originalDescriptor;

      internal PropertyDescriptorEx(string name, PropertyInfo info)
        : base(name, null)
      {
        propertyName = name;
        originalDescriptor = FindOrigPropertyDescriptor(info);
      }

      public override AttributeCollection Attributes
      {
        get
        {
          return originalDescriptor == null ? base.Attributes : originalDescriptor.Attributes;
        }
      }

      public override object GetValue(object component)
      {
        var dynComponent = component as DynamicObject;
        if (dynComponent != null)
        {
          object result;
          if (dynComponent.TryGetMember(new GetMemberBinderEx(propertyName), out result))
            return result;
        }
        return originalDescriptor == null ? null : originalDescriptor.GetValue(component);
      }

      public override void SetValue(object component, object value)
      {
        var dynComponent = component as DynamicObject;
        if (dynComponent != null)
        {
          if (dynComponent.TrySetMember(new SetMemberBinderEx(propertyName), value))
            return;
        }
        if (originalDescriptor == null)
          return;
        originalDescriptor.SetValue(component, value);
      }

      public override bool IsReadOnly
      {
        get
        {
          return originalDescriptor != null && originalDescriptor.IsReadOnly;
        }
      }

      public override Type PropertyType
      {
        get
        {
          return originalDescriptor == null ? typeof(object) : originalDescriptor.PropertyType;
        }
      }

      public override bool CanResetValue(object component)
      {
        return originalDescriptor != null && originalDescriptor.CanResetValue(component);
      }

      public override Type ComponentType
      {
        get
        {
          return originalDescriptor == null ? typeof(object) : originalDescriptor.ComponentType;
        }
      }

      public override void ResetValue(object component)
      {
        if (originalDescriptor == null)
          return;
        originalDescriptor.ResetValue(component);
      }

      public override bool ShouldSerializeValue(object component)
      {
        return originalDescriptor != null && originalDescriptor.ShouldSerializeValue(component);
      }

      private static PropertyDescriptor FindOrigPropertyDescriptor(PropertyInfo propertyInfo)
      {
        return propertyInfo == null ? null : TypeDescriptor.GetProperties(propertyInfo.DeclaringType).Cast<PropertyDescriptor>().FirstOrDefault(propertyDescriptor => propertyDescriptor.Name.Equals(propertyInfo.Name));
      }
    }

    class PropertyInfoEx
    {
      public PropertyInfo Info { get; private set; }
      public object Obj { get; private set; }

      public PropertyInfoEx(PropertyInfo pi, object obj)
      {
        Info = pi;
        Obj = obj;
      }
    }

    class DynamicModel
    {
      public readonly string Prefix;
      public readonly string TypeName;
      public object Model;

      public DynamicModel(string prefix, object model)
      {
        Prefix = prefix;
        Model = model;
        TypeName = model.GetType().Name;
      }
    }

    #endregion

    #region properties

    readonly Dictionary<string, List<string>> dependencies = new Dictionary<string, List<string>>();
    readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();
    readonly List<DynamicModel> models = new List<DynamicModel>();
    [Browsable(false)]
    internal int Count { get { return GetProperties().Count; } }
    [Browsable(false)]
    internal IEnumerable<string> Models { get { return from model in models select model.GetType().Name; } }
    internal bool ModelTypeExists(string prefix, object model) { return models.FirstOrDefault(m => (m.TypeName == model.GetType().Name) && (m.Prefix == prefix)) != null; }

    #endregion

    public ActivePresenter()
    {
    }
    public ActivePresenter(object model)
      : this()
    {
      AddModel(model);
    }

    protected void SetParent(ActivePresenter parent)
    {
      PropertyChanged += (sender, args) => parent.NotifyPropertyChanged(args.PropertyName);
    }

    public object this[string name]
    {
      get
      {
        object result;
        TryGetMember(new GetMemberBinderEx(name), out result);
        return result;
      }
      set
      {
        TrySetMember(new SetMemberBinderEx(name), value);
      }
    }

    public void AddModel(object model)
    {
      AddModel(null, model);
    }
    public void AddModel(string prefix, object model)
    {
      if (ModelTypeExists(prefix, model))
        throw new ArgumentException(string.Format("A model of type '{0}' is already added", model.GetType().Name));
      models.Add(new DynamicModel(prefix, model));

      properties = null;
      propertiesAttrib = null;
      GetProperties();
    }

    public void UpdateModel(object model)
    {
      UpdateModel(null, model);
    }
    public void UpdateModel(string prefix, object model)
    {
      if (!ModelTypeExists(prefix, model))
        throw new ArgumentException(string.Format("No model of type '{0}' is added", model.GetType().Name));

      var index = (from m in models where (m.TypeName == model.GetType().Name) && (m.Prefix == prefix) select models.IndexOf(m)).First();
      models[index].Model = model;
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
      foreach (var model in models)
        names.AddRange(from prop in model.GetType().GetProperties() select prop.Name);
      names.AddRange(from elem in dictionary select elem.Key);
      return names;
    }

    private PropertyInfoEx GetPropertyInfo(string name)
    {
      var pi = GetType().GetProperty(name);
      if (pi != null)
      {
        return new PropertyInfoEx(pi, this);
      }
      foreach (var model in models)
      {
        if (!string.IsNullOrEmpty(model.Prefix))
        {
          if (!name.StartsWith(model.Prefix))
            continue;
          name = name.Substring(model.Prefix.Length);
        }
        pi = model.Model.GetType().GetProperty(name);
        if (pi == null)
          continue;

        return new PropertyInfoEx(pi, model.Model);
      }
      return null;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      var pi = GetPropertyInfo(binder.Name);
      if (pi != null)
      {
        var val = pi.Info.GetValue(pi.Obj, null);
        result = val;
        return true;
      }
      return dictionary.TryGetValue(binder.Name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      var pi = GetPropertyInfo(binder.Name);
      if (pi != null)
      {
        pi.Info.SetValue(pi.Obj, value, null);
        NotifyPropertyChanged(binder.Name);
        return true;
      }
      dictionary[binder.Name] = value;
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
        var desc = new PropertyDescriptorEx(prop.Name, pi);
        properties.Add(desc);
      }
      foreach (var model in models)
      {
        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(model.Model, true))
        {
          var pi = model.Model.GetType().GetProperty(prop.Name);
          var name = prop.Name;
          if (!string.IsNullOrEmpty(model.Prefix))
            name = model.Prefix + name;
          var desc = new PropertyDescriptorEx(name, pi);
          properties.Add(desc);
        }
      }
      foreach (var elem in dictionary)
      {
        var desc = new PropertyDescriptorEx(elem.Key, null);
        properties.Add(desc);
      }

      foreach (PropertyDescriptorEx prop in properties)
      {
        foreach (Attribute attrib in prop.Attributes)
        {
          if (attrib.GetType() != typeof(DependsOnAttribute))
            continue;
          var da = (DependsOnAttribute)attrib;
          if (!dependencies.ContainsKey(da.Name))
            dependencies[da.Name] = new List<string>();

          dependencies[da.Name].Add(prop.Name);
        }
      }

      var myMethods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
      foreach (var method in myMethods)
      {
        var dependsOnAttributes = method.GetCustomAttributes(typeof(DependsOnAttribute), true);
        foreach (DependsOnAttribute attrib in dependsOnAttributes)
        {
          if (!dependencies.ContainsKey(attrib.Name))
            dependencies[attrib.Name] = new List<string>();

          dependencies[attrib.Name].Add(method.Name);
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
      foreach (var model in models)
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
        dispatcherObject.Dispatcher.BeginInvoke(handler, DispatcherPriority.DataBind, new object[] { this, args });
      }
      else // Execute handler as is
      {
        handler(this, args);
      }
    }

    protected void NotifyPropertyChanged(string name)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        ExecuteHandler(handler, name);
      }

      if (!dependencies.ContainsKey(name))
        return;

      foreach (var dependendName in dependencies[name])
      {
				if (handler != null)
        {
          ExecuteHandler(handler, dependendName);
        }
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
  }

}