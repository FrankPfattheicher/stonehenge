//ViewModel:IctBaden.StonehengeSample.AboutVm

define(function (require) {

  var Version = ko.observable();

  return {
    Version: Version,
    activate: function() {
      $.getJSON('/viewmodel/IctBaden.StonehengeSample.AboutVm', function (data) {
        Version(data.Version);
      });
    }
  };
});
