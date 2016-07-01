
// ReSharper disable Es6Feature
import {customElement, inject, bindable} from 'aurelia-framework';

@customElement('stonehengeCustomElementName')
//@bindable
@inject(Element)
export class stonehengeCustomElementClass {

    constructor(element) {
        this.element = element;
    }

    fireEvent = function(name, data) {

        let event;

        if (window.CustomEvent) {
            event = new CustomEvent(name, {
                detail: { value: data },
                bubbles: true
            });
        } else {
            event = document.createEvent('CustomEvent');
            event.initCustomEvent(name, true, true, {value: data});
        }
        this.element.dispatchEvent(event);
    }

}
// ReSharper restore Es6Feature
