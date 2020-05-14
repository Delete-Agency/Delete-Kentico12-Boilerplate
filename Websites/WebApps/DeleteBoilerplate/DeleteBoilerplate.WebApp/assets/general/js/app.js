import { dcFactory } from '@deleteagency/dc';
import { deviceObserver } from '@deleteagency/device-observer';
import { viewports } from 'config';

const DEVICE_MAX = 'max';
export const DEVICE_TABLET = 'tablet';
export const DEVICE_MOBILE = 'mobile';
export const DEVICE_DESKTOP = 'desktop';

class App {
    constructor() {
        this.dcFactory = dcFactory;
        this.config = window.appConfig || {};
    }

    init() {
        deviceObserver.init(
            {
                [DEVICE_MAX]: Infinity,
                [DEVICE_TABLET]: viewports.tablet,
                [DEVICE_MOBILE]: viewports.mobile,
                [DEVICE_DESKTOP]: viewports.desktop,
            },
            { mobileFirst: false }
        );

        this.dcFactory.init();
    }

    getConfig(property, defaultValue = undefined) {
        return property in this.config ? this.config[property] : defaultValue;
    }
}

const instance = new App();
export default instance;
