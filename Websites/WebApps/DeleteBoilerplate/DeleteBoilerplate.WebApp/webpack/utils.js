function isJsonOutput() {
    return process.argv.indexOf('--json') >= 0;
}

function log(message) {
    if (!isJsonOutput()) {
        console.log(message);
    }
}

module.exports = {
    isJsonOutput,
    log
};
