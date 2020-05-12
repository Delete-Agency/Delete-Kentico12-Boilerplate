import axios from 'axios';

const instance = axios.create({
    timeout: 30000,
    withCredentials: true,
});

export default instance;
