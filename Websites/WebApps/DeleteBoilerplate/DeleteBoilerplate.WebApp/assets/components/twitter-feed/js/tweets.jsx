import React from 'preact/compat';
import PropTypes from 'prop-types';
import { deviceObserver } from '@deleteagency/device-observer';
import { nanoid } from 'nanoid';
import { DEVICE_TABLET } from 'general/js/app';

class Tweets extends React.PureComponent {
    constructor(props) {
        super(props);

        const { tweets } = props;

        tweets.forEach((tweet) => {
            tweet.ref = React.createRef();
            tweet.content = this.#makeContent(tweet.text);
        });
    }

    #isDesktop = () => deviceObserver.isGt(DEVICE_TABLET);

    #isCalculated = false;

    #makeContent = (text) => {
        const array = text.split(' ');
        return array
            .map((item) => {
                if (item[0] === '#') {
                    return `<span>${item}</span>`;
                }
                if (item.indexOf('http') === 0) {
                    return `<a href="${item}">${item}</a>`;
                }
                return item;
            })
            .join(' ');
    };

    render() {
        const { tweets } = this.props;

        return tweets.map((tweet) => {
            let factor = null;
            if (tweet.imageUrl) {
                factor = tweet.heightImage / tweet.widthImage;
            }

            const id = nanoid();
            return (
                <li key={id} className="twitter-feed__item tweet" ref={tweet.ref}>
                    <div className="tweet__inner">
                        {factor !== null && (
                            <a
                                className="tweet__media"
                                target="_blank"
                                rel="noopener noreferrer"
                                href={tweet.link}
                                style={{
                                    paddingTop: `${100 * factor}%`,
                                }}
                                aria-labelledby={id}
                            >
                                <img
                                    className="lazyload tweet__img"
                                    data-src={tweet.imageUrl}
                                    alt={tweet.imageAlt || tweet.date}
                                />
                            </a>
                        )}
                        <a
                            className="tweet__date"
                            target="_blank"
                            rel="noopener noreferrer"
                            href={tweet.link}
                        >
                            {tweet.date}
                        </a>
                        <p id={id} dangerouslySetInnerHTML={{ __html: tweet.content }} />
                    </div>
                </li>
            );
        });
    }

    componentDidMount() {
        this.#checkSize();
        deviceObserver.subscribeOnResize(this.#checkSize);
    }

    #checkSize = () => {
        if (this.#isDesktop()) {
            this.#set25Layout();
        } else {
            this.#set50Layout();
        }
    };

    #set50Layout = () => {
        this.#setPosition(2);
    };

    #set25Layout = () => {
        this.#setPosition(4);
    };

    #setPosition = (difIndex) => {
        const { tweets, parentNode } = this.props;
        if (this.#isCalculated) this.#setClean();
        const parentBounds = parentNode.getBoundingClientRect();
        let mostBottom = 0;
        tweets.forEach((tweet, index) => {
            const upperIndex = index - difIndex;
            if (tweets[upperIndex]) {
                const upperBot = tweets[upperIndex].ref.current.getBoundingClientRect().bottom;
                const tweetTop = tweet.ref.current.getBoundingClientRect().top;
                const difference = upperBot - tweetTop;
                if (difference < 0) {
                    tweet.ref.current.style.transform = `translateY(${difference}px)`;
                }
                const bot = tweet.ref.current.getBoundingClientRect().bottom;
                if (bot > mostBottom) mostBottom = bot;
            }
        });
        parentNode.style.marginBottom = `${mostBottom - parentBounds.bottom}px`;
        this.#isCalculated = true;
    };

    #setClean = () => {
        const { tweets } = this.props;
        tweets.forEach((tweet) => {
            tweet.ref.current.style.transform = '';
        });
    };
}

Tweets.propTypes = {
    tweets: PropTypes.array.isRequired,
    parentNode: PropTypes.obj.isRequired,
};

export default Tweets;
