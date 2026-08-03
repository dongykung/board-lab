import { ref } from 'vue'
import { login as loginApi, register as registerApi } from '../api/authApi'
 
export function userAuth() {
    const loading = ref(false);
    const error = ref(null);

    async function withState(fn) {
        loading.value = true
        error.value = null
        try {
            return await fn()
        } catch (e) {
            error.value = e.body?.message ?? '요청 처리 중 오류가 발생했습니다.'
            return null
        } finally {
            loading.value = false
        }
    }

    function login(loginId, password) {
        return withState(() => loginApi(loginId, password))
    }

    function register(payload) {
        return withState(() => registerApi(payload))
    }

    return { login, register, loading, error }
}