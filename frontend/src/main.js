import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import './design-system/tokens/colors.css'
import './design-system/tokens/spacing.css'
import './design-system/tokens/radius.css'
import './design-system/tokens/typography.css'

createApp(App).use(router).mount('#app')
