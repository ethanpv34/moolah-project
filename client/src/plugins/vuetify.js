import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import '@mdi/font/css/materialdesignicons.css'

export default createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'lightRed',
    themes: {
      lightRed: {
        dark: false,
        colors: {
          primary: '#e57373',
          secondary: '#ffcdd2',
          accent: '#f44336',
          error: '#d32f2f',
          info: '#ff8a80',
          success: '#81c784',
          warning: '#ffb74d',
        }
      }
    }
  }
}) 