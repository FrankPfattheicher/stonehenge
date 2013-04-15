//ViewModel:IctBaden.StonehengeSample.FormVm

function poll(Clock) {
  $.getJSON('/viewmodel/Clock', function (data) { Clock(data.Clock); poll(Clock); });
};

define(function (require) {
  var Id = ko.observable();
  var Name = ko.observable();
  var Clock = ko.observable();
  var CanSayHello = ko.observable();


  Name.subscribe(function (newName) {
    $.post('/viewmodel/IctBaden.StonehengeSample.FormVm', 'Name=' + newName, function (data) {
      CanSayHello(data.CanSayHello);
    });
  });
  
  $(function () {
    document.title = "Hello World!";
  });

  //poll(Clock);

  return {
        activate: function () {
          $.getJSON('/viewmodel/IctBaden.StonehengeSample.FormVm', function (data) {
            Id(data.Id);
            Name(data.Name);
            Clock(data.Clock);
            CanSayHello(data.CanSayHello);
          });

    },

    Prompt: 'What is your name?',
    Id: Id,
    Name: Name,
    Clock: Clock,
    SayHello: function () {
      $.post('/viewmodel/IctBaden.StonehengeSample.FormVm', 'Name=' + Name(), function (data) {
        Id(data.Id);
        Name(data.Name);
        Clock(data.Clock);
        CanSayHello(data.CanSayHello);
      });
      //app.showMessage('Hello ' + Name() + '!', 'Greetings');
    },
    CanSayHello: CanSayHello
  };
});

