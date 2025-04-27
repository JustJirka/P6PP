import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://user-service:5189',
        changeOrigin: true,
        secure: false,
        rewrite: path => path.replace(/^\/api/, '/api'),
      },
      '/auth-api': {
        target: 'http://auth-service:8005',
        changeOrigin: true,
        secure: false,
        rewrite: path => path.replace(/^\/auth-api/, '/api'),
      },
      '/booking-api': {
        target: 'http://booking-service:8080',
        changeOrigin: true,
        secure: false,
        rewrite: path => path.replace(/^\/booking-api/, ''),
      },
      '/admin-api': {
        target: 'http://admin-settings:9090',
        changeOrigin: true,
        secure: false,
        rewrite: path => path.replace(/^\/admin-api/, '/api'),
      },
      '/notification-api': {
        target: 'http://notification-service:5181',
        changeOrigin: true,
        secure: false,
        rewrite: path => path.replace(/^\/notification-api/, '/api'),
      },
    },
  },
});