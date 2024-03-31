import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { VueSignalR } from '@dreamonkey/vue-signalr'
import { HttpTransportType, HubConnectionBuilder } from '@microsoft/signalr'

const app = createApp(App)

app.use(router)

const connection = new HubConnectionBuilder()
  .configureLogging('information')
  .withAutomaticReconnect()
  .withUrl('https://localhost:7031/hub/game', {
    transport: HttpTransportType.WebSockets,
    withCredentials: false
  })
  .build()

app.use(VueSignalR, { connection })

app.mount('#app')
