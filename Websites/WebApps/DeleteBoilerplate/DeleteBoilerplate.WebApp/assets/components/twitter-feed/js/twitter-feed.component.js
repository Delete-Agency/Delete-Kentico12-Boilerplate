import { DcBaseComponent } from '@deleteagency/dc';
import { render } from 'preact';
import React from 'preact/compat';
import Tweets from '~/components/twitter-feed/js/tweets';
import api from '~/general/js/api';

export default class TwitterFeed extends DcBaseComponent {
    static getNamespace() {
        return 'twitter-feed';
    }

    onInit() {
        this._requestContent();
    }

    _requestContent = () => {
        api.get(this.options.endpoint).then((result) => {
            render(<Tweets tweets={result.data.tweets} parentNode={this.element} />, this.element);
        });
    };
}
