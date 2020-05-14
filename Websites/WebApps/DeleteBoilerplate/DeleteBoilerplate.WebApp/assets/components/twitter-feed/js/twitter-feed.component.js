import { DcBaseComponent } from '@deleteagency/dc';
import { render } from 'preact';
import React from 'preact/compat';
import Tweets from 'components/twitter-feed/js/tweets';
import api from 'general/js/api';

function toCamel(o) {
    let newO;
    let origKey;
    let newKey;
    let value;
    if (o instanceof Array) {
        return o.map(function (value) {
            if (typeof value === 'object') {
                value = toCamel(value);
            }
            return value;
        });
    }
    newO = {};
    for (origKey in o) {
        if (o.hasOwnProperty(origKey)) {
            newKey = (origKey.charAt(0).toLowerCase() + origKey.slice(1) || origKey).toString();
            value = o[origKey];
            if (value instanceof Array || (value !== null && value.constructor === Object)) {
                value = toCamel(value);
            }
            newO[newKey] = value;
        }
    }

    return newO;
}

export default class TwitterFeed extends DcBaseComponent {
    static getNamespace() {
        return 'twitter-feed';
    }

    onInit() {
        this._requestContent();
    }

    _requestContent = () => {
        api.get(this.options.endpoint).then((result) => {
            render(
                <Tweets tweets={toCamel(result.data.Tweets)} parentNode={this.element} />,
                this.element
            );
        });
    };
}
