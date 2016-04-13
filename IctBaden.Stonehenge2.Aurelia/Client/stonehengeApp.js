
import {Bootstrap} from 'bootstrap';

export class App {

    configureRouter(config, router) {
        config.title = 'stonehengeAppTitle';
        config.map([
          //stonehengeAppRoutes
        ]);

        this.router = router;
    }

}

