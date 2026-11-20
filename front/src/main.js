import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { pt } from 'vuetify/locale'
import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'

// Vuetify em português (textos da tabela, paginação etc.)
const vuetify = createVuetify({
    components,
    directives,
    locale: { locale: 'pt', messages: { pt } }
});

const app = createApp(App)
app.use(vuetify);
app.use(createPinia())
app.use(router)

app.mount('#app')
