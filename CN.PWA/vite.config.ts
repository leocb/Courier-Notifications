import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import { VitePWA } from 'vite-plugin-pwa'
import { gitDescribeSync } from 'git-describe'

// https://vitejs.dev/config/
export default defineConfig({
  build: {
    assetsInlineLimit: 0
  },
  define: {
    APP_VERSION: JSON.stringify(process.env.npm_package_version),
    GIT_HASH: JSON.stringify(gitDescribeSync().hash)
  },
  plugins: [
    vue(),
    vueJsx(),
    VitePWA({
      registerType: 'autoUpdate',
      injectRegister: 'auto',
      workbox: {
        cleanupOutdatedCaches: true,
        globPatterns: ['**/*.{js,css,html,ico,png,svg,jpg,webp,mp3,json,vue,txt,woff2}']
      },
      manifest: {
        theme_color: '#af9281',
        background_color: '#ede1d2',
        display: 'standalone',
        scope: '/',
        start_url: '/',
        name: 'Courier Notifications',
        short_name: 'CN',
        description: 'A simple notification system',
        icons: [
          {
            src: 'pwa-64x64.png',
            sizes: '64x64',
            type: 'image/png'
          },
          {
            src: 'pwa-192x192.png',
            sizes: '192x192',
            type: 'image/png'
          },
          {
            src: 'pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'any'  
          },
          {
            src: 'maskable-icon-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'maskable'
          }
        ]
      }
    })
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  css: {
    preprocessorOptions: {
      scss: {
        additionalData: `@import "@/assets/scss/colors.scss";
          @import "@/assets/scss/input-radio.scss";`
      }
    }
  }
})
