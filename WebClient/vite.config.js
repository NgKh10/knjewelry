import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
    plugins: [react()],
    base: '/admin/',
    server: {
        port: 3000,
        proxy: {
            // Auth service chạy riêng trên port 5002
            '/api/auth': {
                target: 'http://localhost:5002',
                changeOrigin: true,
                secure: false,
            },
            // API service chính trên port 5001
            '/api': {
                target: 'http://localhost:5001',
                changeOrigin: true,
                secure: false,
            },
            '/images': {
                target: 'http://localhost:5001',
                changeOrigin: true,
                secure: false,
            },
            '/img': {
                target: 'http://localhost:5001',
                changeOrigin: true,
                secure: false,
            }
        }
    }
})
