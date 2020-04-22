import { dcFactory } from '@deleteagency/dc';

class App {
    constructor() {
        this.dcFactory = dcFactory;
        this.config = window.appConfig || {};
    }

    init() {
        this.dcFactory.init();
    }

    getConfig(property, defaultValue = undefined) {
        return property in this.config ? this.config[property] : defaultValue;
    }
}

const instance = new App();
export default instance;
