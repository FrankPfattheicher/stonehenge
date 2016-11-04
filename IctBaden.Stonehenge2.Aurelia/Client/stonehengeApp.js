
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

export class Tools {

    getCookie(name) {
        var i = 0; //Suchposition im Cookie
        var suche = name + "=";
        var maxlen = document.cookie.length;
        while (i < maxlen) {
            if (document.cookie.substring(i, i + suche.length) == suche) {
                var ende = document.cookie.indexOf(";", i + suche.length);
                if (ende < 0) {
                    ende = maxlen;
                }
                var cook = document.cookie.substring(i + suche.length, ende);
                return unescape(cook);
            }
            i++;
        }
        return "";
    }

}

