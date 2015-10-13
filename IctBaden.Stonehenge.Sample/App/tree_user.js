
function DemoTreeInitialize() {

  $('.ztree').each(function (i, obj) {
    
    var treeObj = $.fn.zTree.getZTreeObj(obj.id);
    var setting = treeObj.setting;
    if (setting == null)
      setting = {};

    setting.callback.onCheck = function (event, treeId, treeNode) {
      self.DemoTreeSelected(treeNode.id);
      $("#" + event.delegateTarget.id).trigger('checked');
    };
  });
 
  


}


