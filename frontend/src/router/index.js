import { createRouter, createWebHistory } from 'vue-router'
import authRoutes from '@/features/auth/routes'


const routes = [...authRoutes]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

export default router