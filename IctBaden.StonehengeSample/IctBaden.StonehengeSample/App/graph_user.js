
    //$(function () {
      // we use an inline data source in the example, usually data would
      // be fetched from a server
      var data = [], totalPoints = 180;
      function getRandomData() {
        if (data.length > 0)
          data = data.slice(1);

        while (data.length < totalPoints) {
          var prev = data.length > 0 ? data[data.length - 1] : 50;
          var y = prev + Math.random() * 10 - 5;
          if (y < 0)
            y = 0;
          if (y > 100)
            y = 100;
          data.push(y);
        }

        var res = [];
        for (var i = 0; i < data.length; ++i)
          res.push([(new Date("1970/01/01")).getTime() + (i * 360000), data[i]]);
        return res;
      }
      
      // setup plot
      var options = {
        yaxis: { min: 0, max: 100 },
        xaxis: {
          mode: "time",
          timeformat: "%H:%M",
          min: (new Date("1970/01/01 00:00")).getTime(),
          max: (new Date("1970/01/01 23:59")).getTime()
        },
        colors: ["#F90", "#222", "#666", "#BBB"],
        series: {
          lines: {
            lineWidth: 2,
            fill: true,
            fillColor: { colors: [{ opacity: 0.6 }, { opacity: 0.2}] },
            steps: false

          }
        }
      };

      //var plot = $.plot($("#graph"), [getRandomData()], options);
    //});
    
