import { DcBaseComponent } from '@deleteagency/dc';

export default class AttriCssComponent extends DcBaseComponent {
    static getNamespace() {
        return 'attri-css';
    }

    onInit() {
        alert('Test component is working!');
    }
}
