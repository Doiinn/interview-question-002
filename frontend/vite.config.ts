import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import ui from '@nuxt/ui/vite'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    ui(),
    vueDevTools(),
  ],
  server: {
    proxy: {
      // Backend has no CORS configured; proxy API calls in dev.
      // Production deployment needs CORS enabled on the API or same-origin hosting.
      '/api': {
        target: 'http://localhost:5178',
        changeOrigin: true,
      },
    },
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
})
