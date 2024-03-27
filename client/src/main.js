import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { ConnectionServices } from '@/services/gameHubServiceSymbol.js'

const app = createApp(App)

app.use(router)

app.use(ConnectionServices)

app.mount('#app')
