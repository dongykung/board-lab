import { httpPost } from '@/utils/http';

export function login(loginId, password) {
    return httpPost('/users/login', { loginId, password });
}

export function register( { userName, userId, userPassword} ) {
    return httpPost('/users/register', { userName, userId, userPassword });
}