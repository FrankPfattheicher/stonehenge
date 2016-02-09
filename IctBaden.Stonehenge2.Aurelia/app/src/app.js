export class App {


    configureRouter(config, router) {
        config.title = 'Xurelia';
        config.map([
          { route: ['','welcome'], name: 'welcome', moduleId: './welcome', nav: true, title:'Welcome' },
          { route: ['user'], name: 'user', moduleId: './user', nav: false, title:'User' }
        ]);

        this.router = router;
    }

}
