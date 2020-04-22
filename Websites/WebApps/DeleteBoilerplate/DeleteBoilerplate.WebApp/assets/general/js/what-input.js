import whatInput from 'what-input';

if (whatInput.ask('intent') !== 'touch') {
    document.documentElement.classList.add('can-hover');
}

whatInput.registerOnChange((type) => {
    if (type === 'touch') {
        document.documentElement.classList.remove('can-hover');
    } else {
        document.documentElement.classList.add('can-hover');
    }
}, 'intent');
